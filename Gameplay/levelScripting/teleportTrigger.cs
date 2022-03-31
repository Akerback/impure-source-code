using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class teleportTrigger : teleporter
{
    //The actual trigger that teleports something
    static List<Rigidbody2D> justTeleported = new List<Rigidbody2D>();

    void OnTriggerEnter2D(Collider2D other) {
        Rigidbody2D rb = other.gameObject.GetComponent<Rigidbody2D>();

        //Return if there's no rigidbody2d or the object was just teleported
        if (rb == null) return;
        if (justTeleported.Contains(rb)) return;

        //Prevent it from triggering another tp trigger and send it
        justTeleported.Add(rb);
        teleport(rb);
    } 

    void OnTriggerExit2D(Collider2D other) {
        //Doesn't count as exiting a teleporter if the object is still close to the TP destination
        //This prevents infinite back and forth teleporting between two teleporters
        if ((other.transform.position - teleportDestination.position).magnitude <= 1.0f) return;

        Rigidbody2D rb = other.gameObject.GetComponent<Rigidbody2D>();

        if (rb == null) return;

        //Make it teleportable again
        justTeleported.Remove(rb);
    } 
}
