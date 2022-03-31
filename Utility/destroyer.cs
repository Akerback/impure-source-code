using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class destroyer : MonoBehaviour
{
    //Only for destroying stuff through events in the editor

    public void destroyThis() 
    {
        Destroy(gameObject);
    }
}
