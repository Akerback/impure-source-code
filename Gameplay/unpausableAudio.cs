using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class unpausableAudio : MonoBehaviour
{
    //Audio that can't be paused

    AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null) Destroy(this);
        else {
            audioSource.ignoreListenerPause = true;
        }
    }
}
