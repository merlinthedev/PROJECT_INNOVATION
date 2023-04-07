using shared;
using UnityEngine;
using System.Collections.Generic;

public class ExistingItemsPacket : ISerializable {

    public List<TransformPacket> existingItems { get; set; } = new List<TransformPacket>();

    public Dictionary<System.Guid, float> discountMap { get; set; } = new Dictionary<System.Guid, float>();

    public void Serialize(Packet packet) {
        packet.Write(existingItems.Count);
        foreach (TransformPacket transform in existingItems) {
            transform.Serialize(packet);
        }

        packet.Write(discountMap.Count);
        foreach (var kvp in discountMap) {
            packet.Write(kvp.Key);
            packet.Write(kvp.Value);
        }

    }
    public void Deserialize(Packet packet) {
        int count = packet.ReadInt();
        for (int i = 0; i < count; i++) {
            TransformPacket transform = new TransformPacket();
            transform.Deserialize(packet);
            existingItems.Add(transform);
        }

        int mapCount = packet.ReadInt();
        for (int i = 0; i < mapCount; i++) {
            discountMap.Add(packet.ReadGuid(), packet.ReadFloat());
        }
    }

}

