using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(SpriteRenderer))]
public class buttonOrb : respawningActor
{
    //Versatily button that's pressed when it's attacked

    Color activeColor = Color.white;
    public Color pressedColor = Color.gray;

    public bool currentlyPressed = false;
    public bool resetAutomatically = true;
    SpriteRenderer spr;
    pulsatingColor pulse;
    public UnityEvent onPress;
    public UnityEvent onReset;
    [HideInInspector] public UnityEvent sequenceProgress;

    protected override void Awake()
    {
        base.Awake();

        pulse = GetComponent<pulsatingColor>();
        activeColor = pulse.getBaseColor();

        onDeath.AddListener(press);
    }

    void Update() 
    {
        //No animation while it can't be activated
        pulse.isPaused = (hp <= 0);
    }

    public void press() 
    {
        if (!resetAutomatically) currentlyPressed = !currentlyPressed;
        else 
        {
            currentlyPressed = true;
            if (respawnTime >= 0) Invoke("reactivate", respawnTime);
        }

        //Toggle logic
        if (currentlyPressed) 
        {
            pulse.setBaseColor(pressedColor);
            onPress.Invoke();
        }
        else 
        {
            pulse.setBaseColor(activeColor);
            onReset.Invoke();
        }

        if (isAlive) sequenceProgress.Invoke();
    }

    public void reactivate() 
    {
        currentlyPressed = false;
        pulse.setBaseColor(activeColor);
        onReset.Invoke();
    }
}
