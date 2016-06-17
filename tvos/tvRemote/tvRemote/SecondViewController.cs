using System;
using UIKit;

namespace tvRemote
{
	public partial class SecondViewController : UIViewController
	{
		#region Constructors
		public SecondViewController (IntPtr handle) : base (handle)
		{
		}
		#endregion

		#region Override Methods
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();	

			// Wire-up gestures
			var upGesture = new UISwipeGestureRecognizer (() => {
				RemoteView.ArrowPressed = "Up";
				ButtonLabel.Text = "Swiped Up";
			}) {
				Direction = UISwipeGestureRecognizerDirection.Up
			};
			this.View.AddGestureRecognizer (upGesture);

			var downGesture = new UISwipeGestureRecognizer (() => {
				RemoteView.ArrowPressed = "Down";
				ButtonLabel.Text = "Swiped Down";
			}) {
				Direction = UISwipeGestureRecognizerDirection.Down
			};
			this.View.AddGestureRecognizer (downGesture);

			var leftGesture = new UISwipeGestureRecognizer (() => {
				RemoteView.ArrowPressed = "Left";
				ButtonLabel.Text = "Swiped Left";
			}) {
				Direction = UISwipeGestureRecognizerDirection.Left
			};
			this.View.AddGestureRecognizer (leftGesture);

			var rightGesture = new UISwipeGestureRecognizer (() => {
				RemoteView.ArrowPressed = "Right";
				ButtonLabel.Text = "Swiped Right";
			}) {
				Direction = UISwipeGestureRecognizerDirection.Right
			};
			this.View.AddGestureRecognizer (rightGesture);
		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}
		#endregion
	}
}

