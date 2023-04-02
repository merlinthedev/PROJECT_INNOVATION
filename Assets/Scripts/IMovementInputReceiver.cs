using UnityEngine;
public interface IMovementInputReceiver {
    public void DoMove(Vector2 inputVel);
    public void DoView(Vector2 viewValue);
}