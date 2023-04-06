using UnityEngine;

public class LevelManager : MonoBehaviour {


    private void OnEnable() {
        // NetworkEventBus.Subscribe<TestNetworkEvent>(onTestNetworkEventClient);
        NetworkEventBus.Subscribe<ItemSpawnedEvent>(onItemSpawned);
        NetworkEventBus.Subscribe<ItemPickedUpEvent>(onItemPickedUp);

        Debug.LogWarning("Level manager subscribed to events");
    }

    private void OnDisable() {
        // NetworkEventBus.Unsubscribe<TestNetworkEvent>(onTestNetworkEventClient);
        NetworkEventBus.Unsubscribe<ItemSpawnedEvent>(onItemSpawned);
        NetworkEventBus.Unsubscribe<ItemPickedUpEvent>(onItemPickedUp);
    }

    private void onTestNetworkEventClient(TestNetworkEvent testNetworkEvent) {
        Debug.LogWarning("Test network event received on the client");
    }

    private void onItemSpawned(ItemSpawnedEvent itemSpawnedEvent) {
        Debug.LogWarning("Item spawned event received");
        // var item = Instantiate(itemPrefab, Spawner.Spawners[Random.Range(0, Spawner.Spawners.Count - 1)].gameObject.transform.position, Quaternion.identity);
        // var itemNetworkTransform = item.GetComponent<NetworkTransform>();
        // NetworkTransform.Transforms.Add(itemNetworkTransform.Key, itemNetworkTransform);
        // itemNetworkTransform.Initialize();
    }

    private void onItemPickedUp(ItemPickedUpEvent itemPickedUpEvent) {
        Debug.LogWarning("Item picked up event received in the level manager");

        NetworkTransform.Transforms.TryGetValue(itemPickedUpEvent.itemGuid, out NetworkTransform networkTransform);
        if (networkTransform != null) {
            Destroy(networkTransform.gameObject);
        }
    }
}