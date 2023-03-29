using System.Collections.Generic;
using UnityEngine;

public class ServerPicker : MonoBehaviour {

    private UDPPinger udpPinger;
    private Dictionary<string, ServerAddress> servers = new Dictionary<string, ServerAddress>();


    [SerializeField] private int udpPort = 0;

    private void Start() {
        udpPinger = new UDPPinger(udpPort);
    }


    private void onUDPMessageReceived(string message) {
        if (message.StartsWith("ACK")) {
            string[] parts = message.Split(':');
            if (parts.Length == 3) {
                string address = parts[1];
                int port = int.Parse(parts[2]);

                if (!servers.ContainsKey(address)) servers.Add(address, view.AddServer(address, port));
            }
        }
        Debug.Log("wow, we received something back: " + message);
    }
}
