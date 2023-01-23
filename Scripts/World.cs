using Godot;
using System;

public class World : Spatial
{
    [Export]
    private PackedScene actor;

    private TcpClient tcpClient;

    private UdpClient udpClient;

    public override void _Ready()
    {
        var arguments = AutoLoad.Of(this).SceneManager.GetArguments<Scene.WorldArguments>();

        tcpClient = AutoLoad.Of(this).TcpClient;

        tcpClient.Subscribe(TcpListener);

        udpClient = AutoLoad.Of(this).UdpClient;

        Bootstrap(arguments);
    }

    private void Bootstrap(Scene.WorldArguments arguments)
    {
        ConnectToTcp(arguments.token);

        ConnectToUdp(arguments.token);
    }

    private void ConnectToTcp(String token)
    {
        if (!tcpClient.ConnectToHost())
        {
            GoBack();

            return;
        }

        var packet = new OutgoingPacket.HelloFromTcp
        {
            token = token,
        };


        if (!tcpClient.Write(packet.Serialize()))
        {
            GoBack();

            return;
        }
    }

    private void ConnectToUdp(String token)
    {
        if (!udpClient.ConnectToHost())
        {
            GoBack();

            return;
        }

        var packet = new OutgoingPacket.HelloFromUdp
        {
            token = token,
        };

        if (!udpClient.Write(packet.Serialize()))
        {
            GoBack();

            return;
        }
    }


    private void TcpListener(IncomingPacket incoming)
    {
        GD.Print(incoming);
    }

    private void GoBack()
    {
        AutoLoad.Of(this).SceneManager.GoBack();
    }

    public override void _ExitTree()
    {
        tcpClient.Unsubscribe(TcpListener);

        tcpClient?.DisconnectFromHost();

        udpClient?.Close();
    }
}
