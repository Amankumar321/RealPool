using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    private string[] leftList = {"Ball1", "Ball2", "Ball3", "Ball4", "Ball5", "Ball6", "Ball7"};
    private string[] rightList = {"Ball9", "Ball10", "Ball11", "Ball12", "Ball13", "Ball14", "Ball15"};
    public GameObject leftPanel;
    public GameObject rightPanel;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 7; i++) 
        {
            leftPanel.transform.GetChild(i).tag = leftList[i];
            rightPanel.transform.GetChild(i).tag = rightList[i];
        }
    }

    public void RemoveBallDisplay(string ball)
    {
        bool startShifting = false;
        Vector3 previousPos = Vector3.zero;
        Vector3 shiftToPos = Vector3.zero;

        foreach (Transform t in leftPanel.transform)
        {
            if (startShifting) 
            {
                previousPos = t.position;
                t.position = shiftToPos;
                shiftToPos = previousPos;
            }
            if (t.tag == ball) 
            {
                startShifting = true;
                shiftToPos = t.position;
                Destroy(t.gameObject);
            }
        }

        startShifting = false;
        foreach (Transform t in rightPanel.transform)
        {
            if (startShifting) 
            {
                previousPos = t.position;
                t.position = shiftToPos;
                shiftToPos = previousPos;
            }
            if (t.tag == ball) 
            {
                startShifting = true;
                shiftToPos = t.position;
                Destroy(t.gameObject);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
