using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pickup : MonoBehaviour 
{
    //Have the player repeatedly try to pick it up while inside collision
    protected void OnTriggerStay2D(Collider2D hit) {
        playerActor plr = playerCheck(hit);

        if (plr != null)
        {
            //Destroy once picked up
            if (pickUp(plr)) Destroy(gameObject);
        }
    }

    public playerActor playerCheck(Collider2D hit) 
    {
        if (hit.gameObject.tag == "Player") {
            playerActor plr = hit.gameObject.GetComponentInChildren<playerActor>();
            if (plr != null) 
            {
                return plr;
            }
        }
        return null;
    }

    protected virtual bool pickUp(playerActor playerPickingUp) {return true;}
}