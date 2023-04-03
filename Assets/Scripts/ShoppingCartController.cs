using UnityEngine;



public class ShoppingCartController : MonoBehaviour {

    private Vector3 smoothAcceleration = Vector3.zero;
    [SerializeField] private float smootSpeed = 5f;
    // Start is called before the first frame update

    [SerializeField] private AnimationCurve inputCurve;

    [SerializeField] private ButtonPressed gasButton;
    [SerializeField][Range(0, 1)] private float gasSpeed = 1f;
    [SerializeField] private ButtonPressed brakeButton;
    [SerializeField][Range(0, 1)] private float brakeSpeed = 0.5f;

    private IMovementInputReceiver movementInputReceiver;

    private void Awake() {
        GameClient.getInstance().ReceivePlayerTransform(transform);
        movementInputReceiver = GetComponent<IMovementInputReceiver>();
    }

    // Update is called once per frame
    void FixedUpdate() {
        Vector3 acceleration = -Input.acceleration;
        smoothAcceleration = Vector3.Lerp(smoothAcceleration, acceleration, smootSpeed * Time.deltaTime);

        float rotationInput = inputCurve.Evaluate(Mathf.Abs(smoothAcceleration.x)) * -Mathf.Sign(smoothAcceleration.x);


        if (Input.GetKey(KeyCode.A)) {
            rotationInput = -0.5f;
        }

        if (Input.GetKey(KeyCode.D)) {
            rotationInput = 0.5f;
        }

        // needs to be passed to the server
        movementInputReceiver.DoView(new Vector2(rotationInput, 0));

        Vector3 targetVelocity = Vector3.zero;
        float movementInput = 0;

        if (gasButton.isPressed || Input.GetKey(KeyCode.W)) {
            movementInput += gasSpeed;
        }

        if (brakeButton.isPressed || Input.GetKey(KeyCode.S)) {
            // movementInput -= brakeSpeed;
            movementInput -= gasSpeed;
        }




        // needs to be passed to the server
        movementInputReceiver.DoMove(new Vector2(0, movementInput));

    }
}
