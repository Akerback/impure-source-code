using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TDSTools;

public class keyPickup : pickup
{
    public keys givesKey;

    protected override bool pickUp(playerActor playerPickingUp) {
        return playerPickingUp.addKey(givesKey);
    }
}
