using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Path : ScriptableObject
{
    private GameController gameController;
    //public GameObject markerPrefab;

    public List<GameObject> lines = new();

    public float radius;

    public float margin;

    public void SetGameController(GameController gc)
    {
        gameController = gc;
        radius = gc.cueBall.GetComponent<SphereCollider>().radius * gc.cueBall.transform.lossyScale.x;
        margin = radius * 0.001f;
    }

    public void FindForward(Vector3 initialPosition, Vector3 direction, int count)
    {
        if (count == 0) return;

        if (Physics.SphereCast(initialPosition, radius, direction, out RaycastHit hitInfo, 20))
        {
            if (hitInfo.transform.tag.StartsWith("Ball"))
            {
                Vector3 tangent = Vector3.RotateTowards(hitInfo.normal, direction, Mathf.PI/2, 0);
                Vector3 finalDirection1 = tangent;
                Vector3 finalDirection2 = -hitInfo.normal;
                Vector3 finalPosition1 = initialPosition + direction * hitInfo.distance;
                
                AddMarker(initialPosition, finalPosition1);

                //FindForward(finalPosition1, finalDirection1, count - 1);
                FindForward(hitInfo.transform.position, finalDirection2, count - 1);
            }
            else if (hitInfo.transform.tag == "Rail")
            {
                Vector3 tangent = Vector3.RotateTowards(hitInfo.normal, direction, Mathf.PI/2, 0);
                float e = (hitInfo.collider.material.bounciness + gameController.cueBall.GetComponent<SphereCollider>().material.bounciness) / 2;
                Vector3 finalDirection1 = (-e * Vector3.Dot(direction, hitInfo.normal) * hitInfo.normal + Vector3.Dot(direction, tangent) * tangent).normalized;
                Vector3 finalPosition1 = initialPosition + direction * hitInfo.distance;

                AddMarker(initialPosition, finalPosition1);

                FindForward(finalPosition1, finalDirection1, count - 1);
            }
            else
            {
                Vector3 finalPosition1 = initialPosition + direction * hitInfo.distance;
                AddMarker(initialPosition, finalPosition1);
            }
        }

    }


    public bool FindBackward(Ball ball, Vector3 targetPosition, out List<Vector3> toHitList)
    {
        Vector3 beginPosition = ball.transform.position;
        Vector3 v = targetPosition - beginPosition;
        toHitList = new();

        if ((ball.transform.position - targetPosition).magnitude < radius)
        {
            Vector3 toHit = (targetPosition - beginPosition).normalized * 2 * radius;
            toHitList.Add(beginPosition + Quaternion.AngleAxis(60, Vector3.up) * toHit);
            toHitList.Add(beginPosition + Quaternion.AngleAxis(120, Vector3.up) * toHit);
            toHitList.Add(beginPosition + Quaternion.AngleAxis(180, Vector3.up) * toHit);
            toHitList.Add(beginPosition + Quaternion.AngleAxis(240, Vector3.up) * toHit);
            toHitList.Add(beginPosition + Quaternion.AngleAxis(360, Vector3.up) * toHit);
            return true;
        }

        if (!Physics.SphereCast(beginPosition, radius, v, out RaycastHit hitInfo, v.magnitude - margin))
        {
            //AddMarker(beginPosition, targetPosition);
            Vector3 toHit = beginPosition - v.normalized * 2 * radius;
            toHitList.Add(toHit);
            return true;
        }
        return false;
    }


    public bool FindBackwardByRail(Ball ball, Vector3 targetPosition, GameObject rail, out Vector3 toHit, out Vector3 intermediateHit, out float feasibility)
    {
        Vector3 beginPosition = ball.transform.position;
        toHit = Vector3.zero;
        intermediateHit = Vector3.zero;
        feasibility = 0;

        GameObject meshGameObject = rail.transform.Find("Quad").gameObject;
        MeshRenderer face = meshGameObject.GetComponent<MeshRenderer>();
        MeshCollider meshCollider = meshGameObject.GetComponent<MeshCollider>();
        MeshFilter faceFilter = meshGameObject.GetComponent<MeshFilter>();

        Vector3 normal = face.transform.rotation * faceFilter.mesh.normals[0].normalized;

        GameObject fakeMesh = Instantiate(meshGameObject);
        fakeMesh.transform.rotation = meshGameObject.transform.rotation;
        fakeMesh.transform.position = meshGameObject.transform.position + normal * radius;
        fakeMesh.transform.localScale = new(10, 1, 1);
        MeshCollider fakeMeshCollider = fakeMesh.GetComponent<MeshCollider>();
        
        fakeMeshCollider.convex = true;

        //AddMarker(fakeMesh.transform.position, Vector3.zero);

        if (!fakeMeshCollider.Raycast(new Ray(beginPosition, -normal), out RaycastHit hit1, 20))
        {
            DestroyImmediate(fakeMesh);
            return false;
        }
        
        if (!fakeMeshCollider.Raycast(new Ray(targetPosition, -normal), out RaycastHit hit2, 20))
        {
            DestroyImmediate(fakeMesh);
            return false;
        }
            
        float ratio = hit1.distance / hit2.distance;
        Vector3 v = beginPosition + (targetPosition - beginPosition).magnitude * (ratio / (ratio + 1)) * (targetPosition - beginPosition).normalized;
        //Vector3 v = hit1.point + (hit2.point - hit1.point).normalized * (hit2.point - hit1.point).magnitude * ratio / (ratio + 1);    

        meshCollider.convex = true;
        DestroyImmediate(fakeMesh);

        if (!meshCollider.Raycast(new Ray(v, -normal), out RaycastHit hit3, 20))
        {
            meshCollider.convex = false;
            return false;
        }
    
        meshCollider.convex = false;

        Vector3 r1 = hit3.point + radius * normal - beginPosition;
        Vector3 r2 = targetPosition - hit3.point - radius * normal;

        if(Physics.SphereCast(beginPosition, radius, r1.normalized, out RaycastHit hit4, r1.magnitude - margin))
        {
            return false;
            // if (hit4.transform.gameObject != rail)
            // {
            // }
        }

        if(Physics.SphereCast(hit3.point, radius, r2.normalized, out RaycastHit hit5, r2.magnitude - margin))
        {
            return false;
        }

        //AddMarker(beginPosition, hit3.point);
        //AddMarker(hit3.point, targetPosition);
        //AddMarker(hit3.point + radius * normal, Vector3.zero);
        toHit = beginPosition - r1.normalized * 2 * radius;
        intermediateHit = hit3.point + radius * normal;
        feasibility = Mathf.Cos(Vector3.Angle(beginPosition - intermediateHit, targetPosition - intermediateHit) * Mathf.Deg2Rad);
        return true;
    }


    public bool FindBackwardByBallSelf(Ball ball1, Ball ball2, Vector3 targetPosition, out Vector3 toHit, out Vector3 intermediateHit, out float feasibility)
    {
        Vector3 beginPosition = ball1.transform.position;
        toHit = Vector3.zero;
        intermediateHit = Vector3.zero;
        feasibility = 0;

        Vector3 intermediatePosition = new (ball2.transform.position.x, beginPosition.y, ball2.transform.position.z);
        Vector3 vec1 = targetPosition - intermediatePosition;
        float d = vec1.magnitude;
        float x = Mathf.Sqrt(Mathf.Pow(d, 2) - Mathf.Pow(2 * radius, 2));
        float y = 2 * radius * x / d;
        float m = Mathf.Pow(2 * radius, 2) / d;
        Vector3 vec2 = Vector3.RotateTowards(vec1, beginPosition - intermediatePosition, Mathf.PI/2, 0);

        Vector3 p = intermediatePosition + vec1.normalized * m + vec2.normalized * y;

        if (Vector3.Dot(p - beginPosition, targetPosition - p) <= 0)
        {
            return false;
        }

        if (Vector3.Dot(p - beginPosition, intermediatePosition - p) <= 0)
        {
            return false;
        }

        if (Physics.SphereCast(beginPosition, radius, p - beginPosition, out RaycastHit hit1, (p - beginPosition).magnitude - margin))
        {
            return false;
            // if (hit1.transform.tag != ball2.tag)
            // {
            // }
        }

        if (Physics.SphereCast(p, radius, targetPosition - p, out RaycastHit hit2, (targetPosition - p).magnitude - margin))
        {
            return false;
            // if (hit2.transform.tag != ball2.tag)
            // {
            // }
        }

        //AddMarker(beginPosition, p);
        //AddMarker(p, targetPosition);
        toHit = beginPosition - (p - beginPosition).normalized * 2 * radius;
        intermediateHit = p;
        feasibility = Mathf.Cos(Vector3.Angle(intermediateHit - beginPosition, targetPosition - intermediateHit)) * (radius / ((targetPosition - p).magnitude + radius));
        return true;
    }


    public bool FindBackwardByBallTarget(Ball ball1, Ball ball2, Vector3 targetPosition, out Vector3 toHit, out float feasibility)
    {
        Vector3 beginPosition = ball1.transform.position;
        toHit = Vector3.zero;
        feasibility = 0;

        Vector3 intermediatePosition = new (ball2.transform.position.x, beginPosition.y, ball2.transform.position.z);
        Vector3 p = (intermediatePosition - targetPosition).normalized * (2 * radius) + intermediatePosition;
        
        if (Vector3.Dot(p - beginPosition, intermediatePosition - p) <= 0)
        {
            return false;
        }

        if (Physics.SphereCast(beginPosition, radius, p - beginPosition, out RaycastHit hit1, (p - beginPosition).magnitude - margin))
        {
            return false;
            // if (hit1.transform.tag != ball2.tag)
            // {
            // }
        }

        if (Physics.SphereCast(intermediatePosition, radius, targetPosition - intermediatePosition, out RaycastHit hit2, (targetPosition - intermediatePosition).magnitude - margin))
        {
            return false;
            // if (hit2.transform.tag != ball2.tag)
            // {
            // }
        }  

        AddMarker(beginPosition, p);
        AddMarker(p, targetPosition);  
        toHit = beginPosition - (p - beginPosition).normalized * 2 * radius;
        return true;  
    }


    public void ClearMarkers()
    {
        foreach (GameObject line in lines)
        {
            Destroy(line);
        }

        lines = new();
    }

    public void AddMarker(Vector3 pos1, Vector3 pos2)
    {
        GameObject g = new GameObject("Line");
        g.transform.SetParent(gameController.transform);
        
        LineRenderer l = g.AddComponent<LineRenderer>();
        l.startWidth =  0.05f;
        l.endWidth =  0.1f;
        l.positionCount = 2;
        l.SetPosition(0, pos1);
        l.SetPosition(1, pos2);

        lines.Add(g);
    }
}
