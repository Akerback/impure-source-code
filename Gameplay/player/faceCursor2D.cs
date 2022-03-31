using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BSTools;

public class faceCursor2D : MonoBehaviour
{   
    //Used for player weapon aiming

    public float maxParentOffset = 90.0f;
    public bool doFaceCamera = true;

    void Update()
    {   
        if ((!pauseHandler.currentlyPaused) && (doFaceCamera)) {
            Vector2 positionToLookAt = (Vector2)(cCursor.position - transform.position);
            float angleToLookIn = tools.angleFromVector(positionToLookAt);
            
            //Limit turning if a parent is present
            if (transform.parent != null) {
                float parentAngle = transform.parent.eulerAngles.z;

                if (tools.getRelativeRotation(angleToLookIn - parentAngle) > maxParentOffset)
                    angleToLookIn = parentAngle + maxParentOffset;
                else if (tools.getRelativeRotation(angleToLookIn - parentAngle) < -maxParentOffset)
                    angleToLookIn = parentAngle - maxParentOffset;
            }

            //Apply rotation
            transform.rotation = Quaternion.Euler(0, 0, angleToLookIn);
        }

        //Debug.Log(Vector2.Dot(tools.vectorFromAngle(transform.rotation.eulerAngles.z), Vector2.right).ToString());
    }
}
