using System;

using UIKit;
using Foundation;

namespace PhotoHandoff
{
	[Register("FilterViewController")]
	public class FilterViewController : UIViewController
	{
		const string ImageFilterKey = "kImageFilterKey";

		[Outlet("slider")]
		UISlider Slider { get; set; }

		[Outlet("activeSwitch")]
		UISwitch ActiveSwitch { get; set; }

		[Outlet("navigationBar")]
		UINavigationBar NavigationBar { get; set; }

		[Outlet("activityIndicator")]
		public UIActivityIndicatorView ActivityIndicator { get; set; }

		public ImageFilter Filter { get; set; }

		BlurFilter BlurFilter {
			get {
				return Filter as BlurFilter;
			}
		}

		ModifyFilter ModifyFilter {
			get {
				return Filter as ModifyFilter;
			}
		} 

		public DetailViewController MasterViewController { get; set; }

		public FilterViewController(IntPtr handle)
			: base(handle)
		{

		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// decide if we want the "Done" button (for iPhone), iPad doesn't need one
			// for iPhone the presentingViewController is nul, for iPad it's a UINavigationController
			if (PresentingViewController != null)
				NavigationBar.TopItem.RightBarButtonItem = null;
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			Update ();
		}

		[Export("dismiss:")]
		void Dismiss(NSObject sender)
		{
			// inform our MasterViewController we are going away
			Action completionHandler = MasterViewController != null ? MasterViewController.WasDismissed : (Action)null;
			PresentingViewController.DismissViewController (true, completionHandler);
		}

		#region Filtering

		// blue slider value has changed
		[Export("setBlurValue:")]
		void SetBlurValue(NSObject sender)
		{
			if (BlurFilter == null || Slider == null)
				return;

			BlurFilter.BlurRadius = Slider.Value;
			BlurFilter.Dirty = true;
		}

		// sepia intensity slider value has changed
		[Export("setIntensity:")]
		void SetIntensity(NSObject sender)
		{
			if (ModifyFilter == null && Slider == null)
				return;

			ModifyFilter.Intensity = Slider.Value;
			ModifyFilter.Dirty = true;
		}

		// active or on/off switch has changed
		[Export("setActiveValue:")]
		void SetActiveValue(NSObject sender)
		{
			Filter.Active = ActiveSwitch.On;
			Filter.Dirty = true;

			if (Slider != null)
				Slider.Enabled = Filter.Active;
		}

		void Update ()
		{
			if (Filter == null)
				return;

			ActiveSwitch.On = Filter.Active;

			if (Slider == null)
				return;

			Slider.Enabled = Filter.Active;
			if (BlurFilter != null)
				Slider.Value = BlurFilter.BlurRadius;
			else if (ModifyFilter != null)
				Slider.Value = ModifyFilter.Intensity;
		}

		#endregion

		#region UIStateRestoration

		public override void EncodeRestorableState (NSCoder coder)
		{
			base.EncodeRestorableState (coder);
			coder.Encode (Filter, ImageFilterKey);
		}

		public override void DecodeRestorableState (NSCoder coder)
		{
			base.DecodeRestorableState (coder);
			if (coder.ContainsKey (ImageFilterKey))
				Filter = (ImageFilter)coder.DecodeObject (ImageFilterKey);
		}

		public override void ApplicationFinishedRestoringState ()
		{
			base.ApplicationFinishedRestoringState ();
			Update ();
		}

		#endregion
	}
}