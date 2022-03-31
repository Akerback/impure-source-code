using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using BSTools;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class combatActor : MonoBehaviour
{
    //Parent class for any object that can be damaged
    //--List of all instances
    public static List<combatActor> everyActor = new List<combatActor>();
    public static int actorCount = 0;//Useless counter, could use everyActor.Count instead

    //--Internals/settings
    [HideInInspector] public Collider2D col;
    [HideInInspector] public Rigidbody2D rb;
    [Min(1)] public float maxHp = 100;
    protected float hp = -1;//HP gets initalized in Awake()
    [Range(0.0f, 10.0f)] public float damageMultiplier = 1.0f;
    public bool ghost;//Unused, would've been used for a notarget cheat
    public GameObject deathLoot;
    [HideInInspector] public bool isAlive = true;

    //--Events
    public UnityEvent onHurt;
    public UnityEvent onDeath;

    protected virtual void Awake()
    {
        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();

        //More init
        addActor();
        setHealthCapped(maxHp);
    }

    public bool setHealthCapped(float newHp) 
    {
        //Returns if there was a change to hp
        bool result = true;
        
        //Cap it, there's no lower bound
        float cappedHp = Mathf.Min(newHp, maxHp);

        if (cappedHp == hp) result = false;

        hp = cappedHp;

        return result;
    }

    public float getHealth()
    {
        return hp;
    }

    public float getHealthRatio() 
    {
        //Just a div by 0 prevention
        if (maxHp == 0.0f) return -1.0f;

        return hp / maxHp;
    }

    public virtual void hurt(float dmg, GameObject attacker = null, int damageType = 0) 
    {
        float oldHealth = hp;

        //Scale it before application
        dmg *= damageMultiplier;

        hp -= dmg;
        
        if (attacker != gameObject) {
            playerActor playerCheck = attacker.GetComponent<playerActor>();
            if (playerCheck != null) playerCheck.bloodForBlood(dmg);
        }

        //If this hit killed the actor, perform the deathEvent
        if ((hp <= 0) && (isAlive)) deathEvent(attacker);

        onHurt.Invoke();
    }

    public virtual void deathEvent(GameObject killer = null) 
    {
        onDeath.Invoke();

        removeActor();

        isAlive = false;

        Destroy(gameObject);
    }

    protected virtual void OnDestroy() 
    {
        removeActor();
    }

    protected void spawnLoot(GameObject killer = null) 
    {
        if (deathLoot != null) 
        {
            GameObject surpriseMechanic = Instantiate(deathLoot, transform.position, tools.noRotation);
            painSource painfulSurprise = surpriseMechanic.GetComponent<painSource>();

            if (painfulSurprise != null) 
            {
                painfulSurprise.owner = killer;
            }
        }
    }

    public void addActor() 
    {
        everyActor.Add(this);
        actorCount++;
    }

    public void removeActor() 
    {
        if (everyActor.Remove(this)) actorCount--;
    }
    
    //--Some util functions
    public static combatActor nearest(Transform ownTransform) 
    {
        //Get the actor closest to a point
        combatActor chosenOne = null;
        float distance = float.PositiveInfinity;

        foreach (combatActor actor in everyActor) 
        {
            float thisDistance = actor.distanceToPoint(ownTransform.position);
            
            if (thisDistance < distance) 
            {
                distance = thisDistance;
                chosenOne = actor;
            }
        }

        return chosenOne;
    }

    public float distanceToPoint(Vector2 point) 
    {
        //Gets actor to point distance
        Vector2 meToYou = (Vector2)transform.position - point;
        float distance = meToYou.magnitude - simpleRadius();

        return distance;
    }

    public float distanceToPoint(Vector3 point) {
        //Just a overload so i wouldn't need to cast it manually
        return distanceToPoint((Vector2)point);
    }

    public float simpleRadius() 
    {
        return col.bounds.extents.magnitude;
    }
}
