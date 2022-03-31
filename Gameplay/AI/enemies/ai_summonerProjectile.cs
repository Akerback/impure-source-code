using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BSTools;

public class ai_summonerProjectile : baseAI
{
    //AI for an unused "lost soul"-like enemy
    public Animator anim;
    public List<SpriteRenderer> sprs = new List<SpriteRenderer>();

    protected override void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        SpriteRenderer[] container = GetComponentsInChildren<SpriteRenderer>();
        
        foreach (SpriteRenderer spr in container) sprs.Add(spr);

        base.Awake();
    }

    protected override void Update() {
        transform.rotation = Quaternion.Euler(Vector3.zero);
        evaluateTime();

        if (isAlive) {
            move.wander();
            float distToTarget = (transform.position - targetActor.transform.position).magnitude;

            anim.SetFloat("speed", rb.velocity.magnitude);
            anim.SetFloat("horisontalSpeed", rb.velocity.x > 0 ? 1f : -1f);
            anim.SetBool("closeToTarget", (distToTarget <= 4));

            if (distToTarget <= 2) deathEvent();

            foreach (SpriteRenderer spr in sprs) {
                spr.transform.rotation = tools.noRotation;
            }
        }
    }

    public override void postMortem(GameObject killer = null) {
        removeActor();

        audioS.PlayOneShot(deathSound);

        spawnLoot();

        //Disable collision and hide the enemy
        gameObject.layer = 8;
        foreach (SpriteRenderer spr in sprs) {
            spr.enabled = false;
        }

        var partHolder = bleed.part.emission;
        partHolder.rateOverTime = 0;
        bleed.timeToDie = Time.time + 5f;

        rb.bodyType = RigidbodyType2D.Static;
    }
}
