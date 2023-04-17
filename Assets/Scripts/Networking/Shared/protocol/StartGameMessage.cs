using shared;

public class StartGameMessage : ISerializable {

    public bool hasStarted;

    public void Serialize(Packet packet) {
        packet.Write(hasStarted);
    }

    public void Deserialize(Packet packet) {
        hasStarted = packet.ReadBool();
    }
}