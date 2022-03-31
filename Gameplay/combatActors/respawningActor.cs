using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class respawningActor : combatActor
{
    //Immortal actor that respawns after a set amount of time

    public UnityEvent onRespawn;
    public float respawnTime;

    public override void deathEvent(GameObject killer = null)
    {
        //Die, but don't destroy
        onDeath.Invoke();

        isAlive = false;

        //Start the respawn timer
        if (respawnTime >= 0) Invoke("respawn", respawnTime);
    }

    void respawn() 
    {
        onRespawn.Invoke();
        setHealthCapped(maxHp);
        isAlive = true;
    }
}
