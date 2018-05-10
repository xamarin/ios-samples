
namespace Fox2.UI
{
    using CoreGraphics;
    using Foundation;
    using Fox2.Interfaces;
    using SpriteKit;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// The `SKNode` based menu.
    /// </summary>
    public class Menu : SKNode
    {
        private readonly List<Button> cameraButtons = new List<Button>();

        private readonly List<Slider> dofSliders = new List<Slider>();

        private const float ButtonMargin = 250f;
        private const float Duration = 0.3f;
        private const float MenuY = 40f;

        private bool isMenuHidden;

        public Menu(CGSize size) : base()
        {
            // Track mouse event
            base.UserInteractionEnabled = true;

            // Camera buttons
            var buttonLabels = new string[] { "Camera 1", "Camera 2", "Camera 3" };
            this.cameraButtons = buttonLabels.Select(label => new Button(label)).ToList();

            for (int i = 0; i < this.cameraButtons.Count; i++)
            {
                var button = this.cameraButtons[i];

                var x = button.Width / 2 + (i > 0 ? this.cameraButtons[i - 1].Position.X + this.cameraButtons[i - 1].Width / 2 + 10 : ButtonMargin);
                var y = size.Height - MenuY;
                button.Position = new CGPoint(x, y);
                button.SetClickedTarget(this.MenuChanged);
                this.AddChild(button);
            }

            // Depth of Field
            buttonLabels = new string[] { "fStop", "Focus" };
            this.dofSliders = buttonLabels.Select(label => new Slider(300, 10, label)).ToList();

            for (int i = 0; i < dofSliders.Count; i++)
            {
                var slider = dofSliders[i];
                slider.Position = new CGPoint(ButtonMargin, size.Height - i * 30.0f - 70.0f);
                slider.Alpha = 0.0f;
                this.AddChild(slider);
            }

            this.dofSliders[0].SetClickedTarget(this.CameraFStopChanged);
            this.dofSliders[1].SetClickedTarget(this.CameraFocusDistanceChanged);
        }

        public Menu(NSCoder coder)
        {
            throw new NotImplementedException("init(coder:) has not been implemented");
        }

        public IMenuDelegate Delegate { get; set; }

        public override bool Hidden
        {
            get
            {
                return this.isMenuHidden;
            }
            set
            {
                this.isMenuHidden = value;
                if (this.isMenuHidden)
                {
                    this.Hide();
                }
                else
                {
                    this.Show();
                }
            }
        }

        private void MenuChanged(Button sender)
        {
            this.HideSlidersMenu();

            var index = this.cameraButtons.IndexOf(sender);
            if (index >= 0)
            {
                this.Delegate?.DebugMenuSelectCameraAtIndex(index);
                if (index == 2)
                {
                    this.ShowSlidersMenu();
                }
            }
        }

        private void Show()
        {
            foreach (var button in this.cameraButtons)
            {
                button.Alpha = 0.0f;
                button.RunAction(SKAction.FadeInWithDuration(Duration));
            }

            this.isMenuHidden = false;
        }

        private void Hide()
        {
            foreach (var button in this.cameraButtons)
            {
                button.Alpha = 1.0f;
                button.RunAction(SKAction.FadeOutWithDuration(Duration));
            }

            this.HideSlidersMenu();
            this.isMenuHidden = true;
        }

        private void HideSlidersMenu()
        {
            foreach (var slider in this.dofSliders)
            {
                slider.RunAction(SKAction.FadeOutWithDuration(Duration));
            }
        }

        private void ShowSlidersMenu()
        {
            foreach (var slider in this.dofSliders)
            {
                slider.RunAction(SKAction.FadeInWithDuration(Duration));
            }

            this.dofSliders[0].Value = 0.1f;
            this.dofSliders[1].Value = 0.5f;

            this.CameraFStopChanged(this.dofSliders[0]);
            this.CameraFocusDistanceChanged(this.dofSliders[1]);
        }

        private void CameraFStopChanged(object sender)
        {
            if (this.Delegate != null)
            {
                this.Delegate.FStopChanged(this.dofSliders[0].Value + 0.2f);
            }
        }

        private void CameraFocusDistanceChanged(object sender)
        {
            if (this.Delegate != null)
            {
                this.Delegate.FocusDistanceChanged(this.dofSliders[1].Value * 20.0f + 3.0f);
            }
        }
    }
}