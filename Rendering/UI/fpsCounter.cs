using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class fpsCounter : MonoBehaviour
{
    TextMeshProUGUI txt;
    
    // Start is called before the first frame update
    void Start()
    {
        txt = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        float val;
        if (Time.deltaTime > 0) val = (1 / Time.deltaTime);
        else val = 0.0f;
        
        txt.text = val.ToString();
    }
}
