using System;
using System.Collections.Generic;
using UIKit;
using CoreGraphics;
using Foundation;

namespace DragSource
{
	/**
	 An ImageContainerView wraps an image view, sizing it
	 appropriately within its bounds, with margins applied
	 on all sides. The child image view is centered within
	 this container.
	 */
	public class ImageContainerView : UIView
	{
		#region Computed Properties
		public float Margin { get; set; } = 10f;
		public UIImageView ContentView { get; set; }
		#endregion

		#region Constructors
		public ImageContainerView()
		{
		}

		public ImageContainerView(NSCoder coder) : base(coder)
		{
		}

		public ImageContainerView(UIImageView imageView, float margin) : base(CGRect.Empty)
		{
			AddSubview(imageView);

			if (imageView.Image != null)
			{
				imageView.TranslatesAutoresizingMaskIntoConstraints = false;
				imageView.SetContentCompressionResistancePriority((float)UILayoutPriority.DefaultHigh, UILayoutConstraintAxis.Horizontal);
				imageView.SetContentCompressionResistancePriority((float)UILayoutPriority.DefaultHigh, UILayoutConstraintAxis.Vertical);

				var ivContraints = new NSLayoutConstraint[]{
					imageView.WidthAnchor.ConstraintEqualTo(imageView.HeightAnchor, imageView.Image.AspectRatio()),
					imageView.WidthAnchor.ConstraintLessThanOrEqualTo(this.WidthAnchor, 1, -margin),
					imageView.HeightAnchor.ConstraintLessThanOrEqualTo(HeightAnchor, 1, -margin),
					imageView.CenterXAnchor.ConstraintEqualTo(CenterXAnchor),
					imageView.CenterYAnchor.ConstraintEqualTo(CenterYAnchor)
			};

				NSLayoutConstraint.ActivateConstraints(ivContraints);
			}
		}
		#endregion

	}
}
