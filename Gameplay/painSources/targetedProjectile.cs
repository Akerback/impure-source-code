using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class targetedProjectile : bulletMovement
{
    //Unused. Would've been a generic version of dynamite that was AI-useable
    public Vector2 targetPosition = new Vector2(0f, 0f);

    public override void updateFlightTime(float newTime) {
        if (speed <= 0) {
            despawnTime = -1f;
            return;
        }

        flightTime = (targetPosition - (Vector2)transform.position).magnitude / speed;

        despawnTime = startTime + despawnTime;
    }
}
