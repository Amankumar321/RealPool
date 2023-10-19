using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class GameUI8Ball : IGameUI
{
    private string[] solidsBallTags = {"Ball1", "Ball2", "Ball3", "Ball4", "Ball5", "Ball6", "Ball7"};
    private string[] stripesBallTags = {"Ball9", "Ball10", "Ball11", "Ball12", "Ball13", "Ball14", "Ball15"};
    private List<string> pottedBalls = new();
    public GameObject solidsPanel;
    public GameObject stripesPanel;
    public GameObject leftPanel;
    public GameObject rightPanel;
    private GameController gc;
    private UIManager managerUI;

    private GameObject timer1;
    private GameObject timer2;

    private bool isPanelDisplayed = false;

    private List<Vector3> solidsPanelPositions = new();
    private List<Vector3> stripesPanelPositions = new();

    public GameUI8Ball(UIManager manager, GameController gameController)
    {
        GameObject mainUI = MonoBehaviour.Instantiate(manager._8ballUIPrefab, manager.transform);
        //leftPanel =  MonoBehaviour.Instantiate(manager.leftPanelPrefab, manager.transform);
        //rightPanel =  MonoBehaviour.Instantiate(manager.rightPanelPrefab, manager.transform);
        leftPanel = GameObject.FindWithTag("BallShowLeft");
        rightPanel = GameObject.FindWithTag("BallShowRight");
        timer1 = GameObject.FindWithTag("Timer1");
        timer2 = GameObject.FindWithTag("Timer2");

        gc = gameController;
        managerUI = manager;
        
        //Hide();
    }

    public void Hide()
    {
        solidsPanel.SetActive(false);
        stripesPanel.SetActive(false);
        timer2.SetActive(false);
    }

    public void OnShot()
    {
        
    }
    
    public void DisplayPanel()
    {
        string firstBall = gc.pottedBalls[0];
        bool case1 = solidsBallTags.Contains(firstBall) && gc.activePlayer == gc.player1;
        bool case2 = stripesBallTags.Contains(firstBall) && gc.activePlayer == gc.player2;
        bool case3 = solidsBallTags.Contains(firstBall) && gc.activePlayer == gc.player2;
        bool case4 = stripesBallTags.Contains(firstBall) && gc.activePlayer == gc.player1;

        if (case1 || case2)
        {
            solidsPanel = MonoBehaviour.Instantiate(managerUI.solidsDisplayPrefab, leftPanel.transform);
            stripesPanel = MonoBehaviour.Instantiate(managerUI.stripesDisplayPrefab, rightPanel.transform);
        }
        if (case3 || case4)
        {
            solidsPanel = MonoBehaviour.Instantiate(managerUI.solidsDisplayPrefab, rightPanel.transform);
            stripesPanel = MonoBehaviour.Instantiate(managerUI.stripesDisplayPrefab, leftPanel.transform);
        }
        for (int i = 0; i < 7; i++) 
        {
            solidsPanel.transform.GetChild(i).tag = solidsBallTags[i];
            stripesPanel.transform.GetChild(i).tag = stripesBallTags[i];
            solidsPanelPositions.Add(solidsPanel.transform.GetChild(i).position);
            stripesPanelPositions.Add(stripesPanel.transform.GetChild(i).position);
        }
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

    public void Refresh()
    {
        if (!gc.isBreakShot && !gc.isTableOpen && isPanelDisplayed == false)
        {
            isPanelDisplayed = true;
            DisplayPanel();
        }
        PotBalls(gc.pottedBalls);
        CheckToAdd8Ball();       
    }

    public void CheckToAdd8Ball()
    {
        bool pottedAllSolids = true;
        bool pottedAllStripes = true;

        foreach (string solidsBall in solidsBallTags)
        {
            if (pottedBalls.Contains(solidsBall) == false)
            {
                pottedAllSolids = false;
                break;
            }
        }

        foreach (string stripesBall in stripesBallTags)
        {
            if (pottedBalls.Contains(stripesBall) == false)
            {
                pottedAllStripes = false;
                break;
            }
        }

        if (gc.isGameOver == false && isPanelDisplayed && (pottedAllSolids || pottedAllStripes))
        {
            AddBallDisplay("Ball8");
        }
    }

    public void PotBalls(List<string> balls)
    {
        foreach (string ball in balls)
        {
            pottedBalls.Add(ball);
        }
        if (isPanelDisplayed == true)
        {
            RemoveBallDisplay();
        }
    }

    public void AddBallDisplay(string ball)
    {
        bool pottedAllSolids = true;
        bool pottedAllStripes = true;

        foreach (string solidsBall in solidsBallTags)
        {
            if (pottedBalls.Contains(solidsBall) == false)
            {
                pottedAllSolids = false;
                break;
            }
        }

        foreach (string stripesBall in stripesBallTags)
        {
            if (pottedBalls.Contains(stripesBall) == false)
            {
                pottedAllStripes = false;
                break;
            }
        }

        if (pottedAllSolids)
        {
            GameObject g = GameObject.Instantiate(managerUI.Ball8DisplayPrefab, solidsPanel.transform);
            solidsPanel.transform.GetChild(0).tag = "Ball8"; 
            g.transform.position = solidsPanelPositions[0];
        }
        if (pottedAllStripes)
        {
            GameObject g = GameObject.Instantiate(managerUI.Ball8DisplayPrefab, stripesPanel.transform);
            stripesPanel.transform.GetChild(0).tag = "Ball8";  
            g.transform.position = stripesPanelPositions[0];
        }
    }
    
    public void RemoveBallDisplay()
    {
        int foundCount = 0;

        for (int i = 0; i < stripesPanel.transform.childCount; i++)
        {
            Transform child = stripesPanel.transform.GetChild(i);
            child.position = stripesPanelPositions[i - foundCount];
            if (pottedBalls.Contains(child.tag))
            {
                GameObject.Destroy(child.gameObject);
                foundCount++;
            }
        }
        foundCount = 0;
        for (int i = 0; i < solidsPanel.transform.childCount; i++)
        {
            Transform child = solidsPanel.transform.GetChild(i);
            
            child.position = solidsPanelPositions[i - foundCount];
            if (pottedBalls.Contains(child.tag))
            {
                GameObject.Destroy(child.gameObject);
                foundCount++;
            }
        }
    }
}
