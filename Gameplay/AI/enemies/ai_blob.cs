using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ai_blob : ai_simpleBase
{
    //Unused enemy AI
    [HideInInspector] public Animator anim;
    [HideInInspector] public genericMovement move;
    public float speedScaler;

    protected override void Start()
    {
        base.Start();

        anim = GetComponentInParent<Animator>();
        move = GetComponentInParent<genericMovement>();
    }

    protected override void Update()
    {
        base.Update();
    }
}
