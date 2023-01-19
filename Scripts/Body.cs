using Godot;

public class Body : KinematicBody
{
    [Export]
    private float speed = 4f;

    [Export]
    private float sensitiveness = 4f;

    [Export]
    private float gravity = 0.5f;

    [Export]
    private float maxAcceleration = 9f;

    private float acceleration = 0f;

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

        if (@event is InputEventScreenTouch touch && touch.Index == 0 && !@event.IsPressed())
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
        if (!IsOnFloor())
        {
            acceleration = Mathf.Min(acceleration + this.gravity, maxAcceleration);
        }
        else
        {
            acceleration = this.gravity;
        }

        var gravity = new Vector3(0f, -1f * acceleration, 0f);

        var velocity = direction.Rotated(Vector3.Up, Rotation.y) * speed + gravity;

        MoveAndSlide(velocity, Vector3.Up, true);
    }
}
