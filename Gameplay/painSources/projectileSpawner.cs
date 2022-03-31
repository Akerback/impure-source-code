using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class projectileSpawner : painSource
{
    //Versatile projectile spawning, with settings for spread and multi-shots
    public GameObject spawnObject;
    [Min(1)] public uint spawnAmount = 1;
    public float spawnCone = 0f;
    public bool randomizeSpread = false;

    public List<GameObject> spawnAll()
    {
        if (spawnObject == null) return new List<GameObject>();

        //Mostly unused carrier for returning
        List<GameObject> spawnedObjects = new List<GameObject>();

        float minAngle = transform.rotation.eulerAngles.z - spawnCone / 2f;
        float maxAngle = minAngle + spawnCone;

        if (randomizeSpread) {
            for (int i = 0; i < spawnAmount; i++) {
                float angle = Random.Range(minAngle, maxAngle);
                spawnedObjects.Add(createProjectile(angle));
            }
        }
        else {
            //Div by 0 protection
            if (spawnAmount > 1) {
                float stepSize = spawnCone / (spawnAmount - 1);
                float angle = minAngle;

                for (int i = 0; i < spawnAmount; i++) {
                    spawnedObjects.Add(createProjectile(angle));
                    angle += stepSize;
                }
            }
            else {
                //Only one shot going straight forward
                float angle = transform.rotation.eulerAngles.z;
                spawnedObjects.Add(createProjectile(angle));
            }
        }

        //Mostly unused return value. Would've been used to give AI control over homing shots
        return spawnedObjects;
    }

    //Create and impart owner and associates
    GameObject createProjectile(float angle) {
        Vector3 rCopy = spawnObject.transform.rotation.eulerAngles;

        GameObject newProjectile = Instantiate(spawnObject, transform.position, Quaternion.Euler(rCopy.x, rCopy.y, angle));

        impartOwner(newProjectile);
        impartRelatives(newProjectile);

        return newProjectile;
    }
}
