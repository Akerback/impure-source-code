using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class soundOnSpawn : MonoBehaviour
{
    //Plays an AI sound on start

    public aiSound soundObject;
    float volume = 1.0f;
    float duration = 0.1f;

    void Start() 
    {
        aiSound newSound = Instantiate(soundObject, transform);
        newSound.volume = volume;
        newSound.duration = duration;
    }
}
