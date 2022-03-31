using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BSTools;

[ExecuteInEditMode]
[RequireComponent(typeof(SpriteRenderer))]
public class debugArrow : DebugItem
{
    //Visual arrow for debugging
    public Vector2 arrowVector;

    protected override void Awake() 
    {
        base.Awake();
        spr.drawMode = SpriteDrawMode.Sliced;
    }

    void Update() 
    {
        spr.size = new Vector2(arrowVector.magnitude, spr.size.y);

        transform.rotation = Quaternion.Euler(new Vector3(0, 0, tools.angleFromVector(arrowVector)));
    }
}
