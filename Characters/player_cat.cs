using Godot;
using System;

public partial class player_cat : CharacterBody2D
{
	[Export]
	public int Speed { get; set; } = 200;
    private float orientation { get; set; } = 0.0f;

    private AnimationTree _animationTree;

    public override void _Ready()
    {
		_animationTree = GetNode<AnimationTree>("AnimationTree");
    }
    public void SetSpriteOrientation()
    {
        Vector2 direction = (GetGlobalMousePosition() - GlobalPosition).Normalized();
		direction.Y = direction.Y * (-1);
		_animationTree.Set("parameters/blend_position", direction);
    }
    public void GetInput()
	{
		Vector2 inputDirection = Input.GetVector("left", "right", "up", "down");
		if (!inputDirection.IsNormalized())
		{
			inputDirection = inputDirection.Normalized();
		}
		Velocity = inputDirection;
	}
	public void GetRotation()
	{
		Vector2 mousePosition = GetGlobalMousePosition();
		Vector2 direction = (mousePosition - GlobalPosition).Normalized();

		Rotation = Mathf.Atan2(direction.Y, direction.X);

	}
	public override void _PhysicsProcess(double delta)
	{
		GetInput();
		SetSpriteOrientation();
		Velocity = Velocity * Speed;
		MoveAndSlide();
    }
}
