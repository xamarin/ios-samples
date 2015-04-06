using System;
using CoreLocation;
using UIKit;
using CoreGraphics;
using MapKit;
using Foundation;

namespace PrivacyPrompts
{
	public partial class LocationPrivacyViewController : UIViewController, IPrivacyViewController
	{
		public UILabel TitleLabel {
			get {
				return titleLbl;
			}
		}

		public UILabel AccessStatus {
			get {
				return accessStatus;
			}
		}

		public UIButton RequestAccessButton {
			get {
				return requestBtn;
			}
		}

		public UILabel LocationLbl {
			get {
				return locationLbl;
			}
		}

		public MKMapView Map {
			get {
				return map;
			}
		}

		public IPrivacyManager PrivacyManager { get; set; }

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
			PrivacyManager.Dispose ();
		}
	}
}