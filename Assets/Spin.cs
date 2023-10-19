using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Spin : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public Image image;
    public Image spinSelector;
    public Rigidbody cueBall;
    private GameController gc;

    // Start is called before the first frame update
    void Start()
    {
        gc = GameObject.Find("GameController").GetComponent<GameController>();
    }

    // Update is called once per frame
    void Update()
    {   
        
    }

    Vector3 ClampInCircle(Vector3 point)
    {
        float distance = Vector3.Distance(image.transform.position, point);
        float radius1 = image.rectTransform.sizeDelta.x/2 * image.transform.lossyScale.x;
        float radius2 = spinSelector.rectTransform.sizeDelta.x/2 * spinSelector.transform.lossyScale.x;

        
        if (distance > radius1 - radius2) {
            return image.transform.position + (point - image.transform.position).normalized * (radius1 - radius2);
        }
        return point;
    }

    public Vector3 GetSpinVector()
    {
        float radius1 = image.rectTransform.sizeDelta.x/2 * image.transform.lossyScale.x;
        float radius2 = spinSelector.rectTransform.sizeDelta.x/2 * spinSelector.transform.lossyScale.x;

        Vector3 relativePos = (spinSelector.transform.position - image.transform.position) / (radius1 - radius2);
        //Debug.Log("Spin: x:" + relativePos.x + " y:" + relativePos.y + " z:" + relativePos.z);
        //Quaternion rotX = Quaternion.AngleAxis(-180, new Vector3(1,0,0));
        //Quaternion rotY = Quaternion.AngleAxis(90, new Vector3(0,1,0));
        //Quaternion rotZ = Quaternion.AngleAxis(-90, new Vector3(0,0,1));

        Vector3 spinVector = new (relativePos.x, -relativePos.y, 0);
        return spinVector;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        gc.UI.hasControl = true;
        spinSelector.transform.position = ClampInCircle(eventData.position);
    }

    public void OnDrag(PointerEventData eventData)
    {
        spinSelector.transform.position = ClampInCircle(eventData.position);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        gc.UI.hasControl = false;
    }
}
