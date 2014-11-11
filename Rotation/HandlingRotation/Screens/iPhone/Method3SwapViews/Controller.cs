using System;
using UIKit;
using Foundation;

namespace HandlingRotation.Screens.iPhone.Method3SwapViews {
	public class Controller : UIViewController {

		public Controller ()
		{
		}
		
		// Ordinarily, we'd do this constructor, but since we're going to conditionally load
		// either the portraitView or the Landscape view, we should have a blank constructor
		// and then do the view loading in LoadView
		//public Controller () : base("PortraitView", null) { }

		public override void LoadView ()
		{
			base.LoadView ();
			
			switch (InterfaceOrientation) {
				// if we're switching to landscape
				case UIInterfaceOrientation.LandscapeLeft:
				case UIInterfaceOrientation.LandscapeRight:

					NSBundle.MainBundle.LoadNib ("LandscapeView", this, null);
					
					break;
				
				// we're switching back to portrait
				case UIInterfaceOrientation.Portrait:
				case UIInterfaceOrientation.PortraitUpsideDown:

					NSBundle.MainBundle.LoadNib ("PortraitView", this, null);
					
					break;
			}
		}
		
		/// <summary>
		/// Called after the view has been loaded
		/// </summary>
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
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
			switch (toInterfaceOrientation)
			{
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
		/// must handle the rotation of your view and associated subviews
		/// </summary>
		public override void WillAnimateRotation (UIInterfaceOrientation toInterfaceOrientation, double duration)
		{
			base.WillAnimateRotation (toInterfaceOrientation, duration);
			
			switch (toInterfaceOrientation) {
				// if we're switching to landscape
				case UIInterfaceOrientation.LandscapeLeft:
				case UIInterfaceOrientation.LandscapeRight:

					NSBundle.MainBundle.LoadNib ("LandscapeView", this, null);
					
					break;
				
				// we're switch back to portrait
				case UIInterfaceOrientation.Portrait:
				case UIInterfaceOrientation.PortraitUpsideDown:
					
					NSBundle.MainBundle.LoadNib ("PortraitView", this, null);
					
					break;
			}
		}
	}
}
