using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using game;

namespace game {
    [System.Serializable] public class stepSoundPair {
        public soundMaterial typeOfMaterial;
        public AudioClip soundEffect;
    }

    public enum soundMaterial {
        dirt,
        concrete
    };
}

[RequireComponent(typeof(AudioSource))]
public class stepSounds : MonoBehaviour
{
    public float stepsPerUnit = 0.5f;
    public float pitchVariation = 0.25f;
    public List<stepSoundPair> soundsForMaterial = new List<stepSoundPair>();


    float lastStepTime = 0.0f;
    Rigidbody2D rb;
    AudioSource audioSource;
    float basePitch = 1.0f;
    soundMaterial currentMaterial = 0;
    List<stepSoundVolume> touchingVolumes = new List<stepSoundVolume>();

    void Start() {
        rb = GetComponentInParent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();

        basePitch = audioSource.pitch;
    }

    void Update()
    {
        //Throw if there's no rigidbody
        if (rb == null) {
            Debug.LogError("Tried to attach stepSound to an object (family) that has no RigidBody2D");
            this.enabled = false;
            return;
        }

        float speed = rb.velocity.magnitude;

        if (speed > 0.0f) {
            float timeSinceStep = Time.time - lastStepTime;

            if (timeSinceStep >= 1 / stepsPerUnit / speed) {
                AudioClip stepSound = getMaterialSound();

                if (stepSound != null) {
                    float randomPitchOffset = Random.Range(-pitchVariation, pitchVariation);
                    
                    audioSource.pitch = basePitch + randomPitchOffset;
                    audioSource.PlayOneShot(stepSound, audioSource.volume);
                }
                lastStepTime = Time.time;
            }
        }
        else lastStepTime = Time.time;
    }

    AudioClip getMaterialSound() {
        //Find the right sound
        List<stepSoundPair> validSounds = soundsForMaterial.FindAll(m => m.typeOfMaterial == currentMaterial);

        //Warn if no sound is available
        if (validSounds.Count == 0) {
            Debug.LogWarning(gameObject.ToString() + "'s stepSounds has no clip defined for material: " + currentMaterial.ToString());
            return null;
        }

        //Pick a valid sound at random
        int randomIndex = Random.Range(0, validSounds.Count);

        return validSounds[randomIndex].soundEffect;
    }

    void chooseMaterial() {
        if (touchingVolumes.Count == 0) {
            currentMaterial = 0;
            return;
        }
        
        int highestPriority = int.MinValue;

        //Pick based on priorty, if volume is equal, default to newest
        foreach (stepSoundVolume volume in touchingVolumes) {
            if (volume.priority >= highestPriority) {
                highestPriority = volume.priority;
                currentMaterial = volume.materialIndex;
            }
        }
    }

    void addVolume(stepSoundVolume newVolume) {
        touchingVolumes.Add(newVolume);

        chooseMaterial();
    }

    void removeVolume(stepSoundVolume newVolume) {
        touchingVolumes.Remove(newVolume);

        chooseMaterial();
    }

    void OnTriggerEnter2D(Collider2D trigger) {
        stepSoundVolume materialVolume = trigger.GetComponent<stepSoundVolume>();

        if (materialVolume != null) addVolume(materialVolume);
    }

    void OnTriggerExit2D(Collider2D trigger) {
        stepSoundVolume materialVolume = trigger.GetComponent<stepSoundVolume>();

        if (materialVolume != null) removeVolume(materialVolume);
    }
}
