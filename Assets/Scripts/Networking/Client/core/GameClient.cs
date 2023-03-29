using UnityEngine;
using shared;

public class GameClient : MonoBehaviour {

    private TcpMessageChannel tcpMessageChannel;

    private static GameClient instance;

    private void Awake() {
        if(instance != null) {
            Debug.LogError("There is already an exisiting GameClient present. Aborting instantiation.");
            Destroy(this);
            return;
        }

        tcpMessageChannel = new TcpMessageChannel();
    }

    private void Update() {
        if (tcpMessageChannel.Connected) receiveAndProcessMessages();
        
    }

    private void receiveAndProcessMessages() {
        if(!tcpMessageChannel.Connected) {
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
        Debug.Log("Received a message of type " + message.GetType() + ", but we have no way of handling it yet...");
    }

    private void OnApplicationQuit() {
        tcpMessageChannel.Close();
    }
}
