using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class aiSound : MonoBehaviour
{
    //A virtual sound AI reacts to
    public static List<aiSound> everySound = new List<aiSound>();
    public float duration = 1;
    [Min(0.0f)] public float volume = 1;
    //Acts more like a multiplier for how far away a AI can notice this sound

    public UnityEvent onSoundStart, onSoundEnd;

    protected virtual void Start() 
    {
        //Don't remember why both AI and sounds have this list
        //The list owned by baseAI is the only one used
        baseAI.everySound.Add(this);
        everySound.Add(this);

        onSoundStart.Invoke();

        if (duration > 0) Invoke("endSound", duration);
    }

    protected void endSound() 
    {
        baseAI.everySound.Remove(this);

        onSoundEnd.Invoke();

        Destroy(this);
    }

    protected virtual void OnDestroy() 
    {
        everySound.Remove(this);
        endSound();
    }
}
