/*
See LICENSE folder for this sample’s licensing information.

Abstract:
A view controller demonstrating typical creation of views: images, labels, blur, and vibrancy.
 This is the code after dark mode adoption has happened.
*/

namespace AdoptingIOSDarkMode;
public partial class After2MoreViewController : UIViewController {
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

		var starImageView = ConfigureStarImageView ();
		View.AddSubview (starImageView);

		var titleLabel = ConfigureTitleLabel ();
		View.AddSubview (titleLabel);

		var backgroundImageView = ConfigureBackgroundImageView ();
		View.AddSubview (backgroundImageView);

		// DARK MODE ADOPTION: Changed from style: .light to an iOS 13
		// blur style, .systemThinMaterial, that has different appearances in light
		// and dark modes.
		var blurEffect = UIBlurEffect.FromStyle (UIBlurEffectStyle.SystemThinMaterial);
		var blurView = ConfigureBlurView (blurEffect);
		View.AddSubview (blurView);

		var vibrancyView = ConfigureVibrancyView (blurEffect);
		blurView.ContentView.AddSubview (vibrancyView);

		var vibrantLabel = ConfigureVibrantLabel ();
		vibrancyView.ContentView.AddSubview (vibrantLabel);

		// Add constraints to put everything in the right place.
		SetupConstraints ();
	}

	UIImageView? starImageView;
	UIImageView ConfigureStarImageView ()
	{
		starImageView = new UIImageView (UIImage.FromBundle ("StarImage"));
		starImageView.TranslatesAutoresizingMaskIntoConstraints = false;
		// DARK MODE ADOPTION: Changed to a color, from the "After" asset catalog,
		// which has light and dark variants:
		starImageView.TintColor = UIColor.FromName ("LightAndDarkHeaderColor");
		return starImageView;
	}

	UILabel? titleLabel;
	UILabel ConfigureTitleLabel ()
	{
		titleLabel = new UILabel ();
		titleLabel.Font = UIFont.GetPreferredFontForTextStyle (UIFontTextStyle.LargeTitle, TraitCollection);
		titleLabel.Text = "Presented Content";
		// DARK MODE ADOPTION: Changed from .black to a semantic color:
		titleLabel.TextColor = UIColor.LabelColor;
		titleLabel.TranslatesAutoresizingMaskIntoConstraints = false;
		return titleLabel;
	}

	UIImageView? backgroundImageView;
	UIImageView ConfigureBackgroundImageView ()
	{
		// DARK MODE ADOPTION: Changed to an image, from the "After" asset catalog,
		// which has light and dark variants:
		backgroundImageView = new UIImageView ();
		backgroundImageView.Image = UIImage.FromBundle ("LightAndDarkHeaderImage");
		backgroundImageView.TranslatesAutoresizingMaskIntoConstraints = false;
		return backgroundImageView;
	}

	UIVisualEffectView? blurView;
	UIVisualEffectView ConfigureBlurView (UIBlurEffect blurEffect)
	{
		blurView = new UIVisualEffectView ();
		blurView.Effect = blurEffect;
		blurView.TranslatesAutoresizingMaskIntoConstraints = false;
		return blurView;
	}

	UIVisualEffectView? vibrancyView;
	UIVisualEffectView ConfigureVibrancyView (UIBlurEffect blurEffect)
	{
		// DARK MODE ADOPTION: Changed to use a specific iOS 13 vibrancy style:
		vibrancyView = new UIVisualEffectView ();
		var vibrancyEffect = UIVibrancyEffect.FromBlurEffect (blurEffect, UIVibrancyEffectStyle.SecondaryLabel);
		vibrancyView.Effect = vibrancyEffect;
		vibrancyView.TranslatesAutoresizingMaskIntoConstraints = false;
		return vibrancyView;
	}

	UILabel? vibrantLabel;
	UILabel ConfigureVibrantLabel ()
	{
		vibrantLabel = new UILabel ();
		vibrantLabel.TranslatesAutoresizingMaskIntoConstraints = false;
		vibrantLabel.Font = UIFont.GetPreferredFontForTextStyle (UIFontTextStyle.LargeTitle, TraitCollection);
		vibrantLabel.Text = "Vibrant Label";
		return vibrantLabel;
	}

	void SetupConstraints ()
	{
		if ( starImageView is null || backgroundImageView is null || titleLabel is null || vibrantLabel is null || blurView is null || vibrancyView is null || View is null)
			throw new NullReferenceException ("There was a null UI element.");

		starImageView.TopAnchor.ConstraintEqualToSystemSpacingBelowAnchor (View.LayoutMarginsGuide.TopAnchor, 2).Active = true;
		starImageView.LeadingAnchor.ConstraintEqualTo (View.LayoutMarginsGuide.LeadingAnchor).Active = true;
		starImageView.WidthAnchor.ConstraintEqualTo (70).Active = true;
		starImageView.HeightAnchor.ConstraintEqualTo (70).Active = true;

		titleLabel.TopAnchor.ConstraintEqualToSystemSpacingBelowAnchor (starImageView.BottomAnchor, 2).Active = true;
		titleLabel.LeadingAnchor.ConstraintEqualTo (View.LayoutMarginsGuide.LeadingAnchor).Active = true;

		backgroundImageView.TopAnchor.ConstraintEqualToSystemSpacingBelowAnchor (titleLabel.BottomAnchor, 2).Active = true;
		backgroundImageView.LeadingAnchor.ConstraintEqualTo (View.LayoutMarginsGuide.LeadingAnchor).Active = true;
		backgroundImageView.TrailingAnchor.ConstraintEqualTo (View.LayoutMarginsGuide.TrailingAnchor).Active = true;
		backgroundImageView.BottomAnchor.ConstraintEqualTo (View.LayoutMarginsGuide.BottomAnchor).Active = true;

		blurView.TopAnchor.ConstraintEqualTo (backgroundImageView.TopAnchor).Active = true;
		blurView.LeadingAnchor.ConstraintEqualTo (backgroundImageView.LeadingAnchor).Active = true;
		blurView.TrailingAnchor.ConstraintEqualTo (backgroundImageView.TrailingAnchor).Active = true;
		blurView.BottomAnchor.ConstraintEqualTo (backgroundImageView.BottomAnchor).Active = true;

		vibrancyView.TopAnchor.ConstraintEqualTo (blurView.TopAnchor).Active = true;
		vibrancyView.LeadingAnchor.ConstraintEqualTo (blurView.LeadingAnchor).Active = true;
		vibrancyView.TrailingAnchor.ConstraintEqualTo (blurView.TrailingAnchor).Active = true;
		vibrancyView.BottomAnchor.ConstraintEqualTo (blurView.BottomAnchor).Active = true;

		vibrantLabel.TopAnchor.ConstraintEqualTo (vibrancyView.LayoutMarginsGuide.TopAnchor).Active = true;
		vibrantLabel.LeadingAnchor.ConstraintEqualTo (vibrancyView.LayoutMarginsGuide.LeadingAnchor).Active = true;
	}
}
