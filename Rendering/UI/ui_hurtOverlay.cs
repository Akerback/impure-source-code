using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ui_hurtOverlay : MonoBehaviour
{
    //Strawberry jam to cover the screen

    public playerActor monitoredActor;
    public float fadeSpeed = 1f;
    public float lowHealth;
    public float criticalHealthExponent = 0.5f;

    Image img;

    float health = 1;
    float lastHealth = 1;

    float armor = 0;
    float lastArmor = 0;

    void Start()
    {
        img = GetComponent<Image>();
    }

    void Update()
    {
        if (monitoredActor != null) 
        {
            health = monitoredActor.getHealthRatio();
            lastHealth = Mathf.Lerp(lastHealth, health, fadeSpeed * Time.deltaTime);

            armor = monitoredActor.getArmorRatio();
            lastArmor = Mathf.Lerp(lastArmor, armor, fadeSpeed * Time.deltaTime);

            float healthDamage = (lastHealth - health) * 2;
            float armorDamage = (lastArmor - armor) * 2;
            
            float criticalHealthModifier = 0.0f;
            if (lowHealth > 0) criticalHealthModifier = Mathf.Pow(Mathf.Max(0.0f, lowHealth - (health + armor)) / lowHealth, criticalHealthExponent);

            float healthPart = Mathf.Min(1, healthDamage + criticalHealthModifier);

            Color newColor = img.color;
            newColor.r = healthPart;
            newColor.g = armorDamage;
            newColor.a = Mathf.Max(healthPart, armorDamage);
            img.color = newColor;
            //colorAccess.a = Mathf.Min((lastHealth - health) * 2, 0);
        }
        else monitoredActor = playerActor.everyPlayer[0];
    }
}
