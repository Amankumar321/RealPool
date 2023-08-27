using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PowerSlider : MonoBehaviour, IPointerUpHandler
{
    public Slider slider;
    public Rigidbody cueBall;
    public SphereCollider cueBallBody;
    public Cue cue;
    public Spin spin;
    public GameController gameController;

    private const float powerFactor = 0.5f;
    private const float spinFactor = 30f;

    // Start is called before the first frame update
    void Start()
    {
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
        if (slider.value != slider.minValue) {
            //cueBall.AddForce(cue.transform.forward * (powerFactor * slider.value), ForceMode.Acceleration);
            Vector3 relativeSpin = transform.rotation * spin.GetSpinVector();
            //cueBall.AddTorque(Vector3.Cross(cue.transform.forward, relativeSpin) * spinFactor, ForceMode.Acceleration);
            cueBall.velocity = cue.transform.forward * (powerFactor * slider.value);
            cueBall.angularVelocity = Vector3.Cross(cue.transform.forward, relativeSpin) * spinFactor;
            slider.SetValueWithoutNotify(slider.minValue);
            gameController.OnShot();
        }
    }
}
