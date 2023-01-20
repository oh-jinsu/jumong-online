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

        var buffer = new byte[14];

        Buffer.BlockCopy(new byte[] { 2, 0 }, 0, buffer, 0, 2);

        Buffer.BlockCopy(BitConverter.GetBytes(origin.x), 0, buffer, 2, 4);

        Buffer.BlockCopy(BitConverter.GetBytes(origin.y), 0, buffer, 6, 4);

        Buffer.BlockCopy(BitConverter.GetBytes(origin.z), 0, buffer, 10, 4);

        udpClient.Write(buffer);
    }

    private void NotifyRotationY(float y)
    {
        if (lastRotationY == y)
        {
            return;
        }

        var buffer = new byte[6];

        Buffer.BlockCopy(new byte[] { 3, 0 }, 0, buffer, 0, 2);

        Buffer.BlockCopy(BitConverter.GetBytes(y), 0, buffer, 2, 4);

        udpClient.Write(buffer);
    }
}
