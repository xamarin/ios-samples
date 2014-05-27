using System;

using Foundation;
using UIKit;

namespace FrogScroller
{
	public partial class ImageViewController : UIViewController
	{
		public ImageViewController (int pageIndex)
		{
			PageIndex = pageIndex;
		}

		public int PageIndex { get; private set; }

		public static ImageViewController ImageViewControllerForPageIndex (int pageIndex)
		{
			return pageIndex >= 0 && pageIndex < ImageScrollView.ImageCount ? 
				new ImageViewController (pageIndex) : null;
		}

		public override void LoadView ()
		{
			ImageScrollView scrollView = new ImageScrollView () {
				Index = PageIndex,
				AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight,
			};
			
			this.View = scrollView;
		}
	}
}