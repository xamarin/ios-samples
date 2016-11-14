using System;

using Foundation;
using UIKit;

namespace Chat
{
	[Register ("IncomingCell")]
	public class IncomingCell : BubbleCell
	{
		static readonly UIImage normalBubbleImage;
		static readonly UIImage highlightedBubbleImage;

		public static readonly NSString CellId = new NSString ("Incoming");

		static IncomingCell ()
		{
			UIImage mask = UIImage.FromBundle ("BubbleIncoming");

			var cap = new UIEdgeInsets {
				Top = 17f,
				Left = 26f,
				Bottom = 17f,
				Right = 21f,
			};

			var normalColor = UIColor.FromRGB (229, 229, 234);
			var highlightedColor = UIColor.FromRGB (206, 206, 210);

			normalBubbleImage = CreateColoredImage (normalColor, mask).CreateResizableImage (cap);
			highlightedBubbleImage = CreateColoredImage (highlightedColor, mask).CreateResizableImage (cap);
		}

		public IncomingCell (IntPtr handle)
			: base (handle)
		{
			Initialize ();
		}

		public IncomingCell ()
		{
			Initialize ();
		}

		[Export ("initWithStyle:reuseIdentifier:")]
		public IncomingCell (UITableViewCellStyle style, string reuseIdentifier)
			: base (style, reuseIdentifier)
		{
			Initialize ();
		}

		void Initialize ()
		{
			BubbleHighlightedImage = highlightedBubbleImage;
			BubbleImage = normalBubbleImage;

			ContentView.AddConstraints (NSLayoutConstraint.FromVisualFormat ("H:|[bubble]",
				0, 
				"bubble", BubbleImageView));
			ContentView.AddConstraints (NSLayoutConstraint.FromVisualFormat ("V:|-2-[bubble]-2-|",
				0,
				"bubble", BubbleImageView
			));
			BubbleImageView.AddConstraints (NSLayoutConstraint.FromVisualFormat ("H:[bubble(>=48)]",
				0,
				"bubble", BubbleImageView
			));

			var vSpaceTop = NSLayoutConstraint.Create (MessageLabel, NSLayoutAttribute.Top, NSLayoutRelation.Equal, BubbleImageView, NSLayoutAttribute.Top, 1, 10);
			ContentView.AddConstraint (vSpaceTop);

			var vSpaceBottom = NSLayoutConstraint.Create (MessageLabel, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, BubbleImageView, NSLayoutAttribute.Bottom, 1, -10);
			ContentView.AddConstraint (vSpaceBottom);

			var msgLeading = NSLayoutConstraint.Create (MessageLabel, NSLayoutAttribute.Leading, NSLayoutRelation.GreaterThanOrEqual, BubbleImageView, NSLayoutAttribute.Leading, 1, 16);
			ContentView.AddConstraint (msgLeading);

			var msgCenter = NSLayoutConstraint.Create (MessageLabel, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, BubbleImageView, NSLayoutAttribute.CenterX, 1, 3);
			ContentView.AddConstraint (msgCenter);
		}
	}
}
