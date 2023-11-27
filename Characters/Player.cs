using Godot;

namespace Player
{
    public partial class Player : CharacterBody2D
    {
        [Export]
        public int Speed { get; set; } = 260;
        private float Orientation { get; set; } = 0.0f;

        private AnimationTree _AnimationTree;
        public override void _Ready()
        {
            _AnimationTree = GetNode<AnimationTree>("AnimationTree");
        }

        public void UpdateAnimationParameters()
        {

            if (Velocity == Vector2.Zero)
            {
                _AnimationTree.Set("parameters/conditions/is_idle", true);
                _AnimationTree.Set("parameters/conditions/is_walking", false);
            }
            else
            {
                _AnimationTree.Set("parameters/conditions/is_idle", false);
                _AnimationTree.Set("parameters/conditions/is_walking", true);
            }
        }
        public void SetSpriteOrientation()
        {
            Vector2 direction = (GetGlobalMousePosition() - GlobalPosition).Normalized();
            direction.Y *= -1;
            _AnimationTree.Set("parameters/Idle/blend_position", direction);
            _AnimationTree.Set("parameters/Walk/blend_position", direction);
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
            Velocity *= Speed;
            UpdateAnimationParameters();
            SetSpriteOrientation();
            MoveAndSlide();
        }
    }
}