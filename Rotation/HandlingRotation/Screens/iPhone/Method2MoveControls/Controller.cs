using System;
using UIKit;

namespace HandlingRotation.Screens.iPhone.Method2MoveControls {
	public class Controller : UIViewController
	{
		UIButton button1 = UIButton.FromType (UIButtonType.RoundedRect);
		UIButton button2 = UIButton.FromType (UIButtonType.RoundedRect);
		UIImageView image = new UIImageView(UIImage.FromBundle("icon-144.png"));

		public Controller ()
		{
		}

		/// <summary>
		/// Called after the view has been loaded
		/// </summary>
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			View.BackgroundColor = UIColor.White;
			Title = "Manually Moving Controls";
			
			// call our helper method to position the controls
			PositionControls (this.InterfaceOrientation);
			
			// configure our controls and add them to the view
			button1.SetTitle ("Button 1", UIControlState.Normal);
			View.AddSubview (button1);
			button2.SetTitle ("Button 2", UIControlState.Normal);
			View.AddSubview (button2);
			View.AddSubview (image);
	
		}

		/// <summary>
		/// When the device rotates, the OS calls this method to determine if it should try and rotate the
		/// application and then call WillAnimateRotation
		/// </summary>
		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			// we're passed to orientation that it will rotate to. in our case, we could
			// just return true, but this switch illustrates how you can test for the 
			// different cases
			switch (toInterfaceOrientation) {
				case UIInterfaceOrientation.LandscapeLeft:
				case UIInterfaceOrientation.LandscapeRight:
				case UIInterfaceOrientation.Portrait:
				case UIInterfaceOrientation.PortraitUpsideDown:
				default:
					return true;
			}
		}

		/// <summary>
		/// is called when the OS is going to rotate the application. It handles rotating the status bar
		/// if it's present, as well as it's controls like the navigation controller and tab bar, but you 
		/// must handle the rotation of your view and associated subviews. This call is wrapped in an 
		/// animation block in the underlying implementation, so it will automatically animate your control
		/// repositioning.
		/// </summary>
		public override void WillAnimateRotation (UIInterfaceOrientation toInterfaceOrientation, double duration)
		{
			base.WillAnimateRotation (toInterfaceOrientation, duration);
			
			// call our helper method to position the controls
			PositionControls (toInterfaceOrientation);
		}

		/// <summary>
		/// A helper method to position the controls appropriately, based on the 
		/// orientation
		/// </summary>
		protected void PositionControls (UIInterfaceOrientation toInterfaceOrientation)
		{
			// depending one what orientation we start in, we want to position our controls
			// appropriately
			switch (toInterfaceOrientation) {
				// if we're switching to landscape
				case UIInterfaceOrientation.LandscapeLeft:
				case UIInterfaceOrientation.LandscapeRight:
					
					// reposition the buttons
					button1.Frame = new CoreGraphics.CGRect (10, 10, 100, 33);
					button2.Frame = new CoreGraphics.CGRect (10, 200, 100, 33);
					
					// reposition the image
					image.Frame = new CoreGraphics.CGRect (240, 25, image.Frame.Width, image.Frame.Height);
					
					break;
				
				// we're switching back to portrait
				case UIInterfaceOrientation.Portrait:
				case UIInterfaceOrientation.PortraitUpsideDown:
					
					// reposition the buttons
					button1.Frame = new CoreGraphics.CGRect (10, 10, 100, 33);
					button2.Frame = new CoreGraphics.CGRect (200, 10, 100, 33);
					
					// reposition the image
					image.Frame = new CoreGraphics.CGRect (20, 150, this.image.Frame.Width, this.image.Frame.Height);
					
					break;
			}
		}
	}
}