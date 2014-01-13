using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Example_CoreAnimation.Screens.iPad.ViewTransitions
{
	public partial class TransitionViewController : UIViewController
	{
		public Action ContentsClicked;
		public event EventHandler<EventArgs> TransitionClicked;

		public UIViewAnimationOptions SelectedTransition {
			get {
				switch (sgmntTransitionType.SelectedSegment) {
				case 0:
					return UIViewAnimationOptions.TransitionCurlDown;
					break;
				case 1:
					return UIViewAnimationOptions.TransitionCurlUp;
					break;
				case 2:
					return UIViewAnimationOptions.TransitionFlipFromLeft;
					break;
				case 3:
					return UIViewAnimationOptions.TransitionFlipFromRight;
					break;
				default:
					return UIViewAnimationOptions.TransitionCurlDown;
					break;
				}
			}
		}

		#region Constructors

		// The IntPtr and initWithCoder constructors are required for controllers that need
		// to be able to be created from a xib rather than from managed code
		public TransitionViewController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		[Export ("initWithCoder:")]
		public TransitionViewController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}

		public TransitionViewController () : base ("TransitionViewController", null)
		{
			Initialize ();
		}

		void Initialize ()
		{
		}

		#endregion

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			SetToolbarVisibility (InterfaceOrientation);

			btnContents.TouchUpInside += (sender, e) => {
				if (ContentsClicked != null)
					ContentsClicked ();
			};

			btnTransition.TouchUpInside += (s, e) => {
				if (TransitionClicked != null)
					TransitionClicked (this, e);
			};
		}

		public override void WillRotate (UIInterfaceOrientation toInterfaceOrientation, double duration)
		{
			SetToolbarVisibility (toInterfaceOrientation);
			base.WillRotate (toInterfaceOrientation, duration);
		}

		private void SetToolbarVisibility (UIInterfaceOrientation interfaceOrientation)
		{
			toolbar.Hidden = interfaceOrientation == UIInterfaceOrientation.LandscapeLeft ||
			interfaceOrientation == UIInterfaceOrientation.LandscapeRight;
		}
	}
}

