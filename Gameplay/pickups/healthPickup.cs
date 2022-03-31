using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class healthPickup : valuedPickup
{
    protected override bool pickUp(playerActor playerPickingUp) {
        return playerPickingUp.addHealth(value);
    }
}
