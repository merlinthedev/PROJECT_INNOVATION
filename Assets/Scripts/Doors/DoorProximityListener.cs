using UnityEngine;

public class DoorProximityListener : MonoBehaviour {
    [SerializeField] private HingeJoint hinge;

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            // rotate door
            hinge.transform.rotation = Quaternion.Euler(0, 30, 0);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) {

            // rotate door
            hinge.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

}
