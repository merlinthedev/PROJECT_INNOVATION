using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShoppingCartController : MonoBehaviour {
    
    private Vector3 smoothAcceleration = Vector3.zero;
    [SerializeField] private float smootSpeed = 5f;
    // Start is called before the first frame update

    [SerializeField] private AnimationCurve inputCurve;

    [SerializeField] private ButtonPressed gasButton;
    [SerializeField][Range(0, 1)] private float gasSpeed = 1f;
    [SerializeField] private ButtonPressed brakeButton;
    [SerializeField][Range(0, 1)] private float brakeSpeed = 0.5f;

    [SerializeField] private ShoppingCartMovement movement;

    //jumping with effects :tm:
    [SerializeField] private float jumpForce = 5f;
    private ParticleSystem jumpParticles;
    private bool isGrounded = true;
    private bool isJumping = false;

    void Start() {
        jumpParticles = transform.Find("jumpParticles").GetComponent<ParticleSystem>();
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

        movement.DoView(new Vector2(rotationInput, 0));

        Vector3 targetVelocity = Vector3.zero;
        float movementInput = 0;
        
        if (gasButton.isPressed || Input.GetKey(KeyCode.W)) {
            movementInput += gasSpeed;
        }

        if (brakeButton.isPressed || Input.GetKey(KeyCode.S)) {
            movementInput -= brakeSpeed;
        }

        movement.DoMove(new Vector2(0, movementInput));

        // added Jumping test thing
        if (isGrounded && Input.GetKeyDown(KeyCode.Space)) {
            GetComponent<Rigidbody>().AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;

            //with particles *gasp*
            if (jumpParticles != null) {
                isJumping = true;
                //jumpParticles.transform.position = transform.position + Vector3.down * 0.5f; // Spawn particles at player position
                jumpParticles.Play();
            }
        }


        
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.CompareTag("Ground")) {
            isGrounded = true;
        }
    }
    private void OnParticleSystemStopped() {
        isJumping = false;
    }
}
