namespace Conference_Diffable.CompositionalLayout.CellsandSupplementaryViews;

public partial class ConferenceVideoCell : UICollectionViewCell {
	public static readonly NSString Key = new NSString (nameof (ConferenceVideoCell));

	public UIImageView ImageView { get; private set; } = new UIImageView { TranslatesAutoresizingMaskIntoConstraints = false };
	public UILabel TitleLabel { get; private set; } = new UILabel {
		TranslatesAutoresizingMaskIntoConstraints = false,
		Font = UIFont.GetPreferredFontForTextStyle (UIFontTextStyle.Caption1),
		AdjustsFontForContentSizeCategory = true,
	};
	public UILabel CategoryLabel { get; private set; } = new UILabel {
		TranslatesAutoresizingMaskIntoConstraints = false,
		Font = UIFont.GetPreferredFontForTextStyle (UIFontTextStyle.Caption2),
		AdjustsFontForContentSizeCategory = true,
		TextColor = UIColor.PlaceholderTextColor
	};

	[Export ("initWithFrame:")]
	public ConferenceVideoCell (CGRect frame) : base (frame) => Configure ();

	void Configure ()
	{
		ImageView.Layer.BackgroundColor = UIColor.Black.CGColor;
		ImageView.Layer.BorderWidth = 1;
		ImageView.Layer.CornerRadius = 4;
		ImageView.BackgroundColor = CornflowerBlue;
		AddSubview (ImageView);

		AddSubview (TitleLabel);
		AddSubview (CategoryLabel);

		var spacing = 10;
		ImageView.LeadingAnchor.ConstraintEqualTo (ContentView.LeadingAnchor).Active = true;
		ImageView.TrailingAnchor.ConstraintEqualTo (ContentView.TrailingAnchor).Active = true;
		ImageView.TopAnchor.ConstraintEqualTo (ContentView.TopAnchor).Active = true;

		TitleLabel.TopAnchor.ConstraintEqualTo (ImageView.BottomAnchor, spacing).Active = true;
		TitleLabel.LeadingAnchor.ConstraintEqualTo (ContentView.LeadingAnchor).Active = true;
		TitleLabel.TrailingAnchor.ConstraintEqualTo (ContentView.TrailingAnchor).Active = true;

		CategoryLabel.TopAnchor.ConstraintEqualTo (TitleLabel.BottomAnchor).Active = true;
		CategoryLabel.LeadingAnchor.ConstraintEqualTo (ContentView.LeadingAnchor).Active = true;
		CategoryLabel.TrailingAnchor.ConstraintEqualTo (ContentView.TrailingAnchor).Active = true;
		CategoryLabel.BottomAnchor.ConstraintEqualTo (ContentView.BottomAnchor).Active = true;
	}
}
