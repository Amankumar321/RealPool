using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Cue : MonoBehaviour
{
    //private Rigidbody cueBallBody;
    //private CueBall cueBall;
    //private GameObject cueBody;
    private LineRenderer aimLine;
    private LineRenderer aimHitCircle;
    private LineRenderer deviationLine;
    private LineRenderer targetLine;
    private LineRenderer aimLineShadow;
    private LineRenderer aimHitCircleShadow;
    private LineRenderer deviationLineShadow;
    private LineRenderer targetLineShadow;
    private const float lineHeight = 2f;
    private const float accuracy = 5f;

    public Path path;

    private GameObject playingArea;
    private GameObject fullArea;

    //private float strength = 5f;
    private Vector3 prevPos;
    bool startDrag = false;

    private const float powerFactor = 0.5f;
    private const float spinFactor = 50f;

    private GameController gameController;

    // Start is called before the first frame update
    void Start()
    {
        //cueBall = GameObject.FindWithTag("CueBall").GetComponent<CueBall>();
        //cueBallBody = GameObject.FindWithTag("CueBall").GetComponent<Rigidbody>();
        playingArea = GameObject.Find("PlayingArea");
        fullArea = GameObject.Find("FullArea");
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        path = ScriptableObject.CreateInstance<Path>();
    }

    void InitLines()
    {
        aimLine = gameController.cueBall.transform.Find("AimLine").GetComponent<LineRenderer>();
        aimHitCircle = gameController.cueBall.transform.Find("AimHitCircle").GetComponent<LineRenderer>();
        deviationLine = gameController.cueBall.transform.Find("DeviationLine").GetComponent<LineRenderer>();
        targetLine = gameController.cueBall.transform.Find("TargetLine").GetComponent<LineRenderer>();

        // aimLine.startColor = Color.white;
        // aimLine.endColor = Color.white;
        // deviationLine.startColor = Color.white;
        // deviationLine.endColor = Color.white;
        // targetLine.startColor = Color.white;
        // targetLine.endColor = Color.white;
        aimLineShadow = gameController.cueBall.transform.Find("AimLineShadow").GetComponent<LineRenderer>();
        aimHitCircleShadow = gameController.cueBall.transform.Find("AimHitCircleShadow").GetComponent<LineRenderer>();
        deviationLineShadow = gameController.cueBall.transform.Find("DeviationLineShadow").GetComponent<LineRenderer>();
        targetLineShadow = gameController.cueBall.transform.Find("TargetLineShadow").GetComponent<LineRenderer>();
    }


    float GetRadius()
    {
        Ball b = gameController.cueBall.GetComponent<Ball>();
        return b.GetComponent<SphereCollider>().radius * b.transform.lossyScale.x;
    }
    void DragAndMove()
    {
        if (Input.GetMouseButton(0) == true && gameController.cueBall.isDragged == false && !gameController.UI.hasControl) {
            if (startDrag == true) {
                Vector2 mouse = new (Input.mousePosition.x,Input.mousePosition.y);
                Ray ray;
                ray = Camera.main.ScreenPointToRay(mouse);
                
                if(fullArea.GetComponent<MeshCollider>().Raycast(ray,out RaycastHit hit, 100))
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
                
                if(fullArea.GetComponent<MeshCollider>().Raycast(ray, out RaycastHit hit, 100))
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
        float radius = GetRadius();
        float m1 = gameController.cueBallBody.mass;
        float m2 = hitInfo.rigidbody.mass;
        float e = gameController.cueBall.GetComponent<SphereCollider>().material.bounciness;

        float u1 = Vector3.Dot(transform.forward * factor, -hitInfo.normal.normalized);
        Vector3 tangent = Vector3.Cross(-hitInfo.normal, Vector3.down);
        float tangentComponent = Vector3.Dot(transform.forward * factor, tangent);
        float v1 = (1 - e) * u1;

        //Vector3 height = new (0, lineHeight, 0);
        Vector3 end = start + (v1 * -hitInfo.normal.normalized + tangentComponent * tangent);
        
        Vector3[] positionArray = new [] {SetHeight(start, lineHeight), SetHeight(end, lineHeight)};
        deviationLine.positionCount = 2;
        deviationLine.startWidth = radius / 5;
        deviationLine.endWidth = radius / 5;
        deviationLine.SetPositions(positionArray);
    }

    void CalculateTargetLine(RaycastHit hitInfo)
    {
        float factor = 20f;
        float radius = GetRadius();
        //float m1 = gameController.cueBallBody.mass;
        //float m2 = hitInfo.rigidbody.mass;
        float e = gameController.cueBall.GetComponent<SphereCollider>().material.bounciness;
        float u1 = Vector3.Dot(transform.forward * factor, -hitInfo.normal.normalized);
        float v2 = e * u1;

        Vector3 start = hitInfo.rigidbody.position;
        Vector3 end = start + (v2 * -hitInfo.normal.normalized);
        //Vector3 height = new (0, lineHeight, 0);
        
        Vector3[] positionArray = new [] {SetHeight(start, lineHeight), SetHeight(end, lineHeight)};
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

            line.SetPosition(i, SetHeight(new Vector3(x, y, z), lineHeight));

            angle += change;
        }

        angle += (float)Math.PI;
        x = Mathf.Sin(angle) * radius + pos.x;
        y = pos.y;
        z = Mathf.Cos(angle) * radius + pos.z;
        line.SetPosition(i, SetHeight(new Vector3(x,y,z), lineHeight));

        // x = Mathf.Sin(Mathf.Deg2Rad * -45) * radius + pos.x;
        // y = pos.y;
        // z = Mathf.Cos(Mathf.Deg2Rad * -45) * radius + pos.z;
        // line.SetPosition(i + 2, SetHeight(new Vector3(x,y,z), lineHeight));
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

            line.SetPosition(i, SetHeight(new Vector3(x, y, z), lineHeight));

            angle += change;
        }

    }

    void Aim()
    {
        // path.FindForward(gameController.cueBall.transform.position, transform.forward, 2);
        // return;

        // path.ClearMarkers();
        // path.SetGameController(gameController);
        // path.FindForward(gameController.cueBall.transform.position, transform.forward, 6);
        // return;
        InitLines();
        float radius = GetRadius();
        //if (Physics.SphereCast(transform.position, radius, transform.forward, out RaycastHit hitInfo, Mathf.Infinity, 1, QueryTriggerInteraction.Collide))
        if (Physics.SphereCast(transform.position, radius, transform.forward, out RaycastHit hitInfo, 20))
        {    
            //Vector3 height = new (0, lineHeight, 0);
            //Debug.Log(hitInfo.transform.name);

            Vector3 finalPosition = transform.position + transform.forward * hitInfo.distance;
            
            Vector3[] positionArray = new []{SetHeight(gameController.cueBallBody.transform.position, lineHeight), SetHeight(finalPosition, lineHeight)};
            aimLine.positionCount = 2;
            aimLine.startWidth = radius / 5;
            aimLine.endWidth = radius / 5;
            aimLine.SetPositions(positionArray);
            
            Player active = gameController.GetActivePlayer();
            bool isBall = hitInfo.transform.tag.StartsWith("Ball");
            //bool canHitBall = ( && active.BallInGroup(hitInfo.transform.tag)) || !active.HasGroup();

            if (!isBall || (gameController.rules.CheckHitableBall(hitInfo.transform.tag) && isBall))
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

            if (isBall && gameController.rules.CheckHitableBall(hitInfo.transform.tag))
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

            DrawShadow();
        }
    }
    
    void DrawShadow()
    {
        // LineRenderer aimLineShadow = gameController.cueBall.transform.Find("AimLineShadow").GetComponent<LineRenderer>();
        // LineRenderer aimHitCircleShadow = gameController.cueBall.transform.Find("AimHitCircleShadow").GetComponent<LineRenderer>();
        // LineRenderer deviationLineShadow = gameController.cueBall.transform.Find("DeviationLineShadow").GetComponent<LineRenderer>();
        // LineRenderer targetLineShadow = gameController.cueBall.transform.Find("TargetLineShadow").GetComponent<LineRenderer>();

        aimLineShadow.positionCount = aimLine.positionCount;
        aimLineShadow.startWidth = aimLine.startWidth * 2.5f;
        aimLineShadow.endWidth = aimLine.endWidth * 2.5f;
        for (int i = 0; i < aimLine.positionCount; i++)
        {
            aimLineShadow.SetPosition(i, aimLine.GetPosition(i) + Vector3.down * 0.00001f);
        }
        aimLineShadow.numCapVertices = 10; 

        aimHitCircleShadow.positionCount = aimHitCircle.positionCount;
        aimHitCircleShadow.startWidth = aimHitCircle.startWidth * 2.5f;
        aimHitCircleShadow.endWidth = aimHitCircle.endWidth * 2.5f;
        for (int i = 0; i < aimHitCircle.positionCount; i++)
        {
            aimHitCircleShadow.SetPosition(i, aimHitCircle.GetPosition(i) + Vector3.down * 0.00001f);
        }
        aimHitCircleShadow.numCapVertices = 10;

        deviationLineShadow.positionCount = deviationLine.positionCount;
        deviationLineShadow.startWidth = deviationLine.startWidth * 2.5f;
        deviationLineShadow.endWidth = deviationLine.endWidth * 2.5f;
        for (int i = 0; i < deviationLine.positionCount; i++)
        {
            deviationLineShadow.SetPosition(i, deviationLine.GetPosition(i) + Vector3.down * 0.00001f);
        }
        deviationLineShadow.numCapVertices = 10; 

        targetLineShadow.positionCount = targetLine.positionCount;
        targetLineShadow.startWidth = targetLine.startWidth * 2.5f;
        targetLineShadow.endWidth = targetLine.endWidth * 2.5f;
        for (int i = 0; i < targetLine.positionCount; i++)
        {
            targetLineShadow.SetPosition(i, targetLine.GetPosition(i) + Vector3.down * 0.00001f);
        }
        targetLineShadow.numCapVertices = 10; 
    }

    Vector3 SetHeight(Vector3 actualPoint, float atHeight)
    {
        return actualPoint + Vector3.up * atHeight;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        //return;
        aimLine.gameObject.SetActive(false);
        aimHitCircle.gameObject.SetActive(false);
        deviationLine.gameObject.SetActive(false);
        targetLine.gameObject.SetActive(false);

        aimLineShadow.gameObject.SetActive(false);
        aimHitCircleShadow.gameObject.SetActive(false);
        deviationLineShadow.gameObject.SetActive(false);
        targetLineShadow.gameObject.SetActive(false);
    }

    public void Show()
    {
        InitLines();
        gameObject.SetActive(true);
        aimLine.gameObject.SetActive(true);
        aimHitCircle.gameObject.SetActive(true);
        deviationLine.gameObject.SetActive(true);
        targetLine.gameObject.SetActive(true);

        aimLineShadow.gameObject.SetActive(true);
        aimHitCircleShadow.gameObject.SetActive(true);
        deviationLineShadow.gameObject.SetActive(true);
        targetLineShadow.gameObject.SetActive(true);
    }

    void Follow() 
    {
        transform.position = gameController.cueBallBody.position;
    }

    public void TakeShot(float power,  Vector3 spinVector)
    {
        //cueBallBody.AddForce(cue.transform.forward * (powerFactor * slider.value), ForceMode.Acceleration);
        //cueBallBody.AddTorque(Vector3.Cross(cue.transform.forward, relativeSpin) * spinFactor, ForceMode.Acceleration);
        Vector3 relativeSpin = transform.rotation * spinVector;
        gameController.cueBallBody.velocity = transform.forward * (powerFactor * power);
        gameController.cueBallBody.angularVelocity = Vector3.Cross(transform.forward, relativeSpin) * spinFactor;
        gameController.OnShot();
    }

    // Update is called once per frame
    void Update()
    {
        Follow();
        DragAndMove();
        Aim();
    }


    
}
