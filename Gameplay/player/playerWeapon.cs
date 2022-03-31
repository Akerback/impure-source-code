using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerWeapon : MonoBehaviour
{
    public fireMode primaryFire;
    public fireMode secondaryFire = null;
    float nextActionTime = 0;
    public bool dualWield = false;
    //Dual wielded weapons can fire both firemodes at the same time
    GameObject owner;
    public int ammo = 0;
    public int maxAmmo = 1;

    void Start() 
    {
        owner = transform.parent.gameObject;
        
        getFireModes();
    }

    public bool firePrimaryRepeating() 
    {
        if (primaryFire.fullAuto)
            return fire(0);
        else 
            return false;
    }

    public bool fireSecondaryRepeating() 
    {
        //Secondary can be undefined
        if (secondaryFire == null) return false;

        if (secondaryFire.fullAuto)
            return fire(1);
        else 
            return false;
    }
    
    public bool firePrimary()
    {
        return fire(0);
    }

    public bool fireSecondary() 
    {
        if (secondaryFire == null)
            return false;

        return fire(1);
    }

    public bool fire(int modeToFire) 
    {
        //Holds info about firing success
        struct_shotEvent shotState = new struct_shotEvent(false);

        int query = modeToFire / 2;

        //Check if it can be fired now
        if (Time.time < nextActionTime)
            return false;
        if (autoCheck(modeToFire, query) == false) 
            return false;
        if (getAmmoConsumption(modeToFire) > ammo)
            return false;

        //Shotstate is returned by the firemode itself
        switch (query) 
        {
            case 0:
                shotState = primaryFire.fire();
                break;

            case 1:
                if (secondaryFire != null)
                    shotState = secondaryFire.fire();
                break;

            default:
                break;
        }

        nextActionTime = shotState.time;
        ammo -= shotState.value;

        return shotState.succeeded;
    }

    public int getAmmoConsumption(int modeToCheck) 
    {
        int query = modeToCheck / 2;

        switch (query) 
        {
            case 0:
                return primaryFire.ammoConsumption;

            case 1:
                if (secondaryFire != null)
                    return secondaryFire.ammoConsumption;
                return 0;
            default:
                return 0;
        }
    }

    bool autoCheck(int mode) 
    {
        //Even indices are automatic
        if (mode % 2 == 0) return true;

        int modeType = mode / 2;

        switch(modeType) 
        {
            case 0:
                if (primaryFire.fullAuto)
                    return true;
                return false;
            case 1:
                if (secondaryFire.fullAuto)
                    return true;
                return false;
            default:
                return true;
        }
    }

    bool autoCheck(int mode, int modeType) 
    {
        if (mode % 2 == 0) return true;

        switch(modeType) 
        {
            case 0:
                if (primaryFire.fullAuto)
                    return true;
                return false;
            case 1:
                if (secondaryFire.fullAuto)
                    return true;
                return false;
            default:
                return true;
        }
    }
    public void getFireModes() 
    {
        fireMode[] modeContainer = GetComponentsInChildren<fireMode>();
        
        //Only use the two first
        primaryFire = modeContainer[0];
        //Secondary is optional
        if (modeContainer.Length >= 2) secondaryFire = modeContainer[1];
    }

    public bool setAmmoCapped(float newValue) 
    {
        //Returns if any ammo was added
        int cappedValue = Mathf.Min((int)newValue, maxAmmo);

        if (cappedValue == ammo) return false;
        else 
        {
            ammo = cappedValue;
            return true;
        }
    }

    public bool addAmmo(float newValue) 
    {
        return setAmmoCapped(ammo + newValue);
    }

    public void resetCooldown() {
        nextActionTime = Time.time;
        primaryFire.nextUseTime = Time.time;
        if (secondaryFire != null) secondaryFire.nextUseTime = Time.time;
    }

    public bool querySecondAmendment(int modeToFire) 
    {
        float currentTime = Time.time;
        
        int query = Mathf.FloorToInt(modeToFire / 2);
        
        if (modeToFire % 2 != 0) 
        {
            //Cancel if a non-auto mode is trying to be fired automatically
            switch (query) 
            {
                case 0:
                    if (primaryFire.fullAuto == false) 
                        return false;
                    break;
                case 1:
                    if (secondaryFire.fullAuto == false) 
                        return false;
                    break;
                default:
                    break;

            }
        }

        if (currentTime >= nextActionTime)
        {
            //Timing check
            switch (query) 
            {
                case 0:
                    if (currentTime >= primaryFire.getNextActionTime()) 
                        return true;
                    break;
                case 1:
                    if (currentTime >= secondaryFire.getNextActionTime()) 
                        return true;
                    break;
                default:
                    break;

            }
        }

        //Wasn't ready, no shooting here
        return false;
    }
}