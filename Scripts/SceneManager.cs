using Godot;
using static Godot.GD;

public class SceneManager : Node
{
    private MutableObservable<Node> currentScene = new();

    public Observable<Node> CurrentScene
    {
        get
        {
            return currentScene;
        }
    }

    private object arguments;

    public T GetArguments<T>()
    {
        return (T)arguments;
    }

    public override void _Ready()
    {
        var root = GetTree().Root;

        currentScene.Value = root.GetChild(root.GetChildCount() - 1);
    }

    public void GoTo(Scene scene)
    {
        CallDeferred(nameof(DeferredGoTo), scene);
    }

    private void DeferredGoTo(Scene scene)
    {
        currentScene.Value.Free();

        var path = GetPath(scene);

        var nextScene = (PackedScene)Load(path);

        currentScene.Value = nextScene.Instance();

        GetTree().Root.AddChild(currentScene.Value);

        GetTree().CurrentScene = currentScene.Value;
    }

    private static string GetPath(Scene scene)
    {
        switch (scene)
        {
            case Scene.Splash:
                return "res://Scenes/splash.tscn";
            case Scene.Login:
                return "res://Scenes/login.tscn";
            case Scene.World:
                return "res://Scenes/world.tscn";
            default:
                throw new System.Exception("Could not get the path");
        }
    }
}
