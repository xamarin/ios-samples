
namespace Fox2.tvOS
{
    using System;
    using UIKit;

    public partial class GameViewControllerTVOS : UIViewController
    {
        private GameController gameController;

        public GameViewControllerTVOS(IntPtr handle) : base(handle) { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            this.gameController = new GameController(this.gameView);

            // Configure the view
            this.gameView.BackgroundColor = UIColor.Black;
        }
    }
}