using Godot;

public class Window : Node
{
    private AcceptDialog dialog;

    public override void _Ready()
    {
        var node = AutoLoad.Of(this).SceneManager.CurrentScene.Subscribe(OnCurrentSceneChange);

        OnCurrentSceneChange(node);
    }

    private void OnCurrentSceneChange(Node node)
    {
        var scene = GD.Load<PackedScene>("res://Scenes/dialog.tscn");

        dialog = scene.Instance<AcceptDialog>();

        node.AddChild(dialog);
    }

    public void PushDialog(string message)
    {
        dialog.DialogText = message;

        dialog.PopupCentered(new Vector2(288, 160));
    }

    public override void _ExitTree()
    {
        AutoLoad.Of(this).SceneManager.CurrentScene.Unsubscribe(OnCurrentSceneChange);
    }
}
