using System;
using System.Collections.Generic;
using UIKit;
using CoreGraphics;
using CoreLocation;
using Foundation;

namespace DragSource {
	/**
	 A PhotoStackColumnView is a view that is able to arrange
	 a given list of views in a vertical stack, applying spacing
	 and constraints to each view. This wraps and positions a
	 UIStackView. This view's purpose is to help lay out other
	 views in the app.
	 */
	public class PhotoStackColumnView : UIView {
		#region Static Variables
		public static float MinimumMargin = 25f;
		public static float VerticalSpacing = 10f;
		public UIStackView StackView = new UIStackView ();
		#endregion

		#region Constructors
		public PhotoStackColumnView ()
		{
		}

		public PhotoStackColumnView (NSCoder coder) : base (coder)
		{
		}

		public PhotoStackColumnView (UIView [] views) : base (CGRect.Empty)
		{
			StackView.Spacing = VerticalSpacing;
			AddSubview (StackView);

			var totalMarginAmount = 2f * MinimumMargin;
			foreach (UIView view in views) {
				StackView.AddArrangedSubview (view);

				view.SetContentCompressionResistancePriority ((float) UILayoutPriority.DefaultHigh, UILayoutConstraintAxis.Horizontal);
				view.SetContentCompressionResistancePriority ((float) UILayoutPriority.DefaultHigh, UILayoutConstraintAxis.Vertical);

				var heightMultiplier = 1.0f / (float) views.Length;
				var heightConstant = -totalMarginAmount - VerticalSpacing;

				var vContraints = new []{
					view.WidthAnchor.ConstraintLessThanOrEqualTo(this.WidthAnchor, 1.0f, -totalMarginAmount),
					view.HeightAnchor.ConstraintLessThanOrEqualTo(this.HeightAnchor, heightMultiplier, heightConstant)

				};

				NSLayoutConstraint.ActivateConstraints (vContraints);

			}

			StackView.Alignment = UIStackViewAlignment.Center;
			StackView.Axis = UILayoutConstraintAxis.Vertical;
			StackView.TranslatesAutoresizingMaskIntoConstraints = false;

			var svContraints = new []{
				StackView.CenterXAnchor.ConstraintEqualTo(CenterXAnchor),
				StackView.CenterYAnchor.ConstraintEqualTo(CenterYAnchor)
				};

			NSLayoutConstraint.ActivateConstraints (svContraints);

		}
		#endregion

	}
}
