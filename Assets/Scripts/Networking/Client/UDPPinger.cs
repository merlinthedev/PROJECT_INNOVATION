using System.Net.Sockets;
using System.Net;
using System.Text;

/// <summary>
/// A class used by the CLIENT to broadcast a server request and wait for answers
/// </summary>
public class UDPPinger {
    private readonly int UDPPort;
    private readonly string pingMessage = "ACK";
    private readonly UdpClient udpClient = new UdpClient();

    public System.Action<string> OnMessageReceived;

    public UDPPinger(int port) {
        UDPPort = port;
        udpClient.EnableBroadcast = true;
    }

    public void Update() {
        if (udpClient.Available > 0) {
            var remoteEP = new IPEndPoint(IPAddress.Any, 0);
            var data = udpClient.Receive(ref remoteEP);
            var message = Encoding.UTF8.GetString(data);
            OnMessageReceived?.Invoke(message);
        }
    }

    public void Ping() {
        var data = Encoding.UTF8.GetBytes(pingMessage);
        udpClient.Send(data, data.Length, "255.255.255.255", UDPPort);
    }
}
