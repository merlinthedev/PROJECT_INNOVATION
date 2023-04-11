using server;
using TMPro;
using UnityEngine;

public class GetAmountOfClients : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI textMeshProUGUI;
    [SerializeField] private TCPGameServer tcpGameServer;

    private string serverAddress = "";
    private string serverPort = "";

    private void OnEnable() {
        EventBus<JoinQuitEvent>.Subscribe(handleJoinQuitEvent);
        serverAddress = tcpGameServer.GetServerAddress();
        serverPort = tcpGameServer.GetServerPort().ToString();
        textMeshProUGUI.text = "Amount of clients: " + tcpGameServer.GetAmountOfClients().ToString() + " \non server: " + serverAddress + ":" + serverPort;
    }

    private void OnDisable() {
        EventBus<JoinQuitEvent>.Unsubscribe(handleJoinQuitEvent);
    }

    private void Start() {
    }

    private void handleJoinQuitEvent(JoinQuitEvent joinQuitEvent) {
        Debug.Log("JoinQuitEvent received, do stuff mayber?XD");
        textMeshProUGUI.text = "Amount of clients: " + tcpGameServer.GetAmountOfClients().ToString() + " \non server: " + serverAddress + ":" + serverPort;
    }
}
