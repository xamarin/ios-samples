namespace Conference_Diffable.CompositionalLayout.CellsandSupplementaryViews;

public partial class ConferenceNewsFeedCell : UICollectionViewCell {
	public static readonly NSString Key = new (nameof (ConferenceNewsFeedCell));

	public UILabel TitleLabel { get; private set; } = new UILabel {
			TranslatesAutoresizingMaskIntoConstraints = false,
			AdjustsFontForContentSizeCategory = true,
			Lines = 0,
			Font = UIFont.GetPreferredFontForTextStyle (UIFontTextStyle.Title2),
		};

	public UILabel DateLabel { get; private set; } = new UILabel {
			TranslatesAutoresizingMaskIntoConstraints = false,
			AdjustsFontForContentSizeCategory = true,
			Font = UIFont.GetPreferredFontForTextStyle (UIFontTextStyle.Caption2),
		};

	public UILabel BodyLabel { get; private set; } = new UILabel {
			TranslatesAutoresizingMaskIntoConstraints = false,
			AdjustsFontForContentSizeCategory = true,
			Lines = 0,
			Font = UIFont.GetPreferredFontForTextStyle (UIFontTextStyle.Body),
		};

	public UIView SeparatorView { get; private set; } = new UIImageView {
			TranslatesAutoresizingMaskIntoConstraints = false,
			BackgroundColor = UIColor.PlaceholderText
		};

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
		AddSubview (TitleLabel);
		AddSubview (DateLabel);
		AddSubview (BodyLabel);
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
		SeparatorView.Hidden = !ShowsSeparator;
	}
}
