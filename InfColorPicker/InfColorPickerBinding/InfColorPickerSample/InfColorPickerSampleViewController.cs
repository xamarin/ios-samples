using System;
using System.Drawing;

using Foundation;
using UIKit;
using InfColorPicker;

namespace InfColorPickerSample
{
	public partial class InfColorPickerSampleViewController : UIViewController
	{
		#region Private Variables
		private ColorSelectedDelegate _selector;
		#endregion

		#region Constructors
		public InfColorPickerSampleViewController (IntPtr handle) : base (handle)
		{
		}
		#endregion

		#region Private Methods
		private void HandleTouchUpInsideWithStrongDelegate (object sender, EventArgs e)
		{
			InfColorPickerController picker = InfColorPickerController.ColorPickerViewController();
			picker.Delegate = _selector;
			picker.PresentModallyOverViewController (this);
		}

		private void HandleTouchUpInsideWithWeakDelegate (object sender, EventArgs e)
		{
			InfColorPickerController picker = InfColorPickerController.ColorPickerViewController();
			picker.WeakDelegate = this;
			picker.SourceColor = this.View.BackgroundColor;
			picker.PresentModallyOverViewController (this);
		}
		#endregion

		#region Public Methods
		[Export("colorPickerControllerDidFinish:")]
		public void ColorPickerControllerDidFinish (InfColorPickerController controller)
		{
			View.BackgroundColor = controller.ResultColor;
			DismissViewController (false, null);
		}
		#endregion

		#region Override Methods
		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();

			// Release any cached data, images, etc that aren't in use.
		}


		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Strong Delegate
			// ChangeColorButton.TouchUpInside += HandleTouchUpInsideWithStrongDelegate;
			// _selector = new ColorSelectedDelegate (this);

			// Weak delegate
			ChangeColorButton.TouchUpInside += HandleTouchUpInsideWithWeakDelegate;
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
		}

		public override void ViewDidDisappear (bool animated)
		{
			base.ViewDidDisappear (animated);
		}

		#endregion
	}
}

