using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BSTools;

public class bouncingBullet : bulletMovement
{
    //Unused, used to be for a bouncing stick of dynamite

    //Used as a modifier for a calculation
    static readonly float rootTwo = .5f;//Mathf.Sqrt(2);

    Rigidbody2D rb;
    float impactTime = 0;
    public GameObject acceleratedSpawnOnHit;//This will be spawned instead of the normal object if this bullet was accelerated
    Collider2D col; 

    protected override void Start()
    {
        base.Start();

        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.velocity = speed * transform.right;
    }

    protected override void Update()
    {
        base.Update();

        proximityCheck();

        Vector3 tCopy = transform.rotation.eulerAngles;

        //Rotate visual
        transform.rotation = Quaternion.Euler(tCopy.x, tCopy.y, tools.angleFromVector(rb.velocity));
    }

    protected override void OnTriggerEnter2D(Collider2D hit) {}

    void proximityCheck() 
    {
        //Explode if close enough to an actor
        foreach (combatActor actor in combatActor.everyActor) 
        {
            if (actor.gameObject == owner) continue;
            if (associates.Contains(actor.gameObject)) continue;

            if (actor.distanceToPoint(transform.position) <= 0)
                if (tools.minimalVisionCheck(transform.position, actor.transform.position)) gameEnd();
        }
    }

    void OnCollisionEnter2D(Collision2D hit) 
    {   
        //Explode on contant instead of bounching if it's fast enough
        if (rb.velocity.magnitude > 1.1f * speed) 
        {
            //Teleport backward so the explosion isn't inside a wall
            transform.position += (Vector3)rb.velocity.normalized * rootTwo;
            gameEnd();
        } 
    }

    protected override void gameEnd()
    {
        //Replace the regular spawn on hit with the accelerated one if going fast enough
        if (rb.velocity.magnitude > 1.1f * speed) 
            if ((despawnTime >= 0.0f) || (Time.time < despawnTime))
                spawnOnHit = acceleratedSpawnOnHit;

        //Bullet gameEnd() takes care of spawning
        base.gameEnd();
    }
}
