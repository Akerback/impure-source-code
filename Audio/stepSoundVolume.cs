using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using game;

[RequireComponent(typeof(Collider2D))]
public class stepSoundVolume : MonoBehaviour
{
    public soundMaterial materialIndex = 0;
    public int priority = 0;
}
