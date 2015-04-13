using System;

using UIKit;
using Foundation;
using CoreGraphics;

namespace Chat
{
	public partial class OutgoingCell : BubbleCell
	{
		static UIImage bubble;
		protected override UIImage BubbleImg {
			get {
				return bubble;
			}
		}

		static UIImage highlightedBubble;
		protected override UIImage BubbleHighlightedImage {
			get {
				return highlightedBubble;
			}
		}

		public override UIImageView BubbleView {
			get {
				// from outlet
				return BubbleImageView;
			}
		}

		protected override UILabel MessageLbl {
			get {
				// from outlet
				return MessageText;
			}
		}

		public OutgoingCell (IntPtr handle)
			: base(handle)
		{
			// TODO: move to static ctor
			if (bubble == null && highlightedBubble == null) {
				UIImage mask = UIImage.FromBundle ("MessageBubble");

				var cap = new UIEdgeInsets {
					Top = 17,
					Left = 21,
					Bottom = (float)17.5,
					Right = (float)26.5
				};

				var color = UIColor.FromRGB (43, 119, 250);
				bubble = CreateColoredImage (color, mask).CreateResizableImage (cap);

				var highlightedColor = UIColor.FromRGB (32, 96, 200);
				highlightedBubble = CreateColoredImage (highlightedColor, mask).CreateResizableImage (cap);
			}
		}
	}
}