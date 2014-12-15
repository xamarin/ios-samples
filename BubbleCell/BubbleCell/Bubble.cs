//
// Bubble.cs: Provides both a UITableViewCell that can be used with UITableViews
// as well as a ChatBubble which is a MonoTouch.Dialog Element that can be used
// inside a DialogViewController for quick UIs.
//
// Author:
//   Miguel de Icaza
//
using System;
using System.Collections.Generic;
using UIKit;
using MonoTouch.Dialog;
using CoreGraphics;
using Foundation;

namespace BubbleCell
{
	public class BubbleCell : UITableViewCell {
		public static NSString KeyLeft = new NSString ("BubbleElementLeft");
		public static NSString KeyRight = new NSString ("BubbleElementRight");
		public static UIImage bleft, bright, left, right;
		public static UIFont font = UIFont.SystemFontOfSize (14);
		UIView view;
		UIView imageView;
		UILabel label;
		bool isLeft;

		static BubbleCell ()
		{
			bright = UIImage.FromFile ("green.png");
			bleft = UIImage.FromFile ("grey.png");

			// buggy, see https://bugzilla.xamarin.com/show_bug.cgi?id=6177
			//left = bleft.CreateResizableImage (new UIEdgeInsets (10, 16, 18, 26));
			//right = bright.CreateResizableImage (new UIEdgeInsets (11, 11, 17, 18));
			left = bleft.StretchableImage (26, 16);
			right = bright.StretchableImage (11, 11);
		}

		public BubbleCell (bool isLeft) : base (UITableViewCellStyle.Default, isLeft ? KeyLeft : KeyRight)
		{
			var rect = new CGRect (0, 0, 1, 1);
			this.isLeft = isLeft;
			view = new UIView (rect);
			imageView = new UIImageView (isLeft ? left : right);
			view.AddSubview (imageView);
			label = new UILabel (rect) {
				LineBreakMode = UILineBreakMode.WordWrap,
				Lines = 0,
				Font = font,
				BackgroundColor = UIColor.Clear
			};
			view.AddSubview (label);
			ContentView.Add (view);
		}

		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();
			var frame = ContentView.Frame;
			var size = GetSizeForText (this, label.Text) + BubblePadding;
			imageView.Frame = new CGRect (new CGPoint (isLeft ? 10 : frame.Width-size.Width-10, frame.Y), size);
			view.SetNeedsDisplay ();
			frame = imageView.Frame;
			label.Frame = new CGRect (new CGPoint (frame.X + (isLeft ? 12 : 8), frame.Y + 6), size-BubblePadding);
		}

		static internal CGSize BubblePadding = new CGSize (22, 16);

		static internal CGSize GetSizeForText (UIView tv, string text)
		{
			return UIStringDrawing.StringSize (text, font, new CGSize (tv.Bounds.Width*.7f-10-22, 99999));
		}

		public void Update (string text)
		{
			label.Text = text;
			SetNeedsLayout ();
		}
	}

	public class ChatBubble : Element, IElementSizing {
		bool isLeft;

		public ChatBubble (bool isLeft, string text) : base (text)
		{
			this.isLeft = isLeft;
		}

		public override UITableViewCell GetCell (UITableView tv)
		{
			var cell = tv.DequeueReusableCell (isLeft ? BubbleCell.KeyLeft : BubbleCell.KeyRight) as BubbleCell;
			if (cell == null)
				cell = new BubbleCell (isLeft);
			cell.Update (Caption);
			return cell;
		}

		public nfloat GetHeight (UITableView tableView, NSIndexPath indexPath)
		{
			return BubbleCell.GetSizeForText (tableView, Caption).Height + BubbleCell.BubblePadding.Height;
		}
	}
}
