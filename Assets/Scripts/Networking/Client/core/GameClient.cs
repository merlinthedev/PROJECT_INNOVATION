using UnityEngine;
using System.Collections.Generic;
using System;
using shared;

public class GameClient : MonoBehaviour {

    [SerializeField] private NetworkTransform playerPrefab;

    private TcpMessageChannel tcpMessageChannel;

    private static GameClient instance;
    private Guid guid;
    [SerializeField]
    private ClientCartController clientCartController;

    private void Awake() {
        if (instance != null) {
            Debug.LogError("There is already an exisiting GameClient present. Aborting instantiation.");
            Destroy(this);
            return;
        }
        DontDestroyOnLoad(this);
        instance = this;
        tcpMessageChannel = new TcpMessageChannel();

        // Application.targetFrameRate = 60;
    }

    private void Update() {
        safeSendInputData();

        if (tcpMessageChannel.Connected) receiveAndProcessMessages();

        //renderOtherClients();
    }

    private void safeSendInputData() {
        if (clientCartController == null || !tcpMessageChannel.Connected) {
            return;
        }
        try {
            InputPacket inputPacket = clientCartController.GetInputPacket();

            tcpMessageChannel.SendMessage(inputPacket);
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
            case TransformListPacket transformListPacket:
                handleTransformListPacket(transformListPacket);
                break;
        }
    }

    private void handleDisconnectEvent(DisconnectEvent disconnectEvent) {
        Debug.Log("Received a disconnect event with guid: " + disconnectEvent.guid + " however, we have no way of handling it yet...");

    }

    private void handleTransformListPacket(TransformListPacket transformListPacket) {
        transformListPacket.updatedTransforms.ForEach(transformPacket => {
            // update the transform with the corresponding guid
            handleTransformPacket(transformPacket);
        });
    }

    private void handleTransformPacket(TransformPacket transformPacket) {
        // handle transform packet
        NetworkTransform transform;
        NetworkTransform.Transforms.TryGetValue(transformPacket.guid, out transform);
        if (transform != null) {
            transform.UpdateTransform(transformPacket);
        } else {
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
        foreach (var pack in connectEvent.objectTransforms) {
            handleTransformPacket(pack);
        }
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

    public Guid GetGuid() {
        return guid;
    }
}
