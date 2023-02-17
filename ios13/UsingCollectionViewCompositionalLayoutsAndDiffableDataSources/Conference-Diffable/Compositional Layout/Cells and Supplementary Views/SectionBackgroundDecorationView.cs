/*
See LICENSE folder for this sample’s licensing information.

Abstract:
Decoration view for rendering the background of a compositional section
*/

using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Conference_Diffable.CompositionalLayout.CellsandSupplementaryViews {
	public partial class SectionBackgroundDecorationView : UICollectionReusableView {
		[Export ("initWithFrame:")]
		public SectionBackgroundDecorationView (CGRect frame) : base (frame) => Configure ();

		void Configure ()
		{
			BackgroundColor = UIColor.LightGray.ColorWithAlpha (.5f);
			Layer.BorderColor = UIColor.Black.CGColor;
			Layer.BorderWidth = 1;
			Layer.CornerRadius = 12;
		}
	}
}
