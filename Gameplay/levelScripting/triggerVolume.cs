using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using BSTools;

public class triggerVolume : MonoBehaviour
{
    //Just a trigger volume, but also has an option to activate based on LoS

    public UnityEvent onPlayerActivate;
    public UnityEvent whilePlayerActivating;
    public UnityEvent onPlayerDeactivate;
    public dynamicSecret.revealModes triggerMode = dynamicSecret.revealModes.collision;

    Collider2D playerCollider;
    playerActor playerInside;
    SpriteRenderer spr;

    void Start() 
    {
        spr = GetComponent<SpriteRenderer>();
        if (spr != null) 
        {
            spr.enabled = false;
        }
    }

    void Update() {
        if (triggerMode == dynamicSecret.revealModes.centerVisible) {
            //Calls the same events as collision mode, but only if there's LoS
            if (tools.minimalVisionCheck(playerActor.everyPlayer[0].transform.position, transform.position)) {
                //If player enters LoS, call the event
                if (playerInside == null) onPlayerActivate.Invoke();
                playerInside = playerActor.everyPlayer[0];
            }
            else {
                //If player exits LoS, call the event
                if (playerInside != null) onPlayerDeactivate.Invoke();
                playerInside = null;
            }
            //As long as the player can see it, call the activating event
            if (playerInside != null) whilePlayerActivating.Invoke();
        }
    }

    void OnTriggerEnter2D(Collider2D hit) 
    {
        if (triggerMode == dynamicSecret.revealModes.collision) {
            //Debug.Log("Halt! Who goes there?");
            playerActor playerCheck = hit.gameObject.GetComponent<playerActor>();

            if (playerCheck != null) 
            {
                playerCollider = hit;
                playerInside = playerCheck;
                onPlayerActivate.Invoke();
            }
        }
    }

    void OnTriggerStay2D(Collider2D hit) 
    {
        if (triggerMode == dynamicSecret.revealModes.collision) {
            //Debug.Log("Leave! You're trespassing");
            if (playerInside != null) 
            {
                whilePlayerActivating.Invoke();
            }
        }
    }

    void OnTriggerExit2D(Collider2D hit) 
    {
        if (triggerMode == dynamicSecret.revealModes.collision) {
            //Debug.Log("Now be on your way");
            playerActor playerCheck = hit.gameObject.GetComponent<playerActor>();

            if (playerCheck != null) 
            {
                playerCollider = null;
                playerInside = null;
                onPlayerDeactivate.Invoke();
            }
        }
    }
}
