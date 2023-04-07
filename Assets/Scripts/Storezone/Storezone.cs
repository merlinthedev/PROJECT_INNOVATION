using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Storezone : MonoBehaviour {
    [Tooltip("The discount that the player gets when they enter the storezone, should be a number between 0 and 1")]
    [SerializeField] private float storeDiscount = 0.1f;

    [SerializeField] private BoxCollider storezoneCollider;

    public float StoreDiscount {
        get {
            return storeDiscount;
        }
        set {
            storeDiscount = value;
        }
    }

    private void Awake() {
        if (!NetworkManager.IsServer) {
            storezoneCollider.enabled = false;
        }
    }


    private void OnTriggerExit(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            other.gameObject.GetComponent<Player>().DiscardItems();
        }
    }

}
