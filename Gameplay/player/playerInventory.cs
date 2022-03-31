using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TDSTools;

public class playerInventory : MonoBehaviour
{
    public int selectedWeapon = 0;
    //Chosen weapon index
    //--Fields for weapon definitions
    public playerWeapon[] wep;
    public playerWeapon melee;
    public static readonly int[] maxAmmo = {
        36,//Magnum
        20,//Shotgun
        200,//SMGs
        10//Dynamite
    };

    //Keys in the player inv
    public List<keys> keyChain = new List<keys>();
    public bool hasShotgun, hasSmg;
    //Weapons that have to be found
    [SerializeField] float punchForce = 10;
    //Forward momentum of punching

    //--Components
    Animator anim;
    SpriteRenderer spr;
    Rigidbody2D rb;

    void Awake() 
    {
        anim = GetComponent<Animator>();

        spr = GetComponent<SpriteRenderer>();

        rb = GetComponentInParent<Rigidbody2D>();

        selectWeapon(0);
    }

    void Update() 
    {
        playerWeapon currentWeapon = wep[selectedWeapon];

        //Input
        bool mPressL = Input.GetMouseButtonDown(0);
        bool mHoldL  = Input.GetMouseButton(0);
        bool mPressR = Input.GetMouseButtonDown(1);
        bool mHoldR  = Input.GetMouseButton(1);

        //Pick what to shoot based on input
        //No weapons are dual wielded in the final version
        if (mPressL) {
            shoot(0, currentWeapon);
            if (wep[selectedWeapon].dualWield) shoot(2, currentWeapon);
        }
        if (mHoldL) {
            shoot(1, currentWeapon);
            if (wep[selectedWeapon].dualWield) shoot(3, currentWeapon);
        }

        if (mPressR) {
            shoot(0, melee);
            if (wep[selectedWeapon].dualWield) shoot(2, melee);
        }
        if (mHoldR) {
            shoot(1, melee);
            if (wep[selectedWeapon].dualWield) shoot(3, melee);
        }

        /* Unused firing version where weapons had 2 fire modes
        if (wep[selectedWeapon].dualWield == false) {
            //Weapons not dual wielded only allow one firemode at a time
            if (mHoldR) 
            {
                //Alt-fire
                if (mPressL) shoot(2);
                if (mHoldL) shoot(3);
            }
            else 
            {
                //Primary fire
                if (mPressL) shoot(0);
                if (mHoldL) shoot(1);
            }
        }
        else
        //Allow both at the same time (dual smgs)
        {
            //Check right
            if (mPressR) shoot(2);
            if (mHoldR) shoot(3);
            
            //Check left
            if (mPressL) shoot(0);
            if (mHoldL) shoot(1);
            
            //Shooting both at the same time?
            if (mHoldL && mHoldR) anim.SetBool("dualShot", true);
            else anim.SetBool("dualShot", false);
        }
        Flip sprite if player is shooting both smgs
        spr.flipY = mHoldR && (selectedWeapon == 2);
        //*/

        //Weapon switching
        if (Input.GetKeyDown(KeyCode.Alpha1)) selectWeapon(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) selectWeapon(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) selectWeapon(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) selectWeapon(3);
        if (Input.GetKeyDown(KeyCode.Alpha5)) selectWeapon(4);
    }

    //Add to the arsenal
    public bool addWeapon(ammoType typeOfWeapon) 
    {
        int index = (int)typeOfWeapon;
        bool success = false;

        //Only SG and SMG need to be looted, ignore other ammo types
        switch (index) 
        {
            case 1:
                if (!hasShotgun) success = true;
                hasShotgun = true;
                break;
            case 2:
                if (!hasSmg) success = true;
                hasSmg = true;
                break;
            default:
                break;
        }

        return success;
    }

    public bool addKey(keys whichKey) {
        //Add a key
        if (!keyChain.Contains(whichKey)) {
            keyChain.Add(whichKey);
            return true;
        }
        return false;
    }

    //Deals with weapon switching
    public void selectWeapon(int index) 
    {
        int lastWep = selectedWeapon;

        if (!isValidWeaponIndex(index)) return;

        //Debug cannon
        if (wep[index] != null) 
            if (wep[index].gameObject.tag == "debugObject")
                {
                    selectedWeapon = index;
                    return;
                }

        //If switching to a new wep
        if (index != lastWep) 
        {
            //Reset triggers
            anim.ResetTrigger("shotWeapon");
            anim.ResetTrigger("shotPrimary");
            anim.ResetTrigger("shotSecondary");

            switch (index) 
            {
                case 0:
                    //Switch it
                    selectedWeapon = 0;
                    break;
                case 1:
                    //SG & SMG check if the player has them
                    if (hasShotgun) selectedWeapon = 1;
                    break;
                case 2:
                    if (hasSmg) selectedWeapon = 2;
                    break;
                case 3:
                    //Dynamite needs ammo
                    if (wep[3].ammo > 0) selectedWeapon = 3;
                    break;
                case 4:
                    //Unused throwing axes also need ammo
                    if (wep[4].ammo > 0) selectedWeapon = 4;
                    break;
                default:
                    break;
            }

            //A new weapon was successfully selected
            if (selectedWeapon != lastWep) {
                wep[selectedWeapon].resetCooldown();

                anim.SetTrigger("swappedWeapon");
            }
        }

        //Signal which weapon is held now
        anim.SetInteger("currentWeapon", selectedWeapon);
    }

    bool shoot(int mode, playerWeapon weapon) 
    {
        if (pauseHandler.currentlyPaused) return false;

        bool successCheck = false;

        //Check if the weapon can be fired with this mode then try to fire it
        if (weapon.querySecondAmendment(mode))
        {
            successCheck = weapon.fire(mode);
        }

        //If the shot was successful
        if (successCheck) 
        {
            //Forward momentum from punching
            if (weapon == melee) {
                rb.velocity += (Vector2)transform.right * punchForce;
                anim.SetTrigger("punching");
            }
            else {
                //Set anim trigger for primary or secondary
                if ((mode == 0) || (mode == 1)) 
                    anim.SetTrigger("shotPrimary");
                else {
                    //Actually unused. Secondary fire was changed to melee for all weapons
                    anim.SetTrigger("shotSecondary");
                }
            }
            
            //I think this caused some animation bug?
            anim.SetTrigger("shotWeapon");
        }

        return successCheck;
    }

    //Interface with currently held weapon
    public int getCurrentAmmo() 
    {
        if (wep[selectedWeapon] != null) 
        {
            return wep[selectedWeapon].ammo;
        }

        return -1;
    }

    public int getAmmo(int weaponIndex) 
    {
        if (wep[weaponIndex] != null) return wep[weaponIndex].ammo;
        return -1;
    }

    public bool setAmmo(float value, ammoType typeOfAmmo) {
        if (isValidWeaponIndex((int)typeOfAmmo)) return wep[(int)typeOfAmmo].setAmmoCapped(value);
        else return false;
    }

    public bool addAmmo(float value, ammoType typeOfAmmo) 
    {
        if (isValidWeaponIndex((int)typeOfAmmo)) return wep[(int)typeOfAmmo].addAmmo(value);
        else return false;
    }

    //Range check for selected weapon
    public bool isValidWeaponIndex(int index) 
    {
        if (index >= 0)
            if (index < 4)
                return true;
        return false;
    }
}
