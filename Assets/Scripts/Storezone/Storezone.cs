using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Storezone : MonoBehaviour {
    [Tooltip("The discount that the player gets when they enter the storezone, should be a number between 0 and 1")]
    [SerializeField] private float storeDiscount = 0.1f;
    [SerializeField] private float storeDiscountChangeInterval = 60f;
    [SerializeField] private BoxCollider storezoneCollider;

    [SerializeField] private float minDiscount = 0.1f;
    [SerializeField] private float maxDiscount = 0.5f;

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
    }

    private IEnumerator changeDiscount() {
        while (true) {
            storeDiscount = Random.Range(minDiscount, maxDiscount);

            ItemDiscountUpdateEvent itemDiscountUpdateEvent = new ItemDiscountUpdateEvent();
            itemDiscountUpdateEvent.source = System.Guid.Empty;
            itemDiscountUpdateEvent.discount = storeDiscount;

            itemDiscountUpdateEvent.influencedItems.Clear();

            // loop through all the items in Item.Items, check for their spawner and if the spawner has this has a reference to Storezone, update the discount
            foreach (var item in Item.Items) {
                if (item.Storezone == this) {
                    item.itemStats.discount = storeDiscount;
                    var key = item.GetComponent<NetworkTransform>().Key;
                    itemDiscountUpdateEvent.influencedItems.Add(key);
                }
            }

            // Debug.Log("Raising network event with " + itemDiscountUpdateEvent.influencedItems.Count + " items.");

            NetworkEventBus.Raise(itemDiscountUpdateEvent);
            yield return new WaitForSeconds(storeDiscountChangeInterval);
        }
    }


    private void OnTriggerExit(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            var player = other.gameObject.GetComponent<Player>();
            if (player != null) {
                if (!player.IsSafeToLeave) {
                    player.DiscardItems();
                }
            }
        }
    }


}
