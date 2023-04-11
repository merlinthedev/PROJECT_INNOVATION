using UnityEngine;

public class LevelManager : MonoBehaviour {
    [SerializeField] private InteractableConfiguration interactableConfiguration;

    private void OnEnable() {
        // NetworkEventBus.Subscribe<TestNetworkEvent>(onTestNetworkEventClient);
        NetworkEventBus.Subscribe<ItemSpawnedEvent>(onItemSpawned);
        NetworkEventBus.Subscribe<ItemPickedUpEvent>(onItemPickedUp);
        NetworkEventBus.Subscribe<ItemsDroppedOffEvent>(onItemDroppedOff);
        NetworkEventBus.Subscribe<ScoreUpdatedEvent>(onScoreUpdated);
        NetworkEventBus.Subscribe<ItemsDiscardedEvent>(onItemsDiscarded);

        Debug.LogWarning("Level manager subscribed to events");
    }

    private void OnDisable() {
        // NetworkEventBus.Unsubscribe<TestNetworkEvent>(onTestNetworkEventClient);
        NetworkEventBus.Unsubscribe<ItemSpawnedEvent>(onItemSpawned);
        NetworkEventBus.Unsubscribe<ItemPickedUpEvent>(onItemPickedUp);
        NetworkEventBus.Unsubscribe<ItemsDroppedOffEvent>(onItemDroppedOff);
        NetworkEventBus.Unsubscribe<ScoreUpdatedEvent>(onScoreUpdated);
        NetworkEventBus.Unsubscribe<ItemsDiscardedEvent>(onItemsDiscarded);
    }

    private void onItemsDiscarded(ItemsDiscardedEvent itemsDiscardedEvent) {
        if (itemsDiscardedEvent.source != GameClient.getInstance().GetGuid()) {
            return;
        }

        Debug.LogWarning("Items discarded event received in the level manager");
        EventBus<InventoryUIEvent>.Raise(new InventoryUIEvent {
            shouldClear = true,
            discount = 0
        });

        foreach (var GUID in itemsDiscardedEvent.discardedItems) {
            var networkTransform = NetworkTransform.Transforms.TryGetValue(GUID, out NetworkTransform receivedNetworkTransform) ? receivedNetworkTransform : null;
            if (networkTransform != null) {
                networkTransform.gameObject.SetActive(true);
            } else {
                Debug.LogWarning("Network transform not found, cannot discard the item back into the world");
            }
        }
    }

    private void onScoreUpdated(ScoreUpdatedEvent scoreUpdatedEvent) {
        Debug.LogWarning("Score updated event received in the level manager");

        if (scoreUpdatedEvent.source != GameClient.getInstance().GetGuid()) {
            return;
        }

        EventBus<ScoreUIEvent>.Raise(new ScoreUIEvent(scoreUpdatedEvent.score));
    }

    private void onItemDroppedOff(ItemsDroppedOffEvent itemDroppedOffEvent) {
        if (itemDroppedOffEvent.source != GameClient.getInstance().GetGuid()) {
            return;
        }

        Debug.LogWarning("Item dropped off event received in the level manager");
        EventBus<InventoryUIEvent>.Raise(new InventoryUIEvent {
            shouldClear = true,
            discount = 0
        });

        foreach (var GUID in itemDroppedOffEvent.droppedItems) {
            var networkTransform = NetworkTransform.Transforms.TryGetValue(GUID, out NetworkTransform receivedNetworkTransform) ? receivedNetworkTransform : null;
            if (networkTransform != null) {
                Destroy(networkTransform.gameObject);
            } else {
                Debug.LogWarning("Network transform not found, cannot discard the item back into the world");
            }
        }
    }

    private void onTestNetworkEventClient(TestNetworkEvent testNetworkEvent) {
        Debug.LogWarning("Test network event received on the client");
    }

    private void onItemSpawned(ItemSpawnedEvent itemSpawnedEvent) {
        Debug.LogWarning("Item spawned event received");
        var itemPrefab = interactableConfiguration.interactables[itemSpawnedEvent.itemID].clientPrefab;
        var item = Instantiate(itemPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        item.SetKey(itemSpawnedEvent.itemGuid);
        item.Initialize();


        var uiComponent = item.gameObject.GetComponentInChildren<UIItemDiscountHelper>();

        if (uiComponent != null) {
            uiComponent.SetDiscount(itemSpawnedEvent.itemDiscount);
        } else {
            Debug.LogWarning("No UI component found on the item prefab");
        }
    }

    private void onItemPickedUp(ItemPickedUpEvent itemPickedUpEvent) {
        //disable item that is picked up
        NetworkTransform.Transforms.TryGetValue(itemPickedUpEvent.itemGuid, out NetworkTransform networkTransform);
        if (networkTransform != null) {
            networkTransform.gameObject.SetActive(false);
        } else {
            Debug.LogWarning("Network transform not found, cannot destroy the item");
        }

        //check if we picked up the item or someone else did
        if (itemPickedUpEvent.source != GameClient.getInstance().GetGuid()) {
            return;
        }
        // inform the UI when we picked it up
        EventBus<InventoryUIEvent>.Raise(new InventoryUIEvent {
            shouldClear = itemPickedUpEvent.shouldClear,
            discount = itemPickedUpEvent.discount,
        });

    }
}