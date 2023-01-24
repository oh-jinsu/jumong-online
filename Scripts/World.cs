using Godot;

public class World : Spatial
{
    [Export]
    private PackedScene playerScene;

    [Export]
    private PackedScene actorScene;

    private TcpClient tcpClient;

    private UdpClient udpClient;

    private bool tcpHello = false;

    private bool udpHello = false;

    public override void _Ready()
    {
        tcpClient = AutoLoad.Of(this).TcpClient;

        tcpClient.Subscribe(TcpIncomingListener);

        udpClient = AutoLoad.Of(this).UdpClient;

        new System.Threading.Thread(ConnectToTcp).Start();
    }

    private void ConnectToTcp()
    {
        var arguments = AutoLoad.Of(this).SceneManager.GetArguments<Scene.WorldArguments>();

        if (!tcpClient.ConnectToHost())
        {
            GoBack();

            return;
        }

        var packet = new OutgoingPacket.HelloFromTcp
        {
            token = arguments.token,
        };


        if (!tcpClient.Write(packet.Serialize()))
        {
            GoBack();

            return;
        }
    }

    private void ConnectToUdp()
    {
        var arguments = AutoLoad.Of(this).SceneManager.GetArguments<Scene.WorldArguments>();

        if (!udpClient.ConnectToHost())
        {
            GoBack();

            return;
        }

        var packet = new OutgoingPacket.HelloFromUdp
        {
            token = arguments.token,
        };

        var buffer = packet.Serialize();

        for (int i = 0; i < 10; i++)
        {
            if (udpHello)
            {
                return;
            }

            if (!udpClient.Write(buffer))
            {
                GoBack();

                return;
            }

            OS.DelayMsec(250);
        }

        GoBack();
    }


    private void TcpIncomingListener(IncomingPacket packet)
    {
        if (packet is IncomingPacket.HelloFromTcp)
        {
            tcpHello = true;

            new System.Threading.Thread(ConnectToUdp).Start();
        }

        if (packet is IncomingPacket.HelloFromUdp helloFromUdp)
        {
            udpHello = true;

            CreatePlayer(helloFromUdp.id);
        }

        if (packet is IncomingPacket.Introduce introduce)
        {
            foreach (var id in introduce.ids)
            {
                CreateActor(id);
            }
        }

        if (packet is IncomingPacket.Welcome welcome)
        {
            CreateActor(welcome.id);
        }
    }

    private void CreatePlayer(string id)
    {
        var instance = playerScene.Instance();

        instance.Name = id;

        AddChild(instance);
    }

    private void CreateActor(string id)
    {
        var instance = actorScene.Instance();

        instance.Name = id;

        AddChild(instance);
    }

    private void GoBack()
    {
        AutoLoad.Of(this).SceneManager.GoTo(Scene.Login);
    }

    public override void _ExitTree()
    {
        tcpClient.Unsubscribe(TcpIncomingListener);

        tcpClient?.DisconnectFromHost();

        udpClient?.Close();
    }
}
