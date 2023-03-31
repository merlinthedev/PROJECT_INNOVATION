public class InputPacket : shared.ASerializable {

    public System.Guid guid { get; set; }
    public float[] vectors { get; set; } = new float[4];
    public float[] transformData { get; set; } = new float[6];

    public override void Serialize(shared.Packet packet) {
        packet.Write(guid);

        packet.Write(vectors[0]);
        packet.Write(vectors[1]);
        packet.Write(vectors[2]);
        packet.Write(vectors[3]);

        packet.Write(transformData[0]);
        packet.Write(transformData[1]);
        packet.Write(transformData[2]);
        packet.Write(transformData[3]);
        packet.Write(transformData[4]);
        packet.Write(transformData[5]);
    }

    public override void Deserialize(shared.Packet packet) {
        guid = packet.ReadGuid();

        vectors[0] = packet.ReadFloat();
        vectors[1] = packet.ReadFloat();
        vectors[2] = packet.ReadFloat();
        vectors[3] = packet.ReadFloat();

        transformData[0] = packet.ReadFloat();
        transformData[1] = packet.ReadFloat();
        transformData[2] = packet.ReadFloat();
        transformData[3] = packet.ReadFloat();
        transformData[4] = packet.ReadFloat();
        transformData[5] = packet.ReadFloat();

    }
}
