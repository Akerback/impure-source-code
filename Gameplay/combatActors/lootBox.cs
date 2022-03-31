using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lootBox : combatActor
{
    //An actor that only drops something, mainly used for explosive barrels

    public float lootDelay = 0.5f;//Unused delay
    public override void deathEvent(GameObject killer = null)
    {
        spawnLoot(killer);

        base.deathEvent();
    }
}
