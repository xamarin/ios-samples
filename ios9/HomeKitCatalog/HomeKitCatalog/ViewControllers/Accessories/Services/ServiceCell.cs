using System;

using Foundation;
using HomeKit;
using UIKit;
using CoreGraphics;

namespace HomeKitCatalog
{
	// A `UITableViewCell` subclass for displaying a service and the room and accessory where it resides.
	public partial class ServiceCell : UITableViewCell
	{
		public bool IncludeAccessoryText { get; set; }

		HMService service;

		public HMService Service {
			get {
				return service;
			}
			set {
				service = value;
				if (service == null)
					return;

				var accessory = service.Accessory;
				if (accessory == null)
					return;

				var accessoryName = accessory.Name;
				var roomName = accessory.Room.Name;
				TextLabel.Text = service.Name ?? accessoryName;

				var detailTextLabel = DetailTextLabel;
				if (detailTextLabel != null)
					detailTextLabel.Text = IncludeAccessoryText ? string.Format ("{0} in {1}", accessoryName, roomName) : string.Empty;
			}
		}

		public ServiceCell (IntPtr handle)
			: base (handle)
		{
			Initialize ();
		}

		[Export ("initWithCoder:")]
		public ServiceCell (NSCoder coder)
			: base (coder)
		{
			Initialize ();
		}

		[Export ("initWithFrame:")]
		public ServiceCell (CGRect frame)
			: base (frame)
		{
			Initialize ();
		}

		void Initialize ()
		{
			IncludeAccessoryText = true;
		}
	}
}