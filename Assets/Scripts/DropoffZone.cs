using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropoffZone : MonoBehaviour {

    [SerializeField] private MeshCollider m_MeshCollider;
    
    public void Start() {
        if(!NetworkManager.IsServer) {
            m_MeshCollider.enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Player")) {
            var player = other.GetComponent<Player>();
            if(player != null) {
                // drop the items off
                player.DropOffItems();
                Debug.Log("Dropped off items");
            }
        }
    }

}
