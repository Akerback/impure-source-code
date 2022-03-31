using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TDSTools;

public class ammoPickup : healthPickup
{
    public ammoType typeOfAmmo;

    protected override bool pickUp(playerActor playerPickingUp) {
        return playerPickingUp.addAmmo(value, typeOfAmmo);
    }
}
