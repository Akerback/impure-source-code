using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class debug_painSource : MonoBehaviour
{
    //Visualizes who owns a damage source
    debugArrow arrowToOwner;
    painSource debugTarget;
    void Start()
    {
        arrowToOwner = GetComponent<debugArrow>();
        debugTarget = GetComponentInParent<painSource>();
    }

    void Update()
    {
        if (debugTarget.owner != null) arrowToOwner.arrowVector = debugTarget.owner.transform.position - transform.position;
        else arrowToOwner.arrowVector = -transform.position;
    }
}
