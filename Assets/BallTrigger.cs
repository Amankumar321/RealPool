using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallTrigger : MonoBehaviour
{

    private Ball ball;
    private Rigidbody parentBody;
    private GameObject parent;
    private GameController gameController;
    private SphereCollider sphereCollider;

    private Vector3[] previousPos = new Vector3[2];
    private Vector3[] previousVelocity = new Vector3[2];

    // Start is called before the first frame update
    void Start()
    {
        parent = transform.parent.gameObject;
        parentBody = parent.GetComponent<Rigidbody>();
        ball = parent.GetComponent<Ball>();
        sphereCollider = transform.GetComponent<SphereCollider>();
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        previousPos[0] = parent.transform.position;
        previousPos[1] = parent.transform.position;
        previousVelocity[0] = parentBody.velocity;
        previousVelocity[1] = parentBody.velocity;        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        previousPos[0] = previousPos[1];
        previousPos[1] = parent.transform.position;
        previousVelocity[0] = previousVelocity[1];
        previousVelocity[1] = parentBody.velocity;
    }

    void OnTriggerEnter(Collider other)
    {
       
    }
}
