using System;
using Foundation;
using UIKit;
using System.CodeDom.Compiler;

namespace Touch
{
	partial class CustomGestureViewController : UIViewController
	{
		#region Private Variables
		private bool isChecked = false;
		private CheckmarkGestureRecognizer checkmarkGesture;
		#endregion

		#region Constructors
		public CustomGestureViewController (IntPtr handle) : base (handle)
		{
		}
		#endregion

		#region Override Methods
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			// Wire up the gesture recognizer
			WireUpCheckmarkGestureRecognizer();
		}
		#endregion

		#region Private Methods
		private void WireUpCheckmarkGestureRecognizer()
		{
			// Create the recognizer
			checkmarkGesture = new CheckmarkGestureRecognizer();

			// Wire up the event handler
			checkmarkGesture.AddTarget(() =>{
				if (checkmarkGesture.State == (UIGestureRecognizerState.Recognized | UIGestureRecognizerState.Ended))
				{
					if (isChecked)
					{
						CheckboxImage.Image = UIImage.FromBundle("CheckBox_Unchecked.png");
					}
					else
					{
						CheckboxImage.Image = UIImage.FromBundle("CheckBox_Checked.png");
					}
					isChecked = !isChecked;
				}
			});

			// Add the gesture recognizer to the view
			View.AddGestureRecognizer(checkmarkGesture);
		}
		#endregion
	}
}
