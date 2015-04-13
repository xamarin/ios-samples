using System;

using UIKit;
using Foundation;
using CoreGraphics;

namespace Chat
{
	public partial class OutgoingCell : UITableViewCell
	{
		public override UILabel TextLabel {
			get {
				return MessageText;
			}
		}

		UIImage bubble;

		public OutgoingCell (IntPtr handle)
			: base(handle)
		{
			UIImage mask = UIImage.FromBundle ("MessageBubble");

			var fillColor = UIColor.FromRGB (43, 119, 250);
			var cap = new UIEdgeInsets {
				Top = 17,
				Left = 21,
				Bottom = (float)17.5,
				Right = (float)26.5
			};

			bubble = CellHelper.CreateColoredImage (fillColor, mask).CreateResizableImage (cap);
		}

		public override void PrepareForReuse ()
		{
			base.PrepareForReuse ();
			BubbleImageView.Image = bubble;
		}
	}
}

