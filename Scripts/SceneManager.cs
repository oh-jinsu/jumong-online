using Godot;

public class SceneManager : Node
{
    private object arguments;

    private MutableObservable<Node> currentScene = new();

    public Observable<Node> CurrentScene
    {
        get
        {
            return currentScene;
        }
    }

    public T GetArguments<T>()
    {
        return (T)arguments;
    }

    public override void _Ready()
    {
        var root = GetTree().Root;

        currentScene.Value = root.GetChild(root.GetChildCount() - 1);
    }

    public void GoTo(string path, object arguments = null)
    {
        this.arguments = arguments;

        CallDeferred(nameof(DeferredGoTo), path);
    }

    private void DeferredGoTo(string path)
    {
        currentScene.Value.Free();

        var nextScene = (PackedScene)GD.Load(path);

        currentScene.Value = nextScene.Instance();

        GetTree().Root.AddChild(currentScene.Value);

        GetTree().CurrentScene = currentScene.Value;
    }
}
