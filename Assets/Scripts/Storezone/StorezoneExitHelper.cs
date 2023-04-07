using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorezoneExitHelper : MonoBehaviour {

    [SerializeField] private BoxCollider storezoneExitCollider;

    private void Awake() {
        if (!NetworkManager.IsServer) {
            storezoneExitCollider.enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            var player = other.gameObject.GetComponent<Player>();

            if (player != null) {
                player.IsSafeToLeave = true;
                // start a coroutine to reset the flag after a few seconds
                StartCoroutine(player.ResetSafeToLeaveFlag());
            }
        }
    }
}
