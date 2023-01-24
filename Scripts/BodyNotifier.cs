using Godot;
using System;

public class BodyNotifier : Node
{
    [Export]
    private NodePath bodyPath;

    [Export]
    private float tick = 0.2f;

    private float currentTick = 0;

    private Spatial body;

    private UdpClient udpClient;

    private Transform lastTransform;

    private float lastRotationY;

    public override void _Ready()
    {
        udpClient = AutoLoad.Of(this).UdpClient;

        body = GetNode<Spatial>(bodyPath);
    }

    public override void _PhysicsProcess(float delta)
    {
        if (currentTick < tick)
        {
            currentTick += tick;

            return;
        }

        UpdateTransform();

        UpdateRotation();

        currentTick -= tick;
    }

    private void UpdateTransform()
    {
        var currentTransform = body.Transform;

        NotifyOrigin(currentTransform.origin);

        lastTransform = currentTransform;
    }

    private void UpdateRotation()
    {
        var currentRotationY = body.RotationDegrees.y;

        NotifyRotationY(currentRotationY);

        lastRotationY = currentRotationY;
    }


    private void NotifyOrigin(Vector3 origin)
    {
        if (lastTransform.origin == origin)
        {
            return;
        }

        var packet = new OutgoingPacket.UpdateOrigin
        {
            origin = origin
        };

        udpClient.Write(packet.Serialize());
    }

    private void NotifyRotationY(float y)
    {
        if (lastRotationY == y)
        {
            return;
        }

        var packet = new OutgoingPacket.UpdateRotation
        {
            y = y
        };

        udpClient.Write(packet.Serialize());
    }
}
