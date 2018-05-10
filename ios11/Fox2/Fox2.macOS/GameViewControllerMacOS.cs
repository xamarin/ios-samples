
namespace Fox2.macOS
{
    using AppKit;
    using Fox2.Extensions;
    using System;

    public partial class GameViewControllerMacOS : NSViewController
    {
        private GameController gameController;

        public GameViewControllerMacOS(IntPtr handle) : base(handle) { }

        protected GameViewMacOS GameView
        {
            get
            {
                var result = this.View as GameViewMacOS;
                if (result == null)
                {
                    throw new Exception($"Expected {nameof(GameViewMacOS)} from Main.storyboard.");
                }

                return result;
            }
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            this.gameController = new GameController(this.GameView);

            // Configure the view
            this.GameView.BackgroundColor = NSColor.Black;

            // Link view and controller
            this.GameView.ViewController = new WeakReference<GameViewControllerMacOS>(this);
        }

		public bool KeyDown(NSView view, NSEvent @event)
        {
            var result = true;
            var @continue = true;

            var characterDirection = this.gameController.CharacterDirection;
            var cameraDirection = this.gameController.CameraDirection;

            var updateCamera = false;
            var updateCharacter = false;

            switch (@event.KeyCode)
            {
                case 126:
                    // Up
                    if (!@event.IsARepeat)
                    {
                        characterDirection.Y = -1;
                        updateCharacter = true;
                    }
                    break;

                case 125:
                    // Down
                    if (!@event.IsARepeat)
                    {
                        characterDirection.Y = 1;
                        updateCharacter = true;
                    }
                    break;

                case 123:
                    // Left
                    if (!@event.IsARepeat)
                    {
                        characterDirection.X = -1;
                        updateCharacter = true;
                    }
                    break;

                case 124:
                    // Right
                    if (!@event.IsARepeat)
                    {
                        characterDirection.X = 1;
                        updateCharacter = true;
                    }
                    break;

                case 13:
                    // Camera Up
                    if (!@event.IsARepeat)
                    {
                        cameraDirection.Y = -1;
                        updateCamera = true;
                    }
                    break;

                case 1:
                    // Camera Down
                    if (!@event.IsARepeat)
                    {
                        cameraDirection.Y = 1;
                        updateCamera = true;
                    }
                    break;

                case 0:
                    // Camera Left
                    if (!@event.IsARepeat)
                    {
                        cameraDirection.X = -1;
                        updateCamera = true;
                    }
                    break;

                case 2:
                    // Camera Right
                    if (!@event.IsARepeat)
                    {
                        cameraDirection.X = 1;
                        updateCamera = true;
                    }
                    break;

                case 49:
                    // Space
                    if (!@event.IsARepeat)
                    {
                        this.gameController.ControllerJump(true);
                    }

                    @continue = false;
                    break;

                case 8:
                    // c
                    if (!@event.IsARepeat)
                    {
                        this.gameController.ControllerAttack();
                    }

                    @continue = false;
                    break;

                default:
                    result = false;
                    @continue = false;
                    break;
            }

            if (@continue)
            {
                if (updateCharacter)
                {
                    this.gameController.CharacterDirection = characterDirection.AllZero() ? characterDirection : OpenTK.Vector2.Normalize(characterDirection);
                }

                if (updateCamera)
                {
                    this.gameController.CameraDirection = cameraDirection.AllZero() ? cameraDirection : OpenTK.Vector2.Normalize(cameraDirection);
                }
            }

            return result;
        }

		public bool KeyUp(NSView view, NSEvent @event)
        {
            var result = false;
            var @continue = true;

            var characterDirection = this.gameController.CharacterDirection;
            var cameraDirection = this.gameController.CameraDirection;

            var updateCamera = false;
            var updateCharacter = false;

            switch (@event.KeyCode)
            {
                case 36:
                    if (!@event.IsARepeat)
                    {
                        this.gameController.ResetPlayerPosition();
                    }

                    result = true;
                    @continue = false;
                    break;

                case 126:
                    // Up
                    if (!@event.IsARepeat && characterDirection.Y < 0)
                    {
                        characterDirection.Y = 0;
                        updateCharacter = true;
                    }
                    break;

                case 125:
                    // Down
                    if (!@event.IsARepeat && characterDirection.Y > 0)
                    {
                        characterDirection.Y = 0;
                        updateCharacter = true;
                    }
                    break;

                case 123:
                    // Left
                    if (!@event.IsARepeat && characterDirection.X < 0)
                    {
                        characterDirection.X = 0;
                        updateCharacter = true;
                    }
                    break;

                case 124:
                    // Right
                    if (!@event.IsARepeat && characterDirection.X > 0)
                    {
                        characterDirection.X = 0;
                        updateCharacter = true;
                    }
                    break;

                case 13:
                    // Camera Up
                    if (!@event.IsARepeat && cameraDirection.Y < 0)
                    {
                        cameraDirection.Y = 0;
                        updateCamera = true;
                    }
                    break;

                case 1:
                    // Camera Down
                    if (!@event.IsARepeat && cameraDirection.Y > 0)
                    {
                        cameraDirection.Y = 0;
                        updateCamera = true;
                    }
                    break;

                case 0:
                    // Camera Left
                    if (!@event.IsARepeat && cameraDirection.X < 0)
                    {
                        cameraDirection.X = 0;
                        updateCamera = true;
                    }
                    break;

                case 2:
                    // Camera Right
                    if (!@event.IsARepeat && cameraDirection.X > 0)
                    {
                        cameraDirection.X = 0;
                        updateCamera = true;
                    }
                    break;

                case 49:
                    // Space
                    if (!@event.IsARepeat)
                    {
                        this.gameController.ControllerJump(false);
                    }

                    result = true;
                    @continue = false;
                    break;

                default:
                    break;
            }

            if (@continue)
            {
                if (updateCharacter)
                {
                    this.gameController.CharacterDirection = characterDirection.AllZero() ? characterDirection : OpenTK.Vector2.Normalize(characterDirection);
                    @continue = false;
                    result = true;
                }

                if (@continue)
                {
                    if (updateCamera)
                    {
                        this.gameController.CameraDirection = cameraDirection.AllZero() ? cameraDirection : OpenTK.Vector2.Normalize(cameraDirection);
                        result = true;
                    }
                }
            }

            return result;
        }
    }
}