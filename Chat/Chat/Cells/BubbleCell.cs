using System;

using UIKit;
using CoreGraphics;

namespace Chat
{
	public abstract class BubbleCell : UITableViewCell
	{
		protected abstract UIImageView BubbleView { get; }
		public abstract UILabel MessageLbl { get; }

		protected abstract UIImage BubbleImg { get; }
		protected abstract UIImage BubbleHighlightedImage { get; }

		Message msg;
		public Message Message {
			get {
				return msg;
			}
			set {
				msg = value;
				BubbleView.Image = BubbleImg;
				BubbleView.HighlightedImage = BubbleHighlightedImage;

				MessageLbl.Text = msg.Text;

				MessageLbl.UserInteractionEnabled = true;
				BubbleView.UserInteractionEnabled = false;
			}
		}

		public BubbleCell (IntPtr handle)
			: base(handle)
		{
		}

		public override void SetSelected (bool selected, bool animated)
		{
			base.SetSelected (selected, animated);
			BubbleView.Highlighted = selected;
		}

		protected static UIImage CreateColoredImage(UIColor color, UIImage mask)
		{
			var rect = new CGRect (CGPoint.Empty, mask.Size);
			UIGraphics.BeginImageContextWithOptions (mask.Size, false, mask.CurrentScale);
			CGContext context = UIGraphics.GetCurrentContext ();
			mask.DrawAsPatternInRect (rect);
			context.SetFillColor (color.CGColor);
			context.SetBlendMode (CGBlendMode.SourceAtop);
			context.FillRect (rect);
			UIImage result = UIGraphics.GetImageFromCurrentImageContext ();
			UIGraphics.EndImageContext ();
			return result;
		}
	}
}