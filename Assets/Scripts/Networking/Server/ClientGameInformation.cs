public class ClientGameInformation {
    public System.Guid guid { get; private set; }

    public float[] position { get; set; } = new float[3];
    public float[] rotation { get; set; } = new float[3];

    public ClientGameInformation(System.Guid guid) {
        this.guid = guid;
    }

    public ClientGameInformation() {
        this.guid = System.Guid.NewGuid();
    }

}
