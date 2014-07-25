
using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;

namespace Example_SharedResources.Screens.iPhone.Accelerometer
{
	public partial class ShakeScreen : UIViewController
	{
		#region Constructors

		// The IntPtr and initWithCoder constructors are required for items that need 
		// to be able to be created from a xib rather than from managed code

		public ShakeScreen (IntPtr handle) : base(handle)
		{
			Initialize ();
		}

		[Export("initWithCoder:")]
		public ShakeScreen (NSCoder coder) : base(coder)
		{
			Initialize ();
		}

		public ShakeScreen () : base("ShakeScreen", null)
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
			
			Title = "Shake";
		}
		
		// Called by the iOS to determine if the controller can receive touches and action
		// messages (such as motion events)
		public override bool CanBecomeFirstResponder
		{
			get { return true; }
		}
		
		public override void ViewDidAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			// tell the iOS that we want motion events
			BecomeFirstResponder();
		}
		
		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
			// be a good citizen and allow other controllers to become first responder
			ResignFirstResponder();
		}
		
		// Called after the iOS determines the motion wasn't noise (such as walking up stairs).
		public override void MotionEnded (UIEventSubtype motion, UIEvent evt)
		{
			Console.WriteLine("Motion Ended");
			base.MotionEnded(motion, evt);
			
			// if the motion was a shake
			if(motion == UIEventSubtype.MotionShake) {
				Console.Write("Shake Detected");
				
				lblShakeStatus.Text = "Shook!";
			}
		}
	}
}

