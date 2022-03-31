using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class pulsatingColor : MonoBehaviour
{
    //Makes a sprite color pulsate between black and a set color
    //Used for keys and buttons

    const float halfPi = Mathf.PI / 2f;
    SpriteRenderer spr;
    [Range(0.0f, 1.0f)][Tooltip("Hur mycket mörkare färgen kan bli medan den pulserar")] public float pulseIntensity = 0.3f;
    public float pulseSpeed = 1.0f;
    [HideInInspector] public Color cCopy = Color.white;
    public bool isPaused = false;
    float waveScaler = 1.0f;
    float internalTime = 0.0f;

    void Start() {
        spr = GetComponent<SpriteRenderer>();

        setBaseColor(spr.color);
    }

    void Update() {
        //Brightness is a sine wave
        if (!isPaused) {
            waveScaler = 1 - pulseIntensity / 2 + Mathf.Sin(internalTime * pulseSpeed) * pulseIntensity / 2;
            internalTime += Time.deltaTime;
        }
        else {
            waveScaler = 1.0f;
            internalTime = (pulseSpeed != 0f) ? halfPi / pulseSpeed : 0f;
        }
        
        spr.color = new Color(cCopy.r * waveScaler, cCopy.g * waveScaler, cCopy.b * waveScaler, 1);
    }

    public void setBaseColor(Color newColor) {
        cCopy = newColor;
    }

    public Color getBaseColor() {
        if (spr == null) {
            spr = GetComponent<SpriteRenderer>();
            return spr.color;
        }
        return cCopy;
    }
}
