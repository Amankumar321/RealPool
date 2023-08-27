using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolBalls : MonoBehaviour
{
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

    public Ball GetCueball()
    {
        Ball cueBall = null;
        for(int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).tag == "CueBall") 
            {
                cueBall = transform.GetChild(i).GetComponent<Ball>();
                break;
            }
        }
        return cueBall;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
