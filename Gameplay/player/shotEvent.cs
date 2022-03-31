using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct struct_shotEvent
{
    //Just some info about how a attempted shot turned out
    public readonly bool succeeded;
    public readonly float time;
    public readonly int value;

    public struct_shotEvent(bool success, float nextTime = 0, int ammo = 0) 
    {
        succeeded = success;
        time = nextTime;
        value = ammo;
    }
}
