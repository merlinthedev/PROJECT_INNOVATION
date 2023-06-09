using UnityEngine;
using Cinemachine;
using System;
using System.Linq;
using shared;

public class GameClient : MonoBehaviour {

    private System.Collections.Generic.List<UnityEngine.Color> colors = new System.Collections.Generic.List<UnityEngine.Color> {
        UnityEngine.Color.red,
        UnityEngine.Color.blue,
        UnityEngine.Color.green,
        UnityEngine.Color.yellow,
        UnityEngine.Color.magenta,
        UnityEngine.Color.cyan,
    };


    [SerializeField] private NetworkTransform playerPrefab;
    [SerializeField] private NetworkTransform itemPrefab;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    [SerializeField] private GameObject beaconPrefab;

    private TcpMessageChannel tcpMessageChannel;

    private static GameClient instance;
    private Guid guid;
    [SerializeField] private ClientCartController clientCartController;
    [SerializeField] private ButtonPressed PowerUpButton;

    [SerializeField] private InteractableConfiguration interactableConfiguration;

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
            inputPacket.powerUpPressed = PowerUpButton.isPressed || Input.GetKey(KeyCode.Space);
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
            ISerializable message = tcpMessageChannel.ReceiveMessage();
            handleNetworkMessage(message);
        }
    }

    private void handleNetworkMessage(ISerializable message) {
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
            case PlayerDisconnectEvent playerDisconnectEvent:
                handlePlayerDisconnectEvent(playerDisconnectEvent);
                break;
            case NetworkEvent networkEvent:
                handleNetworkEvent(networkEvent);
                break;
            case ExistingItemsPacket existingItemsPacket:
                handleExistingItemsPacket(existingItemsPacket);
                break;
        }
    }

    private void handleExistingItemsPacket(ExistingItemsPacket existingItemsPacket) {
        // existingItemsPacket.existingItems.ForEach(transformPacket => {
        //     var newItem = Instantiate(itemPrefab, transformPacket.Position(), transformPacket.Rotation());
        //     newItem.key = transformPacket.guid;
        //     newItem.kinematic = true;
        //     newItem.Initialize();
        // });

        ItemDiscountUpdateEvent itemDiscountUpdateEvent = new ItemDiscountUpdateEvent();
        itemDiscountUpdateEvent.influencedItems = new System.Collections.Generic.List<System.Guid>();
        itemDiscountUpdateEvent.discount = 0.1f;

        foreach (var kvp in existingItemsPacket.existingItemMap) {
            var newItem = Instantiate(itemPrefab, kvp.Value.Position(), kvp.Value.Rotation());
            newItem.key = kvp.Key;
            newItem.kinematic = true;
            newItem.Initialize();

            itemDiscountUpdateEvent.influencedItems.Add(kvp.Key);

        }

        NetworkEventBus.Raise(itemDiscountUpdateEvent);
    }

    private void handleNetworkEvent(NetworkEvent networkEvent) {
        // Debug.LogWarning("Received a network event with type: " + networkEvent.GetType());
        NetworkEventBus.Raise(networkEvent);
        // Debug.LogWarning("Raised a network event with type: " + networkEvent.GetType());
    }

    private void handlePlayerDisconnectEvent(PlayerDisconnectEvent playerDisconnectEvent) {
        NetworkTransform transform;
        NetworkTransform.Transforms.TryGetValue(playerDisconnectEvent.guid, out transform);
        if (transform != null) {
            Destroy(transform.gameObject);
        } else {
            Debug.LogWarning("Received a disconnect event for a client that is not in our dictionary. How did this happen? :O");
            Debug.LogWarning("GUID: " + playerDisconnectEvent.guid);
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
            Debug.LogWarning("GUID: " + transformPacket.guid);
            //for now, we instantiate a new transform at that position

            var newClient = Instantiate(playerPrefab, transformPacket.Position(), transformPacket.Rotation());
            newClient.key = transformPacket.guid;
            newClient.kinematic = true;
            newClient.Initialize();


            // var newObject = Instantiate(transform, transformPacket.Position(), transformPacket.Rotation());
            // newObject.key = transformPacket.guid;
            // newObject.kinematic = true;
            // newObject.Initialize();
        }
    }

    private void handleConnectEvent(ConnectEvent connectEvent) {
        guid = connectEvent.guid;

        foreach (var interactable in connectEvent.interactables) {
            var newInteractable = Instantiate(interactableConfiguration.interactables[interactable.interactableID].clientPrefab);
            newInteractable.key = interactable.guid;
            newInteractable.kinematic = true;
            newInteractable.Initialize();
            handleTransformPacket(interactable.transformPacket);
        }

        foreach (var pack in connectEvent.objectTransforms) {
            handleTransformPacket(pack);
        }

        var playerCameraPivot = NetworkTransform.Transforms[guid].transform.GetChild(0);
        virtualCamera.Follow = playerCameraPivot;
        virtualCamera.LookAt = playerCameraPivot;

        Debug.Log("Received a connect event with guid: " + guid);

        // instantiate a new cube at the player position
        var beaconCube = Instantiate(beaconPrefab, NetworkTransform.Transforms[guid].transform.position, Quaternion.identity);
        var color = colors[connectEvent.colorID];

        color.a = 0.7f;

        beaconCube.GetComponent<Renderer>().material.color = color;

    }


    private void OnApplicationQuit() {
        DisconnectEvent disconnectEvent = new DisconnectEvent {
            guid = this.guid
        };

        tcpMessageChannel.SendMessage(disconnectEvent);

        tcpMessageChannel.Close();

        Debug.Log("Sent disconnectEvent packet");
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
