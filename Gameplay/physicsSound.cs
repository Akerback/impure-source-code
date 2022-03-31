using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class physicsSound : aiSound
{
    //Makes AI hear physics objects sliding around

    Rigidbody2D rb;
    float scaledMass = 1f;
    float baseVolume = 1f;
    protected override void Start()
    {
        base.Start();

        rb = GetComponent<Rigidbody2D>();
        baseVolume = volume;
        
        updateMass();
    }

    void Update() {
        float metersPerSecond = rb.velocity.magnitude / 3f;

        volume = metersPerSecond * scaledMass * baseVolume;
    }

    public void updateMass() {
        scaledMass = rb.mass / 10f;
    }
}
