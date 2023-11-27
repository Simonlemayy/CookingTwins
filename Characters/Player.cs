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
        public int SlideSpeed = 500;
        private float Orientation { get; set; } = 0.0f;

        private AnimationTree _AnimationTree;
        private AnimationNodeStateMachinePlayback _AnimationStateMachine;
        private Timer _SlideTimer;
        private Vector2 _SlideVelocity;

        private PLAYER_STATE _CurrentState;
        private int _CurrentSpeed;

        private Camera2D _Camera2D;

        public override void _Ready()
        {
            _AnimationTree = GetNode<AnimationTree>("AnimationTree");
            _AnimationStateMachine = (AnimationNodeStateMachinePlayback)_AnimationTree.Get("parameters/playback");

            _SlideTimer = this.GetNode<Timer>("SlideTimer");
            _SlideTimer.Timeout += _HandleSlide;

            _CurrentSpeed = BaseSpeed;
            _Camera2D = GetNode<Camera2D>("Camera2D");
        }

        private void _HandleSlide()
        {
            _CurrentState = PLAYER_STATE.PLAYER_STATE_IDLE;
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
            if (!inputDirection.IsNormalized())
            {
                inputDirection = inputDirection.Normalized();
            }

            // While we're sliding we shouldn't update the player state
            if (inputDirection.LengthSquared() == 0 && _CurrentState != PLAYER_STATE.PLAYER_STATE_SLIDING)
            {
                _CurrentState = PLAYER_STATE.PLAYER_STATE_IDLE;
            }
            else if (inputDirection.LengthSquared() != 0 && _CurrentState != PLAYER_STATE.PLAYER_STATE_SLIDING)
            {
                _CurrentState = PLAYER_STATE.PLAYER_STATE_WALKING;
            }

            if (Input.IsActionPressed("slide") && _CurrentState != PLAYER_STATE.PLAYER_STATE_SLIDING)
            {
                _CurrentState = PLAYER_STATE.PLAYER_STATE_SLIDING;
                _CurrentSpeed = SlideSpeed;
                _SlideVelocity = inputDirection;

                _SlideTimer.Start();
            }

            // If we're currently sliding we want to lock the velocity vector
            if (_CurrentState == PLAYER_STATE.PLAYER_STATE_SLIDING)
            {
                Velocity = _SlideVelocity;
            }
            else
            {
                Velocity = inputDirection;
            }
        }

        public void GetRotation()
        {
            Vector2 mousePosition = GetGlobalMousePosition();
            Vector2 direction = (mousePosition - GlobalPosition).Normalized();

            Rotation = Mathf.Atan2(direction.Y, direction.X);
        }

        public void SetCameraPosition()
        {
            Vector2 CameraOffset = ((GetGlobalMousePosition() - GlobalPosition) / 2).Normalized();
            _Camera2D.Position = _Camera2D.Position.Lerp(CameraOffset * 40, CameraOffset.Length() / 10);
        }

        public override void _PhysicsProcess(double delta)
        {
            GetInput();
            Velocity *= _CurrentSpeed;
            UpdateAnimationParameters();
            SetSpriteOrientation();
            SetCameraPosition();
            MoveAndSlide();
        }
    }
}
