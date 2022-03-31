using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using BSTools;
using TDSTools;

public class ai_SgCultist : baseAI
{
    //AI for the shotgunner
    Animator anim;
    weaponAiming aim;
    fireMode attack;
    float attackEnd = 0;
    public bool attacking = false;
    public bool attackReady = false;
    public bool visual = false;

    protected override void Awake()
    {
        attack = GetComponentInChildren<fireMode>();
        anim = GetComponent<Animator>();
        aim = GetComponent<weaponAiming>();

        base.Awake();
    }

    protected override void Update()
    {
        evaluateTime();

        if (isAlive) {
            float currentTime = Time.time;
            struct_shotEvent attackResult = new struct_shotEvent(false);
            base.Update();

            if (targetActor != null) visual = perceptionCheck(targetActor);
            
            if (!attacking) {
                move.wander();
                if (rb.velocity.magnitude > 0) transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, tools.angleFromVector(rb.velocity));

                if (visual) 
                {
                    Vector2 vectorToTarget = targetActor.transform.position - transform.position;

                    Vector3 rotCopy = transform.rotation.eulerAngles;

                    //Face the target
                    transform.rotation = Quaternion.Euler(rotCopy.x , rotCopy.y, tools.angleFromVector(vectorToTarget));

                    anim.SetTrigger("ready");
                }
            }
            else move.brake();
        }
    }

    protected override void awaken()
    {
        anim.SetTrigger("awoken");

        base.awaken();
    }

    public override void pain()
    {
        anim.SetTrigger("pained");

        attack.interrupt();

        base.pain();
    }

    public override void postMortem(GameObject killer = null)
    {
        bleed.part.Emit(Mathf.FloorToInt(maxHp));
        anim.SetBool("dead", true);
        
        if (stagger != null) 
        {
            var particleEmitter = stagger.part.emission;
            particleEmitter.enabled = false;
        }

        spawnLoot();

        //Debug.Log(gameObject.ToString() + ": I die");
        //Death sound
        if (audioS != null) 
        {
            //Debug.Log(gameObject.ToString() + ": I Hurt");
            //Randomized pitch
            audioS.pitch = basePitch + Random.Range(-pitchVariation / 2, pitchVariation / 2);
            //Play it
            audioS.PlayOneShot(deathSound, 1f);
        }

        //Disable AI and collision
        gameObject.layer = 8;
        spr.sortingOrder = constantsContainer.layer_detail;
        removeActor();
    }

    public void readyAttack() 
    {
        if (visual) anim.SetTrigger("fire");
    }

    public void startAttack() 
    {
        attack.fire();
        attacking = true;
    }

    public void endAttack() 
    {
        attack.interrupt();
        attacking = false;
    }
}
