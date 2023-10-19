using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Rules9Ball : IRules
{
    private GameController gc;
    private string[] ballTags = {"Ball1", "Ball2", "Ball3", "Ball4", "Ball5", "Ball6", "Ball7", "Ball8", "Ball9"};
    private bool[] pottedBall = {false, false, false, false, false, false, false, false, false};
    
    public Rules9Ball(GameController gameController)
    {
        gc = gameController;
    }

    public bool CheckHitableBall(string ball)
    { 
        if (GetLowestNumber() != -1 && ball == ballTags[GetLowestNumber()])
        {
            return true;
        }
        return false;
    }

    public bool CheckPottableBall(string ball)
    {
        if (ballTags.Contains(ball))
        {
            return true;
        }
        return false;
    }

    public List<Ball> GetHitableBalls()
    {
        List<Ball> balls = gc.balls.GetAll();
        List<Ball> hitable = new();

        foreach (Ball b in balls)
        {
            if (CheckHitableBall(b.tag))
            {
                hitable.Add(b);
            }
        }
        return hitable;
    }

    public List<Ball> GetPottableBalls()
    {
        List<Ball> balls = gc.balls.GetAll();
        List<Ball> pottable = new();

        foreach (Ball b in balls)
        {
            if (CheckPottableBall(b.tag))
            {
                pottable.Add(b);
            }
        }
        return pottable;
    }

    void EndGame()
    {
        Debug.Log("Win game");
    }

    int GetLowestNumber()
    {
        for (int i = 0; i < pottedBall.Length; i++)
        {
            if (pottedBall[i] == false)
            {
                return i;
            }
        }
        return -1;
    }

    public bool IsLegalBall(string ball)
    {
        if (GetLowestNumber() != -1 && ball == ballTags[GetLowestNumber()])
        {
            return true;
        }
        return false;
    }

    void CheckLegalBreak()
    {
        int count = 0;

        foreach (string ball in gc.hitRailBalls)
        {
            if (ball != "CueBall")
            {
                count++;
            }
        }

        if (gc.contactedBalls.Count == 0)
        {   
            Debug.Log("No ball contacted");
            gc.SetSwitchTurn(true);
            gc.GetBallInHand();
            return;
        }

        if (gc.contactedBalls.Count > 0 && !IsLegalBall(gc.contactedBalls[0]))
        {
            Debug.Log("Hit illegal ball");
            gc.SetSwitchTurn(true);
            gc.GetBallInHand();
            return;
        }

        if (count < 4 && gc.pottedBalls.Count == 0)
        {
            Debug.Log("illegal break");
            gc.SetSwitchTurn(true);
            gc.GetBallInHand();
            return;
        }

        if (count >= 4 && gc.pottedBalls.Count == 0){
            Debug.Log("Legal break");
            gc.SetSwitchTurn(true);
            return;
        }

        if (gc.pottedBalls.Count > 0) {
            gc.SetSwitchTurn(false);
            return;
        }
    }

    void CheckLegalShot()
    {
        if (gc.contactedBalls.Count == 0)
        {   
            Debug.Log("No ball contacted");
            gc.SetSwitchTurn(true);
            gc.GetBallInHand();
            return;
        }

        if (gc.contactedBalls.Count > 0 && !IsLegalBall(gc.contactedBalls[0]))
        {
            Debug.Log("Hit illegal ball");
            gc.SetSwitchTurn(true);
            gc.GetBallInHand();
            return;
        }

        if (gc.hitRailBalls.Count == 0 && gc.pottedBalls.Count == 0)
        {
            Debug.Log("Foul rail");
            gc.SetSwitchTurn(true);
            gc.GetBallInHand();
            return;
        }

        if (gc.pottedBalls.Count > 0 && gc.isTableOpen)
        {
            //AssignGroups();
            gc.isTableOpen = false;
            gc.SetSwitchTurn(false);
            return;
        }   
            
        if (gc.pottedBalls.Count > 0 && !gc.isTableOpen) {
            gc.SetSwitchTurn(false);
            return;
        }

        gc.SetSwitchTurn(true);
    }

    int GetIndexOf(string[] tags, string ball)
    {
        for (int i = 0; i < tags.Length; i++)
        {
            if (tags[i] == ball)
            {
                return i;
            }
        }
        return -1;
    }

    void PotBalls()
    {
        foreach (string ball in gc.pottedBalls)
        {
            if (GetIndexOf(ballTags, ball) != -1)
            {
                pottedBall[GetIndexOf(ballTags, ball)] = true;
            }
        }
    }


    public void CheckRules()
    {
        if (gc.isGameOver) return;


        if (gc.pottedBalls.Contains("Ball9"))
        {
            if (!gc.pottedBalls.Contains("CueBall") && gc.rules.CheckHitableBall(gc.contactedBalls[0]))
            {
                PotBalls();
                EndGame();
                gc.EndGame();
                return;
            }
            else
            {
                gc.Respawn9Ball();
                pottedBall[8] = false;
                gc.pottedBalls.Remove("Ball9");
                //gc.UI.AddBallDisplay("Ball9");
            }
        }

        if (gc.pottedBalls.Contains("CueBall"))
        {
            Debug.Log("Foul");
            gc.RespawnCueball();
            gc.SetSwitchTurn(true);
            gc.GetBallInHand();
            PotBalls();
            if (gc.isBreakShot == true)
            {
                gc.isBreakShot = false;
                gc.isTableOpen = true;
            }
            return;
        } 

        if (!gc.isBreakShot)
        {
            CheckLegalShot(); 
            PotBalls();           
        }
        else
        {
            CheckLegalBreak();
            PotBalls();
            gc.isBreakShot = false;
            gc.isTableOpen = true;
        }
    }
}
