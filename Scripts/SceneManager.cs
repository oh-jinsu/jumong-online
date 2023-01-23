using Godot;
using System.Collections.Generic;

public class SceneManager : Node
{
    private class Stackable
    {
        public string path;

        public object arguments;
    }

    private Stack<Stackable> stack = new();

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
        return (T)stack.Peek().arguments;
    }

    public override void _Ready()
    {
        var root = GetTree().Root;

        currentScene.Value = root.GetChild(root.GetChildCount() - 1);

        stack.Push(new Stackable
        {
            path = Scene.Login,
        });
    }

    public void GoBack()
    {
        if (stack.Count == 0)
        {
            return;
        }

        stack.Pop();

        var item = stack.Pop();

        CallDeferred(nameof(DeferredGoTo), item.path);
    }

    public void GoTo(string path, object arguments = null)
    {
        stack.Push(new Stackable
        {
            path = path,
            arguments = arguments,
        });

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
