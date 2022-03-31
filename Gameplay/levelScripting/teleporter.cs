using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BSTools;

public class teleporter : MonoBehaviour
{
    //Teleports anything with a rigidbody2d

    public Transform teleportDestination;
    public GameObject teleportEffect;
    
    public bool teleport(Rigidbody2D teleportee) {
        //Returns success
        if (teleportDestination == null) return false;
        if (teleportee.bodyType == RigidbodyType2D.Static) return false;

        if (teleportEffect != null) {
            Instantiate(teleportEffect, teleportee.transform.position, tools.rotationUpward);
            Instantiate(teleportEffect, teleportDestination.position, tools.rotationUpward);
        }

        teleportee.transform.position = teleportDestination.position;

        return true;
    }
}
