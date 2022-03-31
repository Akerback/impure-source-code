using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class crackedWall : combatActor
{
    //An actor that can only be hurt by explosions
    particleDestroyer gibs;
    SpriteRenderer spr;

    protected override void Awake() 
    {
        base.Awake();

        gibs = GetComponentInChildren<particleDestroyer>();
    }

    public override void hurt(float dmg, GameObject attacker = null, int damageType = 0)
    {
        //Can only be hurt by explosives
        if (damageType != 1) dmg = 0;
        
        base.hurt(dmg, attacker, damageType);
    }

    public override void deathEvent(GameObject killer = null)
    {
        hiddenArea secret = GetComponentInChildren<hiddenArea>();

        if (secret != null) secret.reveal();

        if (gibs != null) 
        {
            gibs.part.Emit(20);
            gibs.transform.parent = null;
            gibs.timeToDie = Time.time + 10f;
        }

        base.deathEvent(killer);
    }
}
