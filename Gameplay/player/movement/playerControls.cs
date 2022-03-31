using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using BSTools;

public class playerControls : MonoBehaviour
{
    //Unused tank controls for the player
    public float acceleration = 5.0f;
    public float decelleration = 7.5f;
    public float maxSpeed = 10.0f;
    public float maxTurnSpeed = 45.0f;
    public float turnLagRatio = 0.5f;

    Rigidbody2D rb;
    Vector2 customMovement = Vector2.zero;
    vehicleAnimator sm;
    public TextMeshProUGUI speedometer;

    float direction = 0;
    float moveDirection = 0;
    float directionDelta = 0;

    float speed = 0.0f;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sm = GetComponent<vehicleAnimator>();
    }
    
    void OnCollisionEnter2D(Collision2D hit) 
    {
        speed = rb.velocity.magnitude * Mathf.Sign(speed);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //customMovement is calculated in Update
        rb.velocity = customMovement;
    }

    void Update() 
    {
        //Input
        float hInput = Input.GetAxis("Horizontal");
        float vInput = Input.GetAxis("Vertical");

        //Acceleration and speed cap
        if (speed < maxSpeed) 
        {
            speed += vInput * Time.deltaTime * acceleration;
            if (Mathf.Abs(speed) > maxSpeed) speed = maxSpeed * Mathf.Sign(speed);
        }

        if (((tools.betterSign(vInput) != tools.betterSign(speed)) || (speed > maxSpeed))/* && (sm.getDriftState() == false)*/) speed -= Mathf.Min(decelleration * Time.deltaTime, Mathf.Abs(speed)) * Mathf.Sign(speed);
        
        //Turning
        float turning = -hInput * Time.deltaTime;
        transform.Rotate(maxTurnSpeed * turning * Vector3.forward);
        direction = transform.rotation.eulerAngles.z;

        //A second vector lags behind, turn it toward the current one
        directionDelta = moveDirection - direction;
        while (Mathf.Abs(directionDelta) > 180) directionDelta -= tools.betterSign(directionDelta) * 360;

        //The lagging vector turns slower at higher speeds
        float speedTurnPenalty = 1 - (Mathf.Abs(speed) / maxSpeed);
        //100% control while standing still
        if (speed == 0.0f) moveDirection = direction;
        //Otherwise limited
        else moveDirection -= Mathf.Min(Mathf.Abs(directionDelta), maxTurnSpeed * turnLagRatio * Time.deltaTime) * tools.betterSign(directionDelta);

        //Finally figure out what the move vector is
        customMovement = tools.vectorFromAngle(moveDirection) * speed;

        //Debugging speedometer
        speedometer.text = speed.ToString();
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
