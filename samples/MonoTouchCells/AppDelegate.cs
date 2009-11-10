using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

using System;
using System.Drawing;

[Register]
public partial class AppDelegate : UIApplicationDelegate {
    DetailViewController detailViewController;

    public override void FinishedLaunching (UIApplication app)
    {
		detailViewController = new DetailViewController ("DetailViewController", null);
		detailViewController.LoadView();

		window.AddSubview(navController.View);
		window.MakeKeyAndVisible();
               
    }

	public void ShowDetail (CustomCell cell)
	{
		// make the image frame size the same as the image size
		UIImage checkedImage = UIImage.FromFile ("checked.png");
		
		RectangleF finalFrame = detailViewController.checkedImage.Frame;
		finalFrame.Width = checkedImage.Size.Width;
		finalFrame.Height = checkedImage.Size.Height;
		detailViewController.checkedImage.Frame = finalFrame;
		
		detailViewController.itemTitle.Text = cell.Title;
		detailViewController.checkedImage.Image = cell.Checked ? checkedImage : UIImage.FromFile ("unchecked.png");
		
		this.navController.PushViewController(detailViewController, true);
	}
}
