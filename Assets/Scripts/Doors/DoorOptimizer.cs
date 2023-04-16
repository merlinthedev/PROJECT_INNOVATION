using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOptimizer : MonoBehaviour {
    [SerializeField] private HingeJoint hinge;
    [SerializeField] private Rigidbody doorRigidbody;

    public void Start() {
        if (!NetworkManager.IsServer) {
            Destroy(hinge);
            Destroy(doorRigidbody);
        }
    }
}
