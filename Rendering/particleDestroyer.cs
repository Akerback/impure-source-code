using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class particleDestroyer : MonoBehaviour
{
    //Holds a particle system and automatically despawns after a set time

    public ParticleSystem part;
    public float timeToDie = -1;
    [Tooltip("If greater than 0 this object will be destroyed after this many seconds.")] public float duration = -1;
    [HideInInspector] public baseAI master;

    void Start() 
    {
        part = GetComponent<ParticleSystem>();
        if (duration >= 0) timeToDie = Time.time + duration;
    }

    void Update() 
    {
        if ((timeToDie >= 0) && (Time.time >= timeToDie))
        {
            Destroy(gameObject);
        }
    }
}
