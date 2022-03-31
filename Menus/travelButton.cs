using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class travelButton : MonoBehaviour
{
    //Parent for any button that can change scene

    Animator anim;
    bool hasAnimator;
    string chosenLevel = "";

    void Start() {
        anim = GetComponent<Animator>();

        hasAnimator = (anim != null);
    }

    public virtual void goToLevel(string levelToGoTo) {
        if (hasAnimator) {
            cameraTools.fadeOut();
            
            chosenLevel = levelToGoTo;
            anim.SetTrigger("wasPressed");
        }
        else {
            changeScene(chosenLevel);
        }
    }

    public void changeScene() {
        DungeonMaster.loadScene(chosenLevel);
    }
    
    void changeScene(string nextLevelName) {
        DungeonMaster.loadScene(chosenLevel);
    }
}
