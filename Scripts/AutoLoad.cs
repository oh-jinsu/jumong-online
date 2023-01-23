using Godot;

public class AutoLoad
{
    private Node node;

    private AutoLoad(Node node)
    {
        this.node = node;
    }

    public static AutoLoad Of(Node node)
    {
        return new AutoLoad(node);
    }

    public SceneManager SceneManager
    {
        get
        {
            return node.GetNode<SceneManager>("/root/SceneManager");
        }
    }

    public Window Window
    {
        get
        {
            return node.GetNode<Window>("/root/Window");
        }
    }

    public UdpClient UdpClient
    {
        get
        {
            return node.GetNode<UdpClient>("/root/UdpClient");
        }
    }

    public TcpClient TcpClient
    {
        get
        {
            return node.GetNode<TcpClient>("/root/TcpClient");
        }
    }

    public HttpClient HttpClient
    {
        get
        {
            return node.GetNode<HttpClient>("/root/HttpClient");
        }
    }
}
