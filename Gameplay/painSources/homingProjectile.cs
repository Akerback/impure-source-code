using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BSTools;

public class homingProjectile : bulletMovement
{
    //Unused bullet that turns toward the closest enemy
    public GameObject targetObject;
    public float maxTurnAngle = 5f;
    public bool turnWhileHoming = true;

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        if (targetObject != null) {
            Vector2 vectorToTarget = targetObject.transform.position - transform.position;

            float angularDistance = Vector2.SignedAngle(transform.right, vectorToTarget);
            float turnDirection = Mathf.Sign(angularDistance);

            float maxTurnScaled = maxTurnAngle * Time.deltaTime;

            float newRotation = transform.rotation.eulerAngles.z;

            if (Mathf.Abs(angularDistance) < maxTurnScaled) newRotation = tools.angleFromVector(vectorToTarget);
            else newRotation = newRotation + maxTurnScaled * turnDirection;

            if (turnWhileHoming) {
                Vector3 rCopy = transform.rotation.eulerAngles;
                transform.rotation = Quaternion.Euler(rCopy.x, rCopy.y, newRotation);

                updateDirection();
            }
            else moveVector = tools.vectorFromAngle(newRotation);
        }
    }
}
