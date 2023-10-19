using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class AutoShoot : MonoBehaviour
{
    private GameController gameController;

    private int callCount = 0;
    private bool toTakeShot = false;
    List<Vector3> availableShots = new();
    List<float> availableShotsProbability = new();
    //List<float> availableShotsLength = new();
    List<float> availableShotsLength = new();
    List<float> availableShotsDepth = new();


    private float averageProbability;
    private float highestProbability;


    // Start is called before the first frame update
    void Start()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        transform.GetComponent<Button>().onClick.AddListener(OnClick);
    }

   
    void OnClick()
    {
        gameController.cue.path.SetGameController(gameController);
        gameController.cue.path.ClearMarkers();

        GameObject[] pockets = GameObject.FindGameObjectsWithTag("Pocket");
        GameObject[] rails = GameObject.FindGameObjectsWithTag("Rail");
        List<Ball> balls = gameController.balls.GetAll();
        
        Ball cueBall = gameController.balls.GetBall("CueBall");

        float height =  gameController.cueBall.transform.position.y;

        availableShots = new();
        availableShotsProbability = new();
        averageProbability = 0;
        highestProbability = 0;

        float probability = 1;

        foreach (GameObject pocket in pockets)
        {
            Transform transformAims = pocket.transform.Find("AimSpots");

            if (pocket.name.StartsWith("Corner"))
            {
                probability = 1;
            }
            if (pocket.name.StartsWith("Side"))
            {
                probability = 0.7f;
            }

            foreach (Transform transformAim in transformAims)
            {
                Vector3 targetPos = new (transformAim.position.x, height, transformAim.position.z);


                foreach (Ball b in balls)
                {
                    if (gameController.rules.CheckPottableBall(b.tag) == false) continue;
                    List<Ball> newBalls = balls.ToList();
                    newBalls.Remove(b);
                    List<Vector3> path = new List<Vector3>();
                    StartCoroutine(FindShots(b, targetPos, newBalls, rails, cueBall, probability, path, Vector3.zero, 0, 1));
                }
            }
        }
        
        toTakeShot = true;
    }


    IEnumerator TakeShot()
    {
        if (toTakeShot && callCount == 0)
        {
            Debug.Log("shot");
            if (availableShots.Count > 0)
            {
                float[] shotProbabilities = availableShotsProbability.ToArray();
                Vector3[] shotDirections = availableShots.ToArray();
                float[] shotLengths = availableShotsLength.ToArray();
                float[] shotDepths = availableShotsDepth.ToArray();

                Array.Sort(shotProbabilities.ToArray(), shotDirections);
                Array.Sort(shotProbabilities.ToArray(), shotLengths);
                Array.Sort(shotProbabilities.ToArray(), shotDepths);
                Array.Sort(shotProbabilities);

                int lastIdx = shotProbabilities.Length - 1;

                gameController.cue.transform.forward = shotDirections[lastIdx];
                float feasibility = shotProbabilities[lastIdx] * shotLengths[lastIdx] * shotDepths[lastIdx];
                float power =  shotLengths[lastIdx] * 1.5f * shotDepths[lastIdx] / feasibility;

                yield return new WaitForSeconds(3);
                gameController.cue.TakeShot(Mathf.Min(power, 100), Vector3.zero);
            }
            toTakeShot = false;
        }
        yield return null;
    }


    IEnumerator FindShots(Ball ball, Vector3 targetPos, List<Ball> balls, GameObject[] rails, Ball cueBall, float probability, List<Vector3> path, Vector3 previousVector, float totalDistance, int depthCount)
    {
        callCount++;

        if (depthCount > 4)
        {
            callCount--;
            yield break;
        }

        if (availableShots.Count > 0)
        {
            if (probability * (1 / (totalDistance * depthCount)) < highestProbability)
            {
                callCount--;
                yield break;
            }
        }

        if (gameController.cue.path.FindBackward(ball, targetPos, out List<Vector3> toHit1List))
        {
            foreach (Vector3 toHit1 in toHit1List)
            {
                List<Vector3> pathself = path.ToList();
                pathself.Add(targetPos);
                pathself.Add(toHit1);

                float dist = (targetPos - toHit1).magnitude;
                float feasibilityIntermediate = 1;

                if (previousVector.Equals(Vector3.zero) == false)
                {
                    feasibilityIntermediate = Mathf.Cos(Vector3.Angle(targetPos - toHit1, previousVector) * Mathf.Deg2Rad);
                }

                float factor = 1 / ((dist + totalDistance) * depthCount);
                float prob = probability * feasibilityIntermediate * factor;
        
                if (ball == cueBall && prob > highestProbability)
                {
                    availableShots.Add(targetPos - toHit1);
                    availableShotsProbability.Add(prob);
                    availableShotsLength.Add(dist + totalDistance);
                    availableShotsDepth.Add(depthCount);
                    averageProbability = (averageProbability * (availableShots.Count - 1) + prob) / availableShots.Count;

                    highestProbability = Mathf.Max(highestProbability, prob);

                    for (int i = 0; i < pathself.Count - 1; i++)
                    {
                        gameController.cue.path.AddMarker(pathself[i], pathself[i + 1]);  
                    }
                }

                foreach (Ball otherBall in balls)
                {
                    if (otherBall == cueBall && !gameController.rules.CheckHitableBall(ball.tag)) continue;
                    List<Ball> newBalls = balls.ToList();
                    newBalls.Remove(otherBall);
                    yield return StartCoroutine(FindShots(otherBall, toHit1, newBalls, rails, cueBall, probability * feasibilityIntermediate, pathself, targetPos - toHit1, totalDistance + dist, depthCount + 1));
                }
            }
        }


        foreach (Ball otherBall1 in balls)
        {
            if (ball == cueBall && !gameController.rules.CheckHitableBall(otherBall1.tag)) continue;
            if (gameController.cue.path.FindBackwardByBallSelf(ball, otherBall1, targetPos, out Vector3 toHit2, out Vector3 intermediateHit, out float feasibility))
            {
                List<Vector3> pathself = path.ToList();
                pathself.Add(targetPos);
                pathself.Add(intermediateHit);
                pathself.Add(toHit2);

                float dist = (toHit2 - intermediateHit).magnitude + (intermediateHit - targetPos).magnitude;

                float feasibilityIntermediate = 1;

                if (previousVector.Equals(Vector3.zero) == false)
                {
                    feasibilityIntermediate = Mathf.Cos(Vector3.Angle(targetPos - intermediateHit, previousVector) * Mathf.Deg2Rad);
                }

                float factor = 1 / ((dist + totalDistance) * depthCount);
                float prob = probability * feasibility * feasibilityIntermediate * factor * 0.7f;

                if (ball == cueBall && prob > highestProbability)
                {
                    availableShots.Add(intermediateHit - toHit2);
                    availableShotsProbability.Add(prob);
                    availableShotsLength.Add(dist + totalDistance);
                    availableShotsDepth.Add(depthCount);
                    averageProbability = (averageProbability * (availableShots.Count - 1) + prob) / availableShots.Count;
                
                    highestProbability = Mathf.Max(highestProbability, prob);

                    for (int i = 0; i < pathself.Count - 1; i++)
                    {
                        gameController.cue.path.AddMarker(pathself[i], pathself[i + 1]);   
                    }
                }

                List<Ball> newBalls1 = balls.ToList();
                newBalls1.Remove(otherBall1);

                foreach (Ball otherBall2 in balls)
                {
                    if (otherBall2 == cueBall && !gameController.rules.CheckHitableBall(ball.tag)) continue;
                    List<Ball> newBalls2 = newBalls1.ToList();
                    newBalls2.Remove(otherBall2);
                    yield return StartCoroutine(FindShots(otherBall2, toHit2, newBalls2, rails, cueBall, probability * feasibility * feasibilityIntermediate, pathself, intermediateHit - toHit2, dist + totalDistance, depthCount + 2));
                    break;
                }
            }
        }

        foreach (GameObject rail in rails)
        {
            if (gameController.cue.path.FindBackwardByRail(ball, targetPos, rail, out Vector3 toHit3, out Vector3 intermediateHit, out float feasibility))
            {
                List<Vector3> pathself = path.ToList();
                pathself.Add(targetPos);
                pathself.Add(intermediateHit);
                pathself.Add(toHit3);

                float dist = (toHit3 - intermediateHit).magnitude + (intermediateHit - targetPos).magnitude;

                float feasibilityIntermediate = 1;

                if (previousVector.Equals(Vector3.zero) == false)
                {
                    feasibilityIntermediate = Mathf.Cos(Vector3.Angle(targetPos - intermediateHit, previousVector) * Mathf.Deg2Rad);
                }

                float factor = 1 / ((dist + totalDistance) * depthCount);
                float prob = probability * feasibility * feasibilityIntermediate * factor;

                if (ball == cueBall && prob > highestProbability)
                {
                    availableShots.Add(intermediateHit - toHit3);
                    availableShotsProbability.Add(prob);
                    availableShotsLength.Add(dist + totalDistance);
                    availableShotsDepth.Add(depthCount);
                    averageProbability = (averageProbability * (availableShots.Count - 1) + prob) / availableShots.Count;
       
                    highestProbability = Mathf.Max(highestProbability, prob);

                    for (int i = 0; i < pathself.Count - 1; i++)
                    {
                        gameController.cue.path.AddMarker(pathself[i], pathself[i + 1]);   
                    }
                }
                
                foreach (Ball otherBall in balls)
                {
                    if (otherBall == cueBall && !gameController.rules.CheckHitableBall(ball.tag)) continue;
                    List<Ball> newBalls = balls.ToList();
                    newBalls.Remove(otherBall);
                    yield return StartCoroutine(FindShots(otherBall, toHit3, newBalls, rails, cueBall, probability * feasibility * feasibilityIntermediate, pathself, intermediateHit - toHit3, dist + totalDistance, depthCount + 2));
                }
            }
        }


        callCount--;
        StartCoroutine(TakeShot());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
