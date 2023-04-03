using UnityEngine;

public class ClientCartController : MonoBehaviour, IMovementInputReceiver {

    private Vector2 move;
    private Vector2 view;

    public void Start() {
    }

    public void DoMove(Vector2 inputVel) {
        move = inputVel;
    }
    
    public void DoView(Vector2 viewValue) {
        view = viewValue;
    }
    public InputPacket GetInputPacket() {
        InputPacket inputPacket = new InputPacket();
        inputPacket.move = move;
        inputPacket.view = view;
        return inputPacket;
    }
}
