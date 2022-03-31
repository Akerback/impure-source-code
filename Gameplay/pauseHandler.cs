using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(globalContainer))]
public class pauseHandler : MonoBehaviour
{
    public static pauseHandler pauseMaster;

    public static bool currentlyPaused = false;
    public List<string> unpausableScenes;

    void Awake() {
        //Merge pauseHandlers into the master
        if (pauseMaster != null) {
            pauseMaster.mergeWith(this);
            Destroy(this);
        }
        else pauseMaster = this;
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.P)) pressPause();
        //If a scene can't be paused and is paused, unpause it
        if (currentlyPaused) {
            Scene currentScene = SceneManager.GetActiveScene();
            if (!canSceneBePaused(currentScene.name)) pressPause();
        }
    }

    [HideInInspector] public void mergeWith(pauseHandler otherHandler) {
        unpausableScenes = unpausableScenes.Union(otherHandler.unpausableScenes).ToList();
    }

    public static void pressPauseNoUI() {
        Time.timeScale = 1 - Time.timeScale;
        currentlyPaused = !currentlyPaused;
    }

    public static void unpauseNoUI() {
        Time.timeScale = 1.0f;
        currentlyPaused = false;
    }

    public static void pauseNoUI() {
        Time.timeScale = 0.0f;
        currentlyPaused = true;
    }

    public void pressPause() {
        Scene currentScene = SceneManager.GetActiveScene();

        //If unpauseable scene, force it to resume
        if ((currentlyPaused) || (!canSceneBePaused(currentScene.name))) 
            onUnpause();
        else 
            onPause();
    }

    void onPause() {
        Time.timeScale = 0.0f;
        AudioListener.pause = true;
        
        setChildrenActive(true);

        currentlyPaused = true;
    }
    
    void onUnpause() {
        Time.timeScale = 1.0f;
        AudioListener.pause = false;
        
        setChildrenActive(false);

        currentlyPaused = false;
    }

    void setChildrenActive(bool newState) {
        for (int i = 0; i < transform.childCount; i++) {
            Transform currentChild = transform.GetChild(i);

            currentChild.gameObject.SetActive(newState);
        }
    }

    bool canSceneBePaused(string whichScene) {
        return (!unpausableScenes.Contains(whichScene));
    }
}
