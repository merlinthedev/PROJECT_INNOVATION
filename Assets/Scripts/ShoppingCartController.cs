using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShoppingCartController : MonoBehaviour {
    Rigidbody rb;

    private Vector3 smoothAcceleration = Vector3.zero;
    [SerializeField] private float smootSpeed = 5f;
    // Start is called before the first frame update

    [SerializeField] private float rotationSpeed = 90f;
    [SerializeField] private float rotationForce = 1f;
    [SerializeField] private float maxRotationForce = 1f;

    [SerializeField] private AnimationCurve inputCurve;

    [SerializeField] private ButtonPressed gasButton;
    [SerializeField] private ButtonPressed brakeButton;
    [SerializeField] private float forwardSpeed = 1f;
    [SerializeField] private float backwardSpeed = 1f;
    [SerializeField] private float maxForce = 1f;

    void Start() {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate() {
        Vector3 acceleration = -Input.acceleration;
        smoothAcceleration = Vector3.Lerp(smoothAcceleration, acceleration, smootSpeed * Time.deltaTime);

        float targetRotationSpeed = inputCurve.Evaluate(Mathf.Abs(smoothAcceleration.x)) * -Mathf.Sign(smoothAcceleration.x) * rotationSpeed;


        Vector3 targetVelocity = Vector3.zero;
        
        if (gasButton.isPressed) {
            targetVelocity += transform.forward * forwardSpeed;
        }

        if (brakeButton.isPressed) {
            targetVelocity += -transform.forward * backwardSpeed;
            targetRotationSpeed *= Mathf.Sign(Vector3.Dot(rb.velocity, transform.forward));
        }

        rb.angularVelocity = new Vector3(0, targetRotationSpeed, 0);
        rb.velocity = Vector3.Lerp(rb.velocity, targetVelocity, Time.deltaTime * maxForce);
        
    }
}
