using UnityEngine;
using System.Collections;

public class NetworkEventTester : MonoBehaviour {

    public bool isServer;

    private void OnEnable() {
        NetworkEventBus.Subscribe<TestNetworkEvent>(onNetworkEvent);
    }

    private void OnDisable() {
        NetworkEventBus.Unsubscribe<TestNetworkEvent>(onNetworkEvent);
    }

    private void Start() {
        if (isServer) StartCoroutine(sendNetworkEvent());
    }

    private void onNetworkEvent(TestNetworkEvent testNetworkEvent) {
        Debug.Log("Received network event: " + testNetworkEvent.source);
    }

    private IEnumerator sendNetworkEvent() {
        while (true) {
            NetworkEventBus.Raise(new TestNetworkEvent {
                source = System.Guid.NewGuid()
            });
            yield return new WaitForSeconds(1);
        }
    }


}