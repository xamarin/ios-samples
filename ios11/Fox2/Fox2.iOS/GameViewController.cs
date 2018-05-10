
namespace Fox2
{
    using System;
    using UIKit;

    public partial class GameViewControllerIOS : UIViewController
    {
        private GameController gameController;

        public GameViewControllerIOS(IntPtr handle) : base(handle) { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // 1.3x on iPads
            if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad)
            {
                this.gameView.ContentScaleFactor = NMath.Min(1.3f, this.gameView.ContentScaleFactor);
                this.gameView.PreferredFramesPerSecond = 60;
            }

            this.gameController = new GameController(this.gameView);

            // Configure the view
            this.gameView.BackgroundColor = UIColor.Black;
        }

        public override bool ShouldAutorotate()
        {
            return true;
        }

        public override bool PrefersStatusBarHidden()
        {
            return true;
        }

        public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations()
        {
            var result = UIInterfaceOrientationMask.All;
            if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone)
            {
                result = UIInterfaceOrientationMask.AllButUpsideDown;
            }

            return result;
        }
    }
}