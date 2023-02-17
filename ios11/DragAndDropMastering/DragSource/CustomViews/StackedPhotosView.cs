using System;
using System.Collections.Generic;
using UIKit;
using CoreGraphics;
using CoreLocation;
using Foundation;

namespace DragSource {
	/**
	 A StackedPhotosView represents a series of images
	 stacked on top of each other. The last image in the
	 array will appear at the top of the stack.
	 */
	public class StackedPhotosView : UIView {
		#region Static Variables
		public static float Offset = 10f;
		#endregion

		#region Computed Views
		public List<UIImageView> ImageViews { get; set; } = new List<UIImageView> ();
		#endregion

		#region Constructors
		public StackedPhotosView ()
		{
		}

		public StackedPhotosView (NSCoder coder) : base (coder)
		{
		}

		public StackedPhotosView (UIImage [] images) : base (CGRect.Empty)
		{
			TranslatesAutoresizingMaskIntoConstraints = false;
			var totalStackOffset = (float) (images.Length - 1) * Offset;
			var currentStackOffset = totalStackOffset;

			BackgroundColor = UIColor.Clear;

			foreach (UIImage image in images) {
				var imageView = new UIImageView (image) {
					ContentMode = UIViewContentMode.ScaleAspectFill,
					ClipsToBounds = true
				};

				if (image == null) {
					imageView.Image = UIImage.FromBundle ("Corkboard");
				}

				ImageViews.Add (imageView);
				AddSubview (imageView);

				imageView.TranslatesAutoresizingMaskIntoConstraints = false;
				imageView.SetContentCompressionResistancePriority ((float) UILayoutPriority.DefaultHigh, UILayoutConstraintAxis.Horizontal);
				imageView.SetContentCompressionResistancePriority ((float) UILayoutPriority.DefaultHigh, UILayoutConstraintAxis.Vertical);

				var svContraints = new []{
					imageView.WidthAnchor.ConstraintEqualTo(imageView.HeightAnchor, image.AspectRatio()),
					imageView.TopAnchor.ConstraintEqualTo(this.TopAnchor, currentStackOffset),
					imageView.LeftAnchor.ConstraintEqualTo(this.LeftAnchor, currentStackOffset),
					imageView.WidthAnchor.ConstraintLessThanOrEqualTo(this.WidthAnchor, constant:-totalStackOffset),
					imageView.HeightAnchor.ConstraintLessThanOrEqualTo(this.HeightAnchor, constant:-totalStackOffset)
				};

				NSLayoutConstraint.ActivateConstraints (svContraints);

				currentStackOffset -= Offset;
			}
		}
		#endregion
	}
}
