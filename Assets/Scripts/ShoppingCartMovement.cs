using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShoppingCartMovement : MonoBehaviour {
    
    #region settings
    [Header("Locomotion")]
    [SerializeField] private Transform pivot;
    [SerializeField] private float maxSpeed = 8;
    [SerializeField] private float Acceleration = 10;
    [SerializeField] private AnimationCurve AccelerationFactorFromDot;
    [SerializeField] private float MaxAccelerationForce = 150;
    [SerializeField] private AnimationCurve MaxAccelerationForceFactorFromDot;
    [SerializeField] private Vector3 ForceScale = new Vector3(1, 0, 1);
    [SerializeField] private float GravityScaleDrop = 10;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 2;
    [SerializeField] private bool viewDesiredDirection = false;
    [SerializeField] private AnimationCurve rotationSpeedFactorFromMagnitude;
    [SerializeField] private AnimationCurve rotationDampingCurve;

    #endregion
    
    #region private variables

    private float m_MovementControlDisabledTimer = 0;

    private Vector2 viewValue = Vector2.zero;
    private Vector2 inputVelocity = Vector2.zero;

    private Rigidbody rb;
    
    #endregion


    // Start is called before the first frame update
    void Start() {
        rb = GetComponent<Rigidbody>();
        if (pivot == null)
            pivot = transform;
    }

    // Update is called once per frame
    void FixedUpdate() {
        #region movement
        //map input from pivot to world space on xz plane
        Vector3 inputVel = new Vector3(inputVelocity.x, 0, inputVelocity.y);
        if (inputVel.magnitude > 1)
            inputVel.Normalize();

        Vector3 unitGoal = Quaternion.Euler(0, pivot.eulerAngles.y, 0) * inputVel;

        if (m_MovementControlDisabledTimer > 0) {
            unitGoal = Vector3.zero;
            m_MovementControlDisabledTimer -= Time.fixedDeltaTime;
        }

        Vector3 goalVel = unitGoal * maxSpeed;
        Vector3 unitVel = goalVel.normalized;

        float velDot = Vector3.Dot(unitGoal, unitVel);
        float accel = Acceleration * AccelerationFactorFromDot.Evaluate(velDot);
        float maxAccelForceFactor = MaxAccelerationForceFactorFromDot.Evaluate(velDot);

        Vector3 neededAccel = (goalVel - rb.velocity) / Time.fixedDeltaTime;

        float maxAccel = MaxAccelerationForce * MaxAccelerationForceFactorFromDot.Evaluate(velDot) * maxAccelForceFactor;

        neededAccel = Vector3.ClampMagnitude(neededAccel, maxAccel);

        rb.AddForce(Vector3.Scale(neededAccel * rb.mass, ForceScale));
        #endregion

        #region rotation
        //rotate the player
        if (viewDesiredDirection) {
            Vector3 viewDirection = new Vector3(viewValue.x, 0, viewValue.y);

            //transform the input vector into world space
            Vector3 inputDir = Quaternion.Euler(0, pivot.eulerAngles.y, 0) * viewDirection;

            if (viewDirection.magnitude > 1)
                viewDirection.Normalize();
            float rotationSpeedFactor = rotationSpeedFactorFromMagnitude.Evaluate(viewDirection.magnitude);

            float yAngleDifference = Vector3.SignedAngle(transform.forward, inputDir, Vector3.up);
            yAngleDifference = Mathf.Clamp(yAngleDifference, -rotationSpeed * rotationSpeedFactor, rotationSpeed * rotationSpeedFactor);

            float damping = 1 - rotationDampingCurve.Evaluate(Mathf.Abs(rb.angularVelocity.y / Time.fixedDeltaTime / rotationSpeed));
            yAngleDifference *= damping;

            rb.AddRelativeTorque(Vector3.up * yAngleDifference);
        } else {
            rb.AddRelativeTorque(0, viewValue.x * rotationSpeed, 0);
        }
        #endregion

        #region misc
        //add extra gravity to the player
        rb.AddForce(Vector3.down * GravityScaleDrop * rb.mass);
        #endregion
    }

    public void DoMove(Vector2 inputVel) {
        inputVelocity = inputVel;
    }

    public void DoView(Vector2 newViewValue) {
        viewValue = newViewValue;
    }
}
