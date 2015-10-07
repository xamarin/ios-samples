using System;

using UIKit;
using Foundation;
using CoreGraphics;
using HomeKit;

namespace HomeKitCatalog
{
	// A `UITableViewCell` subclass that displays a characteristic's 'target value'.
	public partial class ActionCell : UITableViewCell
	{
		public ActionCell (UITableViewCellStyle style, string reuseId)
			: base (UITableViewCellStyle.Subtitle, reuseId)
		{
			Initialize ();
		}

		public ActionCell (UITableViewCellStyle style, NSString reuseId)
			: base (UITableViewCellStyle.Subtitle, reuseId)
		{
			Initialize ();
		}


		public ActionCell (IntPtr handle)
			: base (handle)
		{
			Initialize ();
		}

		[Export ("initWithCoder:")]
		public ActionCell (NSCoder coder)
			: base (coder)
		{
			Initialize ();
		}

		[Export ("initWithFrame:")]
		public ActionCell (CGRect frame)
			: base (frame)
		{
			Initialize ();
		}

		void Initialize ()
		{
			SelectionStyle = UITableViewCellSelectionStyle.None;
			Accessory = UITableViewCellAccessory.None;
			var detailTextLabel = DetailTextLabel;
			if (detailTextLabel != null)
				detailTextLabel.TextColor = UIColor.LightGray;
		}

		// Sets the cell's text to represent a characteristic and target value.
		// For example, "Brightness → 60%"
		// Sets the subtitle to the service and accessory that this characteristic represents.
		public void SetCharacteristic (HMCharacteristic characteristic, NSNumber targetValue)
		{
			var targetDescription = string.Format ("{0} → {1}", characteristic.LocalizedDescription, characteristic.DescriptionForValue (targetValue));
			TextLabel.Text = targetDescription;

			var detailTextLabel = DetailTextLabel;
			if (detailTextLabel != null) {
				HMService service = characteristic.Service;
				if (service != null && service.Accessory != null)
					detailTextLabel.Text = string.Format ("{0} in {1}", service.Name, service.Accessory.Name);
				else
					detailTextLabel.Text = "Unknown Characteristic";
			}
		}
	}
}
