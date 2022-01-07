/*
See LICENSE folder for this sample’s licensing information.

Abstract:
A generic cell for a List-list compositional layout
*/

namespace Conference_Diffable.CompositionalLayout.CellsandSupplementaryViews;

public partial class ListCell : UICollectionViewCell {
	public static readonly NSString Key = new NSString (nameof (ListCell));

	public UILabel? Label { get; private set; }
	public UIImageView? AccessoryImageView { get; private set; }
	public UIView? SeparatorView { get; private set; }

	[Export ("initWithFrame:")]
	public ListCell (CGRect frame) : base (frame) => Configure ();

	void Configure ()
	{
		SeparatorView = new UIView {
			TranslatesAutoresizingMaskIntoConstraints = false,
			BackgroundColor = UIColor.LightGray
		};
		ContentView.AddSubview (SeparatorView);

		Label = new UILabel {
			TranslatesAutoresizingMaskIntoConstraints = false,
			AdjustsFontForContentSizeCategory = true,
			Font = UIFont.GetPreferredFontForTextStyle (UIFontTextStyle.Body)
		};
		ContentView.AddSubview (Label);

		AccessoryImageView = new UIImageView { TranslatesAutoresizingMaskIntoConstraints = false };
		ContentView.AddSubview (AccessoryImageView);

		SelectedBackgroundView = new UIView { BackgroundColor = UIColor.LightGray.ColorWithAlpha (.3f) };

		var rtl = EffectiveUserInterfaceLayoutDirection == UIUserInterfaceLayoutDirection.RightToLeft;
		var chevronImageName = rtl ? "chevron.left" : "chevron.right";
		var chevronImage = UIImage.GetSystemImage (chevronImageName);
		AccessoryImageView.Image = chevronImage;
		AccessoryImageView.TintColor = UIColor.LightGray.ColorWithAlpha (.7f);

		var inset = 10f;
		Label.LeadingAnchor.ConstraintEqualTo (ContentView.LeadingAnchor, inset).Active = true;
		Label.TopAnchor.ConstraintEqualTo (ContentView.TopAnchor, inset).Active = true;
		Label.BottomAnchor.ConstraintEqualTo (ContentView.BottomAnchor, -inset).Active = true;
		Label.TrailingAnchor.ConstraintEqualTo (AccessoryImageView.LeadingAnchor, -inset).Active = true;

		AccessoryImageView.CenterYAnchor.ConstraintEqualTo (ContentView.CenterYAnchor).Active = true;
		AccessoryImageView.WidthAnchor.ConstraintEqualTo (13).Active = true;
		AccessoryImageView.HeightAnchor.ConstraintEqualTo (20).Active = true;
		AccessoryImageView.TrailingAnchor.ConstraintEqualTo (ContentView.TrailingAnchor, -inset).Active = true;

		SeparatorView.LeadingAnchor.ConstraintEqualTo (ContentView.LeadingAnchor, inset).Active = true;
		SeparatorView.BottomAnchor.ConstraintEqualTo (ContentView.BottomAnchor).Active = true;
		SeparatorView.TrailingAnchor.ConstraintEqualTo (ContentView.TrailingAnchor).Active = true;
		SeparatorView.HeightAnchor.ConstraintEqualTo (.5f).Active = true;
	}
}
