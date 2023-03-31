using UnityEngine;
using shared;

public class GameClient : MonoBehaviour {

    [SerializeField] private GameObject playerPrefab;

    private TcpMessageChannel tcpMessageChannel;

    private static GameClient instance;
    private System.Guid guid;
    private Transform playerTransform;

    public static System.Collections.Generic.Dictionary<System.Guid, Transform> otherPlayerTransforms = new System.Collections.Generic.Dictionary<System.Guid, Transform>();

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

        renderOtherClients();
    }

    private void renderOtherClients() {
        // get all the transforms from the dictionary and render them with the prefab.
        foreach (var otherPlayerTransform in otherPlayerTransforms) {
            if (otherPlayerTransform.Value == null) {
                Debug.LogWarning("Transform is null, skipping...");
                continue;
            }

            if (otherPlayerTransform.Key == guid) {
                // This is our own transform, we don't need to render it.
                continue;
            }

            if (otherPlayerTransforms.ContainsKey(otherPlayerTransform.Key)) {
                // We already have a game object for this client, update the transform.
                otherPlayerTransforms[otherPlayerTransform.Key].position = otherPlayerTransform.Value.position;
                otherPlayerTransforms[otherPlayerTransform.Key].rotation = otherPlayerTransform.Value.rotation;
            } else {
                // We don't have a game object for this client, create one.
                GameObject otherPlayer = Instantiate(playerPrefab, otherPlayerTransform.Value.position, otherPlayerTransform.Value.rotation);
                otherPlayerTransforms.Add(otherPlayerTransform.Key, otherPlayer.transform);
            }

        }
    }

    private void safeSendInputData() {
        try {
            // send input data from accelorometer & ui
            InputPacket inputPacket = new InputPacket {
                guid = this.guid,
                // More input data
            };
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
            case DisconnectEvent disconnectEvent:
                handleDisconnectEvent(disconnectEvent);
                break;
        }
    }

    private void handleDisconnectEvent(DisconnectEvent disconnectEvent) {
        Debug.Log("Received a disconnect event with guid: " + disconnectEvent.guid + " however, we have no way of handling it yet...");

        if (otherPlayerTransforms.ContainsKey(disconnectEvent.guid)) {
            Destroy(otherPlayerTransforms[disconnectEvent.guid].gameObject);
            otherPlayerTransforms.Remove(disconnectEvent.guid);
        } else {
            Debug.LogWarning("Received a disconnect event for a client that is not in our dictionary. How did this happen? :O");
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

        if (this.guid == transformPacket.guid) {
            // update our local position with the information we received from the server.
            // this is to make sure that we are in sync with the server.
            transform.position = new Vector3(transformPacket.transformData[0], transformPacket.transformData[1], transformPacket.transformData[2]);
            transform.rotation = Quaternion.Euler(transformPacket.transformData[3], transformPacket.transformData[4], transformPacket.transformData[5]);

            Debug.Log("Updated our local position with the information we received from the server.");
        } else {
            // with the guid we can identify the client in our game world and update the transform of the client's game object.
            if (otherPlayerTransforms.ContainsKey(transformPacket.guid)) {
                Transform otherPlayerTransform = otherPlayerTransforms[transformPacket.guid];
                otherPlayerTransform.position = new Vector3(transformPacket.transformData[0], transformPacket.transformData[1], transformPacket.transformData[2]);
                otherPlayerTransform.rotation = Quaternion.Euler(transformPacket.transformData[3], transformPacket.transformData[4], transformPacket.transformData[5]);
            } else {
                Debug.LogWarning("Received a transform packet for a client that we don't know about yet.");
            }
        }
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

    public void ReceivePlayerTransform(Transform playerTransform) {
        this.playerTransform = playerTransform;
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
