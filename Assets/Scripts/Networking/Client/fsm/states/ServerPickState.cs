using shared;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Starting state where you can connect to the server.
 */
public class ServerPickState : ApplicationStateWithView<ServerPickView> {
    [SerializeField] private int broadcastPort = 0;
    [SerializeField] private float broadcastInterval = 0.5f;
    [SerializeField] private float broadcastTime = 5.0f;
    private float broadcastStartTime;
    public bool IsSearching => Time.time > broadcastStartTime + broadcastTime;

    private UDPPinger udpPinger;

    private Dictionary<string, ServerAddress> servers = new Dictionary<string, ServerAddress>();

    public override void EnterState() {
        Debug.LogError("Entering ServerPickState");
        base.EnterState();

        //listen to a connect click from our view
        view.OnServerConnectRequest += tryConnect;

        Debug.LogWarning("Entered state " + this.name + " (linked to view:" + view?.name + ")");
    }

    public override void ExitState() {
        Debug.LogError("Exiting ServerPickState");
        base.ExitState();

        //stop listening to connect requests
        view.OnServerConnectRequest -= tryConnect;

        Debug.LogWarning("Exited state " + this.name + " (linked to view:" + view?.name + ")");
    }

    /**
     * Connect to the server (with some client side validation)
     */
    private void tryConnect(string address, int port) {
        fsm.channel.Connect(address, port);
    }

    private void Start() {
        udpPinger = new UDPPinger(broadcastPort);
        udpPinger.OnMessageReceived += onUDPMessageReceived;
        StartCoroutine(pingCoroutine());
    }

    /// <summary>
    /// Start a search for potential servers in our area
    /// </summary>
    public void Search() {
        StartCoroutine(pingCoroutine());
    }

    /// <summary>
    /// Do a search for potential servers in our area, it will ping at an interval for a predetermined amount of seconds
    /// </summary>
    private IEnumerator pingCoroutine() {
        broadcastStartTime = Time.time;
        while (Time.time - broadcastStartTime < broadcastTime) {
            udpPinger.Ping();
            yield return new WaitForSeconds(broadcastInterval);
        }
    }

    /// //////////////////////////////////////////////////////////////////
    ///                     NETWORK MESSAGE PROCESSING
    /// //////////////////////////////////////////////////////////////////

    private void Update() {
        //if we are connected, start processing messages
        if (fsm.channel.Connected) receiveAndProcessNetworkMessages();

        udpPinger.Update();
    }

    /// <summary>
    /// If we received a response from a server, check if the response is valid and not already in the list, if so, add it to the list of potential servers.
    /// </summary>
    /// <param name="message"></param>
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

    protected override void handleNetworkMessage(ASerializable message) {
        Debug.Log("how did we get here?");
        switch (message) {
            default:
                break;
        }
    }
}