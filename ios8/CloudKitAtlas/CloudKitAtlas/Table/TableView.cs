using System;

using UIKit;
using Foundation;

namespace CloudKitAtlas {
	public partial class TableView : UITableView {
		public TableView (IntPtr handle)
			: base (handle)
		{
		}

		[Export ("initWithCoder:")]
		public TableView (NSCoder coder)
			: base (coder)
		{
			if (Style == UITableViewStyle.Grouped) {
				BackgroundView = null;
				BackgroundColor = new UIColor (0.95f, 0.95f, 0.95f, 1);
			} else if (Style == UITableViewStyle.Plain) {
				TableFooterView = new UIView ();
			}
		}
	}
}
