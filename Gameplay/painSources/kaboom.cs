using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BSTools;

public class kaboom : painSource
{
    //An explosion!

    public float startDelay = 0.0f;
    public float maxDamage = 0;
    public float force;
    [Range(0, 10)] public float ownerDamage = 0.5f;

    Collider2D col;
    bool active = false;

    void Start()
    {
        col = GetComponent<Collider2D>();

        Invoke("goBoom", startDelay);
    }

    void goBoom() 
    {
        List<Collider2D> hits = new List<Collider2D>();

        cameraTools.positionalShake(force / 10000, transform.position);
        
        //Get everything inside collider
        col.OverlapCollider(tools.emptyContactFilter, hits);

        foreach (Collider2D result in hits) 
        {
            //Containers
            Rigidbody2D rb;
            combatActor ca;

            Vector2 meToYou = result.transform.position - transform.position;

            //Raycast
            if (tools.visionCheck(result, transform.position))
            {
                float distance = Mathf.Max(1f, meToYou.magnitude - result.bounds.extents.magnitude);
                float distanceRecip = Mathf.Pow(1f / distance, 0.5f);

                //Get components
                rb = result.GetComponent<Rigidbody2D>();
                ca = result.GetComponent<combatActor>();

                if (rb != null)
                    //Knockback for physics objects
                    rb.AddForce(meToYou.normalized * distanceRecip * force);
                if (ca != null)
                    //Damage for actors
                    if (combatActor.everyActor.Contains(ca))
                    {
                        if (friendlyCheck(ca.gameObject)) 
                            ca.hurt(maxDamage * distanceRecip * ownerDamage, owner, 1);
                        else 
                            ca.hurt(maxDamage * distanceRecip,               owner, 1);
                    }
            }
        }
    }
}
