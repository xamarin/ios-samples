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
		partial void PlayerCountChanged (Foundation.NSObject sender) {

			// Take Action based on the segment
			switch(PlayerCount.SelectedSegment) {
			case 0:
				Player1.Hidden = false;
				Player2.Hidden = true;
				Player3.Hidden = true;
				Player4.Hidden = true;
				break;
			case 1:
				Player1.Hidden = false;
				Player2.Hidden = false;
				Player3.Hidden = true;
				Player4.Hidden = true;
				break;
			case 2:
				Player1.Hidden = false;
				Player2.Hidden = false;
				Player3.Hidden = false;
				Player4.Hidden = true;
				break;
			case 3:
				Player1.Hidden = false;
				Player2.Hidden = false;
				Player3.Hidden = false;
				Player4.Hidden = false;
				break;
			}
		}
		#endregion
	}
}


