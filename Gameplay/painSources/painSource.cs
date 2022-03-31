using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class painSource : MonoBehaviour
{
    //Parent class for everything that does damage
    public static float knockbackMultiplier = 20;
    [HideInInspector] public GameObject owner;
    [HideInInspector] public List<GameObject> associates = new List<GameObject>();
    
    //For transfering ownership from source to source
    public void impartOwner(GameObject target) 
    {
        if (target == null) return;

        painSource painImparted = target.GetComponent<painSource>();

        if (painImparted != null) 
            painImparted.owner = owner;
    } 

    //For transfering familial bonds from source to source
    public void impartRelatives(GameObject target) 
    {
        if (target == null) return;

        painSource painImparted = target.GetComponent<painSource>();

        if (painImparted != null) 
        {
            //Clear, then copy
            painImparted.associates.Clear();
            foreach (GameObject parent in associates) 
            {
                painImparted.associates.Add(parent);
            }
        }
    }

    //Automatically become owned by the first actor up the hierarchy
    public void ownerByActor() 
    {
        combatActor carrier = GetComponentInParent<combatActor>();

        if (carrier != null) 
        {
            owner = carrier.gameObject;
            associatesByRelatives();
        }
    }

    //Automatically add anything with a collider upward the hierarchy to associates
    public void associatesByRelatives() 
    {
        Collider2D[] carrier = GetComponentsInParent<Collider2D>();

        foreach (Collider2D relative in carrier) 
        {
            associates.Add(relative.gameObject);
        }
    }

    public bool friendlyCheck(GameObject thing)
    {
        //Owner?
        if (thing == owner) return true;
        //Associate?
        if (associates.Contains(thing)) return true;
        //Neither. Enemy spotted!
        return false;
    }
}
