using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hitscan : painSource
{
    [SerializeField] float damage = 1;
    [SerializeField] GameObject spawnOnHit = null;
    [SerializeField] protected bool rotateSpawnedObject = false;
    [SerializeField] int pierceCount = 0;
    [SerializeField] float range = 100;
    [SerializeField] float rangeVariation = 0;
    //Range randomization, goes both forward and backward
    [SerializeField] float visualDuration = 0.2f;
    //Time the trail is visible
    [SerializeField] bool renderAsProjectile = true;
    //If false it's rendered as a trail/ray
    
    //--Internals
    float projectileSpeed = 0.0f;
    SpriteRenderer spr;
    
    
    void Start() 
    {
        //Start despawntimer
        Invoke("removeSelf", visualDuration);
        
        //Randomize range
        range += Random.Range(-rangeVariation, rangeVariation);

        //Get sprite render
        spr = GetComponent<SpriteRenderer>();

        //Calculate what was hit
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, transform.right, range, (1 << 9) | 1);
        
        RaycastHit2D hit = new RaycastHit2D();

        bool safeToSpawn = (spawnOnHit != null);

        foreach (RaycastHit2D hitCheck in hits) 
        {
            //Non-pierceable
            if (hitCheck.collider.gameObject.tag == "visionBlocking") {
                if (safeToSpawn) {
                    if (rotateSpawnedObject) Instantiate(spawnOnHit, hitCheck.point, transform.rotation);
                    else Instantiate(spawnOnHit, hitCheck.point, Quaternion.Euler(Vector3.zero));
                }
                if (!friendlyCheck(hitCheck.collider.gameObject))
                {
                    actorCheck(hitCheck);
                    hit = hitCheck;
                    break;
                }
            }
            //Pierceable object
            else if (hitCheck.collider.gameObject.tag == "bulletCollideable") {
                if (safeToSpawn) {
                    if (rotateSpawnedObject) Instantiate(spawnOnHit, hitCheck.point, transform.rotation);
                    else Instantiate(spawnOnHit, hitCheck.point, Quaternion.Euler(Vector3.zero));
                }
                //Obligatory friendly check
                if (!friendlyCheck(hitCheck.collider.gameObject)) 
                {
                    if (actorCheck(hitCheck))
                    {
                        if (pierceCount == 0) 
                        {
                            hit = hitCheck;
                            break;
                        }

                        pierceCount--;
                    }
                }
            }
        }

        //If this hitscan has a visual
        if (spr != null) 
        {   
            //Ray/trail render stuff
            if (renderAsProjectile == false) 
            {
                if (hit.collider == null)
                    spr.size = new Vector2(range, spr.size.y);
                else
                    spr.size = new Vector2(hit.distance, spr.size.y);

            }
            else 
            {
                //Projectile is set to hit target within a set amount of time
                if (visualDuration != 0.0f) {
                    if (hit.collider == null)
                        projectileSpeed = range / visualDuration;
                    else
                        projectileSpeed = hit.distance / visualDuration;
                    //*/

                    projectileSpeed = Mathf.Max(projectileSpeed, range / visualDuration * 0.05f);
                }
            }
        }
    }

    void Update() 
    {
        if (renderAsProjectile == true) 
        {
            transform.Translate(Vector2.right * projectileSpeed * Time.deltaTime);
        }
    }

    void removeSelf() 
    {
        Destroy(gameObject);
    }

    bool actorCheck(RaycastHit2D obj) 
    {
        //Checks if a live actor was hit and hurt it
        combatActor ca = obj.collider.gameObject.GetComponent<combatActor>();

        if (ca != null) 
        {
            if (combatActor.everyActor.Contains(ca))
            {
                //Unneccessary owner check, combatActor.hurt can handle owner == null
                if (owner != null)
                    ca.hurt(damage, owner);
                else 
                    ca.hurt(damage);
                
                //Knockback
                if (ca.rb != null) ca.rb.AddForce(transform.right * damage * knockbackMultiplier);
                return true;
            }
        }

        return false;
    }
}
