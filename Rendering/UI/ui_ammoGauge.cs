using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ui_ammoGauge : MonoBehaviour
{
    public playerInventory monitoredPlayer;
    Image[] icons;
    TextMeshProUGUI counterText;

    void Start() 
    {
        //Get components
        counterText = GetComponentInChildren<TextMeshProUGUI>();

        icons = GetComponentsInChildren<Image>();
    }

    void Update() 
    {
        if (monitoredPlayer != null) 
        {
            pickIcon(monitoredPlayer.selectedWeapon);
            setCounter(monitoredPlayer.getCurrentAmmo());
        }
        else
        {
            //Error behaviour
            //Cycle ammo icons
            pickIcon(-1);
            //Show an error
            setCounter("Error 404");
            //Repeatedly try to find player 1's inventory
            monitoredPlayer = playerActor.everyPlayer[0].inv;
        }
    }

    void pickIcon(int selectedIcon) 
    {
        //If negative, cycle icons
        if (selectedIcon < 0) 
        {
            selectedIcon = (int)(Time.time % 4);
        }

        int i = 0;

        foreach (Image icon in icons) 
        {
            //Enable the selectedIcon, disable others
            if (i == selectedIcon) 
                icon.enabled = true;
            else 
                icon.enabled = false;

            i++;
        }
    }

    void setCounter(int newCounterValue) 
    {
        counterText.text = newCounterValue.ToString();
    }

    //For errors
    void setCounter(string newCounterString) 
    {
        counterText.text = newCounterString;
    }
}
