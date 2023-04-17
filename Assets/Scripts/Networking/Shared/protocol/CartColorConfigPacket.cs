using shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CartColorConfigPacket : ISerializable {
    public Dictionary<Guid, Color> cartColors = new Dictionary<Guid, Color>();
    public void Serialize(Packet packet) {
        packet.Write(cartColors.Count);
        foreach (var kvp in cartColors) {
            packet.Write(kvp.Key);
            packet.Write(kvp.Value.r);
            packet.Write(kvp.Value.g);
            packet.Write(kvp.Value.b);
            packet.Write(kvp.Value.a);
        }
    }

    public void Deserialize(Packet packet) {
        int count = packet.ReadInt();
        for (int i = 0; i < count; i++) {
            Guid id = packet.ReadGuid();
            float r = packet.ReadFloat();
            float g = packet.ReadFloat();
            float b = packet.ReadFloat();
            float a = packet.ReadFloat();
            cartColors.Add(id, new Color(r, g, b, a));
        }
    }

}