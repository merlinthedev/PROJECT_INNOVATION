using shared;
using System.Collections.Generic;

public class TransformListPacket : ASerializable {

    public List<TransformPacket> updatedTransforms { get; set; } = new List<TransformPacket>();

    public override void Serialize(Packet packet) {
        packet.Write(updatedTransforms.Count);
        foreach (TransformPacket transform in updatedTransforms) {
            transform.Serialize(packet);
        }
    }

    public override void Deserialize(Packet packet) {
        int count = packet.ReadInt();
        for (int i = 0; i < count; i++) {
            TransformPacket transform = new TransformPacket();
            transform.Deserialize(packet);
            updatedTransforms.Add(transform);
        }
    }
}