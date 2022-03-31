using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ui_healthPip : MonoBehaviour
{
    //An individual health/armor pip

    Image emptyImage;
    Image filledImg;
    Image pipFilling;
    Image restorableImage;

    bool currentlyFilled = false;

    [SerializeField][Range(-1, 1)] float debugSlider = -1.0f;

    void Start()
    {
        //Fetch parts
        Image[] carrier = GetComponentsInChildren<Image>();
        
        if (carrier.Length >= 4) 
        {
            //Parts have to be in a specific order inside the hierarchy

            //Empty pip background first
            emptyImage = carrier[0];
            //Filled pip background second
            filledImg  = carrier[1];
            //Health/armor filling third
            pipFilling = carrier[3];
            //Trailing filling fourth
            restorableImage = carrier[2];

            //Initialize as filled
            setFilled(true);
        }
        else if (carrier.Length >= 3) 
        {
            //Version with no trailing bar

            //Empty pip bg
            emptyImage = carrier[0];
            //Filled pip bg
            filledImg  = carrier[1];
            //Health/armor filling
            pipFilling = carrier[2];

            //Initialize as filled
            setFilled(true);
        }
    }

    void Update() 
    {
        if (debugSlider >= 0.0f) setFill(debugSlider);
    }

    public void setFill(float fillValue = 1.0f, float restorableValue = 0.0f) 
    {
        //Clamp
        if (fillValue > 1.0f) fillValue = 1.0f;
        if (fillValue < 0.0f) fillValue = 0.0f;
        if (restorableValue > 1.0f) restorableValue = 1.0f;
        if (restorableValue < 0.0f) restorableValue = 0.0f;

        //Fill in
        pipFilling.fillAmount = fillValue;
        if (restorableImage != null) restorableImage.fillAmount = restorableValue;

        //If filling is completly empty, show the smaller empty bg 
        if (Mathf.Max(fillValue, restorableValue) == 0.0f) setFilled(false);
        else setFilled(true);
    }

    void setFilled(bool filled = true) 
    {
        //Filled == true -> show the thicker filled bg
        //Filled == false -> show the thinner empty bg

        if (filled == currentlyFilled) return;

        if (filled) 
        {
            emptyImage.enabled = false;
            filledImg.enabled  = true;

            currentlyFilled = true;
        }
        else 
        {
            filledImg.enabled  = false;
            emptyImage.enabled = true;

            currentlyFilled = false;
        }
    }
}
