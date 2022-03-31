using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using BSTools;

public class player_freeMovement : MonoBehaviour
{
    //Player movement used in the game
    public aiSound walkingSound;
    float walkingSoundBaseVolume;

    float hInput;
    float vInput;

    public float acceleration = 50.0f;
    public float maxSpeed = 10.0f;
    public float dashPower = 20;
    public float dashCooldown = 1;

    bool dashInput;
    bool canDash = true;

    Rigidbody2D rb;
    public TextMeshProUGUI speedoMeter;

    void Start()
    {
        walkingSound = GetComponent<aiSound>();
        rb = GetComponent<Rigidbody2D>();

        walkingSoundBaseVolume = walkingSound.volume;
        walkingSound.volume = 0;
    }

    void Update()
    {
        //Input
        hInput = Input.GetAxisRaw("Horizontal");
        vInput = Input.GetAxisRaw("Vertical");

        dashInput = Input.GetKey(KeyCode.LeftShift);
    }

    void FixedUpdate() 
    {
        Vector2 inputVector = new Vector2(hInput, vInput);

        //DASH (unused)
        if ((dashInput) && (canDash))
        {
            rb.velocity += inputVector * dashPower;

            canDash = false;

            Invoke("resetDash", dashCooldown);
        }

        Vector2 movementTurner = inputVector * rb.velocity.magnitude - rb.velocity;

        movementTurner *= Time.fixedDeltaTime;

        float newSpeed = (rb.velocity + inputVector * acceleration * Time.fixedDeltaTime).magnitude;

        //Accelerate
        if ((newSpeed <= rb.velocity.magnitude) || (newSpeed < maxSpeed)) 
        {
            rb.velocity += inputVector * acceleration * Time.fixedDeltaTime;
        }
        else rb.velocity += movementTurner;

        //Walk sounds
        if ((walkingSound != null) && (rb.velocity.magnitude >= 0.2f * maxSpeed)) 
        {
            walkingSound.volume = walkingSoundBaseVolume * rb.velocity.magnitude / maxSpeed;
        }
    }

    void resetDash() 
    {
        canDash = true;
    }
}
