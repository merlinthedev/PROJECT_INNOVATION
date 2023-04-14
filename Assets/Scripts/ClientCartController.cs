using UnityEngine;

public class ClientCartController : MonoBehaviour, IMovementInputReceiver {

    private Vector2 move;
    private Vector2 view;
    private bool jump = false;

    public void Start() {
    }

    public void DoMove(Vector2 inputVel) {
        move = inputVel;
    }

    public void DoView(Vector2 viewValue) {
        view = viewValue;
    }

    public void DoJump() {
        jump = true;
    }

    public void DoBouce(Vector3 force) {
        throw new System.NotImplementedException();
    }
    public InputPacket GetInputPacket() {
        InputPacket inputPacket = new InputPacket();
        inputPacket.move = move;
        inputPacket.view = view;
        inputPacket.jump = jump;
        jump = false;
        return inputPacket;
    }
}
