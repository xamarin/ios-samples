using Foundation;
using System;
using UIKit;

namespace UICatalog {
	public partial class PageControlViewController : UIViewController {
		// Colors that correspond to the selected page. Used as the background color for "colorView".
		private readonly UIColor [] colors =
		{
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

		public PageControlViewController (IntPtr handle) : base (handle) { }

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// The total number of pages that are available is based on how many available colors we have.
			pageControl.Pages = colors.Length;
			pageControl.CurrentPage = 2;
		}

		partial void PageControlValueChanged (NSObject sender)
		{
			Console.WriteLine ($"The page control changed its current page to {pageControl.CurrentPage}.");
			colorView.BackgroundColor = colors [pageControl.CurrentPage];
		}
	}
}
