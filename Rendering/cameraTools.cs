using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BSTools;

[RequireComponent(typeof(Camera))]
public class cameraTools : MonoBehaviour
{
    //Camera following the player, and effects like Screenshake and fade to black

    public static cameraTools mainCameraTools;
    public Transform followTransform;
    public float cursorTendency = 0.5f;
    public static float shake = 0.0f;
    public static float shakeScale = 1.0f;
    private Vector3 cameraOrigin;
    public float shakeMagnitude = 0.0f;
    public float magnitudeThreshold = 0.05f;
    public float fadeSpeed = 1.0f;
    [HideInInspector] public Image overlay;
    Animator anim;

    void Start() 
    {
        anim = GetComponentInChildren<Animator>();

        cameraOrigin = transform.position;
        mainCameraTools = this;
    }

    void LateUpdate()
    {   
        if (!pauseHandler.currentlyPaused) {
            //Following
            if (followTransform != null) 
            {
                cameraOrigin = Vector2.Lerp(followTransform.position, cCursor.position, cursorTendency);
                cameraOrigin.z = transform.position.z;
            }
            else followTransform = playerActor.everyPlayer[0].transform;

            //Screenshake
            if (shake < shakeMagnitude) 
            {
                shake = shakeMagnitude;
                shakeMagnitude = 0;
            }

            transform.position = cameraOrigin + (Vector3)tools.randomVector() * shake * shakeScale;
            shake = Mathf.Lerp(shake, 0, fadeSpeed * Time.deltaTime);

            if (shake <= magnitudeThreshold) shake = 0;
        }
    }

    public static void positionalShake(float shakeAmount, Vector3 position) {
        float distance = Mathf.Max(1f, (position - Camera.main.transform.position).magnitude);

        shake += shakeAmount / distance;
    }

    void fadeOutSelf() {
        anim.SetTrigger("fadeOut");
    }

    public static void fadeOut() {
        mainCameraTools.fadeOutSelf();
    }

    void fadeInSelf() {
        anim.SetTrigger("fadeIn");
    }

    public static void fadeIn() {
        mainCameraTools.fadeInSelf();
    }
}
