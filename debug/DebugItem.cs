using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugItem : MonoBehaviour
{
    //Only holds a sprite
    protected SpriteRenderer spr;

    protected virtual void Awake() 
    {
        spr = GetComponent<SpriteRenderer>();
    }
}
