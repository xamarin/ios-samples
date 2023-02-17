using System;
using System.Collections.Generic;
using UIKit;
using CoreGraphics;
using Foundation;
using CoreAnimation;

namespace DropDestination {
	/**
	 The DeleteView handles UI for showing a deletion area.
	 Subclasses are responsible for notifying the delegate
	 of deletion.

	 Refer to DroppableDeleteView for an example.
	 */
	public class DeleteView : UIView {
		#region Computed Properties
		public UIImageView IconView { get; set; }
		#endregion

		#region Constructors
		public DeleteView ()
		{
		}

		public DeleteView (NSCoder coder) : base (coder)
		{
		}

		public DeleteView (string labelText)
		{
			// Initialize
			var topDivider = new UIView () {
				TranslatesAutoresizingMaskIntoConstraints = false,
				BackgroundColor = UIColor.DarkGray
			};
			AddSubview (topDivider);
			NSLayoutConstraint.ActivateConstraints (new NSLayoutConstraint []{
				topDivider.TopAnchor.ConstraintEqualTo(TopAnchor, 1),
				topDivider.WidthAnchor.ConstraintEqualTo(WidthAnchor),
				topDivider.HeightAnchor.ConstraintEqualTo(1)
			});

			var trashIcon = new UIImageView (UIImage.FromBundle ("Trash")) {
				TranslatesAutoresizingMaskIntoConstraints = false,
				ClipsToBounds = true
			};
			AddSubview (trashIcon);
			NSLayoutConstraint.ActivateConstraints (new NSLayoutConstraint []{
				trashIcon.WidthAnchor.ConstraintEqualTo(64),
				trashIcon.HeightAnchor.ConstraintEqualTo(64),
				trashIcon.CenterYAnchor.ConstraintEqualTo(CenterYAnchor),
				trashIcon.LeftAnchor.ConstraintEqualTo(LeftAnchor, 20)
			});
			trashIcon.Layer.MinificationFilter = CALayer.FilterTrilinear;
			trashIcon.Layer.CornerRadius = 32;
			IconView = trashIcon;

			var labelView = new UILabel () {
				Text = labelText,
				Font = UIFont.SystemFontOfSize (24),
				TextColor = UIColor.DarkGray,
				AllowsDefaultTighteningForTruncation = true,
				Lines = 1,
				TranslatesAutoresizingMaskIntoConstraints = false
			};
			AddSubview (labelView);
			NSLayoutConstraint.ActivateConstraints (new NSLayoutConstraint []{
				labelView.CenterYAnchor.ConstraintEqualTo(CenterYAnchor),
				labelView.LeftAnchor.ConstraintEqualTo(trashIcon.RightAnchor, 12)
			});

			BackgroundColor = UIColor.White;
			Alpha = 0.5f;
		}
		#endregion

		#region Events
		public delegate void DidDeleteViewsDelegate (List<UIView> views);
		public event DidDeleteViewsDelegate DidDeleteViews;

		internal void RaiseDidDeleteViews (List<UIView> views)
		{
			// Inform caller
			if (DidDeleteViews != null) DidDeleteViews (views);
		}
		#endregion
	}
}
