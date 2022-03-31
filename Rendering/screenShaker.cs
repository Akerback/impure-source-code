using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BSTools;

public class screenShaker : MonoBehaviour
{
    //Unused. Would've separated from cameraTools

    public static float shake = 0.0f;
    public static float shakeScale = 1.0f;
    public float shakeMagnitude = 0.0f;
    public float magnitudeThreshold = 0.05f;
    public float fadeSpeed = 1.0f;

    void Update() 
    {
        if (shake < shakeMagnitude) 
        {
            shake = shakeMagnitude;
            shakeMagnitude = 0;
        }

        transform.localPosition = tools.randomVector() * shake * shakeScale;
        shake = Mathf.Lerp(shake, 0, fadeSpeed * Time.deltaTime);

        if (shake <= magnitudeThreshold) shake = 0;
    }
}
