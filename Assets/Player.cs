using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private string group;
    private List<string> groupSolids = new List<string> {"Ball1", "Ball2", "Ball3", "Ball4", "Ball5", "Ball6", "Ball7"};
    private List<string> groupStripes = new List<string> {"Ball9", "Ball10", "Ball11", "Ball12", "Ball13", "Ball14", "Ball15"};

    private List<string> groupBalls = new List<string>();

    private bool didPotAll = false;
    private bool hasGroup = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetGroup(string g)
    {
        group = g;
        if (g == "Stripes")
        {
            foreach (string s in groupStripes) {
                groupBalls.Add(s);
            }
            hasGroup = true;
        }
        if (g == "Solids")
        {
            foreach (string s in groupSolids) {
                groupBalls.Add(s);
            }
            hasGroup = true;
        }
    }

    public void PotBalls(List<string> balls)
    {
        foreach (string s in balls)
        {
            if (groupBalls.Contains(s)) {
                groupBalls.Remove(s);
            }
        }
        if (hasGroup && groupBalls.Count == 0) 
        {
            didPotAll = true;
        }
    }

    public bool CanPotBlack()
    {
        Debug.Log("can pot black");
        Debug.Log(didPotAll);
        return didPotAll;
    }

    public bool BallInGroup(string ball)
    {
        if (hasGroup && groupBalls.Contains(ball))
        {
            return true;
        }
        if (didPotAll && (ball == "Ball8")) {
            return true;
        }
        return false;
    }

    public bool HasGroup()
    {
        return hasGroup;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
