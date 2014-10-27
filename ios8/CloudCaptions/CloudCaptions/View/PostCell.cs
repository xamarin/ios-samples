using System;

using UIKit;
using Foundation;

namespace CloudCaptions
{
	[Register("PostCell")]
	public class PostCell : UITableViewCell
	{
		string fontName;

		[Outlet("textLabelInCell")]
		UILabel TextLabelInCell { get; set; }

		[Outlet("imageViewInCell")]
		UIImageView ImageViewInCell { get; set; }

		[Outlet("activityIndicator")]
		UIActivityIndicatorView ActivityIndicator { get; set; }

		public PostCell (IntPtr handle)
			: base(handle)
		{
		}

		public PostCell ()
		{

		}

		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();
			UIFont labelFont = UIFont.FromName (fontName, 24);
			TextLabelInCell.Font = labelFont;
		}

		public void DisplayInfoForPost(Post post)
		{
			// Sets how the cell appears based on the Post passed in
			ActivityIndicator.StartAnimating ();

			if(post.ImageRecord != null)
				ImageViewInCell.Image = post.ImageRecord.FullImage;

			fontName = (string)(NSString)post.PostRecord[Post.FontKey];
			TextLabelInCell.Text = (string)(NSString)post.PostRecord[Post.TextKey];
		}
	}
}

