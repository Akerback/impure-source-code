using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneInfo : MonoBehaviour
{
    //Scene info for stuff like pausing

    public static SceneInfo currentScenesInfo;

    public string nickname = "";
    public bool canBePaused = true;

    void Awake() {
        currentScenesInfo = this;
    }
}
