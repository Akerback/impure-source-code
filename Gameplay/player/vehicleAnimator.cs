using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class vehicleAnimator : MonoBehaviour
{
    //Unused, dealt with vehicle animation
    playerControls main;
    Animator anim;
    bool drifting;
    void Start()
    {
        main = GetComponent<playerControls>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        bool moving = (main.getSpeed() != 0);
        drifting = ((Input.GetAxis("Horizontal") != 0.0f) && (Input.GetAxis("Vertical") == 0.0f) && (Mathf.Abs(main.getSpeedRatio()) > 0.5f));
        //Set moving parameter
        anim.SetBool("moving", moving);

        anim.SetBool("drifting", drifting);
    }

    public bool getDriftState() 
    {
        return drifting;
    }
}
