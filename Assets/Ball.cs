using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.SearchService;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public Rigidbody body;
    public Ball ball;
    public SphereCollider sphereCollider;
    public GameController gameController;
    
    // Start is called before the first frame update
    void Start()
    {
        body.maxAngularVelocity = Mathf.Infinity;
        body.drag = 0.5f;
        body.angularDrag = 0.5f;
        body.velocity = Vector3.zero;
        body.angularVelocity = Vector3.zero;
        body.useGravity = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool IsMoving() 
    {
        float v = body.velocity.magnitude;

        if (v > 0)
        {
            return true;
        }
        return false;
    }

    void FixedUpdate()
    {
        if (ball == null) return;

        float v = body.velocity.magnitude;

        if (v > 0)
        {
            body.useGravity = true;
            if (v < 0.1) {
                body.velocity = Vector3.zero;
                body.angularVelocity = Vector3.zero;
                body.useGravity = false;
                body.drag = 0.5f;
                body.angularDrag = 0.5f;
                gameController.CheckMotionStop();
            }
            else if (v < 0.5) {
                body.drag = 1f;
                body.angularDrag = 1f;
            }
            body.velocity = new Vector3(body.velocity.x, 0, body.velocity.z);
        }
    }


    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Pocket")
        {
            gameController.OnBallPot(ball);
            // body.transform.position = new Vector3(0, 0.5f, 0);
            // body.velocity = Vector3.zero;
            // body.angularVelocity = Vector3.zero;
        }
        if (other.gameObject.tag == "Rail")
        {
            //Debug.Log("hit Rail");
            gameController.OnBallRailHit(ball);
        }
        
    }
}
