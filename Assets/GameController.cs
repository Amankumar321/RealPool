using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class GameController : MonoBehaviour
{
    //public GameUI UI;
    private bool inMotion = false;
    public bool isTableOpen = false;
    public bool isBreakShot = true;
    //private int playerTurn = 1;
    public PoolBalls balls;
    public List<string> pottedBalls = new ();
    public List<string> hitRailBalls = new ();
    public List<string> contactedBalls = new ();
    public bool isGameOver = false;
    public Player player1;
    public Player player2;
    public Cue cue;
    public CueBall cueBall;
    public Rigidbody cueBallBody;
    public GameObject cuePrefab;

    public GameObject ballHighlightPrefab;
    private GameObject tailSpot;

    public IRules rules;
    public UIManager UI;

    public Player activePlayer;

    public UnityEvent onTurnEvent = new();

    public GameObject[] rails;

    private bool toSwitchTurn = false;
   
    // Start is called before the first frame update
    void Start()
    {
        activePlayer = player1;
        player1.hasBallInHand = true;
        tailSpot = GameObject.Find("TailSpot");

        rails = GameObject.FindGameObjectsWithTag("Rails");
        //player2.isBot = true;

        string gameType = PlayerPrefs.GetString("GameType");

        gameType = "8Ball";
       
        if (gameType == "8Ball")
        {
            balls.CreateAll("8Ball");
            rules = new Rules8Ball(this);
            UI.Create("8Ball", this);
        }
        else if (gameType == "9Ball")
        {
            balls.CreateAll("9Ball");
            rules = new Rules9Ball(this);
            UI.Create("9Ball", this);
        }
        // else {
        //     Debug.Log("here x");
        //     balls.CreateAll("9Ball");
        //     rules = new Rules9Ball(this);
        //     UI.Create("9Ball", this);
        // }

        cueBall = balls.GetCueball();
        cueBallBody = cueBall.GetComponent<Rigidbody>();
        cue = Instantiate(cuePrefab).GetComponent<Cue>();
    }

    public void OnBallContact(Ball b)
    {
        if (!contactedBalls.Contains(b.tag))
        {
            contactedBalls.Add(b.tag);
        }
    }


    public void OnBallRailHit(Ball b)
    {
        if (!hitRailBalls.Contains(b.tag))
        {
            hitRailBalls.Add(b.tag);
        }
    }

    public void OnBallPot(Ball ball)
    {
        pottedBalls.Add(ball.tag);
        Destroy(ball.gameObject);
    }

    public void EndGame()
    {
        isGameOver = true;
    }

    public void SetSwitchTurn(bool value)
    {
        toSwitchTurn = value;
    }

    public void SwitchTurn()
    {
        Debug.Log("Switch turn");
        if (activePlayer == player1) {
            activePlayer = player2;
        }
        else if (activePlayer = player2) {
            activePlayer = player1;
        }
        UI.OnSwitchTurn();
        cue.Show();
        onTurnEvent.Invoke();
    }

    public void KeepTurn()
    {
        Debug.Log("Keep Turn");
        cue.Show();
        onTurnEvent.Invoke();
    }

    public void RespawnCueball()
    {
        Debug.Log("respawning");
        GameObject g = balls.CreateBall("CueBall", new Vector3(0, 0.5f, 0));
        cueBall = g.GetComponent<CueBall>();
        cueBallBody = g.GetComponent<Rigidbody>();
    }

    public void Respawn9Ball()
    {
        Debug.Log("respawning 9 ball");
        balls.CreateBall("Ball9", new Vector3(tailSpot.transform.position.x, 0.5f, tailSpot.transform.position.z));
    }

    public void GetBallInHand()
    {
        activePlayer.RecieveBallInHand();
    }


    public void OnShot()
    {
        inMotion = true;
        UI.OnShot();
        activePlayer.hasBallInHand = false;
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
            rules.CheckRules();
            UI.Refresh();
            hitRailBalls.Clear();
            pottedBalls.Clear();
            contactedBalls.Clear();
            if (isGameOver == false)
            {
                if (toSwitchTurn)
                {
                    SwitchTurn();
                }
                else
                {
                    KeepTurn();
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //CheckMotionStop();
        //CheckMotionStart();
    }
}
