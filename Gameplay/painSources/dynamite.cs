using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BSTools;

public class dynamite : MonoBehaviour
{
    Animator anim;
    //Dynamite behaviour is in its drop shadow, the sticks are purely visual
    bulletMovement move;
    //The dynamite is essentially a bullet with custom targetting

    Vector2 origin;
    [SerializeField] float maxRange = 10.0f;
    float speed;

    Vector2 moveDir;
    
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        move = GetComponent<bulletMovement>();

        //Copy start point
        origin = transform.position;

        //Hardcoded to target the mouse cursor
        Vector2 target = cCursor.position;

        //Do maths so the dynamite sticks arc right
        Vector2 vectorToTarget = target - origin;

        moveDir = vectorToTarget.normalized;

        //Distance travelled
        float moveDistance = vectorToTarget.magnitude;

        //Distance limit
        if (moveDistance > maxRange) 
        {
            //Cap the range
            vectorToTarget = moveDir * maxRange;
            moveDistance = maxRange;
        }

        //Copy bullet speed and override it
        speed = move.speed;
        move.speed = 0;

        //Reset rotation
        transform.rotation = Quaternion.Euler(Vector3.zero);

        //Time to calculate animation parameters
        float calculatedArc, calculatedDuration;

        calculatedArc = moveDistance / maxRange;//Value for the animation's arc height
        calculatedDuration = speed / moveDistance * 2;//Value for the animation's arc time

        //Set the values
        anim.SetLayerWeight(1, calculatedArc);
        anim.SetFloat("arcMultiplier", calculatedDuration);

        //Update flight time to this so it expires when the arc animation lands again
        move.updateFlightTime(2 / calculatedDuration);
    }

    void Update()
    {
        //Move it, could also just give the bulletmovement control back
        transform.Translate((Vector3)moveDir * speed * Time.deltaTime);
    }
}
