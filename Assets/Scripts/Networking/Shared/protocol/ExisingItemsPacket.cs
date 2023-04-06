using shared;
using UnityEngine;
using System.Collections.Generic;

public class ExistingItemsPacket : ISerializable {

    public List<TransformPacket> existingItems { get; set; } = new List<TransformPacket>();

    public void Serialize(Packet packet) {
        packet.Write(existingItems.Count);
        foreach (TransformPacket transform in existingItems) {
            transform.Serialize(packet);
        }

    }
    public void Deserialize(Packet packet) {
        int count = packet.ReadInt();
        for (int i = 0; i < count; i++) {
            TransformPacket transform = new TransformPacket();
            transform.Deserialize(packet);
            existingItems.Add(transform);
        }
    }

}

