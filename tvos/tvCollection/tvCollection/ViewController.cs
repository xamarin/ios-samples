using System;
using Foundation;
using UIKit;
using tvCollection;

namespace MySingleView
{
	public partial class ViewController : UIViewController
	{
		#region Application Access
		public static AppDelegate App {
			get { return (AppDelegate)UIApplication.SharedApplication.Delegate; }
		}
		#endregion

		#region Constructors
		public ViewController (IntPtr handle) : base (handle)
		{
		}
		#endregion

		#region Override Methods
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			// Perform any additional setup after loading the view, typically from a nib.
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			// Update image with the currently selected one
			CityView.Image = UIImage.FromFile(App.SelectedCity.ImageFilename);
			BackgroundView.Image = CityView.Image;
			CityTitle.Text = App.SelectedCity.Title;
		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}
		#endregion
	}
}


