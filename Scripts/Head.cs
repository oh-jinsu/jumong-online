using Godot;

public class Head : Camera
{
    [Export]
    private float velocity = 0.5f;

    [Export]
    private float maxRotationDegreeX;

    [Export]
    private float minRotationDegreeX;

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventScreenDrag drag)
        {
            var half = GetViewport().GetVisibleRect().Size.x * 0.5;

            if (drag.Position.x <= half)
            {
                return;
            }

            var next = RotationDegrees + new Vector3(-1f  * velocity * drag.Relative.y, 0f, 0f);

            if (next.x < minRotationDegreeX || next.x > maxRotationDegreeX)
            {
                return;
            }

            RotationDegrees = next;
        }
    }
}
