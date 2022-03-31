using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(customHitbox))]
[RequireComponent(typeof(SpriteRenderer))]
public class debug_hitbox : MonoBehaviour
{
    //Visualizes hitbox and its knockback
    public customHitbox hb;
    public debugArrow kbVisual;
    public SpriteRenderer hbVisual;

    void OnEnable() 
    {
        evaluateExistance();

        kbVisual.gameObject.SetActive(true);
        hbVisual.enabled = true;
    }

    void OnDisable() 
    {
        evaluateExistance();
        
        kbVisual.gameObject.SetActive(false);
        hbVisual.enabled = false;
    }

    void Update()
    {
        kbVisual.arrowVector = hb.knockback.x * transform.right + hb.knockback.y * transform.up;
    }

    void fetchThings() 
    {
        hb = GetComponent<customHitbox>();
        kbVisual = GetComponentInChildren<debugArrow>();
        hbVisual = GetComponent<SpriteRenderer>();
    }

    void evaluateExistance() 
    {
        //Try to update if any is missing
        if (hb       == null) fetchThings();
        if (hbVisual == null) fetchThings();
        if (kbVisual == null) fetchThings();
    }
}
