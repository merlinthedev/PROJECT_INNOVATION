public class InputPacket : shared.ASerializable {

    public System.Guid guid { get; set; }
    public float[] vectors { get; set; } = new float[4];

    public override void Serialize(shared.Packet packet) {
        packet.Write(guid);
        packet.Write(vectors[0]);
        packet.Write(vectors[1]);
        packet.Write(vectors[2]);
        packet.Write(vectors[3]);
    }

    public override void Deserialize(shared.Packet packet) {
        guid = packet.ReadGuid();
        vectors[0] = packet.ReadFloat();
        vectors[1] = packet.ReadFloat();
        vectors[2] = packet.ReadFloat();
        vectors[3] = packet.ReadFloat();
    }
}
