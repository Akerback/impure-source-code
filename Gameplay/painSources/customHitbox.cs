using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using BSTools;

public class customHitbox : painSource
{
    //Melee is just guns shooting hitboxes

    public float damage = 1;
    public bool explosive;
    //Is this explosive?
    public bool directedKnockback = false;
    public Vector2 knockback;
    public float force = 500;
    //Knockback force
    Collider2D col;
    List<Collider2D> hitList;
    //Lista över träffade objekt
    public AudioClip hitSound;
    AudioSource audioS;
    public float visualTime;

    void Start()
    {
        col = GetComponent<Collider2D>();
        audioS = GetComponent<AudioSource>();

        hitList = new List<Collider2D>();

        //Melee hits on frame 0
        hit();
        Invoke("gameEnd", visualTime);
    }

    void hit() 
    {
        //Debug.Log("Hitbox was Created");
        //Calculate knockback
        Vector2 actualKnockback = tools.vectorToLocal(knockback, transform);

        //Get everything that was hit
        col.OverlapCollider(tools.emptyContactFilter, hitList);

        int successCount = 1;

        foreach (Collider2D hit in hitList) 
        {
            //Debug.Log("Hitbox entered hitcheck loop");

            //Friendly check
            bool relativeFound = false;

            if (hit.gameObject == owner) 
                relativeFound = true;
            else 
            {
                foreach (GameObject parent in associates) 
                {
                    if (hit.gameObject == parent) 
                    {
                        relativeFound = true;
                        break;
                    }
                }
            }

            //Skip to the next if the object is friendly
            if (relativeFound) continue;

            //Fetch rigidbody and actor
            Rigidbody2D rb = hit.gameObject.GetComponent<Rigidbody2D>();
            combatActor ca = hit.gameObject.GetComponent<combatActor>();

            //No further logic if there is no rb or actor
            if ((rb == null) && (ca == null)) continue;

            //Apply knockback as long as it's a physics object
            if ((knockback != Vector2.zero) && (rb != null)) 
            {
                //Force with minor randomization
                rb.AddForce(actualKnockback * force * Random.Range(0.9f, 1.0f));
            }

            //Hurt actors
            if (ca != null) 
            {
                if (combatActor.everyActor.Contains(ca))
                {
                    if (explosive) {
                        ca.hurt(damage, owner, 1);

                        //Screen shake
                        cameraTools.shake += force / 50000 / successCount;
                        successCount++;
                    }
                    else           
                        ca.hurt(damage, owner, 2);
                    }
            }
        }
        
        //Play hitsound with volume based on amount of actors hit
        if (successCount > 1)
            if ((audioS != null) && (hitSound != null)) audioS.PlayOneShot(hitSound, Mathf.Log10(successCount) + 0.5f);

        //Debug.Log("Hitbox exited hitcheck loop");
    }

    public void gameEnd() 
    {
        Destroy(gameObject);
    }
}
