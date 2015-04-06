using System;

using UIKit;
using Foundation;
using MapKit;
using CoreLocation;

namespace PrivacyPrompts
{
	public partial class LocationPrivacyViewController : UIViewController
	{
		public LocationPrivacyManager PrivacyManager { get; set; }

		public LocationPrivacyViewController(IntPtr handle)
			: base(handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			requestBtn.TouchUpInside += RequestAccessButtonClicked;

			titleLbl.Text = Title;
			accessStatus.Text = "Indeterminate";
			requestBtn.SetTitle ("Request access", UIControlState.Normal);

			accessStatus.Text = PrivacyManager.CheckAccess ();
			locationLbl.Text = string.Empty;
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			PrivacyManager.LocationChanged += OnLocationChanged;
		}

		public override void ViewWillDisappear (bool animated)
		{
			PrivacyManager.LocationChanged -= OnLocationChanged;
		}

		void OnLocationChanged (object sender, EventArgs e)
		{
			locationLbl.Text = PrivacyManager.LocationInfo;
			map.Region = PrivacyManager.Region;
			map.ShowsUserLocation = true;
		}

		async void RequestAccessButtonClicked (object sender, EventArgs e)
		{
			await PrivacyManager.RequestAccess ();
			accessStatus.Text = PrivacyManager.CheckAccess ();
		}
	}
}