using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    public IGameUI handler; 

    public GameObject solidsDisplayPrefab;
    public GameObject stripesDisplayPrefab;
    public GameObject _9ballDisplayPrefab;

    public GameObject _8ballUIPrefab;
    public GameObject _9ballUIPrefab;

    public GameObject Ball8DisplayPrefab;
    public GameObject Ball9DisplayPrefab;

    public bool hasControl = false;

    public GameController gc;


    public void Create(string type, GameController gameController)
    {   
        switch (type)
        {
            case "8Ball":
                handler = new GameUI8Ball(this, gameController);
                break;
            case "9Ball":
                handler = new GameUI9Ball(this, gameController);
                break;
            default:
                handler = new GameUI8Ball(this, gameController);
                break;
        }
        gc = gameController;
    }

    // }
    public void Refresh() {
        if (handler != null)
        {
            handler.Refresh();
        }
    }


    public void OnSwitchTurn()
    {
        if (handler != null)
        {
            handler.OnSwitchTurn();
        }
    }

    public void OnShot()
    {
        if (handler != null)
        {
            handler.OnShot();
        }
    }

    public void AddBallDisplay(string ball)
    {
        if (handler != null)
        {
            handler.AddBallDisplay(ball);
        }
    }
}
