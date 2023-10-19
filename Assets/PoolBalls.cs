using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class PoolBalls : MonoBehaviour
{

    public GameObject[] prefabArray;
    private string[] prefabTags = {"CueBall", "Ball1", "Ball2", "Ball3", "Ball4", "Ball5", "Ball6", "Ball7",
    "Ball8", "Ball9", "Ball10", "Ball11", "Ball12", "Ball13", "Ball14", "Ball15"};

    private string[] _8ballTags = {"CueBall", "Ball1", "Ball2", "Ball3", "Ball4", "Ball5", "Ball6", "Ball7",
    "Ball8", "Ball9", "Ball10", "Ball11", "Ball12", "Ball13", "Ball14", "Ball15"};

    private string[] _9ballTags = {"CueBall", "Ball1", "Ball2", "Ball3", "Ball4", "Ball5", "Ball6", "Ball7",
    "Ball8", "Ball9"};

    private Vector3[] offset = new Vector3[] {new(2e-4f, 0, 6e-4f), new(2e-4f,0,-2e-4f), new(-4e-4f,0,5e-4f), new(-4e-4f,0,2e-4f), new(4e-4f,0,5e-4f), new(8e-4f,0,2e-4f), new(-9e-4f,0,-4e-4f),
    new(3e-4f,0,-5e-4f), new(-8e-4f,0,-7e-4f), new(-5e-4f,0,3e-4f), new(4e-4f,0,3e-4f), new(6e-4f,0,-9e-4f), new(-5e-4f,0,-4e-4f), new(-4e-4f,0,-3e-4f), new(7e-4f,0,-3e-4f), new(6e-4f,0,-5e-4f)};

    // Start is called before the first frame update

    void Start()
    {
        
    }

    public List<Ball> GetAll()
    {
        List<Ball> balls = new List<Ball>();

        for(int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).GetComponent<Ball>() != null) 
            {
                balls.Add(transform.GetChild(i).GetComponent<Ball>());
            }
        }
        return balls;
    }

    public CueBall GetCueball()
    {
        CueBall cueBall = null;
        foreach (Transform t in transform)
        {
            if (t.tag == "CueBall")
            {
                cueBall = GameObject.FindWithTag("CueBall").GetComponent<CueBall>();
                break;
            }
        }
        return cueBall;
    }

    public Ball GetBall(string ball)
    {
        Ball b = null;
        foreach (Transform t in transform)
        {
            if (t.tag == ball)
            {
                b = t.gameObject.GetComponent<Ball>();
                break;
            }
        }
        return b;
    }

    public GameObject CreateBall(string ball, Vector3 position)
    {
        for (int i = 0; i < prefabTags.Length; i++)
        {
            if (prefabTags[i] == ball)
            {
                GameObject g = Instantiate(prefabArray[i], transform);
                g.transform.position = position;
                return g;
            }
        }
        return null;
    }

    int FindPrefabIndex(string toFind)
    {
        for(int i = 0; i < prefabTags.Length; i++)
        {
            if (prefabTags[i] == toFind)
            {
                return i;
            }
        }
        return -1;
    }

    public void CreateAll(string type)
    {   
        GameObject cueBall = Instantiate(prefabArray[0], transform);
        float radius = cueBall.GetComponent<SphereCollider>().radius * cueBall.transform.lossyScale.x * 1.01f;
        GameObject headString = GameObject.Find("HeadString");
        GameObject tailSpot = GameObject.Find("TailSpot");

        if (type == "8Ball")
        {
            cueBall.transform.position = new Vector3(headString.transform.position.x - radius, 0.5f, headString.transform.position.z) + offset[0];
            Vector3 direction1 = Vector3.RotateTowards(Vector3.right, Vector3.forward, Mathf.Deg2Rad * 30, 0);
            Vector3 direction2 = Vector3.back;

            Vector3[] positionArray = {new (0,0,0), new (2,0,2), new (4,0,4), new (6,0,6), new (8,0,0), new (8,0,6), new (6,0,2),
            new (4,0,2), new (2,0,0), new (4,0,0), new (6,0,0), new (8,0,8), new (8,0,2), new (6,0,4), new (8,0,4)};

            for (int i = 1; i < _8ballTags.Length; i++)
            {
                GameObject g = Instantiate(prefabArray[FindPrefabIndex(_8ballTags[i])], transform);
                Vector3 position = positionArray[i - 1].x * radius * direction1 + positionArray[i - 1].z * radius * direction2 + tailSpot.transform.position; 
                g.transform.position = new Vector3(position.x - radius, 0.5f, position.z) + offset[i];
                //g.transform.position = new Vector3(offset[i].x * 2  * 1e4f / 3, 0.5f, offset[i].z  * 1e4f / 3);
                //g.transform.position = new Vector3(UnityEngine.Random.Range(-6, 6), 0.5f, UnityEngine.Random.Range(-4, 4));
            }

            

        }
        if (type == "9Ball")
        {
            cueBall.transform.position = new Vector3(headString.transform.position.x - radius, 0.5f, headString.transform.position.z) + offset[0];
            Vector3 direction1 = Vector3.RotateTowards(Vector3.right, Vector3.forward, Mathf.Deg2Rad * 30, 0);
            Vector3 direction2 = Vector3.back;

            Vector3[] positionArray = {new (0,0,0), new (2,0,0), new (2,0,2), new (4,0,0), new (4,0,4), new (6,0,2), new (6,0,4),
            new (8,0,4), new (4,0,2)};

            for (int i = 1; i < _9ballTags.Length; i++)
            {
                GameObject g = Instantiate(prefabArray[FindPrefabIndex(_8ballTags[i])], transform);
                Vector3 position = positionArray[i - 1].x * radius * direction1 + positionArray[i - 1].z * radius * direction2 + tailSpot.transform.position; 
                g.transform.position = new Vector3(position.x - radius, 0.5f, position.z) + offset[i];
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        List<Ball> balls = GetAll();
        
        for (int i = 0; i < balls.Count; i++)
        {
            for (int j = i + 1; j < balls.Count; j++)
            {
                balls[i].CheckBallCollision(balls[j]);
            }
        }


        for (int i = 0; i < balls.Count; i++)
        {
            balls[i].CheckRailCollision();
        }
    }
}
