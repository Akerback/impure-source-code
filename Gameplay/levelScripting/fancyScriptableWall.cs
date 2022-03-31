using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TDSTools;

public class fancyScriptableWall : scriptableObject
{
    //Scriptable wall with effects for appearing and disappearing

    public GameObject appearEffect;
    public GameObject disappearEffect;

    public override void boolWallState(int newState)
    {
        bool appearing = false;

        switch (newState) {
            default:
                if (spr.enabled) appearing = false;
                else appearing = true;
                break;
            case 0:
                appearing = false;
                break;
            case 1:
                appearing = true;
                break;
        }

        if (appearing) 
            if (appearEffect != null) Instantiate(appearEffect, transform.position, transform.rotation);
        else 
            if (disappearEffect != null) Instantiate(disappearEffect, transform.position, transform.rotation);

        base.boolWallState(newState);
    }
}
