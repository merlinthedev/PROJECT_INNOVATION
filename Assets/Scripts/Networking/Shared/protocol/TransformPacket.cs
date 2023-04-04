using shared;
using UnityEngine;

public class TransformPacket : ASerializable {
    // TODO: More transform data 
    public System.Guid guid { get; set; }
    public float[] transformData { get; set; } = new float[6];

    public void SetTransform(Transform transform) {
        transformData[0] = transform.position[0];
        transformData[1] = transform.position[1];
        transformData[2] = transform.position[2];
        transformData[3] = transform.rotation.eulerAngles[0];
        transformData[4] = transform.rotation.eulerAngles[1];
        transformData[5] = transform.rotation.eulerAngles[2];
    }

    public Vector3 Position() {
        return new Vector3(
            transformData[0],
            transformData[1],
            transformData[2]);
    }

    public Quaternion Rotation() {
        return Quaternion.Euler(
            transformData[3],
            transformData[4],
            transformData[5]);
    }

    public override void Serialize(Packet packet) {
        packet.Write(guid);
        packet.Write(transformData[0]);
        packet.Write(transformData[1]);
        packet.Write(transformData[2]);
        packet.Write(transformData[3]);
        packet.Write(transformData[4]);
        packet.Write(transformData[5]);
    }

    public override void Deserialize(Packet packet) {
        guid = packet.ReadGuid();
        transformData[0] = packet.ReadFloat();
        transformData[1] = packet.ReadFloat();
        transformData[2] = packet.ReadFloat();
        transformData[3] = packet.ReadFloat();
        transformData[4] = packet.ReadFloat();
        transformData[5] = packet.ReadFloat();
    }
}
