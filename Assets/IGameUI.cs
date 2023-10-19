using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGameUI
{
    public void Refresh();
    public void OnSwitchTurn();
    public void OnShot();

    public void AddBallDisplay(string ball);
}
