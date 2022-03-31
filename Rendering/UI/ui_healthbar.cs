using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ui_healthbar : MonoBehaviour
{
    protected TextMeshProUGUI hpNumber;
    ui_healthPip[] pips;
    int pipCount;
    //Unneccessary counter

    public playerActor monitoredActor;
    protected playerActor usedActor;
    //Monitored actor is copied to used actor
    public float nonLinearHealthExponent = 1.0f;

    [SerializeField][Range(-1, 1)] protected float debugSlider = -1.0f;

    protected void Start()
    {
        //Get components
        hpNumber = GetComponentInChildren<TextMeshProUGUI>();
        pips = GetComponentsInChildren<ui_healthPip>();

        //Copy pip count
        pipCount = pips.Length;

        //Copies monitored to used actor
        copyActor();
    }

    protected virtual void Update()
    {
        if (debugSlider < 0.0f) 
        {
            if (usedActor != null) 
            {
                commandPips(Mathf.Pow(usedActor.getHealthRatio(), nonLinearHealthExponent), Mathf.Pow(usedActor.getVirtualHealthRatio(), nonLinearHealthExponent));
                hpNumber.text = Mathf.Ceil(Mathf.Pow(usedActor.getHealthRatio(), nonLinearHealthExponent) * usedActor.maxHp).ToString();
            }
            else usedActor = playerActor.everyPlayer[0];
        }
        else commandPips(debugSlider);
    }

    protected void commandPips(float hpRatio = 1.0f, float restorableRatio = -1f) 
    {
        //Treats all pips as a continuous healthbar
        //Pips are shrunk when empty

        //Calculated values
        float fFilledHp = hpRatio * pipCount;
        int iFilledHp = Mathf.FloorToInt(fFilledHp);
        float lastPipHp = fFilledHp - iFilledHp;

        //Same for trailing health (used to be hp restorable with the unused "rally" mechanic)
        float fFilledR = restorableRatio * pipCount;
        int iFilledR = Mathf.FloorToInt(fFilledR);
        float lastPipR = fFilledR - iFilledR;

        int index = 0;
        if (restorableRatio >= 0) {
            foreach (ui_healthPip pip in pips) 
            {
                //Completly filled pips
                if (index < iFilledHp) 
                {
                    pip.setFill(1, 1);
                }
                //Final health filled pip
                else if (index < iFilledR)
                {
                    if (index == iFilledHp) pip.setFill(lastPipHp, 1);
                    else pip.setFill(0, 1);
                }
                //Final "ralliable" pip 
                else if (index == iFilledR) 
                {
                    if (index == iFilledHp) pip.setFill(lastPipHp, lastPipR);
                    else pip.setFill(0, lastPipR);
                }
                //Nothing to fill in
                else pip.setFill(0, 0);
                
                index++;
            }
        }
        else 
        {
            foreach (ui_healthPip pip in pips) 
            {
                //Simpler version if there is no trailing healthbar

                //Completly filled pip
                if (index < iFilledHp) 
                {
                    pip.setFill(1, 0);
                }
                //Partially filled pip
                else if (index == iFilledHp) pip.setFill(lastPipHp, 0);
                //Empty pip
                else pip.setFill(0, 0);
                
                index++;
            }
        }
    }

    protected virtual void copyActor() 
    {
        usedActor = monitoredActor;
    }
}
