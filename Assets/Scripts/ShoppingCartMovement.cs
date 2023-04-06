using UnityEngine;
using UnityEngine.Events;

public class ShoppingCartMovement : MonoBehaviour, IMovementInputReceiver {

    #region settings
    [Header("Movement")]
    [SerializeField] private float MaxSpeed = 10f;
    [SerializeField] private float BoostMultiplier = 2.5f;
    [SerializeField] private float Stickyness = 0.6f;
    [SerializeField] private bool boosting = false;
    [SerializeField] private float Acceleration = 10f;
    [SerializeField] private float GroundedTime = 0.1f;

    [Header("Rotation")]
    [SerializeField] private float ConstantSteerSpeed = 0f;
    [SerializeField] private float DynamicSteerSpeed = 2f;
    [SerializeField] private float MaxSteerSpeed = 2f;
    private float SteerSpeed {
        get {
            float forwardSpeed = Vector3.Dot(rb.velocity, transform.forward);
            return Mathf.Clamp(ConstantSteerSpeed + DynamicSteerSpeed * forwardSpeed, -MaxSteerSpeed, MaxSteerSpeed);
        }
    }

    [Header("Jumping")]
    [SerializeField] private float JumpForce = 5f;
    [SerializeField] private float JumpCooldown = 0.5f;
    [SerializeField] private UnityEvent JumpEvent;
    private float lastJumpTime = 0f;

    #endregion

    #region private variables

    private float movementControlDisabledTimer = 0;

    private Vector2 viewValue = Vector2.zero;
    private Vector2 inputVelocity = Vector2.zero;

    private float lastGroundTime = 0f;
    private bool isOnGround => Time.time - lastGroundTime < GroundedTime;

    private Rigidbody rb;

    #endregion


    // Start is called before the first frame update
    void Start() {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate() {
        #region groundCheck
        Ray ray = new Ray();
        RaycastHit hit;
        if (rb != null) {
            ray.origin = transform.position;
            ray.direction = Vector3.down;
            bool groundHit = Physics.Raycast(ray, out hit, 1.2f);
            if (groundHit) {
                lastGroundTime = Time.time;
            }
        }
        #endregion

        #region movement
        if (isOnGround) {
            float forwardInput = Mathf.Clamp(inputVelocity.y, -1, 1);
            float speedFactor = boosting ? MaxSpeed * BoostMultiplier : MaxSpeed;
            speedFactor *= Stickyness;
            forwardInput *= speedFactor;
            Vector3 force = transform.forward * forwardInput;
            force -= rb.velocity * Stickyness;
            force *= Acceleration;

            rb.AddForce(force, ForceMode.Force);
        }
        #endregion

        #region rotation
        //rotate the player
        float steerAmount = Mathf.Clamp(viewValue.x, -1, 1) * SteerSpeed;
        Quaternion rotationDelta = Quaternion.Euler(0, steerAmount * Time.fixedDeltaTime, 0);
        transform.rotation *= rotationDelta;
        #endregion

        #region misc
        #endregion
    }

    public void DoMove(Vector2 inputVel) {
        inputVelocity = inputVel;
    }

    public void DoView(Vector2 newViewValue) {
        viewValue = newViewValue;
    }

    public void DoJump() {
        if (isOnGround && Time.time - lastJumpTime > JumpCooldown) {
            rb.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
            lastJumpTime = Time.time;
            JumpEvent.Invoke();
        }
    }
}
