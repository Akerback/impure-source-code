using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class buttonSequence : MonoBehaviour
{
    //Button sequence, all buttons have to be pressed before it's counted as finished

    public bool canReset = false;
    public bool canDecrement = false;
    int sequenceLength = 0;
    int currentlyPressed = 0;
    List<buttonOrb> buttons = new List<buttonOrb>();

    SpriteRenderer spr;

    public UnityEvent onSequenceCompleted;
    public UnityEvent onSequenceReset;

    void Start() 
    {
        spr = GetComponent<SpriteRenderer>();

        if (spr != null) spr.enabled = false;

        buttonOrb[] carrier = GetComponentsInChildren<buttonOrb>();

        foreach (buttonOrb obj in carrier) 
        {
            buttons.Add(obj);
            obj.sequenceProgress.AddListener(buttonPressed);
            obj.onReset.AddListener(buttonReset);
        }

        sequenceLength = carrier.Length;
    }

    public void buttonPressed() 
    {
        currentlyPressed++;
        if (currentlyPressed == sequenceLength) onSequenceCompleted.Invoke();
    }

    public void buttonReset() 
    {
        if (canDecrement) 
        {
            if ((canReset) && (currentlyPressed == sequenceLength)) onSequenceReset.Invoke();
            currentlyPressed--;
        }
    }
}
