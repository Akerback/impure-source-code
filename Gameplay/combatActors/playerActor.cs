using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TDSTools;

public class playerActor : combatActor
{
    //--Statics
    public static List<playerActor> everyPlayer = new List<playerActor>();
    public static int playerCount = 0;//Useless, should use everyPlayer.Count instead

    [Range(0.0f, 10.0f)] public float explosionDamageMultiplier = 0.16f;//Players resist explosive damage to prevent oneshots
    //Unused mechanic where players could take back recently taken damage ("rally" from bloodborne)
    public float lifeStealRatio = 0.2f;
    public float healthRestoreFadeTime = 2;
    public float healthRestoreFadeSpeed = 10;

    [SerializeField][Min(0)] float maxArmor = 100;
    [SerializeField] float armor = 0;

    public playerInventory inv;
    public Collider2D world_col;
    float PreviousHealth;
    public float hurtTime = 0;

    protected override void Awake()
    {
        addPlayer();
        inv = GetComponent<playerInventory>();

        //GetComponentInParent places components attached to this obj first
        Collider2D[] carrier = GetComponentsInParent<Collider2D>();
        if (carrier.Length > 0) world_col = carrier[1];//Wanted the parent's collider

        //Debug.Log("PLAYER STARTED");

        base.Awake();
        PreviousHealth = hp;
    }

    protected void Update() 
    {
        float currentTime = Time.time;
        //Unused "rally" mechanic
        if (currentTime - hurtTime >= healthRestoreFadeTime) PreviousHealth = Mathf.Max(hp, PreviousHealth - healthRestoreFadeSpeed * Time.deltaTime);
    }
    
    public override void hurt(float dmg, GameObject attacker = null, int damageType = 0)
    {
        //Hurt modified to support armor
        dmg *= damageMultiplier;
        if (damageType == 1) dmg *= explosionDamageMultiplier;

        float armorDmg = Mathf.Min(armor, dmg);

        armor -= armorDmg;
        dmg -= armorDmg;
        
        if (damageMultiplier > 0.0f) base.hurt(dmg / damageMultiplier, attacker, damageType);
        else onHurt.Invoke();

        hurtTime = Time.time;
    }

    public override void deathEvent(GameObject killer = null)
    {
        //Overriding so the default behaviour, which instantly destroys the object, isn't used
        onDeath.Invoke();
    }

    protected override void OnDestroy() 
    {
        removePlayer();
        base.OnDestroy();
    }

    public float getArmor()
    {
        return armor;
    }

    public float getArmorRatio() 
    {
        //Div by 0 protection
        if (maxArmor == 0.0f) return -1.0f;

        return armor / maxArmor;
    }

    public void addPlayer() 
    {
        everyPlayer.Add(this);
        playerCount++;
    }

    public void removePlayer() 
    {
        //Decrement only on successful removal
        if (everyPlayer.Remove(this)) playerCount--;
    }

    public bool setArmorCapped(float newValue) 
    {
        //Like setHealthCapped it returns if it was successful
        bool result = true;

        //Cap it
        float cappedValue = Mathf.Min(newValue, maxArmor);

        //Failed if there's no change in value
        if (armor == cappedValue) result = false;

        //Update and return
        armor = cappedValue;
        return result;
    }

    public bool addHealth(float value) 
    {
        return setHealthCapped(hp + value);
    }

    public bool bloodForBlood(float value) 
    {
        //Unused "rally" mechanic 
        if (lifeStealRatio <= 0) return false;
        return addHealth(Mathf.Min(PreviousHealth - hp, value * lifeStealRatio));
    }

    public bool addArmor(float value) 
    {
        return setArmorCapped(armor + value);
    }

    //--Inventory interfacing through the player
    public bool addAmmo(float value, ammoType typeOfAmmo) 
    {
        return inv.addAmmo(value, typeOfAmmo);
    }

    public bool addWeapon(ammoType typeOfAmmo) 
    {
        return inv.addWeapon(typeOfAmmo);
    }

    public static bool hasKey(keys whichKey) {
        return everyPlayer[0].inv.keyChain.Contains(whichKey);
    }

    public bool addKey(keys whichKey) {
        return inv.addKey(whichKey);
    }

    public float getVirtualHealthRatio() 
    {
        return PreviousHealth / maxHp;
    }
}
