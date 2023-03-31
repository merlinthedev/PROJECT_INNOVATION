using UnityEngine;
using System.Collections.Generic;
using System;
using shared;

public class GameClient : MonoBehaviour {

    [SerializeField] private NetworkTransform playerPrefab;

    private TcpMessageChannel tcpMessageChannel;

    private static GameClient instance;
    private Guid guid;
    private NetworkTransform playerTransform;

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

        //renderOtherClients();
    }

    private void renderOtherClients() {
        // get all the transforms from the dictionary and render them with the prefab.

        foreach (var otherPlayerTransform in otherPlayerTransforms) {
            if (otherPlayerTransform.Value == null) {
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
        if (playerTransform == null) {
            return;
        }

        try {
            if(playerTransform.hasPacket) {
                tcpMessageChannel.SendMessage(playerTransform.GetPacket());
            }

            // TODO: send input data from accelorometer & ui instead of our updated transform
        } catch (Exception e) {
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

    }

    private void handleTransformPacket(TransformPacket transformPacket) {
        // handle transform packet
        var transform = NetworkTransform.Transforms[transformPacket.guid];
        if(transform != null) {
            transform.UpdateTransform(transformPacket);
        }
        else {
            Debug.LogWarning("Received a transform packet for a client that is not in our dictionary. How did this happen? :O");

            //for now, we instantiate a new transform at that position
            var newClient = Instantiate(playerPrefab, transformPacket.Position(), transformPacket.Rotation());
            newClient.key = transformPacket.guid;
            newClient.kinematic = true;
            newClient.Initialize();
        }
    }

    private void handleConnectEvent(ConnectEvent connectEvent) {
        guid = connectEvent.guid;
        playerTransform = Instantiate(playerPrefab);
        playerTransform.key = guid;
        playerTransform.Initialize();
        playerTransform.kinematic = false;
        Debug.Log("Received a connect event with guid: " + guid);
    }

    private void OnApplicationQuit() {
        DisconnectEvent disconnectEvent = new DisconnectEvent {
            guid = this.guid
        };

        tcpMessageChannel.SendMessage(disconnectEvent);

        tcpMessageChannel.Close();
    }

    public void ReceivePlayerTransform(NetworkTransform playerTransform) {
        this.playerTransform = playerTransform;

        Debug.Log("Received player transform: " + playerTransform);
    }

    public static GameClient getInstance() {
        return instance;
    }

    public TcpMessageChannel getTcpMessageChannel() {
        return tcpMessageChannel;
    }

    public Guid GetGuid() {
        return guid;
    }
}
