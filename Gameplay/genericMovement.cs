using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class genericMovement : MonoBehaviour
{
    //Unused genericMovement the player and ai_simpleBase would've interfaced with

    //--Settings
    public float maxSpeed = 3;
    public float hardSpeedCap = 6;
    public float acceleration = 3;

    //--Components
    Rigidbody2D rb;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
    }

    public Vector2 move(Vector2 input) {
        if (input.magnitude > 1.0f) input.Normalize();

        float speedInInputDirection = Vector2.Dot(rb.velocity, input);

        if (speedInInputDirection < maxSpeed) rb.velocity += input * acceleration * Time.deltaTime;

        if ((hardSpeedCap > maxSpeed) && (rb.velocity.magnitude > hardSpeedCap)) rb.velocity = rb.velocity.normalized * hardSpeedCap;

        return rb.velocity;
    }
}
