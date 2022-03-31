using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerSpawn : MonoBehaviour
{
    //Spawns the player and loads their state from the previous level
    public GameObject playerPrefab;

    void Start() 
    {
        GetComponent<SpriteRenderer>().enabled = false;
        SpawnPlayer();
    }

    public void SpawnPlayer() 
    {
        GameObject spawnedPlayer = Instantiate(playerPrefab, transform.position, transform.rotation);

        PlayerStateHandler stateHandler = spawnedPlayer.GetComponent<PlayerStateHandler>();
        if (globalContainer.hasSavedPlayer == false) 
            stateHandler.LoadFromGlobal();
        else 
            stateHandler.LoadUnPositionalFromGlobal();

    }
}
