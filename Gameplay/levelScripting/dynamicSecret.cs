using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BSTools;

public class dynamicSecret : hiddenArea
{
    //Secret trigger

    public enum revealModes : uint  {
        collision,
        centerVisible,
        none
    }
    
    public revealModes revealMode = revealModes.centerVisible;
    public bool revealChildren = true;

    protected override void Update()
    {
        base.Update();

        if (revealMode == revealModes.centerVisible) {
            if (playerActor.everyPlayer.Count > 0) {
                if (tools.minimalVisionCheck(transform.position, playerActor.everyPlayer[0].transform.position)) reveal();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (revealMode == revealModes.collision) {
            if (other.gameObject.tag == "Player") reveal();
        }
    }

    public override void reveal(bool isRecursive = false) {
        if (revealChildren && !isRecursive) {
            dynamicSecret[] revealedMasks = GetComponentsInChildren<dynamicSecret>();

            foreach (dynamicSecret revealed in revealedMasks) {
                if (revealed == this) continue;
                revealed.reveal(true);
            }
        }
        
        base.reveal();
    }
}
