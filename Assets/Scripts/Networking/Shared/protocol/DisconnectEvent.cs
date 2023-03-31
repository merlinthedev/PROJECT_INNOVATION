public class DisconnectEvent : shared.ASerializable {

    public System.Guid guid { get; set; }

    public override void Serialize(shared.Packet packet) {
        packet.Write(guid);
    }

    public override void Deserialize(shared.Packet packet) {
        guid = packet.ReadGuid();
    }


}
