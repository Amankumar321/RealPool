using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PowerSlider : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    private Slider slider;
    private Cue cue;
    private Spin spin;
    private GameController gameController;


    // Start is called before the first frame update
    void Start()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        slider = transform.GetComponent<Slider>();
        cue = gameController.cue;
        spin = GameObject.Find("Spin").GetComponent<Spin>();

        slider.minValue = 0;
        slider.maxValue = 100;
        slider.value = 0;
    
    }

    // Update is called once per frame
    void Update()
    {
       
    }
    

    public void OnPointerUp(PointerEventData eventData)
    {
        gameController.UI.hasControl = false;
        if (slider.value != slider.minValue) {
            float val = slider.value;
            slider.SetValueWithoutNotify(slider.minValue);
            cue.TakeShot(val, spin.GetSpinVector());
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        gameController.UI.hasControl = true;
    }
}


