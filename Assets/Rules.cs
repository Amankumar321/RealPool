using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRules
{    
    public void CheckRules();
    public bool IsLegalBall(string ball);

    public List<Ball> GetHitableBalls();
    public List<Ball> GetPottableBalls();

    public bool CheckHitableBall(string ball);
    public bool CheckPottableBall(string ball);
}