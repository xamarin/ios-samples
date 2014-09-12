using System;
using System.Drawing;
using CoreGraphics;
using Foundation;
using UIKit;

namespace AdaptivePhotos
{
	public class OverlayView : UIView
	{
		UILabel label;

		public string Text { 
			get {
				return label.Text;
			} 

			set {
				label.Text = value;
			}
		}

		public override CGSize IntrinsicContentSize {
			get {
				CGSize size = label.IntrinsicContentSize;

				if (TraitCollection.HorizontalSizeClass == UIUserInterfaceSizeClass.Compact) {
					size.Width += 4.0f;
				} else {
					size.Width += 40.0f;
				}

				if (TraitCollection.VerticalSizeClass == UIUserInterfaceSizeClass.Compact) {
					size.Height += 4.0f;
				} else {
					size.Height += 40.0f;
				}

				return size;
			}
		}

		public OverlayView ()
		{
			var effect = UIBlurEffect.FromStyle (UIBlurEffectStyle.Light);
			var backgroundView = new UIVisualEffectView (effect);
			backgroundView.ContentView.BackgroundColor = UIColor.FromWhiteAlpha (0.7f, 0.3f);
			backgroundView.TranslatesAutoresizingMaskIntoConstraints = false;
			Add (backgroundView);

			var views = NSDictionary.FromObjectAndKey (backgroundView, new NSString ("backgroundView"));
			var constraints = NSLayoutConstraint.FromVisualFormat ("|[backgroundView]|", 
				                  NSLayoutFormatOptions.DirectionLeadingToTrailing, null, views);
			AddConstraints (constraints);
			constraints = NSLayoutConstraint.FromVisualFormat ("V:|[backgroundView]|", 
				NSLayoutFormatOptions.DirectionLeadingToTrailing, null, views);
			AddConstraints (constraints);

			label = new UILabel ();
			label.TranslatesAutoresizingMaskIntoConstraints = false;
			Add (label);

			AddConstraint (NSLayoutConstraint.Create (label, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, 
				backgroundView, NSLayoutAttribute.CenterX, 1.0f, 0.0f));
			AddConstraint (NSLayoutConstraint.Create (label, NSLayoutAttribute.CenterY, NSLayoutRelation.Equal, 
				backgroundView, NSLayoutAttribute.CenterY, 1.0f, 0.0f));
		}

		public override void TraitCollectionDidChange (UITraitCollection previousTraitCollection)
		{
			if (previousTraitCollection == null)
				previousTraitCollection = new UITraitCollection ();

			base.TraitCollectionDidChange (previousTraitCollection);

			if ((TraitCollection.VerticalSizeClass != previousTraitCollection.VerticalSizeClass) ||
			    (TraitCollection.VerticalSizeClass != previousTraitCollection.HorizontalSizeClass)) {
				InvalidateIntrinsicContentSize ();
			}
		}
	}
}

