using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_HealthBar : MonoBehaviour
{
    private Entity entity => GetComponentInParent<Entity>();
    private CharacterStats myStats => GetComponentInParent<CharacterStats>();
    private RectTransform myTransform;
    private Slider slider;

    private void Start()
    {
        myTransform = GetComponent<RectTransform>();
        slider = GetComponentInChildren<Slider>();

        updateHealthUI();
    }

    private void updateHealthUI()
    {
        slider.maxValue = myStats.getMaxHealthValue();
        slider.value = myStats.currentHealth;
    }

    private void OnEnable()
    {
        entity.onFlipped += flipUI; //Add something to the event
        myStats.onHealthChanged += updateHealthUI;
    }

    private void OnDisable()
    {
        if(entity != null)
            entity.onFlipped -= flipUI; //Unsubscribe from event

        if (myStats != null)
            myStats.onHealthChanged -= updateHealthUI;
    }
    private void flipUI() => myTransform.Rotate(0, 180, 0); //Remove rotation of UI bar
}
