using System;

using UIKit;

namespace CloudKitAtlas
{
	public class ResultOrErrorViewController : UIViewController
	{
		protected UIBarButtonItem DoneButton { get; set; }

		protected bool IsDrilldown { get; set; } = false;

		public ResultOrErrorViewController (IntPtr handle)
			: base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			NavigationItem.Title = "Error";
			DoneButton = new UIBarButtonItem ("Done", UIBarButtonItemStyle.Done, BackToCodeSample);

			if (!IsDrilldown) {
				NavigationItem.HidesBackButton = true;
				NavigationItem.RightBarButtonItem = DoneButton;
			}
		}

		void BackToCodeSample (object sender, EventArgs e)
		{
			DismissViewController (true, null);
		}
	}
}
