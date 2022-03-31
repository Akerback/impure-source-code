using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TDSTools;

namespace TDSTools {
    public enum state
    {
        toggle = -1,
        disabled = 0,
        enabled = 1,
        ignore = 2
    }
}

public class scriptableObject : MonoBehaviour
{   
    //Interface for everything that can be scripted in a level

    public state collisionState = state.enabled;
    public state visibilityState = state.enabled;
    public state physicsState = state.disabled;
    protected Collider2D col;
    protected Rigidbody2D rb;
    protected SpriteRenderer spr;

    protected void Start() 
    {
        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        spr = GetComponent<SpriteRenderer>();

        //Defaults
        changeState(collisionState, visibilityState, physicsState);
    }

    public void changeWallState(state newState) 
    {
        changeState(newState, newState, state.ignore);
    }

    public void changeVisibility(state newState) 
    {
        changeState(state.ignore, newState, state.ignore);
    }

    public void changeSolidity(state newState) 
    {
        changeState(newState, state.ignore, state.ignore);
    }

    public virtual void boolWallState(int newState) {
        //Interface for use in the unity editor
        switch (newState) {
            default:
                changeWallState(state.toggle);
                return;
            case 0:
                changeWallState(state.disabled);
                break;
            case 1:
                changeWallState(state.enabled);
                break;
        }
    }

    protected void changeState(state colState, state sprState, state rbState) 
    {
        //Applies a change to this object and all its children
        Collider2D[] carrierC = GetComponentsInChildren<Collider2D>();
        SpriteRenderer[] carrierS = GetComponentsInChildren<SpriteRenderer>();
        Rigidbody2D[] carrierR = GetComponentsInChildren<Rigidbody2D>();

        foreach (Collider2D obj in carrierC) 
        {
            switch ((int)colState) 
            {
                default:
                    obj.enabled = !obj.enabled;
                    break;
                case 0:
                    obj.enabled = false;
                    break;
                case 1:
                    obj.enabled = true;
                    break;
                case 2:
                    break;
            }
        }

        foreach (SpriteRenderer obj in carrierS) 
        {
            switch ((int)colState) 
            {
                default:
                    obj.enabled = !obj.enabled;
                    break;
                case 0:
                    obj.enabled = false;
                    break;
                case 1:
                    obj.enabled = true;
                    break;
                case 2:
                    break;
            }
        }

        foreach (Rigidbody2D obj in carrierR) 
        {
            switch ((int)colState) 
            {
                default:
                    obj.simulated = !obj.simulated;
                    break;
                case 0:
                    obj.simulated = false;
                    break;
                case 1:
                    obj.simulated = true;
                    break;
                case 2:
                    break;
            }
        }
    }
}
