using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Spin : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    public Image image;
    public Image spinSelector;
    public Rigidbody cueBall;

    // Start is called before the first frame update
    void Start()
    {
        
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
        Vector3 spinVector = Quaternion.AngleAxis(-90, new Vector3(0,0,1)) * relativePos;
        return spinVector;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        spinSelector.transform.position = ClampInCircle(eventData.position);
    }

    public void OnDrag(PointerEventData eventData)
    {
        spinSelector.transform.position = ClampInCircle(eventData.position);
    }
}
