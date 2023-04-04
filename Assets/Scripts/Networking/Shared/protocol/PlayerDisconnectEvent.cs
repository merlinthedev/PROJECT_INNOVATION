using shared;

public class PlayerDisconnectEvent : ISerializable {
    public System.Guid guid { get; set; }

    public void Serialize(Packet packet) {
        packet.Write(guid);
    }

    public void Deserialize(Packet packet) {
        guid = packet.ReadGuid();
    }
}