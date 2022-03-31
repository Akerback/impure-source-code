using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class debug_visual : DebugItem
{
    public void setTransform(Transform newTransform) 
    {
        transform.position   = newTransform.position;
        transform.rotation   = newTransform.rotation;
        transform.localScale = newTransform.localScale;
    }
}
