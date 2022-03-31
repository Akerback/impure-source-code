using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BSTools;
using TDSTools;

public class ai_simpleBase : combatActor
{
    //Unused rewrite of baseAI
    [HideInInspector] public aiTarget visualTarget = new aiTarget(Vector3.zero);
    [HideInInspector] public aiTarget audioTarget = new aiTarget(Vector3.zero);
    public combatActor targetActor;
    public float painThreshold = 20.0f;
    float painHealth = 0.0f; //If greater than 0 from the start AI aren't guaranteed to aggro when hurt
    public bool isAmbushing = false;
    public float lookDistance = 50.0f;
    public float fieldOfView = 130.0f;
    public float hearingDistance = 20.0f;
    public float audioWallDampening = 1.0f;
    public float audioInterestDecay = 0.1f;
    public float updatesPerSecond = 2f;
    public float corpseDuration = 5f;
    float audioInterest = 0.0f;
    float timeBetweenUpdates = 0.5f;
    float nextUpdateTime = 0.0f;
    [HideInInspector] public bool isAwake = false;
    particleDestroyer bloodSource;

    protected virtual void Start() {
        bloodSource = GetComponentInChildren<particleDestroyer>();

        if (updatesPerSecond >= 0) timeBetweenUpdates = 1 / updatesPerSecond;
        else timeBetweenUpdates = -1;
    }

    protected virtual void Update() {
        if ((timeBetweenUpdates >= 0) && (Time.time > nextUpdateTime)) {
            observe();
            nextUpdateTime = Time.time + timeBetweenUpdates;
        }
    }

    protected void awaken() {
        if (isAwake) return;

        painHealth = painThreshold;

        isAwake = true;
    }

    protected void observe() {
        //Deaggro from dead actors
        if (!combatActor.everyActor.Contains(targetActor)) targetActor = null;

        look();
        listen();

        if (!isAwake) {
            if (visualTarget != null) awaken();
            else if ((!isAmbushing) && (audioTarget != null)) awaken();
        }
    }

    protected void look() {
        if (targetActor == null) targetActor = playerActor.everyPlayer[0];

        if (perceptionCheck(targetActor.transform.position)) visualTarget = new aiTarget(targetActor.transform.position, true);
    }

    protected void listen() {
        audioInterest = Mathf.Lerp(audioInterest, 0, audioInterestDecay);
        float distanceThreshold = 1 / hearingDistance;

        foreach (aiSound sound in aiSound.everySound) {
            if (sound.volume <= 0) continue;

            Vector2 vectorToSound = sound.transform.position - transform.position;
            float distance = vectorToSound.magnitude;

            //Approximate sound attenuation caused by walls
            //One raycast from this actor, one from the sound
            float freeListen = Physics2D.Raycast(transform.position, vectorToSound, distance, 1 << 9).distance;
            float freeEmit   = Physics2D.Raycast(sound.transform.position, -vectorToSound, distance, 1 << 9).distance;
            
            float distanceThroughWalls = distance - freeListen - freeEmit;

            float hearingScaled = hearingDistance * sound.volume;

            //Range check, then decide how interesting this sound is
            if (hearingScaled > distance) {
                //Interest == volume
                float thisInterest = 1 - distance / hearingScaled;

                if (thisInterest > audioInterest) {
                    audioInterest = thisInterest;
                    audioTarget = new aiTarget(sound.transform.position, true);
                }
            }
        }
    }

    public override void hurt(float dmg, GameObject attacker = null, int damageType = 0)
    {
        painHealth -= dmg * damageMultiplier;

        if (painHealth <= 0) pain(attacker);

        if (bloodSource != null) {
            bloodSource.part.Emit((int)dmg);
        }

        base.hurt(dmg, attacker, damageType);
    }

    protected virtual void pain(GameObject attacker) {
        combatActor actorCheck = attacker.GetComponent<combatActor>();

        if (actorCheck != null) targetActor = actorCheck;

        resetPain();
    }

    public void resetPain() {
        painHealth = painThreshold;
    }

    protected bool perceptionCheck(Vector2 targetPosition) {
        Vector2 vectorToTarget = targetPosition - (Vector2)transform.position;

        if (vectorToTarget.magnitude <= lookDistance) //Range
            if (Mathf.Abs(Vector2.SignedAngle(transform.right, vectorToTarget)) < fieldOfView / 2f) //FoV
                if (tools.minimalVisionCheck(transform.position, targetPosition)) //Raycast
                    return true;
        
        return false;
    }
}
