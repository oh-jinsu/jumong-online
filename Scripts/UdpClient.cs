using Godot;

public class UdpClient : Node
{
    private PacketPeerUDP peer = new PacketPeerUDP();

    public override void _Ready()
    {
        peer.ConnectToHost("127.0.0.1", 3001);
    }

    public bool Write(byte[] buffer)
    {
        if (!peer.IsConnectedToHost())
        {
            return false;
        }

        peer.PutPacket(buffer);

        return true;
    }

    public override void _Process(float delta)
    {
        while (peer.GetAvailablePacketCount() > 0)
        {
            var packet = peer.GetPacket();

            GD.Print(packet.GetStringFromUTF8());
        }
    }
}
