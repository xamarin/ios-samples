using System;
using System.Drawing;

using UIKit;
using Foundation;

using Common;
using CoreGraphics;

namespace Lister
{
	[Register("ListColorCell")]
	public class ListColorCell : UITableViewCell
	{
		[Outlet("gray")]
		UIView Gray { get; set; }

		[Outlet("blue")]
		UIView Blue { get; set; }

		[Outlet("green")]
		UIView Green { get; set; }

		[Outlet("yellow")]
		UIView Yellow { get; set; }

		[Outlet("orange")]
		UIView Orange { get; set; }

		[Outlet("red")]
		UIView Red { get; set; }

		public ListViewController ViewController { get; set; }

		public ListColor SelectedColor { get; private set; }

		public ListColorCell (IntPtr handle)
			: base(handle)
		{
		}

		public void Configure()
		{
			UITapGestureRecognizer colorTapGestureRecognizer = new UITapGestureRecognizer (ColorTap);
			colorTapGestureRecognizer.NumberOfTapsRequired = 1;
			colorTapGestureRecognizer.NumberOfTouchesRequired = 1;

			AddGestureRecognizer (colorTapGestureRecognizer);
		}

		void ColorTap(UITapGestureRecognizer tapGestureRecognizer)
		{
			if (tapGestureRecognizer.State != UIGestureRecognizerState.Ended)
				return;

			CGPoint tapLocation = tapGestureRecognizer.LocationInView (ContentView);
			UIView view = ContentView.HitTest (tapLocation, null);

			// If the user tapped on a color (identified by its tag), notify the delegate.
			ListColor color = (ListColor)(int)view.Tag;
			SelectedColor = color;
			ViewController.OnListColorCellDidChangeSelectedColor (SelectedColor);
		}
	}
}

