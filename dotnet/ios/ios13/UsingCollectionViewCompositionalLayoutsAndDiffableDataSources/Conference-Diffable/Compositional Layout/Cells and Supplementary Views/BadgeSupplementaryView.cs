/*
See LICENSE folder for this sample’s licensing information.

Abstract:
Supplementary view for bading an item
*/

namespace Conference_Diffable.CompositionalLayout.CellsandSupplementaryViews;

public partial class BadgeSupplementaryView : UICollectionReusableView {
	public static readonly NSString Key = new (nameof (BadgeSupplementaryView));

	public UILabel? Label { get; private set; }

	public override CGRect Frame {
		get => base.Frame;
		set {
			base.Frame = value;
			ConfigureBorder ();
		}
	}

	public override CGRect Bounds {
		get => base.Bounds;
		set {
			base.Bounds = value;
			ConfigureBorder ();
		}
	}

	[Export ("initWithFrame:")]
	public BadgeSupplementaryView (CGRect frame) : base (frame) => Configure ();

	void Configure ()
	{
		Label = new UILabel {
			TranslatesAutoresizingMaskIntoConstraints = false,
			AdjustsFontForContentSizeCategory = true,
			Font = UIFont.GetPreferredFontForTextStyle (UIFontTextStyle.Caption1),
			TextAlignment = UITextAlignment.Center,
			TextColor = UIColor.Black
		};
		AddSubview (Label);

		BackgroundColor = UIColor.Green;

		Label.CenterXAnchor.ConstraintEqualTo (CenterXAnchor).Active = true;
		Label.CenterYAnchor.ConstraintEqualTo (CenterYAnchor).Active = true;

		ConfigureBorder ();
	}

	void ConfigureBorder ()
	{
		var radius = Bounds.Width / 2;
		Layer.CornerRadius = radius;
		Layer.BorderColor = UIColor.Black.CGColor;
		Layer.BorderWidth = 1;
	}
}
