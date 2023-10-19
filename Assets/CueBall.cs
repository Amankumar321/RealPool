using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class CueBall : MonoBehaviour
{
    public bool isDragged = false;
    private GameController gameController;
    private GameObject playingArea;
    private GameObject headArea;
    private GameObject fullArea;
  

    // Start is called before the first frame update
    void Start()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        playingArea = GameObject.Find("PlayingArea");
        headArea = GameObject.Find("HeadArea");
        fullArea = GameObject.Find("FullArea");
    }

    void Drag()
    {
        if (Input.GetMouseButton(0) == true && gameController.activePlayer.hasBallInHand && !gameController.UI.hasControl)
        {
            if (isDragged)
            {
                Vector3 mouse = Input.mousePosition;
                Ray ray = Camera.main.ScreenPointToRay(mouse);
                MeshCollider meshFull = fullArea.GetComponent<MeshCollider>();
                MeshCollider meshPlay = playingArea.GetComponent<MeshCollider>();
                MeshCollider meshHead = headArea.GetComponent<MeshCollider>();

                float radius = transform.GetComponent<SphereCollider>().radius * transform.lossyScale.x;

                MeshCollider meshBound = gameController.isBreakShot ? meshHead : meshPlay;

                RaycastHit[] hitArray = Physics.RaycastAll(ray, Mathf.Infinity);

                foreach (RaycastHit hitInfo in hitArray)
                {
                    if (hitInfo.transform.gameObject == fullArea)
                    {
                        float clampedX = Mathf.Clamp(hitInfo.point.x, meshBound.bounds.min.x + radius, meshBound.bounds.max.x - radius);
                        float clampedZ = Mathf.Clamp(hitInfo.point.z, meshBound.bounds.min.z + radius, meshBound.bounds.max.z - radius);

                        Vector3 newPos = new (clampedX, transform.position.y, clampedZ);
                        Collider[] colliders = Physics.OverlapSphere(newPos, radius);
                        bool allowDrag = true;
                        foreach (Collider c in colliders)
                        {
                            if (c.tag.StartsWith("Ball") || c.tag.StartsWith("Pocket") || c.tag == "Rail")
                            {
                                allowDrag = false;
                            }
                        }
                        if (allowDrag) {
                            transform.position = newPos;
                        }
                    }
                }
            }
            else
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit[] hitArray = Physics.RaycastAll(ray, Mathf.Infinity);
                
                foreach (RaycastHit hit in hitArray)
                {
                    if (hit.transform != null && hit.transform.tag == "CueBall")
                    { 
                        isDragged = true;
                    }
                }
            }
        }
        else
        {
            isDragged = false;
        }
    }

    

    // Update is called once per frame
    void Update()
    {
        Drag();
    }

}
