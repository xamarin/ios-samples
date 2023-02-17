using System;
using UIKit;
using CoreGraphics;
using Foundation;
using StoreKit;
using System.Linq;

namespace SoupKit.UI {
	[Register ("MenuItemView")]
	public class MenuItemView : UIView {
		public enum SubView {
			ImageView = 777,
			TitleLabel = 888,
			SubTitleLabel = 999
		}

		public UIImageView ImageView { get; set; } = new UIImageView (CGRect.Empty);
		public UILabel TitleLabel { get; set; } = new UILabel (CGRect.Empty);
		public UILabel SubTitleLabel { get; set; } = new UILabel (CGRect.Empty);

		[Export ("initWithFrame:")]
		public MenuItemView (CGRect rect) : base (rect)
		{
			SetupView ();
		}

		[Export ("initWithCoder:")]
		public MenuItemView (NSCoder coder) : base (coder)
		{
			SetupView ();
		}

		public void SetupView ()
		{
			var bundle = NSBundle.FromClass (this.Class);
			var nib = UINib.FromName ("MenuItemView", bundle);

			var stackView = nib.Instantiate (this, null).FirstOrDefault () as UIStackView;
			if (stackView is null) { return; }

			var imageView = stackView.ViewWithTag ((int) SubView.ImageView) as UIImageView;
			if (imageView is null) { return; }

			var titleLabel = stackView.ViewWithTag ((int) SubView.TitleLabel) as UILabel;
			if (titleLabel is null) { return; }

			var subTitleLabel = stackView.ViewWithTag ((int) SubView.SubTitleLabel) as UILabel;
			if (subTitleLabel is null) { return; }

			AddSubview (stackView);
			ImageView = imageView;
			TitleLabel = titleLabel;
			SubTitleLabel = subTitleLabel;

			ImageView.ClipsToBounds = true;
			ImageView.Layer.CornerRadius = 8;
		}

		#region xamarin
		// This constructor is used when Xamarin.iOS needs to create a new
		// managed object for an already-existing native object.
		public MenuItemView (IntPtr handle) : base (handle) { }
		#endregion
	}
}
