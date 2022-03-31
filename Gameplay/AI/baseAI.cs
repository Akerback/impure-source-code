using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using BSTools;
using TDSTools;

[RequireComponent(typeof(Rigidbody2D))]
public class baseAI : combatActor
{
    //--Statics
    public static List<baseAI> everyAI = new List<baseAI>();
    public static List<aiSound> everySound = new List<aiSound>();

    //--General settings
    [Tooltip("Damage required per drop of blood.")] public float bleedThreshold = 2;
    [Tooltip("Damage required to trigger pain.")] public float painTolerance = 20;
    [Tooltip("How long an enemy remains in pain.")] public float painTime = 0.5f;
    [Tooltip("Damage required to trigger infighting.")] public float threatResistance = 10.0f;
    public int updatesPerSecond = 2;//Maybe unnecessary limit to how often an AI looks or listens while idle

    //--Components
    protected ai_movement move;
    protected particleDestroyer bleed;
    protected particleDestroyer stagger;
    public SpriteRenderer spr;

    //--Sight and hearing
    public bool wallhacks = false;//If true the enemy can see through walls
    [Tooltip("How far an AI can see.")] public float perception = 10;
    //AI vision range, perception <= 0 -> blind
    public float fov = 90;
    [Tooltip("How far an AI can hear.")] public float hearing = 10;
    //Hearing range, this is multiplied by each sound's volume, hearing <= 0 -> deaf
    [Tooltip("Within how many angles an AI can pinpoint sound location.")] public float hearingAngle = 10;
    [Tooltip("Within how far an AI can pinpoint sound location.")] public float hearingPrecision = 1;
    [Tooltip("How much walls dampen sound this AI listens to.")] public float wallSoundDampening = 2;
    public float attentionSpan = 0.5f;
    //What a sound's interest value is mulitplied by every time Listen() is called

    //--Actor references
    public combatActor lastAttacker;
    public combatActor targetActor;

    //--Goals
    public aiTarget currentTarget;//Unused?
    protected aiTarget visualTarget;
    [HideInInspector] public aiTarget movementTarget;
    
    //--Internals
    protected bool hasWokenUp = false;
    public float expireCorpse = 10f;

    //Idle update timing
    protected float updateDelay = -1;
    protected float nextUpdateTime = 0;

    protected float painEnd;//Time when AI exits the pain state
    protected float painHealth;//Hidden healthbar, when this reaches 0 the AI enters pain
    protected float attackerThreat = 0.0f;//How much damage has been taken from the same attacker
    protected float unbled = 0;//Fractional part of blood loss, blood loss is based on the integer part

    //--Sound FX
    public AudioClip alertSound;
    public AudioClip painSound;
    public AudioClip deathSound;
    public float pitchVariation = 0f;
    protected float basePitch = 1f;
    protected AudioSource audioS;

    //--Events
    public UnityEvent onAwaken;
    
    protected override void Awake()
    {
        base.Awake();

        initialize();
    }

    protected void initialize() 
    {
        move = GetComponent<ai_movement>();
        rb = GetComponent<Rigidbody2D>();
        spr = GetComponent<SpriteRenderer>();

        //Find all particle handlers
        particleDestroyer[] carrier = GetComponentsInChildren<particleDestroyer>();

        int i = 0;

        foreach (particleDestroyer part in carrier) 
        {
            //Only one source of blood, and one source of stagger particles
            if (i == 0) 
                bleed = part;
            else
            {
                //Stagger was a unused mechanic
                stagger = part;
                break;
            }
            i++;
        }

        if (stagger != null) 
        {
            var particleEmitter = stagger.part.emission;
            particleEmitter.enabled = false;
        }

        //Init goals
        visualTarget   = new aiTarget(transform.position);
        movementTarget = new aiTarget(transform.position);

        //Max out threat counter so AI aggroes to the first attacker
        attackerThreat = threatResistance;

        //Add to static list
        everyAI.Add(this);

        //Save the default pitch for randomization later
        audioS = GetComponent<AudioSource>();

        if (audioS != null) 
        {
            basePitch = audioS.pitch;
        }
        else Debug.LogWarning(gameObject.ToString() + ": no AudioSource found!");

        //Some more initialization
        setDecisionRate(updatesPerSecond);
        resetPain();
    }

    //Enemy has awoken
    protected virtual void awaken() 
    {
        if (hasWokenUp) return;
        onAwaken.Invoke();

        //Alert sound
        if (audioS != null) 
        {
            //Pitch randomization
            audioS.pitch = basePitch + Random.Range(-pitchVariation / 2, pitchVariation / 2);
            //Play it
            //Debug.Log(gameObject.ToString() + ": Plays alert sound.");
            audioS.PlayOneShot(alertSound, 1f);
        }

        hasWokenUp = true;

        attackerThreat = 0.0f;
    }

    protected virtual void Update()
    {
        evaluateTime();//This will both check time and forward to behaviour if time is right
    }

    //Resets painHealth
    public void resetPain() 
    {
        painHealth = painTolerance;
        if (stagger != null) 
        {
            //Unused stagger mechanic. Enemies would be staggered while in pain
            var particleEmitter = stagger.part.emission;
            particleEmitter.enabled = false;
        }
    }

    //Checks time and forwards to decision making
    protected void evaluateTime() 
    {
        float currentTime = Time.time;
        move.decayInterest(attentionSpan);

        //Är det dags att göra något?
        if ((currentTime > nextUpdateTime) && (nextUpdateTime >= 0))
        {
            //Gör något och uppdatera tiden
            makeDecision();
            nextUpdateTime = currentTime + updateDelay;
        }
    }

    public virtual void makeDecision() 
    {
        //Hard coded destruction. Almost all AI override this to prevent instant destruction
        if (!isAlive) Destroy(gameObject);

        observe();
    }

    //Look and listen
    public virtual void observe() 
    {
        //Awaken if pushed
        if ((rb.velocity.magnitude > 0.1f) && (targetActor == null)) awaken();
        
        //Untarget dead actors, unused in most AI since their behaviour is handled in Update()
        if (!combatActor.everyActor.Contains(targetActor)) targetActor = null;

        lookfor(targetActor);
        //Listen only if nothing was seen
        if (targetActor == null) listen();
    }

    //If looking for a null actor it defaults to looking for the player
    protected void lookfor(combatActor actorToLookFor) 
    {
        //Flag for if something new was spotted
        bool freshDiscovery = false;

        if (perception > 0) 
        {
            //Check if there is no target
            if (actorToLookFor == null) 
            {
                //Init a list of seen players
                List<playerActor> seenPlayers = new List<playerActor>();
                int visualCount = 0;//Useless counter, can use seenPlayers.Count instead

                //Loop over all players
                foreach (playerActor coActor in playerActor.everyPlayer) 
                {
                    if (perceptionCheck(coActor)) 
                    {
                        seenPlayers.Add(coActor);
                        visualCount++;
                    }
                }
                
                //Pick a random player from all visible players
                if (visualCount > 0) actorToLookFor = seenPlayers[(int)Random.Range(0, visualCount - 1)];
                else return;

                freshDiscovery = true;
            }

            //No perception check needed as one was done in the previous step if this flag is set.
            if (freshDiscovery) 
            {
                if (targetActor == null) awaken();
                targetActor = actorToLookFor;
                //Last known position
                move.petitionNewTarget(new aiTarget(actorToLookFor.transform.position, true, true));

                return;
            }
            else 
            {
                //Look for an already targetted actor
                if (perceptionCheck(actorToLookFor)) 
                {
                    //Last known position
                    move.petitionNewTarget(new aiTarget(actorToLookFor.transform.position, true, true));
                }
            }
        }
    }

    protected bool perceptionCheck(combatActor target) 
    {
        Vector2 meToYou = target.transform.position - transform.position;
        float distance = target.distanceToPoint(transform.position);
        
        //--Test everything, returns true if all tests pass
        if (perception >= distance) //Range
            if (Mathf.Abs(tools.getRelativeRotation(meToYou, transform)) <= fov / 2) //FoV
                if (wallhacks || tools.minimalVisionCheck(transform.position, target.transform.position)) //Wall check
                    return true;

        return false;
    }

    protected void listen() 
    {
        Vector2 meToYou;
        float distance;
        float thisInterest;
        
        if (hearing > 0) 
        {
            bool soundWasFound = false;

            foreach (aiSound noise in baseAI.everySound) 
            {
                //Skip muted sounds
                if (noise.volume == 0.0f) continue;

                meToYou = noise.transform.position - transform.position;
                distance = meToYou.magnitude;
                
                //Approximate sound attenuation from walls using two raycasts
                float freeListen = Physics2D.Raycast(transform.position, meToYou, meToYou.magnitude, 1 << 9).distance;
                float freeEmit   = Physics2D.Raycast(noise.transform.position, -meToYou, meToYou.magnitude, 1 << 9).distance;

                //Debug.Log(gameObject.ToString() + " is listening to " + noise.gameObject.ToString() + ". Occlusion is: " + freeListen.ToString() + " listen & " + freeEmit.ToString() + " emit.");
                float audioDampening = distance - freeEmit + freeListen;
                if (audioDampening == distance) audioDampening = 0;

                //Sound attenuation works by adding distance
                distance += audioDampening * (wallSoundDampening - 1);

                float hearingScaled = hearing * noise.volume;

                //Range check
                if (hearingScaled >= distance)
                {
                    soundWasFound = true;

                    //Calculate interest
                    float distanceScaler = 1 - distance / hearingScaled;

                    //Louder sounds are more interesting
                    thisInterest = noise.volume * distanceScaler;

                    //Random offset for where the target position gets put
                    Vector2 vectorToGoal = noise.transform.position - transform.position;
                    //Randomize distance
                    float distToGoal = vectorToGoal.magnitude + Random.Range(-hearingPrecision / 2, hearingPrecision / 2);
                    //Randomize angle 
                    vectorToGoal = tools.vectorInCone(tools.angleFromVector(vectorToGoal), hearingAngle / 2);

                    move.petitionNewTarget(new aiTarget(transform.position + (Vector3)vectorToGoal * distToGoal, true, false, thisInterest));
                }
            }
            //Wake the AI if there was a sound
            if ((soundWasFound) && (targetActor == null)) awaken();
        }
    }

    protected void setDecisionRate(int newRate) 
    {
        updatesPerSecond = newRate;

        //Negative rates freeze the AI
        if (updatesPerSecond > 0) 
            updateDelay = 1 / updatesPerSecond;
        else
            updateDelay = -1;
    }

    public override void hurt(float dmg, GameObject attacker = null, int damageType = 0)
    {
        if (!isAlive) return;
        //Ignore pain if it wasn't reset
        bool ignorePain = (painHealth <= 0);
        
        //Calculate and apply damage
        painHealth -= dmg * damageMultiplier;

        base.hurt(dmg, attacker, damageType);

        //Bleeding
        int flooredBleed = Mathf.FloorToInt(dmg * damageMultiplier / bleedThreshold + unbled);
        if (bleed != null) bleed.part.Emit(flooredBleed);

        //Save the fractional part for future bleeding calculations
        unbled = dmg * damageMultiplier / bleedThreshold + unbled - flooredBleed;

        //Enter pain if painhealth reached 0
        if ((ignorePain == false) && (painHealth <= 0)) pain();

        //Target changing for infighting
        combatActor carrier = attacker.GetComponent<combatActor>();

        if (carrier != null) 
        {
            if (carrier != lastAttacker) 
            {
                //Reset threat only if there has been a previous attacker
                if (lastAttacker != null) attackerThreat = 0;
                //Update who the last attacker was
                lastAttacker = carrier;
            }

            //Add to the threat counter
            attackerThreat += dmg;

            //Check if it exceed's the limit
            if (attackerThreat >= threatResistance) 
            {
                //Wake the AI
                if (targetActor == null) awaken();
                //Target the attacker
                targetActor = carrier;
                //Reset threat
                attackerThreat = 0;
            }
            if (attacker != null) move.petitionNewTarget(new aiTarget(attacker.transform.position, true, true));
        }
    }
    
    public override void deathEvent(GameObject killer = null) 
    {
        everyAI.Remove(this);
        isAlive = false;
        onDeath.Invoke();
        postMortem(killer);

        if (expireCorpse > 0) nextUpdateTime = Time.time + expireCorpse;
        else nextUpdateTime = -1;
    }
    
    public virtual void postMortem(GameObject killer = null) {
        removeActor();

        audioS.PlayOneShot(deathSound);

        spawnLoot();

        //Disable collision and move the visual down in the sorting order.
        gameObject.layer = 8;
        spr.sortingOrder = constantsContainer.layer_detail;
        spr.enabled = false;
    }

    public void disablePhysics() {
        rb.bodyType = RigidbodyType2D.Static;
    }

    protected override void OnDestroy() 
    {
        everyAI.Remove(this);
        removeActor();
    }

    //Only damages pain health
    public void painDamage(float dmg, GameObject attacker = null)
    {
        //basically identical to hurt()
        bool ignorePain = (painHealth <= 0);
        
        painHealth -= dmg * damageMultiplier;

        if ((ignorePain == false) && (painHealth <= 0)) pain();

        //Threat is also applied
        combatActor carrier = attacker.GetComponent<combatActor>();

        if (carrier != null) 
        {
            if (carrier != lastAttacker) 
            {
                if (lastAttacker != null) attackerThreat = 0;
                lastAttacker = carrier;
            }

            attackerThreat += dmg;

            if (attackerThreat >= threatResistance) 
            {
                if (targetActor == null) awaken();
                targetActor = carrier;
                move.petitionNewTarget(new aiTarget(targetActor.transform.position, true, true));
                attackerThreat = 0;
            }
        }
    }

    //Pain
    public virtual void pain() 
    {
        //Delay the next update by paintime
        nextUpdateTime += painTime;
        painEnd = Time.time + painTime;
        
        if (stagger != null) 
        {
            var particleEmitter = stagger.part.emission;
            var particleMain = stagger.part.main;
            particleEmitter.enabled = true;
            
        }

        //Pain sound
        if ((audioS != null) && (isAlive)) 
        {
            //Random pitch
            audioS.pitch = basePitch + Random.Range(-pitchVariation / 2, pitchVariation / 2);
            //Play it
            //Debug.Log(gameObject.ToString() + ": Plays pain sound.");
            audioS.PlayOneShot(painSound, 1f);
        }
        else 
        {
            if (audioS == null) Debug.LogWarning(gameObject.ToString() + ": no AudioSource found! (Pain)");
            //else Debug.LogWarning(gameObject.ToString() + ": AI is already dead! (Pain)");
        }

        //Debug.Log("Ouch");
    }
    //--DEBUG FUNKTIONER

    //Poke the AI's brain
    public aiTarget pokeThoughts(string desiredKnowledge) 
    {
        aiTarget response = new aiTarget(transform.position, false);

        if (desiredKnowledge == "visual") response = visualTarget;
        if (desiredKnowledge == "plans")  response = movementTarget;

        return response;
    }

}
