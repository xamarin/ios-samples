/*
See LICENSE folder for this sample’s licensing information.

Abstract:
Generic text cell
*/

namespace Conference_Diffable.CompositionalLayout.CellsandSupplementaryViews;

public partial class TextCell : UICollectionViewCell {
	public static readonly NSString Key = new (nameof (TextCell));

	public UILabel Label { get; private set; } = new UILabel {
		TranslatesAutoresizingMaskIntoConstraints = false,
		AdjustsFontForContentSizeCategory = true,
		Font = UIFont.GetPreferredFontForTextStyle (UIFontTextStyle.Caption1)
	};

	[Export ("initWithFrame:")]
	public TextCell (CGRect frame) : base (frame) => Configure ();

	void Configure ()
	{
		ContentView.AddSubview (Label);

		var inset = 10f;
		Label.LeadingAnchor.ConstraintEqualTo (ContentView.LeadingAnchor, inset).Active = true;
		Label.TrailingAnchor.ConstraintEqualTo (ContentView.TrailingAnchor, -inset).Active = true;
		Label.TopAnchor.ConstraintEqualTo (ContentView.TopAnchor, inset).Active = true;
		Label.BottomAnchor.ConstraintEqualTo (ContentView.BottomAnchor, -inset).Active = true;
	}
}
