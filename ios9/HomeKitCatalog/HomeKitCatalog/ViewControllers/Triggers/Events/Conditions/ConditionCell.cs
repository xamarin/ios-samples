using System;

using Foundation;
using HomeKit;
using UIKit;
using CoreGraphics;

namespace HomeKitCatalog
{
	// A `UITableViewCell` subclass that displays a trigger condition.
	[Register ("ConditionCell")]
	public class ConditionCell : UITableViewCell
	{
		/// A static, short date formatter.
		static NSDateFormatter DateFormatter {
			get {
				var dateFormatter = new NSDateFormatter ();
				dateFormatter.DateStyle = NSDateFormatterStyle.None;
				dateFormatter.TimeStyle = NSDateFormatterStyle.Short;
				dateFormatter.Locale = NSLocale.CurrentLocale;
				return dateFormatter;
			}
		}

		[Export ("initWithCoder:")]
		public ConditionCell (NSCoder coder)
			: base (coder)
		{
			Initialize ();
		}

		[Export ("initWithFrame:")]
		public ConditionCell (CGRect frame)
			: base (frame)
		{
			Initialize ();
		}

		public ConditionCell (UITableViewCellStyle style, string reuseId)
			: base (UITableViewCellStyle.Subtitle, reuseId)
		{
			Initialize ();
		}

		public ConditionCell (UITableViewCellStyle style, NSString reuseId)
			: base (UITableViewCellStyle.Subtitle, reuseId)
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

			HMService service = characteristic.Service;
			if (service != null && service.Accessory != null)
				TrySetDetailText (string.Format ("{0} in {1}", service.Name, service.Accessory.Name));
			else
				TrySetDetailText ("Unknown Characteristic");
		}

		// Sets the cell's text to represent an ordered time with a set context string.
		public void SetOrder (TimeConditionOrder order, string timeString, string contextString)
		{
			string formatString;
			switch (order) {
			case TimeConditionOrder.Before:
				formatString = "Before {0}";
				break;
			case TimeConditionOrder.After:
				formatString = "After {0}";
				break;
			case TimeConditionOrder.At:
				formatString = "At {0}";
				break;
			default:
				throw new InvalidOperationException ();
			}
			TextLabel.Text = string.Format (formatString, timeString);
			TrySetDetailText (contextString);
		}

		// Sets the cell's text to represent an exact time condition.
		public void SetOrder (TimeConditionOrder order, NSDateComponents dateComponents)
		{
			var date = NSCalendar.CurrentCalendar.DateFromComponents (dateComponents);
			var timeString = DateFormatter.StringFor (date);
			SetOrder (order, timeString, "Relative to Time");
		}

		// Sets the cell's text to represent a solar event time condition.
		public void SetOrder (TimeConditionOrder order, TimeConditionSunState sunState)
		{
			string timeString;
			switch (sunState) {
			case TimeConditionSunState.Sunrise:
				timeString = "Sunrise";
				break;
			case TimeConditionSunState.Sunset:
				timeString = "Sunset";
				break;
			default:
				throw new InvalidOperationException ();
			}
			SetOrder (order, timeString, "Relative to sun");
		}

		// Sets the cell's text to indicate the given condition is not handled by the app.
		public void SetUnknown ()
		{
			const string unknownString = "Unknown Condition";
			TrySetDetailText (unknownString);
			TextLabel.Text = unknownString;
		}

		void TrySetDetailText (string text)
		{
			var detailTextLabel = DetailTextLabel;
			if (detailTextLabel != null)
				detailTextLabel.Text = text;
		}
	}
}