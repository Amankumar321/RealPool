using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameUI UI;
    private bool inMotion = false;
    private bool isTableOpen = false;
    private bool isBreakShot = true;
    //private int playerTurn = 1;
    public PoolBalls balls;
    private List<string> pottedBalls = new ();
    private List<string> hitRailBalls = new ();
    private bool isGameOver = false;
    public Player player1;
    public Player player2;
    public Cue cue;

    private Player activePlayer;
   
    private string[] ballTags = {"Ball1", "Ball2", "Ball3", "Ball4", "Ball5", "Ball6", "Ball7", "Ball8", "Ball9", "Ball10", "Ball11", "Ball12", "Ball13", "Ball14", "Ball15"};

    // Start is called before the first frame update
    void Start()
    {
        activePlayer = player1;
        //cueBall = GameObject.FindWithTag("CueBall");
    }


    public void OnBallRailHit(Ball b1)
    {
        bool repeat = false;
        foreach (string b2 in hitRailBalls)
        {
            if (b1.tag == b2)
            {
                repeat = true;
            }
        }
        if (!repeat)
        {
            hitRailBalls.Add(b1.tag);
        }
    }

    public void OnBallPot(Ball ball)
    {
        pottedBalls.Add(ball.tag);
        //UI.RemoveBallDisplay(ball.gameObject);
        //Destroy(ball.gameObject);
        if (ball.tag != "CueBall")
        {
            //UI.RemoveBallDisplay(ball.gameObject);
            Destroy(ball.gameObject);
        }
        if (ball.tag == "CueBall") {
            ball.transform.position = Vector3.zero;
            ball.body.useGravity = false;
            ball.body.velocity = Vector3.zero;
            ball.body.angularVelocity = Vector3.zero;
            ball.enabled = false;
        }
    }

    void EndGame()
    {
        bool didPotCueball = false;
        foreach (string b in pottedBalls)
        {
            if (b == "CueBall")
            {
                didPotCueball = true;
            }
        }

        if (activePlayer.CanPotBlack() && !didPotCueball) {
            Debug.Log("Win game");
        }
        else {
            Debug.Log("Lose game");
        }
        isGameOver = true;
    }

    void SwitchTurn()
    {
        Debug.Log("Switch turn");
        if (activePlayer == player1) {
            activePlayer = player2;
        }
        else if (activePlayer = player2) {
            activePlayer = player1;
        }
        cue.Show();
    }

    void KeepTurn()
    {
        Debug.Log("Keep Turn");
        cue.Show();
    }

    void RespawnCueball()
    {
        Debug.Log("respawning");
        GameObject cueBall = GameObject.FindWithTag("CueBall");
        cueBall.transform.position = new Vector3(0, 0.5f, 0);
        cueBall.GetComponent<Ball>().enabled = true;
    }

    void CheckLegalBreak()
    {
        foreach (string ball in pottedBalls) 
        {
            if (ball == "CueBall")
            {
                Debug.Log("Foul at Break");
                RespawnCueball();
                SwitchTurn();
                return;
            }
        }

        int count = 0;

        foreach (string ball in hitRailBalls)
        {
            if (ball != "CueBall")
            {
                count++;
            }
        }

        if (count < 4 && pottedBalls.Count == 0)
        {
            Debug.Log("illegal break");
            SwitchTurn();
        }
        else if (count >= 4 && pottedBalls.Count == 0){
            Debug.Log("Legal break");
            SwitchTurn();
        }
        else if (pottedBalls.Count > 0) {
            KeepTurn();
        }
    }

    void CheckLegalShot()
    {
        foreach (string ball in pottedBalls) 
        {
            if (ball == "CueBall")
            {
                Debug.Log("Foul");
                RespawnCueball();
                SwitchTurn();
                return;
            }
        }

        if (hitRailBalls.Count == 0 && pottedBalls.Count == 0)
        {
            Debug.Log("Foul rail");
            SwitchTurn();
        }
        else if (isTableOpen) {
            if (pottedBalls.Count > 0)
            {
                string firstBall = pottedBalls[0];
                int index = 0;

                foreach (string tag in ballTags)
                {
                    index++;
                    if (firstBall == tag) break;
                }
                if (index < 8) {
                    Player inactivePlayer = activePlayer == player1 ? player2 : player1;
                    activePlayer.SetGroup("Solids");
                    inactivePlayer.SetGroup("Stripes");
                    Debug.Log("Solids");
                }
                if (index > 8) {
                    Player inactivePlayer = activePlayer == player1 ? player2 : player1;
                    activePlayer.SetGroup("Stripes");
                    inactivePlayer.SetGroup("Solids");
                    Debug.Log("Stripes");
                }
                isTableOpen = false;
                KeepTurn();
            }
            else
            {
                SwitchTurn();
            }
        }
        else if (pottedBalls.Count > 0) {
            bool didPotOwnBall = false;
            foreach (string tag in pottedBalls)
            {
                if (activePlayer.BallInGroup(tag))
                {
                    didPotOwnBall = true;
                }
            }
            if (didPotOwnBall)
            {
                KeepTurn();
            }
            else
            {
                SwitchTurn();
            }
        }
        else {
            SwitchTurn();
        }
    }

    void CheckRules()
    {
        if (isGameOver) return;

        bool didPot8Ball = false;
        foreach (string ball in pottedBalls) 
        {
            if (ball == "Ball8")
            {
                didPot8Ball = true;
            }
            if (ball != "Ball8" && ball != "CueBall")
            {
                UI.RemoveBallDisplay(ball);
                //Destroy(ball.gameObject);
            }
        }

        if (didPot8Ball)
        {
            EndGame();
            return;
        }

        if (!isBreakShot)
        {
            CheckLegalShot();            
        }
        else
        {
            CheckLegalBreak();
            isBreakShot = false;
            isTableOpen = true;
        }

        player1.PotBalls(pottedBalls);
        player2.PotBalls(pottedBalls);

        hitRailBalls.Clear();
        pottedBalls.Clear();
    }

    public void OnShot()
    {
        inMotion = true;
        cue.Hide();
    }

    public Player GetActivePlayer()
    {
        return activePlayer;
    }

    public void CheckMotionStop()
    {
        if (!inMotion) return;

        bool anyBallMoving = false;

        List<Ball> list = balls.GetAll();
        foreach (Ball b in list)
        {
            if (b.IsMoving() == true)
            {
                anyBallMoving = true;
                break;
            }
        }

        if (!anyBallMoving)
        {
            inMotion = false;
            Debug.Log("stop");
            CheckRules();
        }
    }

    // Update is called once per frame
    void Update()
    {
        //CheckMotionStop();
        //CheckMotionStart();
    }
}
