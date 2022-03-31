using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using BSTools;

public class fireMode : painSource
{
    //Unused. More complicated projectileSpawner that also deals with ammo and cooldowns

    public GameObject projectile;
    public uint shotCount = 0;
    public float shotSpread = 0;
    public bool randomizeSpread = false;
    public bool fullAuto = false;
    public int ammoConsumption = 0;
    public float cooldownSelf = 0;
    //Time between shots of this weapon
    public float cooldownWeapon = 0;
    //Cooldown applied to all weapons in the player's arsenal
    public float delay = 0;
    //Delay between trigger pull and firing projectiles, mainly used for melee
    public AudioClip soundEffect;
    public float pitchVariation = 0.0f;
    float basePitch = 1.0f;
    public aiSound aiSoundEffect;
    public float screenShake = 0;
    //Audio
    [HideInInspector] public float nextUseTime = 0;
    bool interruptFlag = false;
    AudioSource audioS;

    void Start() 
    {
        audioS = GetComponent<AudioSource>();
        if (audioS != null) basePitch = audioS.pitch;
        //Auto-assign owner
        ownerByActor();
    }

    public struct_shotEvent fire(int ammo = 0)
    {   
        float currentTime = Time.time;

        if (!isReady()) return new struct_shotEvent(false);

        //Apply the delay
        Invoke("createShots", delay);

        //Self cooldown
        float nextActionTime = currentTime + cooldownWeapon;
        //Overall cooldown
        nextUseTime = currentTime + cooldownSelf;

        return new struct_shotEvent(true, nextActionTime, ammoConsumption);
    }

    public bool isReady() {
        //Does all the checks for if this weapon can be shot
        if (shotCount == 0)//Ammo check
            return false;
        if (projectile == null)//Projectiles are defined
            return false;
        if (Time.time < nextUseTime)//Not in cooldown
            return false;

        return true;
    }

    //Cancels if there's a delay in progress
    public void interrupt() 
    {
        interruptFlag = true;

        Invoke("allowAttack", delay);
    }

    //Unused
    void allowAttack() 
    {
        interruptFlag = false;
    }

    //Deals with spawning shots
    void createShots() 
    {
        if (interruptFlag) 
        {
            interruptFlag = false;
            return;
        }

        cameraTools.shake += screenShake;

        //Some precalculations
        float halfAngle = shotSpread / 2;
        float ownRotation = transform.rotation.eulerAngles.z;
        
        //Hold spawn state
        float angleStep = 0;
        float currentAngle = ownRotation;

        //--Pick initial shot angles
        //Constant spread
        if ((shotCount > 1) && (randomizeSpread == false))
        {
            //Recalculate anglestep
            angleStep = shotSpread / (shotCount - 1);
            currentAngle = ownRotation - (halfAngle);
        }
        //Randomized spread
        else currentAngle = Random.Range(ownRotation - halfAngle, ownRotation + halfAngle);

        //Iterate and fire all shots
        for (int i = 0; i < shotCount; i++) 
        {
            Quaternion rotationContainer = Quaternion.Euler(0, 0, currentAngle);
            //Create using the rotationCounter above
            GameObject obj = Instantiate(projectile, transform.position, rotationContainer);
            //Is it a bullet?
            bulletMovement bul = obj.GetComponent<bulletMovement>();

            //Impart owner and associates
            impartOwner(obj);
            impartRelatives(obj);

            //Force set the bullet's movement vector
            if (bul != null) bul.moveVector = tools.vectorFromAngle(transform.rotation.eulerAngles.z);

            //--Pick the next angle
            //Randomized
            if (randomizeSpread) currentAngle = Random.Range(ownRotation - halfAngle, ownRotation + halfAngle);
            //Constant
            else currentAngle += angleStep;
        }

        //Make AI hear this shot
        if (aiSoundEffect != null) 
        {
            Instantiate(aiSoundEffect, transform.parent);
        }

        //Play sound FX
        if (audioS != null) 
        {
            //Random pitch
            audioS.pitch = basePitch + Random.Range(-pitchVariation / 2, pitchVariation / 2);
            //Play it
            audioS.PlayOneShot(soundEffect, 1f);
        }
    }

    void assignOwner(GameObject obj, GameObject ownr, Transform target) 
    {
        //Unused. Was replaced by painSource.impartOwner()
        bulletMovement bul;
        hitscan hScan;

        //Had to be split since two types
        //Bullets first since they were more common
        bul = obj.GetComponent<bulletMovement>();
        
        if (bul != null) 
        {
            bul.owner = ownr;
            return;
        }
        
        hScan = obj.GetComponent<hitscan>();
        
        if (hScan != null) 
        {
            hScan.owner = ownr;
            return;
        }
    }

    public float getNextActionTime() 
    {
        return nextUseTime;
    }
}