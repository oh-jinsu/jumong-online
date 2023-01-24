using Godot;

public class Actor : KinematicBody
{
    private TcpClient tcpClient;

    private UdpClient udpClient;

    public override void _Ready()
    {
        tcpClient = AutoLoad.Of(this).TcpClient;

        tcpClient.Subscribe(TcpIncomingListener);

        udpClient = AutoLoad.Of(this).UdpClient;

        udpClient.Subscribe(UdpIncomingListener);
    }

    private void UdpIncomingListener(IncomingPacket packet)
    {
        if (packet is IncomingPacket.UpdateOrigin updateOrigin)
        {
            if (updateOrigin.id != Name)
            {
                return;
            }

            Translation = updateOrigin.origin;
        }

        if (packet is IncomingPacket.UpdateRotation updateRotation)
        {
            if (updateRotation.id != Name)
            {
                return;
            }

            RotationDegrees = new Vector3(RotationDegrees.x, updateRotation.y, RotationDegrees.z);
        }
    }

    private void TcpIncomingListener(IncomingPacket packet)
    {

        if (packet is IncomingPacket.GoodBye goodBye)
        {
            if (goodBye.id != Name)
            {
                return;
            }

            QueueFree();
        }
    }

    public override void _ExitTree()
    {
        tcpClient.Unsubscribe(TcpIncomingListener);

        udpClient.Unsubscribe(UdpIncomingListener);
    }
}
