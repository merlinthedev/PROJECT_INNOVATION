public class DisconnectEvent : shared.ISerializable {

    public System.Guid guid { get; set; }

    public void Serialize(shared.Packet packet) {
        packet.Write(guid);
    }

    public void Deserialize(shared.Packet packet) {
        guid = packet.ReadGuid();
    }


}
