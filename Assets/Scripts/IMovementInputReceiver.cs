using UnityEngine;

public interface IMovementInputReceiver {
    public void DoMove(Vector2 inputVel);
    public void DoView(Vector2 viewValue);
    public void DoJump();
    public void DoBouce(Vector3 force);
}