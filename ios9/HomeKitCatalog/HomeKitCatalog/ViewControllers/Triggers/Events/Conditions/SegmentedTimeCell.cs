using System;

using CoreGraphics;
using Foundation;
using UIKit;

namespace HomeKitCatalog
{
	// A `UITableViewCell` subclass with a `UISegmentedControl`, used for selecting the time type.
	public partial class SegmentedTimeCell : UITableViewCell
	{
		[Outlet ("segmentedControl")]
		public UISegmentedControl SegmentedControl { get; set; }

		#region ctors

		public SegmentedTimeCell (IntPtr handle)
			: base (handle)
		{
		}

		[Export ("initWithFrame:")]
		public SegmentedTimeCell (CGRect frame)
			: base (frame)
		{
		}

		#endregion
	}
}
