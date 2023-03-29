using System.Collections.Generic;
using UnityEngine;

public class ServerPicker : MonoBehaviour {

    private UDPPinger udpPinger;
    private Dictionary<string, ServerAddress> servers = new Dictionary<string, ServerAddress>();
    private float broadcastStartTime;

    [SerializeField] private ServerPickView serverPickView;

    [SerializeField] private int udpPort = 0;
    [SerializeField] private float broadcastInterval = 0.5f;
    [SerializeField] private float broadcastTime = 5.0f;


    private void OnEnable() {
        serverPickView.OnServerConnectRequest += tryConnect;
    }

    private void OnDisable() {
        serverPickView.OnServerConnectRequest -= tryConnect;
    }

    private void Start() {
        udpPinger = new UDPPinger(udpPort);
        udpPinger.OnMessageReceived += onUDPMessageReceived;
        StartCoroutine(pingCoroutine());
    }

    private void Update() {
        // if connected, handle messages

        udpPinger.Update();
    }

    private void tryConnect(string address, int port) {
         // Connect tcpChannel
    }

    public void Search() {
        StartCoroutine(pingCoroutine());
    }

    private System.Collections.IEnumerator pingCoroutine() {
        broadcastStartTime = Time.time;
        while (Time.time - broadcastStartTime < broadcastTime) {
            udpPinger.Ping();
            yield return new WaitForSeconds(broadcastInterval);
        }
    }

    private void onUDPMessageReceived(string message) {
        if (message.StartsWith("ACK")) {
            string[] parts = message.Split(':');
            if (parts.Length == 3) {
                string address = parts[1];
                int port = int.Parse(parts[2]);

                if (!servers.ContainsKey(address)) servers.Add(address, serverPickView.AddServer(address, port));
            }
        }
        Debug.Log("wow, we received something back: " + message);
    }
}
