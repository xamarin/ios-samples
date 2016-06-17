using System;
using UIKit;

namespace tvRemote
{
	public partial class FirstViewController : UIViewController
	{
		#region Constructors
		public FirstViewController (IntPtr handle) : base (handle)
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
		partial void TouchSurfaceClicked (Foundation.NSObject sender) {
			// Highlight tapped button
			RemoteView.ButtonPressed = "Touch";
			ButtonLabel.Text = "Touch Surface Clicked";
		}

		partial void MenuPressed (Foundation.NSObject sender) {
			// Highlight tapped button
			RemoteView.ButtonPressed = "Menu";
			ButtonLabel.Text = "Menu Button Pressed";
		}

		partial void PlayPausePressed (Foundation.NSObject sender) {
			// Highlight tapped button
			RemoteView.ButtonPressed = "PlayPause";
			ButtonLabel.Text = "Play/Pause Button Pressed";
		}

		partial void UpPressed (Foundation.NSObject sender) {
			// Highlight tapped button
			RemoteView.ArrowPressed = "Up";
			ButtonLabel.Text = "D-Pad Up Pressed";
		}

		partial void DownPressed (Foundation.NSObject sender) {
			// Highlight tapped button
			RemoteView.ArrowPressed = "Down";
			ButtonLabel.Text = "D-Pad Down Pressed";
		}

		partial void LeftPressed (Foundation.NSObject sender) {
			// Highlight tapped button
			RemoteView.ArrowPressed = "Left";
			ButtonLabel.Text = "D-Pad Left Pressed";
		}

		partial void RightPressed (Foundation.NSObject sender) {
			// Highlight tapped button
			RemoteView.ArrowPressed = "Right";
			ButtonLabel.Text = "D-Pad Right Pressed";
		}
		#endregion
	}
}

