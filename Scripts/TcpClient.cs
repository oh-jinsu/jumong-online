using Godot;
using System;

public class TcpClient : Node
{
    public delegate void IncomingListener(IncomingPacket packet);

    private IncomingListener listener;

    private StreamPeerTCP peer = new StreamPeerTCP();

    public bool ConnectToHost()
    {
        if (peer.ConnectToHost("127.0.0.1", 3000) != Error.Ok)
        {
            return false;
        }

        for (int i = 0; i < 5; i++)
        {
            if (peer.GetStatus() != StreamPeerTCP.Status.Connecting)
            {
                break;
            }

            OS.DelayMsec(100);
        }

        return IsConnectedToHost();
    }

    public bool IsConnectedToHost()
    {
        return peer.GetStatus() == StreamPeerTCP.Status.Connected;
    }

    public void Subscribe(IncomingListener listener)
    {
        this.listener += listener;
    }

    public void Unsubscribe(IncomingListener listener)
    {
        this.listener -= listener;
    }

    public bool Write(byte[] buffer)
    {
        if (!IsConnectedToHost())
        {
            return false;
        }

        var fullBufer = new byte[2 + buffer.Length];

        var length = BitConverter.GetBytes(Convert.ToUInt16(buffer.Length));

        Buffer.BlockCopy(length, 0, fullBufer, 0, 2);

        Buffer.BlockCopy(buffer, 0, fullBufer, 2, buffer.Length);

        if (peer.PutData(fullBufer) != Error.Ok)
        {
            DisconnectFromHost();
        }

        return true;
    }

    public override void _Process(float delta)
    {
        if (!IsConnectedToHost())
        {
            return;
        }

        int length;

        while ((length = peer.GetAvailableBytes()) > 0)
        {
            var data = peer.GetPartialData(length);

            if ((Error)data[0] != Error.Ok)
            {
                DisconnectFromHost();

                return;
            }

            var buffer = (byte[])data[1];

            int index = 0;

            while (index < buffer.Length)
            {
                var size = BitConverter.ToUInt16(buffer, index);

                if (size > buffer.Length - index - 2)
                {
                    GD.Print("bytes not enough " + (buffer.Length - index - 2) + "/" + size);

                    return;
                }

                var packetBuffer = new byte[size];

                Buffer.BlockCopy(buffer, index + 2, packetBuffer, 0, size);

                var packet = IncomingPacket.Deserialize(packetBuffer);

                listener?.Invoke(packet);

                index += size + 2;
            }
        }
    }

    public void DisconnectFromHost()
    {
        peer.DisconnectFromHost();
    }
}
