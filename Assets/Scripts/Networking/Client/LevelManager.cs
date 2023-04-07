using UnityEngine;

public class LevelManager : MonoBehaviour {

    [SerializeField] private GameObject itemPrefab;

    private void OnEnable() {
        // NetworkEventBus.Subscribe<TestNetworkEvent>(onTestNetworkEventClient);
        NetworkEventBus.Subscribe<ItemSpawnedEvent>(onItemSpawned);
        NetworkEventBus.Subscribe<ItemPickedUpEvent>(onItemPickedUp);
        NetworkEventBus.Subscribe<ItemDroppedOffEvent>(onItemDroppedOff);
        NetworkEventBus.Subscribe<ScoreUpdatedEvent>(onScoreUpdated);

        Debug.LogWarning("Level manager subscribed to events");
    }

    private void OnDisable() {
        // NetworkEventBus.Unsubscribe<TestNetworkEvent>(onTestNetworkEventClient);
        NetworkEventBus.Unsubscribe<ItemSpawnedEvent>(onItemSpawned);
        NetworkEventBus.Unsubscribe<ItemPickedUpEvent>(onItemPickedUp);
        NetworkEventBus.Unsubscribe<ItemDroppedOffEvent>(onItemDroppedOff);
        NetworkEventBus.Unsubscribe<ScoreUpdatedEvent>(onScoreUpdated);
    }

    private void onScoreUpdated(ScoreUpdatedEvent scoreUpdatedEvent) {
        Debug.LogWarning("Score updated event received in the level manager");
        EventBus<ScoreUIEvent>.Raise(new ScoreUIEvent(scoreUpdatedEvent.score));
    }

    private void onItemDroppedOff(ItemDroppedOffEvent itemDroppedOffEvent) {
        Debug.LogWarning("Item dropped off event received in the level manager");
        EventBus<InventoryUIEvent>.Raise(new InventoryUIEvent(0));
    }

    private void onTestNetworkEventClient(TestNetworkEvent testNetworkEvent) {
        Debug.LogWarning("Test network event received on the client");
    }

    private void onItemSpawned(ItemSpawnedEvent itemSpawnedEvent) {
        Debug.LogWarning("Item spawned event received");
        var item = Instantiate(itemPrefab, Spawner.Spawners[Random.Range(0, Spawner.Spawners.Count - 1)].gameObject.transform.position, Quaternion.identity);
        var itemNetworkTransform = item.GetComponent<NetworkTransform>();
        itemNetworkTransform.SetKey(itemSpawnedEvent.source);
        itemNetworkTransform.Initialize();


    }

    private void onItemPickedUp(ItemPickedUpEvent itemPickedUpEvent) {
        Debug.LogWarning("Item picked up event received in the level manager");

        // inform the UI;
        EventBus<InventoryUIEvent>.Raise(new InventoryUIEvent(itemPickedUpEvent.inventorySize));

        NetworkTransform.Transforms.TryGetValue(itemPickedUpEvent.itemGuid, out NetworkTransform networkTransform);
        if (networkTransform != null) {
            Destroy(networkTransform.gameObject);
        }
    }
}