using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using BSTools;

public class playerPhysicalMovement : MonoBehaviour
{
    //Physics version of unused tank controls
    public float acceleration = 5.0f;
    public float decelleration = 7.5f;
    public float maxSpeed = 10.0f;
    public float maxTurnSpeed = 45.0f;
    public float turnFriction = 0.5f;

    Rigidbody2D rb;
    Vector2 customMovement = Vector2.zero;
    public TextMeshProUGUI speedometer;

    float direction = 0;
    float moveDirection = 0;
    float directionDelta = 0;

    float speed = 0.0f;
    float angularSpeed = 0.0f;

    float previousSpeed = 0.0f;

    Vector3 previousPos;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        previousPos = transform.position;
    }
    
    void OnCollisionHit2D(Collision2D hit) 
    {
        //ContactPoint2D contactPing = hit.GetContact(0);

        Vector2 normal = hit.GetContact(0).normal;

        float dotProduct = Vector2.Dot(normal, rb.velocity);

        if (dotProduct != 0) rb.velocity -= normal * dotProduct; 
    }

    void FixedUpdate() 
    {
        //Input
        float hInput = Input.GetAxis("Horizontal");
        float vInput = Input.GetAxis("Vertical");

        //Get speed and direction
        float dotDir = Vector2.Dot(rb.velocity.normalized, transform.right);

        speed = rb.velocity.magnitude * dotDir;

        float accel = 0;
        
        //Only allow acceleration below speed cap
        if (speed * tools.betterSign(vInput) < maxSpeed) accel = vInput;

        //Apply the accel
        speed += (accel * acceleration) * Time.fixedDeltaTime;
        
        //Stabilize speed around the max speed and standing still
        if ((previousSpeed < maxSpeed) && (speed > maxSpeed)) speed = maxSpeed;
        if ((previousSpeed > 0) && (speed < 0)) speed = 0;

        //Turning
        float currentAngle = transform.rotation.eulerAngles.z;

        float turning = -hInput * maxTurnSpeed * Time.fixedDeltaTime;
        transform.Rotate(turning * Vector3.forward);

        //Force for turning
        Vector2 turnChange = (Vector2)transform.right * speed - rb.velocity;
        rb.velocity += turnChange * turnFriction;

        bool except = false;
        
        float velocityAngle = currentAngle + turning;
        
        Vector2 moveStep = rb.velocity;

        if (speed > 0) velocityAngle = tools.angleFromVector(moveStep);
        if (speed < 0) velocityAngle = tools.angleFromVector(-moveStep);

        if (tools.oppositeSigns(tools.getRelativeRotation(velocityAngle - currentAngle), tools.getRelativeRotation(turning))) except = true;

        if (except == false)
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, velocityAngle));
        
        speedometer.text = speed.ToString();

        previousSpeed = speed;
        previousPos = transform.position;
    }

    public float getSpeed()
    {
        return speed;
    }

    public float getSpeedRatio()
    {
        return speed / maxSpeed;
    }
}
