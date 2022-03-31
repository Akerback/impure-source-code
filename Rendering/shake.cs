using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BSTools;

public struct shake
{
    //Unused. Was going to be used for screen shake

    public float magnitude;

    float sinePeriod;
    float sineOffset;
    float sineMagnitude;

    float cosinePeriod;
    float cosineOffset;
    float cosineMagnitude;

    public shake(float mag = 1) 
    {
        magnitude = mag;
        
        sinePeriod = tools.floatRandomRange(-1, 1);
        sineOffset = tools.floatRandomRange(-1, 1);
        sineMagnitude = tools.floatRandomRange(0.25f, 1);

        if (tools.randomChance(0.5f)) sineMagnitude = -sineMagnitude;

        cosinePeriod = tools.floatRandomRange(-1, 1);
        cosineOffset = tools.floatRandomRange(-1, 1);
        cosineMagnitude = tools.floatRandomRange(0.25f, 1);

        if (tools.randomChance(0.5f)) sineMagnitude = -sineMagnitude;
    }

    public float getValue(float time) 
    {
        float val;

        val = (sineMagnitude   * Mathf.Sin(sinePeriod   * time + sineOffset) + cosineMagnitude * Mathf.Cos(cosinePeriod * time + cosineOffset)) * magnitude;

        return val;
    }
}
