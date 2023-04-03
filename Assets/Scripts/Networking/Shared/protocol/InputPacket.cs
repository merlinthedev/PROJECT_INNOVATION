using UnityEngine;

public class InputPacket : shared.ASerializable {

    public Vector2 view;
    public Vector2 move;

    public override void Serialize(shared.Packet packet) {
        packet.Write(view.x);
        packet.Write(view.y);
        packet.Write(move.x);
        packet.Write(move.y);
    }

    public override void Deserialize(shared.Packet packet) {
        view.x = packet.ReadFloat();
        view.y = packet.ReadFloat();
        move.x = packet.ReadFloat();
        move.y = packet.ReadFloat();
    }
}
