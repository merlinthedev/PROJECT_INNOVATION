using UnityEngine;
using System;

public interface IMovementInputReceiver {
    public void DoMove(Vector2 inputVel);
    public void DoView(Vector2 viewValue);
}