using UnityEngine;
using shared;

public class GameClient : MonoBehaviour {

    private TcpMessageChannel tcpMessageChannel;

    private static GameClient instance;
    private System.Guid guid;

    private void Awake() {
        if (instance != null) {
            Debug.LogError("There is already an exisiting GameClient present. Aborting instantiation.");
            Destroy(this);
            return;
        }
        DontDestroyOnLoad(this);
        instance = this;
        tcpMessageChannel = new TcpMessageChannel();
    }

    private void Update() {
        safeSendInputData();

        if (tcpMessageChannel.Connected) receiveAndProcessMessages();
    }

    private void safeSendInputData() {
        try {
            // send input data from accelorometer & ui
        } catch (System.Exception e) {
            Debug.LogError("Error while sending input data: " + e.Message);
        }
    }

    private void receiveAndProcessMessages() {
        if (!tcpMessageChannel.Connected) {
            Debug.LogWarning("Trying to receive messages but we are no longer connected...");
            return;
        }

        while (tcpMessageChannel.HasMessage() && gameObject.activeSelf) {
            ASerializable message = tcpMessageChannel.ReceiveMessage();
            handleNetworkMessage(message);
        }
    }

    private void handleNetworkMessage(ASerializable message) {
        // handle messages
        // Debug.Log("Received a message of type " + message.GetType() + ", but we have no way of handling it yet...");

        switch (message) {
            case ConnectEvent connectEvent:
                handleConnectEvent(connectEvent);
                break;
            // Transform packets
            case TransformPacket transformPacket:
                // handle transform packet
                handleTransformPacket(transformPacket);
                break;
        }
    }

    private void handleTransformPacket(TransformPacket transformPacket) {
        // handle transform packet

        /*
            We will receive transform packets for every client connected to the server. The guid passed through the packet
            will be used to identify the client that sent the packet. We can then use this guid to identify the client in
            our game world and update the transform of the client's game object.
        */

        Debug.Log("Received a transform packet with guid: " + transformPacket.guid + " however, we have no way of handling it yet...");
    }

    private void handleConnectEvent(ConnectEvent connectEvent) {
        guid = connectEvent.guid;
        Debug.Log("Received a connect event with guid: " + guid);
    }

    private void OnApplicationQuit() {
        DisconnectEvent disconnectEvent = new DisconnectEvent {
            guid = this.guid
        };

        tcpMessageChannel.SendMessage(disconnectEvent);

        tcpMessageChannel.Close();
    }

    public static GameClient getInstance() {
        return instance;
    }

    public TcpMessageChannel getTcpMessageChannel() {
        return tcpMessageChannel;
    }

    public System.Guid GetGuid() {
        return guid;
    }
}
