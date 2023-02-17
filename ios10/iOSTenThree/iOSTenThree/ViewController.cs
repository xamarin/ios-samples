using System;
using UIKit;
using CoreGraphics;

namespace iOSTenThree {
	public partial class ViewController : UIViewController {
		#region Constructors
		protected ViewController (IntPtr handle) : base (handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}
		#endregion

		#region Override Methods
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Set the size of the scoll view content
			ScrollView.ContentSize = new CGSize (XamarinImage.Frame.Width, XamarinImage.Frame.Height);

			// Can the app select a different icon?
			PrimaryIconButton.Enabled = UIApplication.SharedApplication.SupportsAlternateIcons;
			AlternateIconButton.Enabled = UIApplication.SharedApplication.SupportsAlternateIcons;
		}
		#endregion

		#region Custom Actions
		partial void UsePrimaryIcon (Foundation.NSObject sender)
		{
			UIApplication.SharedApplication.SetAlternateIconName (null, (err) => {
				Console.WriteLine ("Set Primary Icon: {0}", err);
			});
		}

		partial void UseAlternateIcon (Foundation.NSObject sender)
		{
			UIApplication.SharedApplication.SetAlternateIconName ("AppIcon2", (err) => {
				Console.WriteLine ("Set Alternate Icon: {0}", err);
			});
		}

		partial void ToggleIndexDisplayMode (Foundation.NSObject sender)
		{
			// Toggle the display mode
			if (ScrollView.IndexDisplayMode == UIScrollViewIndexDisplayMode.AlwaysHidden) {
				ScrollView.IndexDisplayMode = UIScrollViewIndexDisplayMode.Automatic;
				Console.WriteLine ("Scroll View Index Display: Automatic Mode");
			} else {
				ScrollView.IndexDisplayMode = UIScrollViewIndexDisplayMode.AlwaysHidden;
				Console.WriteLine ("Scroll View Index Display: Always Hidden Mode");
			}
		}
		#endregion
	}
}
