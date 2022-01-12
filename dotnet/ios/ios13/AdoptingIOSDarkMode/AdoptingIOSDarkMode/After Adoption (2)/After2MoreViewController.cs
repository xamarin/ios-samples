/*
See LICENSE folder for this sample’s licensing information.

Abstract:
A view controller demonstrating typical creation of views: images, labels, blur, and vibrancy.
 This is the code after dark mode adoption has happened.
*/

namespace AdoptingIOSDarkMode;
public partial class After2MoreViewController : UIViewController {

	UITraitCollection? Traits;

	public After2MoreViewController (IntPtr handle) : base (handle)
	{
	}

	public override void LoadView ()
	{
		// Create our view controller's view.

		View = new UIView {
			TranslatesAutoresizingMaskIntoConstraints = false,
			// DARK MODE ADOPTION: Changed from .white to a semantic color.
			BackgroundColor = UIColor.SystemBackgroundColor
		};

		// Add various subviews, from top to bottom:
		// - Star image (our app's logo)
		// - Title label
		// - Background image view
		// - Visual effect view blurring that background image
		// - Visual effect view for vibrancy
		// - Vibrant label

		Traits = TraitCollection;
		TitleLabel.Font = UIFont.GetPreferredFontForTextStyle (UIFontTextStyle.LargeTitle, Traits);
		VibrantLabel.Font = UIFont.GetPreferredFontForTextStyle (UIFontTextStyle.LargeTitle, Traits);

		View.AddSubview (StarImageView);

		View.AddSubview (TitleLabel);

		View.AddSubview (BackgroundImageView);

		// DARK MODE ADOPTION: Changed from style: .light to an iOS 13
		// blur style, .systemThinMaterial, that has different appearances in light
		// and dark modes.
		View.AddSubview (BlurView);

		BlurView.ContentView.AddSubview (VibrancyView);

		VibrancyView.ContentView.AddSubview (VibrantLabel);

		// Add constraints to put everything in the right place.
		SetupConstraints ();
	}

	readonly UIImageView StarImageView = new (UIImage.FromBundle ("StarImage"))
	{
		TranslatesAutoresizingMaskIntoConstraints = false,
		// DARK MODE ADOPTION: Changed to a color, from the "After" asset catalog,
		// which has light and dark variants:
		TintColor = UIColor.FromName ("LightAndDarkHeaderColor"),
	};
	readonly UILabel TitleLabel = new ()
	{
		Text = "Presented Content",
		// DARK MODE ADOPTION: Changed from .black to a semantic color:
		TextColor = UIColor.LabelColor,
		TranslatesAutoresizingMaskIntoConstraints = false,
	};


	readonly UIImageView BackgroundImageView = new ()
	{
		Image = UIImage.FromBundle ("LightAndDarkHeaderImage"),
		TranslatesAutoresizingMaskIntoConstraints = false,
	};

	readonly static UIBlurEffect BlurEffect = UIBlurEffect.FromStyle (UIBlurEffectStyle.SystemThinMaterial);

	readonly UIVisualEffectView BlurView = new ()
	{
		Effect = BlurEffect,
		TranslatesAutoresizingMaskIntoConstraints = false,
	};

	// DARK MODE ADOPTION: Changed to use a specific iOS 13 vibrancy style:
	readonly UIVisualEffectView VibrancyView = new ()
	{
		Effect = UIVibrancyEffect.FromBlurEffect (BlurEffect, UIVibrancyEffectStyle.SecondaryLabel),
		TranslatesAutoresizingMaskIntoConstraints = false,
	};

	readonly UILabel VibrantLabel = new ()
	{
		TranslatesAutoresizingMaskIntoConstraints = false,
		Text = "Vibrant Label",
	};

	void SetupConstraints ()
	{
		if (View is null)
			throw new InvalidOperationException ("View");

		StarImageView.TopAnchor.ConstraintEqualToSystemSpacingBelowAnchor (View.LayoutMarginsGuide.TopAnchor, 2).Active = true;
		StarImageView.LeadingAnchor.ConstraintEqualTo (View.LayoutMarginsGuide.LeadingAnchor).Active = true;
		StarImageView.WidthAnchor.ConstraintEqualTo (70).Active = true;
		StarImageView.HeightAnchor.ConstraintEqualTo (70).Active = true;

		TitleLabel.TopAnchor.ConstraintEqualToSystemSpacingBelowAnchor (StarImageView.BottomAnchor, 2).Active = true;
		TitleLabel.LeadingAnchor.ConstraintEqualTo (View.LayoutMarginsGuide.LeadingAnchor).Active = true;

		BackgroundImageView.TopAnchor.ConstraintEqualToSystemSpacingBelowAnchor (TitleLabel.BottomAnchor, 2).Active = true;
		BackgroundImageView.LeadingAnchor.ConstraintEqualTo (View.LayoutMarginsGuide.LeadingAnchor).Active = true;
		BackgroundImageView.TrailingAnchor.ConstraintEqualTo (View.LayoutMarginsGuide.TrailingAnchor).Active = true;
		BackgroundImageView.BottomAnchor.ConstraintEqualTo (View.LayoutMarginsGuide.BottomAnchor).Active = true;

		BlurView.TopAnchor.ConstraintEqualTo (BackgroundImageView.TopAnchor).Active = true;
		BlurView.LeadingAnchor.ConstraintEqualTo (BackgroundImageView.LeadingAnchor).Active = true;
		BlurView.TrailingAnchor.ConstraintEqualTo (BackgroundImageView.TrailingAnchor).Active = true;
		BlurView.BottomAnchor.ConstraintEqualTo (BackgroundImageView.BottomAnchor).Active = true;

		VibrancyView.TopAnchor.ConstraintEqualTo (BlurView.TopAnchor).Active = true;
		VibrancyView.LeadingAnchor.ConstraintEqualTo (BlurView.LeadingAnchor).Active = true;
		VibrancyView.TrailingAnchor.ConstraintEqualTo (BlurView.TrailingAnchor).Active = true;
		VibrancyView.BottomAnchor.ConstraintEqualTo (BlurView.BottomAnchor).Active = true;

		VibrantLabel.TopAnchor.ConstraintEqualTo (VibrancyView.LayoutMarginsGuide.TopAnchor).Active = true;
		VibrantLabel.LeadingAnchor.ConstraintEqualTo (VibrancyView.LayoutMarginsGuide.LeadingAnchor).Active = true;
	}
}
