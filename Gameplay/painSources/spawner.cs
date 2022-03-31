using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BSTools;

public class spawner : painSource
{
    //Repeatedly spawns something
    public GameObject spawnObject;
    public float timeBetweenSpawns = 1.0f;
    protected float nextSpawnTime = 0.0f;

    protected virtual void Update()
    {
        if (spawnObject != null) {
            float currentTime = Time.time;

            if (currentTime >= nextSpawnTime) {
                spawn();

                nextSpawnTime = currentTime + timeBetweenSpawns;
            }
        }
    }

    //Deal with spawning, also assigns owner and relatives
    public virtual void spawn() {
        GameObject newObject = Instantiate(spawnObject, transform.position, transform.rotation);
        impartOwner(newObject);
        impartRelatives(newObject);
    }
}
