using System;
using System.Threading;

using UIKit;
using CoreImage;
using Foundation;
using CoreGraphics;

namespace LookInside
{
	public class OverlayViewController : UIViewController
	{
		CIImage baseCIImage;

		UIVisualEffectView backgroundView;
		UIVisualEffectView foregroundContentView;

		UIScrollView foregroundContentScrollView;

		UIBlurEffect blurEffect;
		UIImageView imageView;

		OverlayVibrantLabel hueLabel;
		UISlider hueSlider;

		OverlayVibrantLabel saturationLabel;
		UISlider saturationSlider;

		OverlayVibrantLabel brightnessLabel;
		UISlider brightnessSlider;

		UIButton saveButton;

		PhotoCollectionViewCell currentPhotoView;
		public PhotoCollectionViewCell PhotoView {
			get {
				return currentPhotoView;
			}
			set {
				if (currentPhotoView != value) {
					currentPhotoView = value;
					ConfigureCIObjects ();
				}
			}
		}

		public OverlayViewController()
		{
			ModalPresentationStyle = UIModalPresentationStyle.Custom;
			Setup ();
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			foregroundContentView = new UIVisualEffectView(UIVibrancyEffect.FromBlurEffect(blurEffect));
			backgroundView = new UIVisualEffectView (blurEffect);
			foregroundContentScrollView = new UIScrollView ();

			ConfigureViews ();
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			PrepareForReuse ();
			UpdateLabels ();
		}

		void ConfigureCIObjects()
		{
			baseCIImage = CIImage.FromCGImage (PhotoView.Image.CGImage);
		}

		void OnSliderChanged(object sender, EventArgs e)
		{
			UpdateLabels ();
			ApplyFilters ();
		}

		void UpdateLabels()
		{
			// Update labels
			hueLabel.Text = string.Format ("Hue: {0}", hueSlider.Value);
			saturationLabel.Text = string.Format ("Saturation: {0}", saturationSlider.Value);
			brightnessLabel.Text = string.Format ("Brightness: {0}", brightnessSlider.Value);
		}

		void ApplyFilters()
		{
			float hue = hueSlider.Value;
			float saturation = saturationSlider.Value;
			float brightness = brightnessSlider.Value;

			// Apply effects to image
			CIImage result = null;

			using(var colorControlsFilter = new CIColorControls ()) {
				colorControlsFilter.Image = baseCIImage;
				colorControlsFilter.Saturation = saturation;
				colorControlsFilter.Brightness = brightness;
				result = colorControlsFilter.OutputImage;
			}

			using(var hueAdjustFilter = new CIHueAdjust ()) {
				hueAdjustFilter.Image = result;
				hueAdjustFilter.Angle = hue;
				result = hueAdjustFilter.OutputImage;
			}

			using (var context = CIContext.FromOptions (null)) {
				using (CGImage cgImage = context.CreateCGImage (result, result.Extent)) {
					if (imageView.Image != null) {
						imageView.Image.CGImage.Dispose ();
						imageView.Image.Dispose ();
					}
					imageView.Image = UIImage.FromImage (cgImage);
				}
			}

			result.Dispose ();
		}

		void OnSaveClicked(object sender, EventArgs e)
		{
			PhotoView.Image.Dispose ();
			PhotoView.Image = imageView.Image;
			PresentingViewController.DismissViewController (true, () => {
			});
		}

		UISlider ConfiguredOverlaySlider()
		{
			UISlider slider = new UISlider ();
			slider.TranslatesAutoresizingMaskIntoConstraints = false;
			slider.ValueChanged += OnSliderChanged;
			slider.Continuous = false;
			return slider;
		}

		void Setup()
		{
			imageView = new UIImageView ();
			imageView.ContentMode = UIViewContentMode.ScaleAspectFit;
			imageView.TranslatesAutoresizingMaskIntoConstraints = false;
			blurEffect = UIBlurEffect.FromStyle (UIBlurEffectStyle.ExtraLight);
		}

		void ConfigureViews()
		{
			View.BackgroundColor = UIColor.Clear;

			backgroundView.TranslatesAutoresizingMaskIntoConstraints = false;
			foregroundContentScrollView.TranslatesAutoresizingMaskIntoConstraints = false;
			foregroundContentView.TranslatesAutoresizingMaskIntoConstraints = false;
			hueLabel = new OverlayVibrantLabel ();
			hueSlider = ConfiguredOverlaySlider ();
			hueSlider.MaxValue = 10;

			saturationLabel = new OverlayVibrantLabel ();
			saturationSlider = ConfiguredOverlaySlider ();
			saturationSlider.MaxValue = 2;

			brightnessLabel = new OverlayVibrantLabel ();
			brightnessSlider = ConfiguredOverlaySlider ();
			brightnessSlider.MinValue = -0.5f;
			brightnessSlider.MaxValue = 0.5f;

			saveButton = new UIButton (UIButtonType.System);
			saveButton.TranslatesAutoresizingMaskIntoConstraints = false;
			saveButton.SetTitle ("Save", UIControlState.Normal);
			saveButton.TitleLabel.Font = UIFont.SystemFontOfSize (32);
			saveButton.TouchUpInside += OnSaveClicked;

			View.AddSubview (backgroundView);
			View.AddSubview (foregroundContentScrollView);

			foregroundContentScrollView.AddSubview (foregroundContentView);

			foregroundContentView.ContentView.AddSubview (hueLabel);
			foregroundContentView.ContentView.AddSubview (hueSlider);

			foregroundContentView.ContentView.AddSubview (saturationLabel);
			foregroundContentView.ContentView.AddSubview (saturationSlider);

			foregroundContentView.ContentView.AddSubview (brightnessLabel);
			foregroundContentView.ContentView.AddSubview (brightnessSlider);

			foregroundContentView.ContentView.AddSubview (saveButton);

			foregroundContentScrollView.AddSubview (imageView);

			View.AddConstraints (NSLayoutConstraint.FromVisualFormat ("H:|[backgroundView]|",
				(NSLayoutFormatOptions)0,
				"backgroundView", backgroundView));
			View.AddConstraints (NSLayoutConstraint.FromVisualFormat("V:|[backgroundView]|",
				(NSLayoutFormatOptions)0,
				"backgroundView", backgroundView));

			View.AddConstraints (NSLayoutConstraint.FromVisualFormat ("H:|[foregroundContentScrollView]|",
				(NSLayoutFormatOptions)0,
				"foregroundContentScrollView", foregroundContentScrollView));
			View.AddConstraints (NSLayoutConstraint.FromVisualFormat ("V:|[foregroundContentScrollView]|",
				(NSLayoutFormatOptions)0,
				"foregroundContentScrollView", foregroundContentScrollView));

			View.AddConstraints (NSLayoutConstraint.FromVisualFormat ("H:|[foregroundContentView]|",
				(NSLayoutFormatOptions)0,
				"foregroundContentView", foregroundContentView));
			View.AddConstraints (NSLayoutConstraint.FromVisualFormat ("V:|[foregroundContentView]|",
				(NSLayoutFormatOptions)0,
				"foregroundContentView", foregroundContentView));

			View.AddConstraints (NSLayoutConstraint.FromVisualFormat ("H:|-[hueLabel]-|",
				(NSLayoutFormatOptions)0,
				"hueLabel", hueLabel));
			View.AddConstraints (NSLayoutConstraint.FromVisualFormat ("H:|-[hueSlider]-|",
				(NSLayoutFormatOptions)0,
				"hueSlider", hueSlider));
			View.AddConstraints (NSLayoutConstraint.FromVisualFormat ("H:|-[saturationLabel]-|",
				(NSLayoutFormatOptions)0,
				"saturationLabel", saturationLabel));
			View.AddConstraints (NSLayoutConstraint.FromVisualFormat ("H:|-[saturationSlider]-|",
				(NSLayoutFormatOptions)0,
				"saturationSlider", saturationSlider));
			View.AddConstraints (NSLayoutConstraint.FromVisualFormat ("H:|-[brightnessLabel]-|",
				(NSLayoutFormatOptions)0,
				"brightnessLabel", brightnessLabel));
			View.AddConstraints (NSLayoutConstraint.FromVisualFormat ("H:|-[brightnessSlider]-|",
				(NSLayoutFormatOptions)0,
				"brightnessSlider", brightnessSlider));
			View.AddConstraints (NSLayoutConstraint.FromVisualFormat ("H:|-[saveButton]-|",
				(NSLayoutFormatOptions)0,
				"saveButton", saveButton));
			View.AddConstraints (NSLayoutConstraint.FromVisualFormat ("H:|[imageView(==foregroundContentScrollView)]|",
				(NSLayoutFormatOptions)0,
				"imageView", imageView,
				"foregroundContentScrollView", foregroundContentScrollView));
			View.AddConstraints (NSLayoutConstraint.FromVisualFormat ("V:|-(>=30)-[hueLabel]-[hueSlider]-[saturationLabel]-[saturationSlider]-[brightnessLabel]-[brightnessSlider]-[saveButton]-(>=10)-[imageView(==200)]|",
				(NSLayoutFormatOptions)0,
				"hueLabel", hueLabel,
				"hueSlider", hueSlider,
				"saturationLabel", saturationLabel,
				"saturationSlider", saturationSlider,
				"brightnessLabel", brightnessLabel,
				"brightnessSlider", brightnessSlider,
				"saveButton", saveButton,
				"imageView", imageView));
		}

		void PrepareForReuse()
		{
			imageView.Image = PhotoView.Image;

			hueSlider.Value = 0;
			saturationSlider.Value = 1;
			brightnessSlider.Value = 0;
		}

		protected override void Dispose (bool disposing)
		{
			base.Dispose (disposing);
		}
	}
}

