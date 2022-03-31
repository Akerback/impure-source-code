using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using BSTools;

public class Ai_wreck : baseAI
{
    //AI for the wreck (zombie)
    Animator anim;
    fireMode attack;
    float attackEnd = 0;

    public bool attacking = false;
    public bool attackReady = false;
    public bool visual = false;

    protected override void Awake()
    {
        attack = GetComponentInChildren<fireMode>();
        anim = GetComponent<Animator>();

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
                if (targetActor != null) {
                    Vector2 vectorToTarget = targetActor.transform.position - transform.position;
                    float dotToTarget = Vector2.Dot(vectorToTarget, transform.right);

                    if ((targetActor.distanceToPoint(transform.position) <= 1f) && (dotToTarget >= 0.25f)) {
                        anim.SetTrigger("fire");
                    }
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

        Invoke("resetPain", painTime);
        attack.interrupt();

        base.pain();
    }

    public override void postMortem(GameObject killer = null)
    {
        endAttack();

        base.postMortem(killer);
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
