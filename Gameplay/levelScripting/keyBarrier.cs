using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TDSTools;

public class keyBarrier : scriptableObject
{
    //Disappears when touched by a player that has its key

    public keys requiredKey = keys.none;

    void OnCollisionEnter2D(Collision2D hitEvent) {
        if (hitEvent.gameObject.tag == "Player") {
            if ((requiredKey == keys.none) || (playerActor.hasKey(requiredKey))) {
                changeState(state.disabled, state.disabled, state.ignore);
            }
        }
    }
}
