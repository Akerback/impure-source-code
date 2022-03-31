using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class exitLevelButton : combatActor
{
    //Actor that changes the level when it dies

    Animator anim;
    public string nextLevel;

    protected override void Awake()
    {
        anim = GetComponent<Animator>();
        base.Awake();
    }

    public override void deathEvent(GameObject killer = null)
    {
        if (nextLevel.Length > 0) {
            pauseHandler.pauseNoUI();

            cameraTools.fadeOut();

            if (anim != null) anim.SetTrigger("pressed");
        }
        else {
            hp = 1;
            Debug.LogWarning(gameObject.ToString() + " tried to change to a unspecified scene!");
        }
    }

    public void changeScene() 
    {
        pauseHandler.unpauseNoUI();
        PlayerStateHandler.MasterHandler.Save(PlayerStateHandler.plrPk);
        DungeonMaster.loadScene(nextLevel);
    }
}
