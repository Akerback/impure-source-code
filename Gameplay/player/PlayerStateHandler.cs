using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TDSTools;

public class PlayerStateHandler : MonoBehaviour 
{
    public static bool started = false;
    public static PlayerStateHandler MasterHandler;

    public static PlayerPack plrPk;

    void OnEnable() 
    {
        plrPk.plrAct = GetComponentInChildren<playerActor>();
        plrPk.plrInv = GetComponentInChildren<playerInventory>();

        plrPk.plrAct.onDeath.AddListener(playerDeath);

        if (MasterHandler == null) MasterHandler = this;

        started = true;
    }

    public void playerDeath()
    {
        DungeonMaster.restartLevel();
    }

    public void SaveToGlobal(playerState savedState) 
    {
        globalContainer.savedPlayer = savedState;
        globalContainer.hasSavedPlayer = true;
    }

    public void LoadFromGlobal() 
    {
        Load(globalContainer.savedPlayer);
    }

    public void LoadUnPositionalFromGlobal() 
    {
        LoadUnPositional(globalContainer.savedPlayer);
    }

    public void Save(PlayerPack inputPk) 
    {
        if (!started) OnEnable();

        playerState savedState = new playerState();

        savedState.SetPos(transform.position);
        savedState.hp = inputPk.plrAct.getHealth();
        savedState.armor = inputPk.plrAct.getArmor();
        savedState.currentWeapon = inputPk.plrInv.selectedWeapon;
        savedState.hasShotgun = inputPk.plrInv.hasShotgun;
        savedState.hasSmg = inputPk.plrInv.hasSmg;
        savedState.ammoP  = inputPk.plrInv.getAmmo(0);
        savedState.ammoSg = inputPk.plrInv.getAmmo(1);
        savedState.ammoS  = inputPk.plrInv.getAmmo(2);
        savedState.ammoD  = inputPk.plrInv.getAmmo(3);
        
        Debug.Log("SAVED PLAYER STATE: " + savedState.ToString());

        SaveToGlobal(savedState);
    }

    public void Save() 
    {
        if (!started) OnEnable();

        playerState savedState = new playerState();

        savedState.SetPos(transform.position);
        savedState.hp = plrPk.plrAct.getHealth();
        savedState.armor = plrPk.plrAct.getArmor();
        savedState.currentWeapon = plrPk.plrInv.selectedWeapon;
        savedState.hasShotgun = plrPk.plrInv.hasShotgun;
        savedState.hasSmg = plrPk.plrInv.hasSmg;
        savedState.ammoP  = plrPk.plrInv.getAmmo(0);
        savedState.ammoSg = plrPk.plrInv.getAmmo(1);
        savedState.ammoS  = plrPk.plrInv.getAmmo(2);
        savedState.ammoD  = plrPk.plrInv.getAmmo(3);

        Debug.Log("SAVED PLAYER STATE: " + savedState.ToString());

        SaveToGlobal(savedState);
    }

    public void Load(playerState savedState = null) 
    {
        if (!started) OnEnable();

        if (savedState == null) savedState = new playerState();

        //plrPk.plrAct.transform.position = savedState.GetPos();
        plrPk.plrAct.setHealthCapped(savedState.hp); 
        plrPk.plrAct.setArmorCapped(savedState.armor);
        plrPk.plrInv.hasShotgun = savedState.hasShotgun;
        plrPk.plrInv.hasSmg = savedState.hasSmg;
        plrPk.plrInv.setAmmo(savedState.ammoP, ammoType.pistol);
        plrPk.plrInv.setAmmo(savedState.ammoSg, ammoType.shotgun);
        plrPk.plrInv.setAmmo(savedState.ammoS, ammoType.smg);
        plrPk.plrInv.setAmmo(savedState.ammoD, ammoType.dynamite);
        plrPk.plrInv.selectWeapon(savedState.currentWeapon);
        
        Debug.Log("LOADED PLAYER STATE: " + savedState.ToString());

    }

    public void LoadUnPositional(playerState savedState = null) 
    {
        if (!started) OnEnable();

        if (savedState == null) savedState = new playerState();
        
        plrPk.plrAct.setHealthCapped(savedState.hp); 
        plrPk.plrAct.setArmorCapped(savedState.armor);
        plrPk.plrInv.hasShotgun = savedState.hasShotgun;
        plrPk.plrInv.hasSmg = savedState.hasSmg;
        plrPk.plrInv.setAmmo(savedState.ammoP, ammoType.pistol);
        plrPk.plrInv.setAmmo(savedState.ammoSg, ammoType.shotgun);
        plrPk.plrInv.setAmmo(savedState.ammoS, ammoType.smg);
        plrPk.plrInv.setAmmo(savedState.ammoD, ammoType.dynamite);
        plrPk.plrInv.selectWeapon(savedState.currentWeapon);
        
        Debug.Log("LOADED PLAYER STATE: " + savedState.ToString());

    }
}

public class playerState
{
    public float posX;
    public float posY;
    public float posZ;
    public float hp;
    public float armor;
    public int currentWeapon;
    public bool hasShotgun;
    public bool hasSmg;
    public int ammoP, ammoSg, ammoS, ammoD;

    public static readonly playerState defaultState = new playerState();
    public static readonly playerState godState = new playerState(Vector3.zero, true, true, new Vector4(1000, 1000, 1000, 1000), 0, 100, 100); 
    
    public playerState(Vector3 position, bool hasShotgun, bool hasSmg, Vector4 ammo, int currentWeapon, float hp, float armor) 
    {
        this.posX = position.x;
        this.posY = position.y;
        this.posZ = position.z;
        this.hasShotgun = hasShotgun;
        this.hasSmg = hasSmg;
        this.ammoP  = (int)ammo[0];
        this.ammoSg = (int)ammo[1];
        this.ammoS  = (int)ammo[2];
        this.ammoD  = (int)ammo[3];
        this.currentWeapon = currentWeapon;
        this.hp = hp;
        this.armor = armor;
    }

    public playerState() 
    {
        this.posX = 0;
        this.posY = 0;
        this.posZ = 0;
        this.hasShotgun = false;
        this.hasSmg = false;
        this.ammoP  = 12;
        this.ammoSg = 0;
        this.ammoS  = 0;
        this.ammoD  = 0;
        this.currentWeapon = 0;
        this.hp = 100;
        this.armor = 0;
    }

    public void SetPos(Vector3 newPos) 
    {
        posX = newPos.x;
        posY = newPos.y;
        posZ = newPos.z;
    }

    public Vector3 GetPos() 
    {
        return new Vector3(posX, posY, posZ);
    }

    public override string ToString()
    {
        return "Position: " + GetPos().ToString() + " Vitals: " + hp.ToString() + ", " + armor.ToString() + " Arms: " + hasShotgun.ToString() + ", " + hasSmg.ToString() + " SelectedWeapon: " + currentWeapon.ToString() + " Ammo: " + ammoP.ToString() + "pistol " + ammoSg.ToString() + " shotgun " + ammoS.ToString() + " smg " + ammoD + " dynamite";
    }
}
