using shared;
using UnityEngine;
using System.Collections.Generic;

public class ExistingItemsPacket : ISerializable {

    public List<TransformPacket> existingItems { get; set; } = new List<TransformPacket>();

    public Dictionary<System.Guid, TransformPacket> existingItemMap { get; set; } = new Dictionary<System.Guid, TransformPacket>();

    public void Serialize(Packet packet) {
        // packet.Write(existingItems.Count);
        // foreach (TransformPacket transform in existingItems) {
        //     transform.Serialize(packet);
        // }

        packet.Write(existingItemMap.Count);
        foreach (var kvp in existingItemMap) {
            packet.Write(kvp.Key);
            kvp.Value.Serialize(packet);
        }

    }
    public void Deserialize(Packet packet) {
        // int count = packet.ReadInt();
        // for (int i = 0; i < count; i++) {
        //     TransformPacket transform = new TransformPacket();
        //     transform.Deserialize(packet);
        //     existingItems.Add(transform);
        // }

        int mapCount = packet.ReadInt();
        for (int i = 0; i < mapCount; i++) {
            System.Guid key = packet.ReadGuid();
            TransformPacket transform = new TransformPacket();
            transform.Deserialize(packet);
            existingItemMap.Add(key, transform);
        }
    }

}

