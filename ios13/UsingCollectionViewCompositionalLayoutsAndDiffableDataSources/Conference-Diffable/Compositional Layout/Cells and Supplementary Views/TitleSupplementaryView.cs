/*
See LICENSE folder for this sample’s licensing information.

Abstract:
Simple example of a self-sizing supplementary title view
*/

using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Conference_Diffable.CompositionalLayout.CellsandSupplementaryViews {
	public partial class TitleSupplementaryView : UICollectionReusableView {
		public static readonly NSString Key = new NSString (nameof (TitleSupplementaryView));

		public UILabel Label { get; private set; }

		[Export ("initWithFrame:")]
		public TitleSupplementaryView (CGRect frame) : base (frame) => Configure ();

		void Configure ()
		{
			Label = new UILabel {
				TranslatesAutoresizingMaskIntoConstraints = false,
				AdjustsFontForContentSizeCategory = true,
				Font = UIFont.GetPreferredFontForTextStyle (UIFontTextStyle.Title3),
			};
			AddSubview (Label);

			var inset = 10;
			Label.LeadingAnchor.ConstraintEqualTo (LeadingAnchor, inset).Active = true;
			Label.TrailingAnchor.ConstraintEqualTo (TrailingAnchor, -inset).Active = true;
			Label.TopAnchor.ConstraintEqualTo (TopAnchor, inset).Active = true;
			Label.BottomAnchor.ConstraintEqualTo (BottomAnchor, -inset).Active = true;
		}
	}
}
