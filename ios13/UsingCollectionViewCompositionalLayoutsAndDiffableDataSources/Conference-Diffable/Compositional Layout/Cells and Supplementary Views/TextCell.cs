/*
See LICENSE folder for this sample’s licensing information.

Abstract:
Generic text cell
*/

using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Conference_Diffable.CompositionalLayout.CellsandSupplementaryViews {
	public partial class TextCell : UICollectionViewCell {
		public static readonly NSString Key = new NSString (nameof (TextCell));

		public UILabel Label { get; private set; }

		[Export ("initWithFrame:")]
		public TextCell (CGRect frame) : base (frame) => Configure ();

		void Configure ()
		{
			Label = new UILabel {
				TranslatesAutoresizingMaskIntoConstraints = false,
				AdjustsFontForContentSizeCategory = true,
				Font = UIFont.GetPreferredFontForTextStyle (UIFontTextStyle.Caption1)
			};
			ContentView.AddSubview (Label);

			var inset = 10f;
			Label.LeadingAnchor.ConstraintEqualTo (ContentView.LeadingAnchor, inset).Active = true;
			Label.TrailingAnchor.ConstraintEqualTo (ContentView.TrailingAnchor, -inset).Active = true;
			Label.TopAnchor.ConstraintEqualTo (ContentView.TopAnchor, inset).Active = true;
			Label.BottomAnchor.ConstraintEqualTo (ContentView.BottomAnchor, -inset).Active = true;
		}
	}
}
