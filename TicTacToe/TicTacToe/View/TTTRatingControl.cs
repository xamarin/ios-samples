using System;
using UIKit;
using CoreGraphics;
using System.Collections.Generic;

namespace TicTacToe
{
	public class TTTRatingControl : UIControl
	{
		const int MinimumRating = 0;
		const int MaximumRating = 4;
		int rating;

		public int Rating {
			get { return rating; }
			set {
				rating = value;
				updateButtonImages ();
			}
		}

		UIImageView backgroundImageView;
		List<UIButton> buttons;

		public TTTRatingControl ()
		{
			Rating = MinimumRating;
		}

		public TTTRatingControl (CGRect frame) : base (frame)
		{
			Rating = MinimumRating;
		}

		static UIImage backgroundImage;

		public static UIImage BackgroundImage {
			get {
				if (backgroundImage == null) {
					float cornerRadius = 4f;
					float capSize = 2f * cornerRadius;
					float rectSize = 2f * capSize + 1f;
					CGRect rect = new CGRect (0f, 0f, rectSize, rectSize);
					UIGraphics.BeginImageContextWithOptions (rect.Size, false, 0f);

					UIColor.FromWhiteAlpha (0f, 0.2f).SetColor ();
					UIBezierPath bezierPath = UIBezierPath.FromRoundedRect (rect, cornerRadius);
					bezierPath.Fill ();

					UIImage image = UIGraphics.GetImageFromCurrentImageContext ();
					image = image.CreateResizableImage (new UIEdgeInsets (capSize, capSize,
					                                                      capSize, capSize));
					image = image.ImageWithRenderingMode (UIImageRenderingMode.AlwaysTemplate);
					UIGraphics.EndImageContext ();

					backgroundImage = image;
				}

				return backgroundImage;
			}
		}

		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();

			if (backgroundImageView == null) {
				backgroundImageView = new UIImageView (BackgroundImage);
				AddSubview (backgroundImageView);
			}

			backgroundImageView.Frame = Bounds;

			if (buttons == null) {
				buttons = new List<UIButton> ();

				for (var rating = MinimumRating; rating <= MaximumRating; rating++) {
					UIButton button = UIButton.FromType (UIButtonType.Custom);
					button.SetImage (UIImage.FromBundle ("unselectedButton"),
					                 UIControlState.Normal);
					button.SetImage (UIImage.FromBundle ("unselectedButton"),
					                 UIControlState.Highlighted);
					button.SetImage (UIImage.FromBundle ("favoriteButton"),
					                 UIControlState.Selected);
					button.SetImage (UIImage.FromBundle ("favoriteButton"),
					                 UIControlState.Highlighted | UIControlState.Selected);
					button.Tag = rating;
					button.TouchUpInside += touchButton;
					button.AccessibilityLabel = String.Format ("{0} stars", rating + 1);
					AddSubview (button);
					buttons.Add (button);
				}

				updateButtonImages ();
			}

			CGRect buttonFrame = Bounds;
			float width = (float)buttonFrame.Size.Width / (MaximumRating - MinimumRating + 1);
			for (int index = 0; index < buttons.Count; index++) {
				UIButton button = buttons [index];
				buttonFrame.Width = (float)Math.Round (width * (index + 1) - buttonFrame.X);
				button.Frame = buttonFrame;
				buttonFrame.X += buttonFrame.Size.Width;
			}
		}

		void touchButton (object sender, EventArgs e)
		{
			UIButton button = (UIButton)sender;
			Rating = (int)button.Tag;
			SendActionForControlEvents (UIControlEvent.ValueChanged);
		}

		void updateButtonImages ()
		{
			if (buttons != null) {
				for (int i = 0; i < buttons.Count; i++)
					buttons [i].Selected = i + TTTRatingControl.MinimumRating <= Rating;
			}
		}

		public override bool IsAccessibilityElement {
			get {
				return false;
			}
		}
	}
}

