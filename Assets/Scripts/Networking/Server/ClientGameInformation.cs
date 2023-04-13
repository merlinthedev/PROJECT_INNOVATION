using UnityEngine;
using shared;

public class ClientGameInformation {
    public readonly TcpMessageChannel tcpMessageChannel;
    public IMovementInputReceiver movementInputReceiver;
    public Player player;
    

    public ClientGameInformation(TcpMessageChannel tcpMessageChannel) {
        this.tcpMessageChannel = tcpMessageChannel;

    }
}
