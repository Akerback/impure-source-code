using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using BSTools;

public class bulletMovement : painSource
{
    public GameObject spawnOnHit = null;
    //Spawned object when bullet expires or hits something
    [SerializeField] protected bool rotateSpawnedObject = false;
    [SerializeField] float damage = 1;
    public float flightTime = 10.0f;
    //Time before expiration
    public float speed = 10;
    [SerializeField] protected float speedVariation = 0;
    //Variation is applied both forward and backward
    [SerializeField] float accel = 0;
    //Acceleration
    [SerializeField] bool allowReverse = false;
    //If negative acceleration allows the bullet to change direction
    [SerializeField] bool wallsOnly = false;

    protected float startTime;
    protected float despawnTime = -1.0f;
    [HideInInspector] public Vector2 moveVector = new Vector2(0, 0);

    protected virtual void Start() 
    {
        //Randomize speed
        speed += Random.Range(-speedVariation, speedVariation);
        //Snapshot start time
        startTime = Time.time;
        
        //More init
        updateFlightTime(flightTime);
        updateDirection();
    }
    
    protected virtual void Update()
    {
        //Expiration
        if ((despawnTime >= 0.0f) && (Time.time > despawnTime)) gameEnd();

        //step is how far this bullet moves in this frame. moveVector is set by updateDirection()
        Vector2 step = moveVector * speed * Time.deltaTime;

        //Acceleration
        if (accel != 0.0f) 
        {
            //Acceleration this frame
            float accelerationStep = accel * Time.deltaTime;

            //Reversal logic
            if (allowReverse || (Mathf.Sign(speed + accelerationStep) == Mathf.Sign(speed))) 
                speed += accelerationStep;
            else 
                //Set speed to 0 if reversal is disallowed
                speed = 0;
        }

        //Move it
        transform.position += (Vector3)step;
        //transform.Translate(step);
    }

    public void updateDirection() 
    {
        moveVector = transform.right;
    }

    protected virtual void OnTriggerEnter2D(Collider2D hit)
    {
        //Debug.Log("A bullet Hit: " + hit.gameObject.ToString());
        //Ignore friendlies
        if (friendlyCheck(hit.gameObject)) return;

        //Is it a valid thing to collide with?
        bool invalidCollision = false;

        if (wallsOnly) 
            invalidCollision = (hit.gameObject.tag != "visionBlocking");
        else 
            invalidCollision = ((hit.gameObject.tag != "bulletCollideable") && (hit.gameObject.tag != "visionBlocking"));
        
        //Abort if invalid
        if (invalidCollision) return;

        //Check if it was an actor
        combatActor actorHit = hit.gameObject.GetComponent<combatActor>();

        if (actorHit != null) 
        {
            //Ignore dead actors
            if (actorHit.getHealthRatio() <= 0.0f) return;

            //Unnecessary check, hurt already handles if attacker is null
            if (owner != null)
                actorHit.hurt(damage, owner);
            else
                actorHit.hurt(damage);

            //Knockback based on damage
            if (actorHit.rb != null) actorHit.rb.AddForce(transform.right * damage * knockbackMultiplier);
            
            gameEnd();
        }
        else gameEnd();
    }

    public virtual void updateFlightTime(float newTime = -1.0f) 
    {
        //Calculates end time based on start time
        flightTime = newTime;
        
        if (flightTime >= 0) 
            despawnTime = startTime + flightTime;
        else 
            despawnTime = -1.0f;
    }

    protected virtual void gameEnd() 
    {
        //Handles bullet destruction with spawning

        //Containers
        Quaternion spawnRotation = Quaternion.Euler(Vector3.zero);
        GameObject spawnedObject = null;
 
        if (rotateSpawnedObject) spawnRotation = transform.rotation;

        //Create an instance of spawnOnHit
        if (spawnOnHit != null) 
            spawnedObject = Instantiate(spawnOnHit, transform.position, Quaternion.Euler(Vector3.zero));

        impartOwner(spawnedObject);
        
        Destroy(gameObject);
    }
}
