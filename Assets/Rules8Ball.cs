using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;

public class Rules8Ball : IRules
{
    private GameController gc;
    private string[] ballTags = {"Ball1", "Ball2", "Ball3", "Ball4", "Ball5", "Ball6", "Ball7", "Ball8", "Ball9", "Ball10", "Ball11", "Ball12", "Ball13", "Ball14", "Ball15"};
    private readonly string[] solidsBallTags = {"Ball1", "Ball2", "Ball3", "Ball4", "Ball5", "Ball6", "Ball7"};
    private readonly string[] stripesBallTags = {"Ball9", "Ball10", "Ball11", "Ball12", "Ball13", "Ball14", "Ball15"};

    private bool[] pottedSolidsBalls = {false, false, false, false, false, false, false};
    private bool[] pottedStripesBalls = {false, false, false, false, false, false, false};

    public string group1;
    public string group2;
    
    public Rules8Ball(GameController gameController)
    {
        gc = gameController;
    }


    public bool CheckHitableBall(string b)
    {
        if (gc.isBreakShot)
        {
            return ballTags.Contains(b);
        }
        else if (gc.isTableOpen)
        {
            return ballTags.Contains(b);
        }
        else
        {
            if (gc.activePlayer == gc.player1)
            {
                if (group1 == "Solids")
                {
                    return solidsBallTags.Contains(b) || (b == "Ball8" && !pottedSolidsBalls.Contains(false));
                }
                if (group1 == "Stripes")
                {
                    return stripesBallTags.Contains(b) || (b == "Ball8" && !pottedStripesBalls.Contains(false));
                }
            }   
            
            if(gc.activePlayer == gc.player2)
            {
                if (group2 == "Solids")
                {
                    return solidsBallTags.Contains(b)  || (b == "Ball8" && !pottedSolidsBalls.Contains(false));
                }
                if (group2 == "Stripes")
                {
                    return stripesBallTags.Contains(b)  || (b == "Ball8" && !pottedStripesBalls.Contains(false));
                }
            }
        }
        return false;
    }

    public bool CheckPottableBall(string b)
    {
        if (gc.isBreakShot)
        {
            return ballTags.Contains(b) && b != "Ball8";
        }
        else if (gc.isTableOpen)
        {
            return ballTags.Contains(b) && b != "Ball8";
        }
        else
        {
            if (gc.activePlayer == gc.player1)
            {
                if (group1 == "Solids")
                {
                    return solidsBallTags.Contains(b) || (b == "Ball8" && !pottedSolidsBalls.Contains(false));
                }
                if (group1 == "Stripes")
                {
                    return stripesBallTags.Contains(b) || (b == "Ball8" && !pottedStripesBalls.Contains(false));
                }
            }   
            
            if(gc.activePlayer == gc.player2)
            {
                if (group2 == "Solids")
                {
                    return solidsBallTags.Contains(b)  || (b == "Ball8" && !pottedSolidsBalls.Contains(false));
                }
                if (group2 == "Stripes")
                {
                    return stripesBallTags.Contains(b)  || (b == "Ball8" && !pottedStripesBalls.Contains(false));
                }
            }
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

    void AssignGroups()
    {
        string firstBall = gc.pottedBalls[0];

        if (solidsBallTags.Contains(firstBall)) {
            if (gc.activePlayer == gc.player1)
            {
                group1 = "Solids";
                group2 = "Stripes";
            }
            else
            {
                group2 = "Solids";
                group1 = "Stripes";
            }
        }
        if (stripesBallTags.Contains(firstBall)) {
            if (gc.activePlayer == gc.player1)
            {
                group2 = "Solids";
                group1 = "Stripes";
            }
            else
            {
                group1 = "Solids";
                group2 = "Stripes";
            }
        }
    }

    public bool IsLegalBall(string ball)
    {
        if (gc.activePlayer == gc.player1)
        {
            if (group1 == "Solids")
            {
                return solidsBallTags.Contains(ball) || (ball == "Ball8" && !pottedSolidsBalls.Contains(false));
            }
            if (group1 == "Stripes")
            {
                return stripesBallTags.Contains(ball) || (ball == "Ball8" && !pottedStripesBalls.Contains(false));
            }
        }   
        
        if(gc.activePlayer == gc.player2)
        {
            if (group2 == "Solids")
            {
                return solidsBallTags.Contains(ball)  || (ball == "Ball8" && !pottedSolidsBalls.Contains(false));
            }
            if (group2 == "Stripes")
            {
                return stripesBallTags.Contains(ball)  || (ball == "Ball8" && !pottedStripesBalls.Contains(false));
            }
        }
        return true;
    }

    void EndGame()
    {
        bool didPotCueball = false;
        foreach (string b in gc.pottedBalls)
        {
            if (b == "CueBall")
            {
                didPotCueball = true;
            }
        }

        if (CheckPottableBall("Ball8") && !didPotCueball && CheckHitableBall("Ball8") && gc.contactedBalls[0] == "Ball8") {
            Debug.Log("Win game");
        }
        else {
            Debug.Log("Lose game");
        }
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

        if (gc.contactedBalls.Count > 0 && !CheckHitableBall(gc.contactedBalls[0]))
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
            AssignGroups();
            gc.isTableOpen = false;
            gc.SetSwitchTurn(false);
            return;
        }   
            
        if (gc.pottedBalls.Count > 0 && !gc.isTableOpen) {
            bool anyLegalBall = false;

            foreach (string ball in gc.pottedBalls)
            {
                if (CheckPottableBall(ball))
                {
                    anyLegalBall = true;
                    break;
                }
            }

            if (anyLegalBall)
            {
                gc.SetSwitchTurn(false);
            }
            else 
            {
                gc.SetSwitchTurn(true);
            }
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
            if (GetIndexOf(solidsBallTags, ball) != -1)
            {
                pottedSolidsBalls[GetIndexOf(solidsBallTags, ball)] = true;
            }
        }
        foreach (string ball in gc.pottedBalls)
        {
            if (GetIndexOf(stripesBallTags, ball) != -1)
            {
                pottedStripesBalls[GetIndexOf(stripesBallTags, ball)] = true;
            }
        }
    }

    public void CheckRules()
    {
        if (gc.isGameOver) return;

        if (gc.pottedBalls.Contains("Ball8"))
        {
            PotBalls();
            EndGame();
            gc.EndGame();
            return;
        }

        if (gc.pottedBalls.Contains("CueBall"))
        {
            Debug.Log("Foul");
            gc.RespawnCueball();
            gc.SetSwitchTurn(true);
            gc.GetBallInHand();
            if (gc.isBreakShot == true)
            {
                gc.isBreakShot = false;
                gc.isTableOpen = true;
            }
            PotBalls();
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

        //gc.player1.PotBalls(gc.pottedBalls);
        //gc.player2.PotBalls(gc.pottedBalls);
    }
}
