using System;

using CoreGraphics;
using Foundation;
using UIKit;

namespace HomeKitCatalog
{
	// A `UITableViewCell` subclass with a `UIDatePicker`, used for selecting a specific time of day.
	public partial class TimePickerCell : UITableViewCell
	{
		[Outlet ("datePicker")]
		public UIDatePicker DatePicker { get; set; }

		#region ctors

		public TimePickerCell (IntPtr handle)
			: base (handle)
		{
		}

		[Export ("initWithFrame:")]
		public TimePickerCell (CGRect frame)
			: base (frame)
		{
		}

		#endregion
	}
}
