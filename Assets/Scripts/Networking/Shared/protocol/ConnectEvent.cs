using System.Collections.Generic;

public class ConnectEvent : shared.ISerializable {
    public System.Guid guid { get; set; }
    public int colorID { get; set; }

    public class InteractablePacket : shared.ISerializable {
        public System.Guid guid { get; set; }
        public int interactableID { get; set; }
        public TransformPacket transformPacket { get; set; }

        public void Serialize(shared.Packet packet) {
            packet.Write(guid);
            packet.Write(interactableID);
            packet.Write(transformPacket);
        }

        public void Deserialize(shared.Packet packet) {
            guid = packet.ReadGuid();
            interactableID = packet.ReadInt();
            transformPacket = packet.Read<TransformPacket>();
        }
    }

    public List<InteractablePacket> interactables = new List<InteractablePacket>();
    public List<TransformPacket> objectTransforms = new List<TransformPacket>();
    public List<System.Guid> playerGuids = new List<System.Guid>();

    public void Serialize(shared.Packet packet) {
        packet.Write(guid);
        packet.Write(interactables.Count);
        foreach (var interactablePacket in interactables) {
            packet.Write(interactablePacket);
        }
        packet.Write(objectTransforms.Count);
        foreach (var transformPacket in objectTransforms) {
            packet.Write(transformPacket);
        }
        packet.Write(playerGuids.Count);
        foreach (var playerGuid in playerGuids) {
            packet.Write(playerGuid);
        }

        packet.Write(colorID);
    }

    public void Deserialize(shared.Packet packet) {
        guid = packet.ReadGuid();
        int count = packet.ReadInt();
        for (int i = 0; i < count; i++) {
            interactables.Add(packet.Read<InteractablePacket>());
        }
        count = packet.ReadInt();
        for (int i = 0; i < count; i++) {
            objectTransforms.Add(packet.Read<TransformPacket>());
        }
        count = packet.ReadInt();
        for (int i = 0; i < count; i++) {
            playerGuids.Add(packet.ReadGuid());
        }

        colorID = packet.ReadInt();

    }

}
