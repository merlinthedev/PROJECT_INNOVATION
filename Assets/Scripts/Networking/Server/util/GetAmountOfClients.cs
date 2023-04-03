using server;
using TMPro;
using UnityEngine;

public class GetAmountOfClients : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI textMeshProUGUI;
    [SerializeField] private TCPGameServer tcpGameServer;

    private string serverAddress;

    private void OnEnable() {
        EventBus<JoinQuitEvent>.Subscribe(handleJoinQuitEvent);
    }

    private void OnDisable() {
        EventBus<JoinQuitEvent>.Unsubscribe(handleJoinQuitEvent);
    }

    private void Start() {
        serverAddress = tcpGameServer.GetServerAddress();
        textMeshProUGUI.text = "Amout of clients: " + tcpGameServer.GetAmountOfClients().ToString() + " on server: " + serverAddress;
    }

    private void handleJoinQuitEvent(JoinQuitEvent joinQuitEvent) {
        Debug.Log("JoinQuitEvent received, do stuff mayber?XD");
        textMeshProUGUI.text = "Amount of clients: " + tcpGameServer.GetAmountOfClients().ToString() + " on server: " + serverAddress;
    }
}
