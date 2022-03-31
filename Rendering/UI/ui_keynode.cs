using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TDSTools;

[RequireComponent(typeof(Image))]
public class ui_keynode : MonoBehaviour
{
    //Shows if a player owns this key

    public keys representsKey;
    Image img;
    bool currentlyVisible = true;

    void Start() {
        img = GetComponent<Image>();
    }

    void Update()
    {
        if (playerActor.hasKey(representsKey)) {
            if (!currentlyVisible) {
                Color cCopy = img.color;
                cCopy.a = 1;
                img.color = cCopy;
                currentlyVisible = true;
            }
        }
        else {
            if (currentlyVisible) {
                Color cCopy = img.color;
                cCopy.a = 0;
                img.color = cCopy;
                currentlyVisible = false;
            }
        }
    }
}
