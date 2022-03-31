using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BSTools;

public class nav_node : MonoBehaviour
{
    //Unused custom pathfinding system
    public static List<nav_node> navTree = new List<nav_node>();

    public Vector2 position;
    public List<nav_node> connections = new List<nav_node>();

    void Start() 
    {
        newNode();
    }

    void Update() 
    {
        
    }

    public void newNode() 
    {
        //Add self
        navTree.Add(this);

        //Iterate
        foreach (nav_node node in navTree) 
        {
            if (node == this) continue;

            //Make a connection if there's line of sight between them
            if (tools.minimalVisionCheck(transform.position, node.transform.position)) 
            {
                //Two way connection
                newConnection(node);
                node.newConnection(this);
            }
        }
    }

    public void removeNode() 
    {
        navTree.Remove(this);

        //Tell the other nodes to remove their connections to this node
        foreach (nav_node connection in connections) 
        {
            connection.removeConnection(this);
        }

        //Remove the object, bringing its own list of connection with it
        Destroy(gameObject);
    }

    public bool newConnection(nav_node connectTo) 
    {
        //Returns if the connection was added

        //Check if it's a duplicate
        foreach (nav_node connection in connections) 
        {
            if (connection == connectTo) return false;
        }

        //Add it and return
        connections.Add(connectTo);
        return true;
    }

    public bool removeConnection(nav_node lostConnection) 
    {
        //Remove and return success
        return connections.Remove(lostConnection);
    }
}
