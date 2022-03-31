using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TDSTools;

public class debug_newAI : MonoBehaviour
{
    //May not work
    public enum follows {
        visual,
        audio
    };

    ai_simpleBase brain;
    public follows followsTarget;
    void Start()
    {
        brain = GetComponentInParent<ai_simpleBase>();
    }

    void Update()
    {
        if (brain == null) return;

        //Standard
        transform.position = brain.transform.position;

        if (followsTarget == follows.visual){
            if (brain.visualTarget != null) transform.position = brain.visualTarget.pos;
        }
        else if (followsTarget == follows.audio) {
            if (brain.audioTarget != null) transform.position = brain.audioTarget.pos;
        }
    }
}
