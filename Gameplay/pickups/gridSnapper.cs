using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BSTools;

public class gridSnapper : MonoBehaviour 
{
    //Snaps this to the grid. Mostly unused
    public List<Transform> children = new List<Transform>();
    Grid thisGrid;

    public bool snapToGrid;

    float timeToUpdateList = 0;

    void Start() 
    {
        snapToGrid = false;
    }

    void OnEnable() 
    {
        children.Clear();

        thisGrid = GetComponent<Grid>();

        //Get all children
        Transform[] carrier = GetComponentsInChildren<Transform>();

        foreach (Transform member in carrier) 
        {
            combatActor actorCheck = member.gameObject.GetComponent<combatActor>();
            pickup pickupCheck = member.gameObject.GetComponent<pickup>();

            if ((actorCheck != null) || (pickupCheck != null)) children.Add(member);
        }
    }

    void updateList() 
    {
        float currentTime = Time.time;

        if (currentTime >= timeToUpdateList) 
        {
            //Unnecessarily complicated. Best way would be to clear and reassign the children list
            List<Transform> newFindings = new List<Transform>();
            Transform[] carrier = GetComponentsInChildren<Transform>();

            foreach (Transform member in carrier) 
            {
                newFindings.Add(member);
            }

            //Only add new members
            foreach (Transform member in children) 
            {
                if (newFindings.Contains(member)) 
                    continue;
                else 
                    removeMember(member);
            }

            //Update every 5 seconds
            timeToUpdateList = Time.time + 5;
        }
    }

    public void addMember(Transform newMember) 
    {
        children.Add(newMember);
    }

    public void removeMember(Transform removedMember) 
    {
        children.Remove(removedMember);
    }

    public void gridAlign(Transform member) 
    {
        Transform tCopy = member.transform;

        member.transform.position = tools.RoundVector2(tCopy.position);//thisGrid.CellToWorld(thisGrid.LocalToCell(tCopy.position));
    }

    void Update() 
    {
        if (snapToGrid) 
        {
            updateList();

            foreach (Transform member in children) 
            {
                gridAlign(member);
            }
        }
    }
}