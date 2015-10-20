using System;

using Foundation;
using UIKit;

namespace UICatalog
{
	[Register ("ActivityIndicatorViewController")]
	public class ActivityIndicatorViewController : UITableViewController
	{
		[Outlet]
		UIActivityIndicatorView grayStyleActivityIndicatorView { get ; set; }

		[Outlet]
		UIActivityIndicatorView tintedActivityIndicatorView { get; set; }

		public ActivityIndicatorViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			ConfigureGrayActivityIndicatorView ();
			ConfigureTintedActivityIndicatorView ();
		}

		void ConfigureGrayActivityIndicatorView ()
		{
			grayStyleActivityIndicatorView.ActivityIndicatorViewStyle = UIActivityIndicatorViewStyle.Gray;
			grayStyleActivityIndicatorView.StartAnimating ();
			grayStyleActivityIndicatorView.HidesWhenStopped = true;
		}

		void ConfigureTintedActivityIndicatorView ()
		{
			tintedActivityIndicatorView.ActivityIndicatorViewStyle = UIActivityIndicatorViewStyle.Gray;
			tintedActivityIndicatorView.Color = ApplicationColors.Purple;
			tintedActivityIndicatorView.StartAnimating ();
		}
	}
}
