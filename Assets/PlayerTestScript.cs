using UnityEngine;
public class PlayerTestScript : MonoBehaviour {

    [SerializeField] private Rigidbody playerRigidbody;
    [SerializeField] private float speed;

    private Vector3 move;

    private void Update() {

        if (Input.GetKey(KeyCode.W)) {
            move.z = -1;
        } else if (Input.GetKey(KeyCode.S)) {
            move.z = 1;
        } else {
            move.z = 0;
        }

        if (Input.GetKey(KeyCode.A)) {
            move.x = 1;
        } else if (Input.GetKey(KeyCode.D)) {
            move.x = -1;
        } else {
            move.x = 0;
        }

        move = move.normalized;

        playerRigidbody.MovePosition(transform.position + move * speed * Time.deltaTime);
    }
}
