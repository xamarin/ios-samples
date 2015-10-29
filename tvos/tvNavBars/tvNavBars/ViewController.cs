using System;
using Foundation;
using UIKit;

namespace MySingleView
{
	public partial class ViewController : UIViewController
	{
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

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}
		#endregion

		#region Custom Actions
		partial void ShowFirstHotel (Foundation.NSObject sender) {
			// Change background image
			HotelImage.Image = UIImage.FromFile("Motel01.jpg");
		}

		partial void ShowSecondHotel (Foundation.NSObject sender) {
			// Change background image
			HotelImage.Image = UIImage.FromFile("Motel02.jpg");
		}

		partial void ShowThirdHotel (Foundation.NSObject sender) {
			// Change background image
			HotelImage.Image = UIImage.FromFile("Motel03.jpg");
		}
		#endregion
	}
}


