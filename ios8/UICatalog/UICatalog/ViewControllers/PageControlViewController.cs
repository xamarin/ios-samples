using System;

using Foundation;
using UIKit;

namespace UICatalog
{
	[Register ("PageControlViewController")]
	public class PageControlViewController : UIViewController
	{
		// Colors that correspond to the selected page. Used as the background color for "colorView".
		readonly UIColor[] colors = {
			UIColor.Black,
			UIColor.Gray,
			UIColor.Red,
			UIColor.Green,
			UIColor.Blue,
			UIColor.Cyan,
			UIColor.Yellow,
			UIColor.Magenta,
			UIColor.Orange,
			UIColor.Purple
		};

		[Outlet]
		UIPageControl PageControl { get; set; }

		[Outlet]
		UIView ColorView { get; set; }

		public PageControlViewController (IntPtr handle)
			: base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			configurePageControl ();
			pageControlValueDidChange (PageControl, EventArgs.Empty);
		}

		void configurePageControl ()
		{
			// The total number of pages that are available is based on how many available colors we have.
			PageControl.Pages = colors.Length;
			PageControl.CurrentPage = 2;

			PageControl.TintColor = ApplicationColors.Blue;
			PageControl.PageIndicatorTintColor = ApplicationColors.Green;
			PageControl.CurrentPageIndicatorTintColor = ApplicationColors.Purple;

			PageControl.ValueChanged += pageControlValueDidChange;
		}

		void pageControlValueDidChange (object sender, EventArgs e)
		{
			Console.WriteLine ("The page control changed its current page to {0}.", PageControl.CurrentPage);
			ColorView.BackgroundColor = colors [PageControl.CurrentPage];
		}
	}
}
