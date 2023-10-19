using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Ball : MonoBehaviour
{
    private Rigidbody body;
    private Ball ball;
    private SphereCollider sphereCollider;
    private GameController gameController;

    float radius;

    private Vector3[] previousPos = new Vector3[2];
    private Vector3[] previousVelocity = new Vector3[2];

    //public SphereCollider trigger;

    //public bool collisionResolved = false;
    
    // Start is called before the first frame update
    void Start()
    {
        body = transform.GetComponent<Rigidbody>();
        ball = transform.GetComponent<Ball>();
        sphereCollider = transform.GetComponent<SphereCollider>();
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        radius = sphereCollider.radius * transform.lossyScale.x;

        // trigger = gameObject.AddComponent<SphereCollider>();
        // trigger.radius = sphereCollider.radius + 0.0001f/transform.lossyScale.x;
        // trigger.isTrigger = true;
        
        //sphereCollider.contactOffset = 0.1f;

        body.maxAngularVelocity = Mathf.Infinity;
        body.drag = 0.5f;
        body.angularDrag = 0.5f;
        body.velocity = Vector3.zero;
        body.angularVelocity = Vector3.zero;
        body.useGravity = false;

        previousPos[0] = transform.position;
        previousPos[1] = transform.position;
        previousVelocity[0] = body.velocity;
        previousVelocity[1] = body.velocity; 

        // Collider[] c = Physics.OverlapSphere(transform.position, sphereCollider.radius);

        // foreach (Collider x in c){
        //     Debug.Log(x.name);
        // }

        //sphereCollider.isTrigger = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool IsMoving() 
    {
        if (ball == null) return false;
        
        float v = body.velocity.magnitude;

        if (v > 0)
        {
            return true;
        }
        return false;
    }

    void FixedUpdate()
    {
        //if (ball == null) return;

        float v = body.velocity.magnitude;
        float angularV = body.angularVelocity.magnitude;

        if (v > 0)
        {
            body.useGravity = true;
            if (v < 0.1 && angularV < 0.1) {
                body.velocity = Vector3.zero;
                body.angularVelocity = Vector3.zero;
                body.useGravity = false;
                body.drag = 0.5f;
                body.angularDrag = 0.5f;
                gameController.CheckMotionStop();
            }
            else if (v < 1 && angularV < 1) {
                body.drag = 1f;
                body.angularDrag = 1f;
            }
            //body.velocity = new Vector3(body.velocity.x, 0, body.velocity.z);
            
            //body.angularVelocity = body.angularVelocity * Mathf.Max();
        }

        previousPos[0] = previousPos[1];
        previousPos[1] = transform.position;
        previousVelocity[0] = previousVelocity[1];
        previousVelocity[1] = body.velocity;
        //collisionResolved = false;

        //CheckBallCollisions();
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "PocketTrigger")
        {
            //Debug.Log("pocket");
            //gameController.OnBallPot(ball);
            gameController.OnBallPot(ball);

        }
    }


    public void CheckBallCollision(Ball other)
    {
        if (gameController == null) return;
       
        if (Vector3.Distance(transform.position, other.transform.position) < 2 * radius + (body.velocity.magnitude + other.body.velocity.magnitude) * Time.fixedDeltaTime)
        {
            HandleBallCollision(other);
        }
    }

    public void CheckRailCollision()
    {
        if (gameController == null) return;
        
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius + 0.000f);
        
        foreach (Collider collider in colliders)
        {
            if (collider.tag == "Rail")
            {
                HandleRailCollision(collider);
                gameController.OnBallRailHit(ball);
            }
        }
    }

    
    void HandleBallCollision(Ball other)
    {
        Vector3 u1 = previousVelocity[1];
        Vector3 u2 = other.previousVelocity[1];
        Vector3 uRelative = u1 - u2;
        Vector3 p1 = previousPos[1];
        Vector3 p2 = other.previousPos[1];
        Vector3 P1 = transform.position;
        Vector3 P2 = other.transform.position;
        Vector3 pRelative = p2 - p1;

        if (uRelative.magnitude == 0)
        {
            return;
        }

        if (Vector3.Dot(u1, p2 - p1) + Vector3.Dot(u2, p1 - p2) <= 0)
        {
            return;
        }

        // if (pRelative.magnitude - (2 * radius) <= 0 || Vector3.Dot(pRelative.normalized, uRelative) <= 0)
        // {
        //     return;
        // }

        
        float m = Vector3.Dot(pRelative, uRelative.normalized);
        float n = Mathf.Sqrt(Mathf.Pow(pRelative.magnitude, 2) - Mathf.Pow(m, 2));

        if (n > 2 * radius)
        {
            return;
        }

        float c = Mathf.Sqrt(Mathf.Pow(2 * radius, 2) - Mathf.Pow(n, 2));
        float d = m - c;

        if (d > (P1 - p1).magnitude)
        {
            return;
        }

        float t = d / uRelative.magnitude;

        if (float.IsNaN(t))
        {
            return;
        }

        Vector3 finalPosition1 = p1 + u1 * t;
        Vector3 finalPosition2 = p2 + u2 * t;


        //Debug.Log("Collision");
        if (tag == "CueBall")
        {
            gameController.OnBallContact(other);
        }
        if (other.tag == "CueBall")
        {
            gameController.OnBallContact(ball);
        }

        float e = sphereCollider.material.bounciness;

        Vector3 normal = (finalPosition1 - finalPosition2).normalized;
        Vector3 tangent = Vector3.RotateTowards(normal, uRelative.normalized, Mathf.PI/2, 0);
        
        float u1Normal= Vector3.Dot(u1, normal);
        float u2Normal= Vector3.Dot(u2, normal);

        float u1Tangent = Vector3.Dot(u1, tangent);
        float u2Tangent = Vector3.Dot(u2, tangent);
            
        float v1Normal = (1 - e) * u1Normal + e * u2Normal;
        float v2Normal = (1 - e) * u2Normal + e * u1Normal;
        
        Vector3 finalVelocity1 = v1Normal * normal + u1Tangent * tangent;
        Vector3 finalVelocity2 = v2Normal * normal + u2Tangent * tangent;

        transform.position = finalPosition1;
        body.velocity = finalVelocity1;

        other.transform.position = finalPosition2;
        other.body.velocity = finalVelocity2;
    }


    public void HandleRailCollision(Collider collider)
    {
        Vector3 u = previousVelocity[1];
        Vector3 p = previousPos[1];
        Vector3 P = transform.position;

        RaycastHit[] allHits = Physics.SphereCastAll(p, radius + 0.000f, u.normalized, (P - p).magnitude);

        if (u.magnitude == 0)
        {
            return;
        }
        
        foreach (RaycastHit hit in allHits)
        {
            if (hit.transform.tag == collider.tag)
            {
                if (Vector3.Dot(u, -hit.normal) <= 0)
                {
                    return;
                }

                float t = hit.distance / u.magnitude;
                float e = (sphereCollider.material.bounciness + collider.material.bounciness) / 2;
                e = 0.7f;
                
                Vector3 finalPosition = p + u * t;
                
                Vector3 normal = hit.normal.normalized;
                Vector3 tangent = Vector3.RotateTowards(hit.normal, u.normalized, Mathf.PI/2, 0);
                
                float uNormal= Vector3.Dot(u, normal);
                float uTangent = Vector3.Dot(u, tangent);
                  
                float vNormal = -uNormal;
                
                Vector3 finalVelocity = vNormal * normal + uTangent * tangent;
                
                transform.position = finalPosition;
                body.velocity = finalVelocity * e;
                body.angularVelocity = -Vector3.Reflect(body.angularVelocity, normal) * e;
            }
        }
    }
}
