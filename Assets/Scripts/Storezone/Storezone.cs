using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Storezone : MonoBehaviour {
    [Tooltip("The discount that the player gets when they enter the storezone, should be a number between 0 and 1")]

    [SerializeField] private StoreDiscountDisplay storezoneDisplay;

    [SerializeField] private float storeDiscount = 0.1f;
    [SerializeField] private float storeDiscountChangeInterval = 60f;
    [SerializeField] private Collider storezoneCollider;

    [SerializeField] private float minDiscount = 0.1f;
    [SerializeField] private float maxDiscount = 0.5f;

    public List<Spawner> Spawners = new List<Spawner>();

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

        StartCoroutine(changeDiscount());

        //storezoneMap = GameObject.
    }

    private IEnumerator changeDiscount() {
        while (true) {

            storeDiscount = Random.Range(minDiscount, maxDiscount);

            foreach (var spawner in Spawners) {
                spawner.UpdateItemStats();
            }
            storezoneDisplay.UpdateColor(storeDiscount);

            yield return new WaitForSeconds(storeDiscountChangeInterval - 4.5f);
            NetworkEventBus.Raise(new PlayOneShotEvent {
                source = System.Guid.Empty,
                audioClipID = 5,
                position = Vector3.one,
                onServer = true,
            });
            yield return new WaitForSeconds(4.5f);
        }
    }

    public void OnTriggerEnter(Collider other) {
        Debug.Log("Entered storezone");
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            var player = other.gameObject.GetComponent<Player>();
            if (player != null) {
                if (!player.IsSafeToLeave) {
                    player.DiscardItems();
                } else {
                    player.FlagSafeItems();
                }
            }
        }
    }
}
