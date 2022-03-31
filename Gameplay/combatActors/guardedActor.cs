using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TDSTools;

public class guardedActor : combatActor
{
    //An actor that can only be hurt if the player has a certain key
    public keys requiredKey = keys.none;

    public override void hurt(float dmg, GameObject attacker = null, int damageType = 0)
    {
        if ((requiredKey == keys.none) || (playerActor.everyPlayer[0].gameObject == attacker)) {
            dmg = playerActor.hasKey(requiredKey) ? dmg : 0;
        }
        else dmg = 0;

        base.hurt(dmg, attacker, damageType);
    }
}
