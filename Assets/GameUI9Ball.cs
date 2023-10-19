using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class GameUI9Ball : IGameUI
{
    private GameController gc;
    public GameObject centerPanel;
    public GameObject ballHighlight;

    private string[] centerList = {"Ball1", "Ball2", "Ball3", "Ball4", "Ball5", "Ball6", "Ball7", "Ball8", "Ball9"};
    private UIManager managerUI;

    private List<Vector3> panelPositions = new ();
    
    private GameObject timer1;
    private GameObject timer2;

    public GameObject _9ballPanel;

    
    public GameUI9Ball(UIManager manager, GameController gameController)
    {
        GameObject mainUI = MonoBehaviour.Instantiate(manager._9ballUIPrefab, manager.transform);
        //leftPanel =  MonoBehaviour.Instantiate(manager.leftPanelPrefab, manager.transform);
        //rightPanel =  MonoBehaviour.Instantiate(manager.rightPanelPrefab, manager.transform);
        centerPanel = GameObject.FindWithTag("BallShowCenter");
        timer1 = GameObject.FindWithTag("Timer1");
        timer2 = GameObject.FindWithTag("Timer2");
        ballHighlight = GameObject.Instantiate(gameController.ballHighlightPrefab);
        ballHighlight.SetActive(false);

        gc = gameController;
        managerUI = manager;
        _9ballPanel = MonoBehaviour.Instantiate(managerUI._9ballDisplayPrefab, centerPanel.transform, false);

        for (int i = 0; i < 9; i++) 
        {
            _9ballPanel.transform.GetChild(i).tag = centerList[i];
            panelPositions.Add(_9ballPanel.transform.GetChild(i).position);
        }
        
        timer2.SetActive(false);
        HighLightBall();
    }

    public void OnSwitchTurn()
    {
        if (timer1.activeSelf == true)
        {
            timer1.SetActive(false);
            timer2.SetActive(true);
        }
        else
        {
            timer1.SetActive(true);
            timer2.SetActive(false);
        }
    }

    public void OnShot()
    {
        ballHighlight.SetActive(false);
        //ballHighlight.GetComponent<Renderer>().enabled = false;
    }

    public void AddBallDisplay(string ball)
    {
          
    }

    public void RemoveBallDisplay(List<string> balls)
    {
        int foundCount = 0;
        for (int i = 0; i < _9ballPanel.transform.childCount; i++)
        {
            Transform child = _9ballPanel.transform.GetChild(i);
            child.position = panelPositions[i - foundCount];
            if (balls.Contains(child.tag))
            {
                GameObject.Destroy(child.gameObject);
                foundCount++;
            }
        }
    }

    public void HighLightBall()
    {
        foreach (string ball in centerList)
        {
            if (gc.rules.CheckHitableBall(ball))
            {
                //Debug.Log(ball);
                Transform g = gc.balls.GetBall(ball).transform;
                SphereCollider s1 = g.GetComponent<SphereCollider>();
                Renderer renderer = ballHighlight.GetComponent<Renderer>();
                float r1 = s1.radius * s1.transform.lossyScale.x;
                float r2 = renderer.bounds.extents.x;

                float ratio = r1 * 1.15f / r2;
                ballHighlight.transform.localScale = new (renderer.transform.localScale.x * ratio, 0, renderer.transform.localScale.z * ratio);
                ballHighlight.transform.position = s1.transform.position;
                ballHighlight.SetActive(true);
            }
        }
    }

    public void Refresh()
    {
        RemoveBallDisplay(gc.pottedBalls);
        HighLightBall();
    }
}