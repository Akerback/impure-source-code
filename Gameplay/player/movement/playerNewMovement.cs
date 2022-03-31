using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerNewMovement : genericMovement
{
    //Interfaces with genericMovement

    float hInput, vInput;
    void Update()
    {
        hInput = Input.GetAxisRaw("Horizontal");
        vInput = Input.GetAxisRaw("Vertical");

        Vector2 moveVector = new Vector2(hInput, vInput);

        if (moveVector != Vector2.zero) move(new Vector2(hInput, vInput));
    }
}
