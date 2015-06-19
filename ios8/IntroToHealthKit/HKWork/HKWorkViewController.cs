using System;
using CoreGraphics;

using Foundation;
using UIKit;
using HealthKit;

namespace HKWork
{
	public partial class HKWorkViewController : UIViewController
	{
		public HKWorkViewController (IntPtr handle) : base (handle)
		{
			HeartRateModel.Instance.EnabledChanged += OnEnabledChanged;
			HeartRateModel.Instance.ErrorMessageChanged += OnErrorMessageChanged;
			HeartRateModel.Instance.HeartRateStored += OnHeartBeatStored;
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}


		#region View lifecycle

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			StoreData.Enabled = false;
			HeartRateModel.Instance.Enabled = false;

		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			PermissionsLabel.SizeToFit ();
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


		partial void StoreData_TouchUpInside (UIButton sender)
		{
			var s = heartRate.Text;
			ushort result = 0;
			if (UInt16.TryParse (s, out result)) {
				var quantity = HeartRateModel.Instance.HeartRateInBeatsPerMinute(result);
				HeartRateModel.Instance.StoreHeartRate(quantity);
			}
			heartRate.ResignFirstResponder();
		}

		#endregion

		void OnEnabledChanged (object sender, GenericEventArgs<bool> args)
		{
			StoreData.Enabled = args.Value;
			PermissionsLabel.Text = args.Value ? "Ready to record" : "Not authorized to store data.";
			PermissionsLabel.SizeToFit ();
		}

		void OnErrorMessageChanged (object sender, GenericEventArgs<string> args)
		{
			PermissionsLabel.Text = args.Value;
		}

		void OnHeartBeatStored (object sender, GenericEventArgs<double> args)
		{
			PermissionsLabel.Text = String.Format ("Stored {0} BPM", args.Value);
		}
	}
}

