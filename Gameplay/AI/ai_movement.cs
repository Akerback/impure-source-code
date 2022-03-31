using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BSTools;
using TDSTools;

[RequireComponent(typeof(baseAI))]
public class ai_movement : MonoBehaviour
{
    //--Components
    protected Rigidbody2D rb;
    protected baseAI brain;

    //--Settings
    public float maxSpeed = 4;
    public float acceleration = 10;
    public float speedMultiplier = 1.0f;
    [Tooltip("How close an AI has to get to its wander target before it's considered visited.")][Min(0.1f)] public float targetRadius = 1;

    //--Internals
    [HideInInspector]public Vector2 walkingTarget;
    protected Vector2 movementDirection = Vector2.zero;
    Vector2 wanderDirection = Vector2.zero;
    public aiTarget currentTarget;
    public aiTarget movementTarget;
    float contactTime = 0;

    void Start() 
    {
        rb = GetComponent<Rigidbody2D>();
        brain = GetComponent<baseAI>();

        currentTarget = new aiTarget(transform.position);
    }

    public void wander(float amount = 1) 
    {
        evaluateGoals();

        //Avoid walls
        if (!tools.rangedVisionCheck(transform.position, wanderDirection, brain.simpleRadius() + 1f)) wanderDirection = (tools.randomChance(0.5f) ? 1f : -1f) * Vector2.Perpendicular(wanderDirection);

        Vector2 inputVector = amount * wanderDirection.normalized;

        move(inputVector);
    }

    protected void move(Vector2 inputVector) {
        float moveSpeed = rb.velocity.magnitude;

        //--Copy of player movement--------------------------------------------------------------------------------------------------

        Vector2 movementTurner = inputVector * rb.velocity.magnitude - rb.velocity;

        movementTurner *= Time.deltaTime;

        float newSpeed = (rb.velocity + inputVector * acceleration * Time.fixedDeltaTime).magnitude;

        //Accelerate
        if ((newSpeed <= rb.velocity.magnitude) || (newSpeed < maxSpeed * speedMultiplier)) 
        {
            rb.velocity += inputVector * acceleration * Time.fixedDeltaTime;
        }
        else rb.velocity += movementTurner;
    }

    public void brake(float amount = 1) 
    {
        rb.velocity -= rb.velocity.normalized * acceleration * amount * Time.deltaTime;
    }

    void evaluateGoals() 
    {
        //Inform the AI when the current movement target has been reached
        if (currentTarget.isNotChecked) {
            if ((wanderDirection == Vector2.zero) || (tools.minimalVisionCheck(transform.position, currentTarget.pos))) {
                updateTarget();
            }
            if ((currentTarget.pos - transform.position).magnitude <= targetRadius) currentTarget.isNotChecked = false;
        }
    }

    void updateTarget() {
        wanderDirection = (currentTarget.pos - transform.position).normalized;
    }

    public aiTarget petitionNewTarget(aiTarget newTarget) {

        if (newTarget.isVisual) {
            currentTarget = newTarget;
        }
        //Audio targets only take priority over visited visual targets and less interesting audio targets
        else if (newTarget.interest > currentTarget.interest) currentTarget = newTarget;

        if (currentTarget == newTarget) updateTarget();
        return currentTarget;
    }

    public void decayInterest(float amount) {
        currentTarget.interest -= amount * Time.deltaTime;
    }
}
