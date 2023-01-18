using Godot;

public class Body : KinematicBody
{
    [Export]
    private float speed = 4f;

    [Export]
    private float sensitiveness = 4f;

    private Vector3 direction = Vector3.Zero;

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventScreenDrag drag)
        {
            var half = GetViewport().GetVisibleRect().Size.x * 0.5;

            if (drag.Position.x < half)
            {
                MoveDirection(drag);
            }

            if (drag.Position.x > half)
            {
                MoveRotation(drag);
            }
        }

        if (@event is InputEventScreenTouch && !@event.IsPressed())
        {
            direction = Vector3.Zero;
        }
    }

    private void MoveDirection(InputEventScreenDrag drag)
    {
        var relative = new Vector3(drag.Relative.x, 0f, drag.Relative.y);

        if (relative.Length() < sensitiveness)
        {
            return;
        }

        direction = relative.Normalized();
    }

    private void MoveRotation(InputEventScreenDrag drag)
    {
        RotationDegrees += new Vector3(0f, -1f * drag.Relative.x, 0f);
    }

    public override void _PhysicsProcess(float delta)
    {
        var vector = direction.Rotated(Vector3.Up, Rotation.y) * speed * delta;

        MoveAndCollide(vector);
    }
}
