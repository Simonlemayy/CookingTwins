using Godot;

namespace Player
{
    public partial class Player : CharacterBody2D
    {
        private enum PLAYER_STATE : byte
        {
            PLAYER_STATE_IDLE,
            PLAYER_STATE_WALKING,
            PLAYER_STATE_SLIDING
        }

        [Export]
        public int BaseSpeed = 260;

        [Export]
        public int SlideSpeed = 360;
        private float Orientation { get; set; } = 0.0f;

        private AnimationTree _AnimationTree;
        private AnimationNodeStateMachinePlayback _AnimationStateMachine;

        private PLAYER_STATE _CurrentState;

        private int _CurrentSpeed;

        public override void _Ready()
        {
            _AnimationTree = GetNode<AnimationTree>("AnimationTree");
            _AnimationStateMachine = (AnimationNodeStateMachinePlayback)_AnimationTree.Get("parameters/playback");

            _CurrentSpeed = BaseSpeed;
        }

        public void UpdateAnimationParameters()
        {
            switch (_CurrentState)
            { 
                case PLAYER_STATE.PLAYER_STATE_IDLE:
                    _AnimationStateMachine.Travel("Idle");
                    break;

                case PLAYER_STATE.PLAYER_STATE_WALKING:
                    _AnimationStateMachine.Travel("Walk");
                    break;

                case PLAYER_STATE.PLAYER_STATE_SLIDING:
                    // TODO: Set player sliding animation here
                    break;
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
            if (inputDirection.LengthSquared() != 0 && _CurrentState != PLAYER_STATE.PLAYER_STATE_SLIDING)
            {
                _CurrentState = PLAYER_STATE.PLAYER_STATE_WALKING;
            }
            else if (inputDirection.LengthSquared() == 0)
            {
                _CurrentState = PLAYER_STATE.PLAYER_STATE_IDLE;
            }

            if (!inputDirection.IsNormalized())
            {
                inputDirection = inputDirection.Normalized();
            }

            if (Input.IsActionPressed("slide") && _CurrentState != PLAYER_STATE.PLAYER_STATE_SLIDING)
            {
                _CurrentState = PLAYER_STATE.PLAYER_STATE_SLIDING;
                _CurrentSpeed = SlideSpeed;
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
            Velocity *= _CurrentSpeed;
            UpdateAnimationParameters();
            SetSpriteOrientation();
            MoveAndSlide();
        }
    }
}