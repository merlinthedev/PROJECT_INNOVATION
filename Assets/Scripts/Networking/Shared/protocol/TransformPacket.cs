using shared;

public class TransformPacket : ASerializable {
    // TODO: More transform data 
    public System.Guid guid { get; set; }
    public float[] transformData { get; set; } = new float[6];

    public override void Serialize(Packet packet) {
        packet.Write(guid);
        packet.Write(transformData[0]);
        packet.Write(transformData[1]);
        packet.Write(transformData[2]);
        packet.Write(transformData[3]);
        packet.Write(transformData[4]);
        packet.Write(transformData[5]);
    }

    public override void Deserialize(Packet packet) {
        guid = packet.ReadGuid();
        transformData[0] = packet.ReadFloat();
        transformData[1] = packet.ReadFloat();
        transformData[2] = packet.ReadFloat();
        transformData[3] = packet.ReadFloat();
        transformData[4] = packet.ReadFloat();
        transformData[5] = packet.ReadFloat();
    }
}
