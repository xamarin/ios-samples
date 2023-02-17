using FontList.Models;
using System;
using UIKit;

namespace FontList {
	public partial class FontDetailsViewController : UIViewController {
		public FontDetailsViewController (IntPtr handle) : base (handle) { }

		public FontItem FontItem { get; internal set; }

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			Title = FontItem.Name;
			textView.Font = FontItem.Font;
		}
	}
}
