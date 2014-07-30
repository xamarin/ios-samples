using System;
using CoreGraphics;

using Foundation;
using UIKit;

namespace TextKitDemo
{
	public partial class CollectionViewCell : UICollectionViewCell
	{
		public CollectionViewCell (IntPtr handle) : base (handle)
		{
			BackgroundColor = UIColor.DarkGray;
			Layer.CornerRadius = 5;

			UIApplication.Notifications.ObserveContentSizeCategoryChanged (delegate {
				CalculateAndSetFonts (); 
			});
		}

		public void FormatCell (DemoModel demo)
		{
			containerView.Layer.CornerRadius = 2;
			labelView.Text = demo.Title;
			textView.AttributedText = demo.GetAttributedText ();

			CalculateAndSetFonts ();
		}

		void CalculateAndSetFonts ()
		{
			const float cellTitleTextScaleFactor = 0.85f;
			const float cellBodyTextScaleFactor = 0.7f;

			UIFont cellTitleFont = Font.GetPreferredFont (labelView.Font.FontDescriptor.TextStyle, cellTitleTextScaleFactor);
			UIFont cellBodyFont = Font.GetPreferredFont (textView.Font.FontDescriptor.TextStyle, cellBodyTextScaleFactor);

			labelView.Font = cellTitleFont;
			textView.Font = cellBodyFont;
		}
	}
}

