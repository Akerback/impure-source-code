using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hiddenArea : MonoBehaviour
{
    //Area mask with a fade animation in code

    public static List<hiddenArea> allSecrets = new List<hiddenArea>();

    protected SpriteMask mask;
    public float fadeTime = 0.0f;
    protected float fadeStart = 0.0f;

    protected void Start() 
    {
        mask = GetComponent<SpriteMask>();
        allSecrets.Add(this);
    }

    protected virtual void Update() {
        if (fadeStart > 0.0f) {
            if (fadeTime > 0.0f) {
                mask.alphaCutoff = (Time.time - fadeStart) / fadeTime;
            }
        }
    }

    public virtual void reveal(bool isRecursive = false) 
    {
        if (fadeStart == 0.0f) {
            fadeStart = Time.time;
            if (fadeTime > 0) Invoke("gameEnd", fadeTime);
            else gameEnd();
        }
    }

    void gameEnd() {
        allSecrets.Remove(this);
        Destroy(gameObject);
    }
}
