using UnityEngine;
using shared;

public class ClientGameInformation {
    public readonly TcpMessageChannel tcpMessageChannel;
    public IMovementInputReceiver movementInputReceiver;
    

    public ClientGameInformation(TcpMessageChannel tcpMessageChannel) {
        this.tcpMessageChannel = tcpMessageChannel;

    }
}
