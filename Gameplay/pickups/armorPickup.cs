using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class armorPickup : healthPickup
{
    protected override bool pickUp(playerActor playerPickingUp) {
        return playerPickingUp.addArmor(value);
    }
}
