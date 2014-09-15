using System;

using Foundation;
using UIKit;

namespace UICatalog
{
	[Register ("ActivityIndicatorViewController")]
	public class ActivityIndicatorViewController : UITableViewController
	{
		[Outlet]
		private UIActivityIndicatorView grayStyleActivityIndicatorView { get ; set; }

		[Outlet]
		private UIActivityIndicatorView tintedActivityIndicatorView { get; set; }

		public ActivityIndicatorViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			ConfigureGrayActivityIndicatorView ();
			ConfigureTintedActivityIndicatorView ();
		}

		private void ConfigureGrayActivityIndicatorView()
		{
			grayStyleActivityIndicatorView.ActivityIndicatorViewStyle = UIActivityIndicatorViewStyle.Gray;
			grayStyleActivityIndicatorView.StartAnimating ();
			grayStyleActivityIndicatorView.HidesWhenStopped = true;
		}

		private void ConfigureTintedActivityIndicatorView()
		{
			tintedActivityIndicatorView.ActivityIndicatorViewStyle = UIActivityIndicatorViewStyle.Gray;
			tintedActivityIndicatorView.Color = ApplicationColors.Purple;
			tintedActivityIndicatorView.StartAnimating ();
		}
	}
}
