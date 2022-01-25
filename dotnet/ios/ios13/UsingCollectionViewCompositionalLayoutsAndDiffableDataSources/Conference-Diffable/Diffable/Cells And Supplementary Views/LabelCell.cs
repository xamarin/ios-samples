/*
See LICENSE folder for this sample’s licensing information.

Abstract:
Generic label cell
*/

namespace Conference_Diffable.Diffable.CellsAndSupplementaryViews;

public partial class LabelCell : UICollectionViewCell {
	public static readonly NSString Key = new (nameof (LabelCell));

	public UILabel Label { get; private set; } = new UILabel {
		TranslatesAutoresizingMaskIntoConstraints = false,
		AdjustsFontForContentSizeCategory = true,
		Font = UIFont.GetPreferredFontForTextStyle (UIFontTextStyle.Body)
	};

	[Export ("initWithFrame:")]
	public LabelCell (CGRect frame) : base (frame) => Configure ();

	void Configure ()
	{
		ContentView.AddSubview (Label);

		Layer.BorderWidth = 1;
		Layer.BorderColor = UIColor.SystemGray2Color.CGColor;

		var inset = 10;
		Label.LeadingAnchor.ConstraintEqualTo (ContentView.LeadingAnchor, inset).Active = true;
		Label.TrailingAnchor.ConstraintEqualTo (ContentView.TrailingAnchor, -inset).Active = true;
		Label.CenterYAnchor.ConstraintEqualTo (ContentView.CenterYAnchor).Active = true;
	}
}
