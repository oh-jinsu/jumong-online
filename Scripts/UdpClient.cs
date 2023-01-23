using Godot;

public class UdpClient : Node
{
    public delegate void Listener(IncomingPacket packet);

    private PacketPeerUDP peer = new PacketPeerUDP();

    private Listener listener;

    public bool ConnectToHost()
    {
        if (peer.ConnectToHost("127.0.0.1", 3000) != Error.Ok)
        {
            return false;
        }

        return IsConnectedToHost();
    }

    public bool IsConnectedToHost()
    {
        return peer.IsConnectedToHost();
    }

    public void Subscribe(Listener listener)
    {
        this.listener += listener;
    }

    public void Unsubscribe(Listener listener)
    {
        this.listener -= listener;
    }

    public bool Write(byte[] buffer)
    {
        if (!IsConnectedToHost())
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
            var buffer = peer.GetPacket();

            var packet = IncomingPacket.Deserialize(buffer);

            listener?.Invoke(packet);
        }
    }

    public void Close()
    {
        peer.Close();
    }
}
