using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BSTools;

public class weaponAiming : MonoBehaviour
{   
    //Look at cursor, identical behaviour to faceCursor2D

    public float maxParentOffset = 90.0f;
    public bool doFaceCamera = true;
    public Transform rotationParent;

    void Update()
    {
        if (doFaceCamera) {
            Vector2 positionToLookAt = (Vector2)(cCursor.position - transform.position);
            float angleToLookIn = tools.angleFromVector(positionToLookAt);
            
            //Limit rotation if a parent is present
            if (rotationParent != null) {
                float parentAngle = rotationParent.rotation.eulerAngles.z;

                if (tools.getRelativeRotation(angleToLookIn - parentAngle) > maxParentOffset)
                    angleToLookIn = parentAngle + maxParentOffset;
                else if (tools.getRelativeRotation(angleToLookIn - parentAngle) < -maxParentOffset)
                    angleToLookIn = parentAngle - maxParentOffset;
            }

            //Apply rot
            transform.rotation = Quaternion.Euler(0, 0, angleToLookIn);
        }
    }
}
