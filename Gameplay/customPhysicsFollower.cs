using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class customPhysicsFollower : MonoBehaviour
{
    //Unused follower that was for a centipede monster's segments

    public Transform master;
    float followDistance;
    Rigidbody2D rb;

    void Start() 
    {
        rb = GetComponent<Rigidbody2D>();
        followDistance = (master.position - transform.position).magnitude;
    }

    void Update() 
    {
        Vector2 directionToMaster = master.position - transform.position;

        rb.velocity += (directionToMaster - directionToMaster.normalized * followDistance);// * Time.deltaTime;
    }
}
