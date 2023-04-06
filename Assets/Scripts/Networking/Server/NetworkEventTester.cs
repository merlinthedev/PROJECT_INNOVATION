using UnityEngine;
using System.Collections;

public class NetworkEventTester : MonoBehaviour {

    public bool isServer;

    private void OnEnable() {
        NetworkEventBus.Subscribe<TestNetworkEvent>(onTestNetworkEvent);
        NetworkEventBus.Subscribe<ItemSpawnedEvent>(onItemSpawnedEvent);
    }

    private void OnDisable() {
        NetworkEventBus.Unsubscribe<TestNetworkEvent>(onTestNetworkEvent);
        NetworkEventBus.Unsubscribe<ItemSpawnedEvent>(onItemSpawnedEvent);
    }

    private void Start() {
        if (isServer) StartCoroutine(sendNetworkEvent());
    } 

    private void onItemSpawnedEvent(ItemSpawnedEvent itemSpawnedEvent) {
        Debug.Log("ItemSpawnedEvent event received");
    }

    private void onTestNetworkEvent(TestNetworkEvent itemPickedUpEvent) {
        Debug.Log("TestNetworkEvent event received");
    }

    private IEnumerator sendNetworkEvent() {
        while (true) {
            NetworkEventBus.Raise(new TestNetworkEvent {
                source = System.Guid.NewGuid()
            });
            NetworkEventBus.Raise(new ItemSpawnedEvent {
                source = System.Guid.NewGuid()
            });
            yield return new WaitForSeconds(5);
        }
    }


}