using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TDSTools;

public static class DungeonMaster
{
    //Deals with scene loading
    public static void loadScene(string nextScene) 
    {
        globalContainer.lastLevel = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(nextScene, LoadSceneMode.Single);
    }

    public static void restartLevel() 
    {
        loadScene(SceneManager.GetActiveScene().name);
    }
}

public class globalContainer : MonoBehaviour
{
    //Deals with globals
    const bool cheatsEnabled = true;
    static readonly string ce_message_start = "CHEAT ENGINE: ";

    public static globalContainer masterContainer;
    public static playerState savedPlayer;
    public static string lastLevel;
    public static bool hasSavedPlayer = false;
    
    void Start() 
    {
        if (masterContainer != null) Destroy(this);
        else {
            masterContainer = this;
            
            DontDestroyOnLoad(gameObject);

            Debug.Log("globalContainer lives on the object: " + gameObject.ToString());
        }
    }

    void Update() 
    {
        if (cheatsEnabled) handleCheats();
    }

    void handleCheats() {
        if (Input.GetKey(KeyCode.C)) {
            bool ce_addedAmmo = false;
            bool ce_addedWeapons = false;
            bool ce_changedNoClip = false;

            Collider2D playerWorldCol = playerActor.everyPlayer[0].world_col;

            //C + 1 = +1000 pistol ammo
            if (Input.GetKeyDown(KeyCode.Alpha1)) {
                playerActor.everyPlayer[0].addAmmo(1000, ammoType.pistol);
                ce_addedAmmo = true;
            }
            //C + 2 = +1000 shotgun ammo
            if (Input.GetKeyDown(KeyCode.Alpha2)) {
                playerActor.everyPlayer[0].addAmmo(1000, ammoType.shotgun);
                ce_addedAmmo = true;
            }
            //C + 3 = +1000 smg ammo
            if (Input.GetKeyDown(KeyCode.Alpha3)) {
                playerActor.everyPlayer[0].addAmmo(1000, ammoType.smg);
                ce_addedAmmo = true;
            }
            //C + 4 = +1000 dynamite
            if (Input.GetKeyDown(KeyCode.Alpha4)) {
                playerActor.everyPlayer[0].addAmmo(1000, ammoType.dynamite);
                ce_addedAmmo = true;
            }
            //C + 5 = Give all weps
            if (Input.GetKeyDown(KeyCode.Alpha5)) {
                playerActor.everyPlayer[0].addWeapon(ammoType.shotgun);
                playerActor.everyPlayer[0].addWeapon(ammoType.smg);
                ce_addedWeapons = true;
            }

            //C + N = Toggle noclip
            if (Input.GetKeyDown(KeyCode.N)) {
                playerWorldCol = playerActor.everyPlayer[0].world_col;

                playerWorldCol.enabled = !playerWorldCol.enabled;

                ce_changedNoClip = true;
            }

            //Log cheats used
            if (ce_addedAmmo) Debug.Log(ce_message_start + "Added ammo.");
            if (ce_addedWeapons) Debug.Log(ce_message_start + "Added weapons.");
            if (ce_changedNoClip) Debug.Log(ce_message_start + "NoClip " + (!playerWorldCol.enabled ? "ON" : "OFF"));
        }
    }
}
