namespace Conference_Diffable.CompositionalLayout.CellsandSupplementaryViews;

public partial class ConferenceNewsFeedCell : UICollectionViewCell {
	public static readonly NSString Key = new (nameof (ConferenceNewsFeedCell));

	public UILabel? TitleLabel { get; private set; }
	public UILabel? DateLabel { get; private set; }
	public UILabel? BodyLabel { get; private set; }
	public UIView? SeparatorView { get; private set; }

	bool showsSeparator = true;
	public bool ShowsSeparator {
		get => showsSeparator;
		set {
			showsSeparator = value;
			UpdateSeparator ();
		}
	}

	[Export ("initWithFrame:")]
	public ConferenceNewsFeedCell (CGRect frame) : base (frame) => Configure ();

	void Configure ()
	{
		TitleLabel = new UILabel {
			TranslatesAutoresizingMaskIntoConstraints = false,
			AdjustsFontForContentSizeCategory = true,
			Lines = 0,
			Font = UIFont.GetPreferredFontForTextStyle (UIFontTextStyle.Title2),
		};
		AddSubview (TitleLabel);

		DateLabel = new UILabel {
			TranslatesAutoresizingMaskIntoConstraints = false,
			AdjustsFontForContentSizeCategory = true,
			Font = UIFont.GetPreferredFontForTextStyle (UIFontTextStyle.Caption2),
		};
		AddSubview (DateLabel);

		BodyLabel = new UILabel {
			TranslatesAutoresizingMaskIntoConstraints = false,
			AdjustsFontForContentSizeCategory = true,
			Lines = 0,
			Font = UIFont.GetPreferredFontForTextStyle (UIFontTextStyle.Body),
		};
		AddSubview (BodyLabel);

		SeparatorView = new UIImageView {
			TranslatesAutoresizingMaskIntoConstraints = false,
			BackgroundColor = UIColor.PlaceholderTextColor
		};
		AddSubview (SeparatorView);

		string [] keys = { "title", "date", "body", "separator" };
		UIView [] values = { TitleLabel, DateLabel, BodyLabel, SeparatorView };
		var views = NSDictionary.FromObjectsAndKeys (values, keys, keys.Length);

		var constraints = new List<NSLayoutConstraint> ();
		constraints.AddRange (NSLayoutConstraint.FromVisualFormat ("H:|-[title]-|", 0, null, views));
		constraints.AddRange (NSLayoutConstraint.FromVisualFormat ("H:|-[date]-|", 0, null, views));
		constraints.AddRange (NSLayoutConstraint.FromVisualFormat ("H:|-[body]-|", 0, null, views));
		constraints.AddRange (NSLayoutConstraint.FromVisualFormat ("H:|-[separator]-|", 0, null, views));
		constraints.AddRange (NSLayoutConstraint.FromVisualFormat ("V:|-[title]-[date]-[body]-20-[separator(==1)]|", 0, null, views));
		NSLayoutConstraint.ActivateConstraints (constraints.ToArray ());
	}

	void UpdateSeparator ()
	{
		if (SeparatorView is not null)
			SeparatorView.Hidden = !ShowsSeparator;
	}
}
