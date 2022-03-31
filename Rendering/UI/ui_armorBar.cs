using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ui_armorBar : ui_healthbar
{
    protected override void Update()
    {
        if (debugSlider < 0.0f) 
        {
            if (usedActor != null) 
            {
                commandPips(usedActor.getArmorRatio());
                hpNumber.text = Mathf.Floor(usedActor.getArmor()).ToString();
            }
            else usedActor = playerActor.everyPlayer[0];
        }
        else commandPips(debugSlider);
    }
}
