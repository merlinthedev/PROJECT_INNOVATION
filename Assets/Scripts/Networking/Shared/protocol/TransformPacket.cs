using shared;

public class TransformPacket : ASerializable {
    // TODO: More transform data 
    public System.Guid guid { get; set; }

    public override void Serialize(Packet packet) {
        packet.Write(guid);
    }

    public override void Deserialize(Packet packet) {
        guid = packet.ReadGuid();
    }
}
