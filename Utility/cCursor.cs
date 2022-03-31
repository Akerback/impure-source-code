using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BSTools;

public class cCursor : MonoBehaviour
{
    //Essentially a virtual cursor
    public static Vector3 position = Vector3.zero;

    void LateUpdate() {
        position = tools.getMouseWorldPos();
    }
}
