using Godot;
using System;

public partial class player_cat : CharacterBody2D
{
	[Export]
	public int Speed { get; set; } = 200;
	public void GetInput()
	{
		Vector2 inputDirection = Input.GetVector("left", "right", "up", "down");
		if (!inputDirection.IsNormalized())
		{
			inputDirection = inputDirection.Normalized();
		}
		Velocity = inputDirection;
	}
	public override void _PhysicsProcess(double delta)
	{
		GetInput();
		Velocity = Velocity * Speed;
		MoveAndSlide();
    }
}
