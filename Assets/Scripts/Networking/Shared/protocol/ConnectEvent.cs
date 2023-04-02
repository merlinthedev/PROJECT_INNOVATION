using System.Collections.Generic;

public class ConnectEvent : shared.ASerializable {
    public System.Guid guid { get; set; }

    public List<TransformPacket> objectTransforms = new List<TransformPacket>();

    public override void Serialize(shared.Packet packet) {
        packet.Write(guid);
        packet.Write(objectTransforms.Count);
        foreach (var transformPacket in objectTransforms) {
            packet.Write(transformPacket);
        }
    }

    public override void Deserialize(shared.Packet packet) {
        guid = packet.ReadGuid();
        int count = packet.ReadInt();
        for (int i = 0; i < count; i++) {
            objectTransforms.Add(packet.Read<TransformPacket>());
        }

    }

}
