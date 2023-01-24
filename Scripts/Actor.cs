using Godot;

public class Actor : KinematicBody
{
    private UdpClient udpClient;

    public override void _Ready()
    {
        udpClient = AutoLoad.Of(this).UdpClient;

        udpClient.Subscribe(TcpIncomingListener);
    }

    private void TcpIncomingListener(IncomingPacket packet)
    {
        if (packet is IncomingPacket.UpdateOrigin updateOrigin)
        {
            if (updateOrigin.id != Name)
            {
                return;
            }
            GD.Print(updateOrigin.origin);
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
        udpClient.Unsubscribe(TcpIncomingListener);
    }
}
