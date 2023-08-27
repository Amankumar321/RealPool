using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cue : MonoBehaviour
{
    public Rigidbody cueBall;
    public PhysicMaterial ballPhysicMaterial;
    //private GameObject cueBody;
    public LineRenderer aimLine;
    public LineRenderer aimHitCircle;
    public LineRenderer deviationLine;
    public LineRenderer targetLine;
    private const float lineHeight = 1.2f;
    private const float accuracy = 5f;

    //private float strength = 5f;
    private Vector3 prevPos;
    bool startDrag = false;

    public GameController gameController;

    // Start is called before the first frame update
    void Start()
    {
        //Application.targetFrameRate = 120;
        //aimLine.colorGradient = Gradient;
        aimLine.startColor = Color.white;
        aimLine.endColor = Color.white;
        deviationLine.startColor = Color.white;
        deviationLine.endColor = Color.white;
        targetLine.startColor = Color.white;
        targetLine.endColor = Color.white;
    }


    float GetRadius()
    {
        return cueBall.GetComponent<SphereCollider>().radius * cueBall.transform.lossyScale.x;
    }
    void DragAndMove()
    {
        if (Input.GetMouseButton(0) == true) {
            if (startDrag == true) {
                Vector2 mouse = new (Input.mousePosition.x,Input.mousePosition.y);
                Ray ray;
                ray = Camera.main.ScreenPointToRay(mouse);
                
                if(Physics.Raycast(ray,out RaycastHit hit, 100))
                {
                    Vector3 a = new (hit.point.x, transform.position.y, hit.point.z);
                    Vector3 b = new (transform.position.x, transform.position.y, transform.position.z);

                    Quaternion targetRotation = Quaternion.FromToRotation(prevPos, a - b);
                    transform.rotation = transform.rotation * targetRotation;
                    
                    prevPos = a - b; 
                } 
            }
            else {
                startDrag = true;
                Vector2 mouse = new (Input.mousePosition.x, Input.mousePosition.y);
                Ray ray;
                ray = Camera.main.ScreenPointToRay(mouse);
                
                if(Physics.Raycast(ray, out RaycastHit hit, 100))
                {
                    Vector3 a = new (hit.point.x, transform.position.y, hit.point.z);
                    Vector3 b = new (transform.position.x, transform.position.y, transform.position.z);

                    prevPos = a - b; 
                }
            }
        }  
        else {
            startDrag = false;
        }  
    }

    void CalculateDeviation(RaycastHit hitInfo, Vector3 start)
    {
        float factor = 20f;
        float radius = GetRadius() * 9/10;
        float m1 = cueBall.mass;
        float m2 = hitInfo.rigidbody.mass;
        float e = ballPhysicMaterial.bounciness;
   
        float u1 = Vector3.Dot(transform.forward * factor, -hitInfo.normal.normalized);
        Vector3 tangent = Vector3.Cross(-hitInfo.normal, new Vector3(0, -1, 0));
        float tangentComponent = Vector3.Dot(transform.forward * factor, tangent);
        float v1 = (m1 * u1 - e * m2 * u1) / (m1 + m2);
        
        //Vector3 height = new (0, lineHeight, 0);
        Vector3 end = start + (v1 * -hitInfo.normal.normalized + tangentComponent * tangent);
        
        Vector3[] positionArray = new [] {ToCameraPerspective(start, lineHeight), ToCameraPerspective(end, lineHeight)};
        deviationLine.positionCount = 2;
        deviationLine.startWidth = radius / 5;
        deviationLine.endWidth = radius / 5;
        deviationLine.SetPositions(positionArray);
    }

    void CalculateTargetLine(RaycastHit hitInfo)
    {
        float factor = 20f;
        float radius = GetRadius() * 9/10;
        float m1 = cueBall.mass;
        float m2 = hitInfo.rigidbody.mass;
        float e = ballPhysicMaterial.bounciness;
        float u1 = Vector3.Dot(transform.forward * factor, -hitInfo.normal.normalized);
        float v2 = (1 + e) * m1 * u1 / (m1 + m2);

        Vector3 start = hitInfo.rigidbody.position;
        Vector3 end = start + (v2 * -hitInfo.normal.normalized);
        //Vector3 height = new (0, lineHeight, 0);
        
        Vector3[] positionArray = new [] {ToCameraPerspective(start, lineHeight), ToCameraPerspective(end, lineHeight)};
        targetLine.positionCount = 2;
        targetLine.startWidth = radius / 5;
        targetLine.endWidth = radius / 5;
        targetLine.SetPositions(positionArray);
    }

    void DrawErrorCircle(LineRenderer line, Vector3 pos, int segments, float radius)
    {
        float x, y, z;

        float change = (float)(2 * Math.PI / segments);
        float angle = change + (float)Math.PI/4;

        line.positionCount = segments + 2;
        int i;

        for (i = 0; i < (segments + 1); i++)
        {
            x = Mathf.Sin(angle) * radius + pos.x;
            y = pos.y;
            z = Mathf.Cos(angle) * radius + pos.z;

            line.SetPosition(i, ToCameraPerspective(new Vector3(x, y, z), lineHeight));

            angle += change;
        }

        angle += (float)Math.PI;
        x = Mathf.Sin(angle) * radius + pos.x;
        y = pos.y;
        z = Mathf.Cos(angle) * radius + pos.z;
        line.SetPosition(i, ToCameraPerspective(new Vector3(x,y,z), lineHeight));

        // x = Mathf.Sin(Mathf.Deg2Rad * -45) * radius + pos.x;
        // y = pos.y;
        // z = Mathf.Cos(Mathf.Deg2Rad * -45) * radius + pos.z;
        // line.SetPosition(i + 2, ToCameraPerspective(new Vector3(x,y,z), lineHeight));
    }

    void DrawCircle(LineRenderer line, Vector3 pos, int segments, float radius)
    {
        float x, y, z;

        float change = (float)(2 * Math.PI / segments);
        float angle = change;

        line.positionCount = segments + 1;

        for (int i = 0; i < (segments + 1); i++)
        {
            x = Mathf.Sin(angle) * radius + pos.x;
            y = pos.y;
            z = Mathf.Cos(angle) * radius + pos.z;

            line.SetPosition(i, ToCameraPerspective(new Vector3(x, y, z), lineHeight));

            angle += change;
        }

    }

    void Aim()
    {
        float radius = GetRadius() * 9/10;
        if (Physics.SphereCast(transform.position, radius, transform.forward, out RaycastHit hitInfo, Mathf.Infinity))
        {    
            //Vector3 height = new (0, lineHeight, 0);
            Vector3 finalPosition = hitInfo.point + hitInfo.normal * radius;
            
            Vector3[] positionArray = new []{ToCameraPerspective(cueBall.transform.position, lineHeight), ToCameraPerspective(finalPosition, lineHeight)};
            aimLine.positionCount = 2;
            aimLine.startWidth = radius / 5;
            aimLine.endWidth = radius / 5;
            aimLine.SetPositions(positionArray);
            
            Player active = gameController.GetActivePlayer();
            bool isBall = hitInfo.transform.tag.StartsWith("Ball");
            bool canHitBall = (active.HasGroup() && active.BallInGroup(hitInfo.transform.tag)) || !active.HasGroup();

            if (!isBall || (canHitBall && isBall))
            {
                aimHitCircle.startColor = Color.white;
                aimHitCircle.endColor = Color.white;
                DrawCircle(aimHitCircle, finalPosition, 100, radius);
            }
            else
            {
                aimHitCircle.startColor = Color.red;
                aimHitCircle.endColor = Color.red;
                DrawErrorCircle(aimHitCircle, finalPosition, 100, radius);
            }
            aimHitCircle.startWidth = radius / 5;
            aimHitCircle.endWidth = radius / 5;

            if (isBall && canHitBall)
            {
                CalculateDeviation(hitInfo, finalPosition);
                CalculateTargetLine(hitInfo);
            }
            else
            {
                deviationLine.startWidth = 0;
                deviationLine.endWidth = 0;
                targetLine.startWidth = 0;
                targetLine.endWidth = 0;
            }
        }
    }


    Vector3 ToCameraPerspective(Vector3 actualPoint, float atHeight)
    {
        float ratio = (Camera.main.transform.position.y - atHeight) / (Camera.main.transform.position.y - actualPoint.y);
        Vector3 vec = actualPoint - Camera.main.transform.position;
        Vector3 direction = vec.normalized;
        float distance = vec.magnitude * ratio;

        return direction * distance + Camera.main.transform.position;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        aimLine.gameObject.SetActive(false);
        aimHitCircle.gameObject.SetActive(false);
        deviationLine.gameObject.SetActive(false);
        targetLine.gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
        aimLine.gameObject.SetActive(true);
        aimHitCircle.gameObject.SetActive(true);
        deviationLine.gameObject.SetActive(true);
        targetLine.gameObject.SetActive(true);
    }

    void Follow() 
    {
        transform.position = cueBall.position;
    }

    // Update is called once per frame
    void Update()
    {
        Follow();
        DragAndMove();
        Aim();
    }


    
}
