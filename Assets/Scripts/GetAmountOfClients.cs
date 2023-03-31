using server;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GetAmountOfClients : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI textMeshProUGUI;
    [SerializeField] private TCPGameServer tcpGameServer;

    private void OnEnable() {
        EventBus<JoinQuitEvent>.Subscribe(handleJoinQuitEvent);
    }

    private void OnDisable() {
        EventBus<JoinQuitEvent>.Unsubscribe(handleJoinQuitEvent);
    }

    private void Start() {
        textMeshProUGUI.text += tcpGameServer.GetAmountOfClients().ToString();
    }

    private void handleJoinQuitEvent(JoinQuitEvent joinQuitEvent) {
        textMeshProUGUI.text += joinQuitEvent.amountOfClients.ToString();
    }




}
