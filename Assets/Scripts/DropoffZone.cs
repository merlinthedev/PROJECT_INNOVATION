using UnityEngine;

public class DropoffZone : MonoBehaviour {

    [SerializeField] private Collider m_Collider;
    [SerializeField] private System.Guid m_PlayerGuid;

    public void Start() {
        if (!NetworkManager.IsServer) {
            m_Collider.enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            var player = other.GetComponent<Player>();
            if (player != null) {

                if (m_PlayerGuid == System.Guid.Empty) {
                    if (player.dropoffZone == null) {
                        m_PlayerGuid = player.key;
                        player.dropoffZone = this;
                    }
                }

                // drop the items off
                if (player.key == m_PlayerGuid) {
                    player.DropOffItems();
                    Debug.Log("Dropped off items");
                }
            }
        }
    }

}
