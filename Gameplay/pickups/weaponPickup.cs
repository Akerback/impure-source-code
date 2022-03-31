using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TDSTools;

public class weaponPickup : ammoPickup
{   
    //Acts like a ammo pickup if the player already has the weapon
    protected override bool pickUp(playerActor playerPickingUp) {
        bool addedAmmo = playerPickingUp.addAmmo(value, typeOfAmmo);
        bool addedWeapon = playerPickingUp.addWeapon(typeOfAmmo);

        return (addedAmmo || addedWeapon);
    }
}
