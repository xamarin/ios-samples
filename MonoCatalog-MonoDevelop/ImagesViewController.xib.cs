//
// Images sample port to C#/MonoTouch
//
using System;
using UIKit;
using Foundation;

namespace MonoCatalog {
	
	public partial class ImagesViewController : UIViewController {
	
		// Loads the inteface from the NIB file.
		public ImagesViewController () : base ("ImagesViewController", null)
		{
		}
	
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			Title = "Images";
	
			imageView.AnimationImages = new UIImage [] {
				UIImage.FromFile ("images/scene1.jpg"),
				UIImage.FromFile ("images/scene2.jpg"),
				UIImage.FromFile ("images/scene3.jpg"),
				UIImage.FromFile ("images/scene4.jpg"),
				UIImage.FromFile ("images/scene5.jpg")
			};
			imageView.AnimationDuration = 5;
			imageView.StopAnimating ();
		}
	
		//
		// Try to clean up resources to assist the GC, this is called
		// in response to low-memory conditions
		//
		public override void ViewDidUnload ()
		{
			base.ViewDidUnload ();
			imageView = null;
			slider = null;
		}
	
		partial void sliderAction (UISlider sender)
		{
			imageView.AnimationDuration = sender.Value;
			if (!imageView.IsAnimating)
				imageView.StartAnimating ();
		}
	
		public override void ViewWillDisappear (bool animated)
		{
			imageView.StopAnimating ();
			NavigationController.NavigationBar.BarStyle = UIBarStyle.Default;
			UIApplication.SharedApplication.StatusBarStyle = UIStatusBarStyle.Default;
		}
	
		public override void ViewWillAppear (bool animated)
		{
			imageView.StartAnimating ();
			
			// for aesthetic reasons (the background is black), make the nav bar black for this particular page                                  
			NavigationController.NavigationBar.BarStyle = UIBarStyle.Black;
			UIApplication.SharedApplication.StatusBarStyle = UIStatusBarStyle.BlackOpaque;
		}
	
	}
}
