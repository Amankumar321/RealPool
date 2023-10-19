using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public bool hasBallInHand = false;
    private GameController gc;

    public bool isBot = false;

    public GameObject markerPrefab;

    // Start is called before the first frame update
    void Start()
    {
        gc = GameObject.Find("GameController").GetComponent<GameController>();
        gc.onTurnEvent.AddListener(CheckRecieveTurn);
    }

    public void RecieveBallInHand()
    {
        hasBallInHand = true;
    }

    public void CheckRecieveTurn()
    {
        if (isBot && this == gc.activePlayer)
        {
            AutomaticShot();
        }
    }

    public void MakeBot()
    {
        isBot = true;
    }

    IEnumerator TakeShot()
    {
        yield return new WaitForSeconds(2);
        gc.cue.TakeShot(100, Vector3.zero);
    }

    public void AutomaticShot()
    {
        Debug.Log("Bot playing");

        List<Ball> legalBalls = new();

        foreach (Ball b in gc.balls.GetAll())
        {
            if (gc.rules.IsLegalBall(b.tag))
            {
                legalBalls.Add(b);
            }
        }

        List<Vector3> availablePoints = new();
        List<Ball> availableBall = new();
        List<float> availableAngle = new();


        GameObject[] rails = GameObject.FindGameObjectsWithTag("Rail");
        GameObject[] pockets = GameObject.FindGameObjectsWithTag("Pocket");

        foreach (Ball b in legalBalls)
        {
            if (b.tag == "CueBall") continue;
            foreach (GameObject pocket in pockets)
            {
                Vector3 pocketTargetPosition = new (pocket.transform.position.x, b.transform.position.y, pocket.transform.position.z);
                Ray ray = new Ray(b.transform.position, b.transform.position - pocketTargetPosition);
                float radius = b.GetComponent<SphereCollider>().radius * b.transform.lossyScale.x;

                Vector3 contactPoint = ray.GetPoint(2 * radius);
                Vector3 start = gc.cueBall.transform.position;
                float length = (contactPoint - start).magnitude;
                RaycastHit[] hitAll = Physics.SphereCastAll(start, radius, contactPoint - start, length);
                bool canHitDirectly = true;

                float angleShot = Vector3.Angle(contactPoint - start, pocketTargetPosition - b.transform.position);
                if (angleShot > 90)
                {
                    continue;
                }

                foreach (RaycastHit hit in hitAll)
                {
                    if (!(hit.transform.tag == "CueBall" || hit.transform.tag == b.transform.tag)) {
                        //Debug.Log(hit.transform.name + "when hitting " +  b.transform.tag);
                        canHitDirectly = false;
                        break;  
                    }
                }

                Vector3 directionPot = pocketTargetPosition - b.transform.position;
                float lengthPot = directionPot.magnitude;

                RaycastHit[] potAll = Physics.SphereCastAll(b.transform.position, radius, directionPot, lengthPot);
                bool canPotDirectly = true;

                foreach (RaycastHit hit in potAll)
                {
                    if (!(hit.transform.tag == "Pocket" || hit.transform.tag == b.transform.tag)) {
                        //Debug.Log(hit.transform.name + "when potting " +  b.transform.tag);
                        canPotDirectly = false;
                        break;
                    }
                }

                if (canHitDirectly && canPotDirectly)
                {
                    availablePoints.Add(contactPoint);
                    availableBall.Add(b);
                    availableAngle.Add(angleShot);
                }
            }
        }

        foreach (Vector3 pos in  availablePoints)
        {
            //Debug.Log(b.tag);
            //GameObject g = Instantiate(markerPrefab);
            //g.transform.position = pos;
        }

        Debug.Log("Available Shots: " + availablePoints.Count);

        if (availablePoints.Count > 0)
        {
            //int randomInt = UnityEngine.Random.Range(0, availablePoints.Count);
            int index = 0;
            float highestPercentage = 0;
            for (int i = 0; i < availablePoints.Count; i++)
            {
                if (1 / availableAngle[i] > highestPercentage) {
                    index = i;
                }
            }

            gc.cue.transform.forward = availablePoints[index] - gc.cueBall.transform.position;
            
            StartCoroutine(TakeShot());
        }

        else
        {
            Debug.Log("No point");
        }

        //gameObject.GetComponent<BoxCollider>().boun
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
