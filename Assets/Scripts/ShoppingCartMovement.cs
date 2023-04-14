using UnityEngine;
using UnityEngine.Events;

public class ShoppingCartMovement : MonoBehaviour, IMovementInputReceiver {

    #region settings
    [Header("Movement")]
    [SerializeField] private float MaxSpeed = 10f;
    [SerializeField] private float Stickyness = 0.6f;
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

    private bool isBoosting { get => boostStartTime + boostDuration >= Time.time; }
    private float boostStartTime = 0f;
    private float boostMultiplier = 1f;
    private float boostDuration = 0f;
    public bool IsBoosting { get => isBoosting; }
    public float BoostProgress { get => Mathf.Clamp01((Time.time - boostStartTime) / boostDuration); }

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
            // visualize the raycast
            Debug.DrawRay(ray.origin, ray.direction * 1.2f, Color.red);
            if (groundHit) {
                lastGroundTime = Time.time;
            }
        }
        #endregion

        #region movement
        if (isOnGround) {
            float forwardInput = Mathf.Clamp(inputVelocity.y, -1, 1);
            float speedFactor = isBoosting ? MaxSpeed * boostMultiplier : MaxSpeed;
            speedFactor *= Stickyness;
            forwardInput *= speedFactor;
            Vector3 force = transform.forward * forwardInput;
            force -= rb.velocity * Stickyness;
            force *= Acceleration;


            Ray collisionRay = new Ray();
            RaycastHit collisionHit;
            if (rb != null) {
                collisionRay.origin = transform.position;
                collisionRay.direction = Vector3.forward;
                bool collisionHitGround = Physics.Raycast(collisionRay, out collisionHit, 1.2f);
                // visualize the raycast
                Debug.DrawRay(collisionRay.origin, collisionRay.direction * 1.2f, Color.yellow);
                if (collisionHitGround) {
                    if (collisionHit.collider.tag == "Player") {
                        // bounce off the player
                        var playerObject = collisionHit.collider.gameObject;
                        var otherMover = playerObject.GetComponent<IMovementInputReceiver>();

                        // DoBouce(force);
                        // otherMover.DoBouce(force);

                        Debug.Log("BOUCE BOUCE BOUCE BITCH");
                    }
                }
            }

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
        // Q: Is it possible to check if we collided with an object with a specific tag?
        // A: Yes, you can use the tag property of the collider you hit.
        //    For example, if you hit a collider with the tag "Player", you can do this:

        #endregion
    }

    public void DoMove(Vector2 inputVel) {
        inputVelocity = inputVel;
    }

    public void DoView(Vector2 newViewValue) {
        viewValue = newViewValue;
    }

    public void DoBouce(Vector3 force) {
        // Create a vector 3 that moves the player up and backwards
        // Add force to the rigidbody
        // rb.AddForce(Vector3.up * 10f, ForceMode.Impulse);

        rb.AddForce((Vector3.up + -force) * 10f, ForceMode.Impulse);
    }

    public void DoJump() {
        if (isOnGround && Time.time - lastJumpTime > JumpCooldown) {
            rb.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
            lastJumpTime = Time.time;
            JumpEvent.Invoke();
        }
    }

    public void Boost(float boostMultiplier, float boostDuration) {
        this.boostMultiplier = boostMultiplier;
        this.boostDuration = boostDuration;
        boostStartTime = Time.time;
    }
}
