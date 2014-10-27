using System;

using Foundation;
using UIKit;

namespace TableSearch
{
	public partial class DetailViewController : UIViewController
	{
		public Product Product { get; set; }

		public DetailViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			Title = Product.Title;
			Year.Text = Product.YearIntroduced.ToString ();
			Price.Text = string.Format ("{0:C}", Product.IntroPrice);
		}
	}
}
