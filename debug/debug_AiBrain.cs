using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TDSTools;

public class debug_AiBrain : MonoBehaviour
{
    //Visual debugging for AI. May no longer work since it wasn't maintained after AI was working well enough
    public baseAI brain;
    public ai_movement legBrain;
    public debug_visual looking;
    public debug_visual movementPlanning;
    public debug_visual targetting;

    void Start()
    {
        brain = GetComponentInParent<baseAI>();
        legBrain = GetComponentInParent<ai_movement>();

        debug_visual[] visualCarrier = GetComponentsInChildren<debug_visual>();

        looking =          visualCarrier[0];
        movementPlanning = visualCarrier[1];
        targetting =       visualCarrier[2];
    }

    void Update()
    {
        aiTarget visual = brain.pokeThoughts("visual");
        aiTarget plans  = legBrain.currentTarget;
        Vector2 walkingGoal = legBrain.walkingTarget;

        if (visual.isNotChecked) looking.transform.position = visual.pos;
        movementPlanning.transform.position = plans.pos;
        targetting.transform.position = walkingGoal;
    }
}
