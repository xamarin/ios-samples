using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.CoreImage;

using System.Collections.Generic;
using MonoTouch.Dialog;
using System.Drawing;
using System.Linq;
using System.Reflection;
using MonoTouch.CoreGraphics;

namespace coreimage
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		CIImage flower = CIImage.FromCGImage (UIImage.FromFile ("flower.png").CGImage);
		CIImage clouds = CIImage.FromCGImage (UIImage.FromFile ("clouds.jpg").CGImage);
		CIImage heron = CIImage.FromCGImage (UIImage.FromFile ("heron.jpg").CGImage);
		UIWindow window;
		
		#region UIApplicationDelegate Methods
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			var root = new RootElement ("Effects") {
				new Section () {
					new RootElement ("Color Adjustment"){
						new Section () {
							new RootElement ("ColorControls", (x) => Demo (ColorControls)),
							new RootElement ("ColorMatrix", (x) => Demo (ColorMatrix)),
							new RootElement ("ExposureAdjust", (x) => Demo (ExposureAdjust)),
							new RootElement ("GammaAdjust", (x) => Demo (GammaAdjust)),
							new RootElement ("HueAdjust", (x) => Demo (HueAdjust)),
							new RootElement ("TemperatureAndTint", (x) => Demo (TemperatureAndTint)),
							new RootElement ("ToneCurve", (x) => Demo (ToneCurve)),
							new RootElement ("Vibrance", (x) => Demo (Vibrance)),
							new RootElement ("WhitePointAdjust", (x) => Demo(WhitePointAdjust))
						}
					},
					new RootElement ("Color Effect"){
						new Section () {
							new RootElement ("ColorCube", (x) => Demo (ColorCube)),
							new RootElement ("ColorInvert", (x) => Demo (ColorInvert)),
							new RootElement ("ColorMonochrome", (x) => Demo (ColorMonochrome)),
							new RootElement ("FalseColor", (x) => Demo (FalseColor)),
							new RootElement ("SepiaTone", (x) => Demo (SepiaTone)),
						}
					},
					new RootElement ("Composite Operation"){
						new Section () {
							new RootElement ("AdditionCompositing", (x) => Demo (AdditionCompositing)),
							new RootElement ("ColorBlendMode", (x) => Demo (ColorBlendMode)),
							new RootElement ("ColorDodgeBlendMode", (x) => Demo (ColorDodgeBlendMode)),
							new RootElement ("DifferenceBlendMode", (x) => Demo (DifferenceBlendMode)),
							new RootElement ("ExclusionBlendMode", (x) => Demo (ExclusionBlendMode)),
							new RootElement ("HardLightBlendMode", (x) => Demo (HardLightBlendMode)),
							new RootElement ("HueBlendMode", (x) => Demo (HueBlendMode)),
							new RootElement ("LightenBlendMode", (x) => Demo (LightenBlendMode)),
							new RootElement ("LuminosityBlendMode", (x) => Demo (LuminosityBlendMode)),
							new RootElement ("MaximumCompositing", (x) => Demo (MaximumCompositing)),
							new RootElement ("MultiplyCompositing", (x) => Demo (MultiplyCompositing)),
							new RootElement ("MultiplyBlendMode", (x) => Demo (MultiplyBlendMode)),
							new RootElement ("OverlayBlendMode", (x) => Demo (OverlayBlendMode)),
							new RootElement ("SaturationBlendMode", (x) => Demo (SaturationBlendMode)),
							new RootElement ("ScreenBlendMode", (x) => Demo (ScreenBlendMode)),
							new RootElement ("SoftLightBlendMode", (x) => Demo (SoftLightBlendMode)),
							new RootElement ("SourceAtopCompositing", (x) => Demo (SourceAtopCompositing)),
							new RootElement ("SourceInCompositing", (x) => Demo(SourceInCompositing)),
							new RootElement ("SourceOutCompositing", (x) => Demo(SourceOutCompositing)),
							new RootElement ("SourceOverCompositing", (x) => Demo (SourceOverCompositing)),
						}
					},
					new RootElement ("Distortions"){
						new Section () {
						}
					},
					new RootElement ("Generators"){
						new Section () {
							new RootElement ("CheckerboardGenerator", (x) => Demo (CheckerboardGenerator)),
							new RootElement ("ConstantColorGenerator", (x) => Demo (ConstantColorGenerator)),
							new RootElement ("StripesGenerator", (x) => Demo (StripesGenerator)),
						}
					},
					new RootElement ("Geometry Adjust"){
						new Section () {
							new RootElement ("AffineTransform", (x) => Demo (AffineTransform)),
							new RootElement ("Crop", (x) => Demo (Crop)),
							new RootElement ("StraightenFilter", (x) => Demo (StraightenFilter)),
						}
					},
					new RootElement ("Gradients"){
						new Section () {
							new RootElement ("GaussianGradient", (x) => Demo (GaussianGradient)),
							new RootElement ("LinearGradient", (x) =>Demo(LinearGradient)),
							new RootElement ("RadialGradient", (x) => Demo (RadialGradient)),
						}
					},
					new RootElement ("Stylize"){
						new Section () {
							new RootElement ("HighlightShadowAdjust", (x) => Demo (HighlightShadowAdjust)),
						}
					},
					new RootElement ("Vignette", (x) => Demo (Vignette))
				}
			};					
			
			window = new UIWindow (UIScreen.MainScreen.Bounds) {
				RootViewController = new UINavigationController (new DialogViewController (root))
			};
			window.MakeKeyAndVisible ();

			return true;
		}
		#endregion
		
		#region Helper Methods
		// 
		// Utility function used by pure-output generation filters
		//
		public CIImage Crop (CIFilter input)
		{
			return new CICrop () { 
				Image = input.OutputImage,
				Rectangle = new CIVector (0, 0, window.Bounds.Width, window.Bounds.Height) 
			}.OutputImage;			
		}
		
		public delegate CIImage ImageFilter ();
		
		public UIViewController Demo (ImageFilter makeDemo)
		{
			var v = new UIViewController ();
			var imageView = new UIImageView (v.View.Bounds);
			v.View.AddSubview (imageView);
			
			var output = makeDemo ();
			var context = CIContext.FromOptions (null);
			var result = context.CreateCGImage (output, output.Extent);
			imageView.Image = UIImage.FromImage (result);
			return v;
		}	
		#endregion
		
		#region Filter Methods
		#region CICategoryColorAdjustment
		/// <summary>
		/// Multiplies source color values and adds a bias factor to each color component.
		/// </summary>
		/// <returns>
		/// The altered Image
		/// </returns>
		[Filter]
		public CIImage ColorMatrix()
		{
			var rVector = new CIVector (.5F, 0F, 0F); // Multiple the Red Values by .5 (s.r = dot(s, rVector))
			var gVector = new CIVector (0F, 1.5F, 0F); // Multiple the Green Vector by 1.5 (s.g = dot(s, gVector))
			var bVector = new CIVector (0F, 0F, .75F); // Multiple the Blue Vectoer by .75 (s.b = dot(s, bVector))
			var aVector = new CIVector (0F, 0F, 0F, 1.25F); // Multiple the Alpha values by 1.25 (s.a = dot(s, bVector))
			var biasVector = new CIVector (0, 1, 0, 0); // A Bias to be Added to each Color Vector (s = s + bias)
			
			var colorMatrix = new CIColorMatrix ()
			{
				Image = flower,
				RVector = rVector,
				GVector = gVector,
				BVector = bVector,
				AVector = aVector,
				BiasVector = biasVector
			};
			
			return colorMatrix.OutputImage;
		}
		
		/// <summary>
		/// Adjusts saturation, brightness, and contrast values.
		/// </summary>
		/// <returns>
		/// Altered Image
		/// </returns>
		[Filter]
		public CIImage ColorControls ()
		{
			var colorCtrls = new CIColorControls (){
				Image = flower,
				Brightness = .5F,
				Saturation = 1.2F,
				Contrast = 3.1F
			};
			
			return colorCtrls.OutputImage;
		}
		
		/// <summary>
		/// Changes the overall hue, or tint, of the source pixels.
		/// </summary>
		/// <returns>
		/// The Altered Image.
		/// </returns>
		[Filter]
		public CIImage HueAdjust()
		{
			var hueAdjust = new CIHueAdjust()
			{
				Image = flower,
				Angle = 1F // Default is 0
			};
			
			return hueAdjust.OutputImage;
		}
		
		/// <summary>
		/// Adapts the reference white point for an image.
		/// </summary>
		/// <returns>
		/// The Color Adjusted Image
		/// </returns>
		[Filter]
		public CIImage TemperatureAndTint()
		{
			var temperatureAdjust = new CITemperatureAndTint()
			{
				Image = flower,
				Neutral = new CIVector(6500, 0), // Default [6500, 0]
				TargetNeutral = new CIVector(4000, 0), // Default [6500, 0]
			};
			
			return temperatureAdjust.OutputImage;
		}
		
		/// <summary>
		/// Adjusts tone response of the R, G, and B channels of an image.
		/// </summary>
		/// <returns>
		/// The adjusted image
		/// </returns>
		[Filter]
		public CIImage ToneCurve()
		{
			var point0 = new CIVector(0,0); // Default [0 0]
			var point1 = new CIVector(.1F, .5F); // Default [.25 .25]
			var point2 = new CIVector(.3F, .15F); // Default [.3 .15]
			var point3 = new CIVector(.6F, .6F); // Default [.75 .75]
			var point4 = new CIVector(1.1F, 1F); // Default [1 1]
			
			var toneCurve = new CIToneCurve()
			{
				Image = flower,
				Point0 = point0,
				Point1 = point1,
				Point2 = point2,
				Point3 = point3,
				Point4 = point4,
			};
			
			return toneCurve.OutputImage;
		}
		
		/// <summary>
		/// Adjusts the saturation of an image while keeping pleasing skin tones.
		/// </summary>
		[Filter]
		public CIImage Vibrance()
		{
			var vibrance = new CIVibrance()
			{
				Image = flower,
				Amount = -1.0F // Default 0
			};
			
			return vibrance.OutputImage;
		}
		
		/// <summary>
		/// Add a reduction of an image's brightness or saturation at the periphery compared to the image center.
		/// </summary>
		[Filter]
		public CIImage Vignette()
		{
			var vignette = new CIVignette()
			{
				Image = flower,
				Intensity = 2F,
				Radius = 10F,
			};
			
			return vignette.OutputImage;
		}
		
		/// <summary>
		/// Adjusts the reference white point for an image and maps all colors in the source using the new reference.
		/// </summary>
		/// <returns>
		/// The Color Adjusted Image
		/// </returns>
		public CIImage WhitePointAdjust()
		{
			var whitePointAdjust = new CIWhitePointAdjust()
			{
				Image = flower,
				Color = new CIColor(new CGColor(255F, 0, 187F)) // A Magenta Color
			};
			
			return whitePointAdjust.OutputImage;
		}
		#endregion
		
		#region CICategoryColorEffect
		
		/// <summary>
		/// Applies a Sepia Filter to an Image
		/// </summary>
		/// <returns>
		/// Image with Sepia Filter
		/// </returns>
		[Filter]
		public CIImage SepiaTone ()
		{
			var sepia = new CISepiaTone () {
				Image = flower,
				Intensity = .8f
			};
			return sepia.OutputImage;
		}
			
		/// <summary>
		/// Uses a three-dimensional color table to transform the source image pixels.
		/// </summary>
		/// <returns>
		/// The Altered Image
		/// </returns>
		[Filter]
		public CIImage ColorCube ()
		{
			var data = new NSData ();
			
			var cube = new CIColorCube ()
			{
				Image = flower,
				CubeDimension = Convert.ToSingle (Math.Pow (2F, 2F)),
			//CubeData = 
			};
			
			return cube.OutputImage;
		}
		
		/// <summary>
		/// Inverts the colors in an image.
		/// </summary>
		/// <returns>
		/// The Altered Image
		/// </returns>
		public CIImage ColorInvert ()
		{
			var invert = new CIColorInvert ()
			{
				Image = flower
			};
			
			return invert.OutputImage;
		}
		
		/// <summary>
		/// Remaps colors so they fall within shades of a single color.
		/// </summary>
		/// <returns>
		/// The Altered Image
		/// </returns>
		[Filter]
		public CIImage ColorMonochrome ()
		{
			var inputColor = new CIColor (new CGColor (100F, 0F, 100F)); // Make it Purple R + B = Purple
			var monoChrome = new CIColorMonochrome ()
			{
				Image = flower,
				Color = inputColor,
				Intensity = 1F,
			};
			
			return monoChrome.OutputImage;
		}
		
		/// <summary>
		/// Maps luminance to a color ramp of two colors.
		/// </summary>
		/// <returns>
		/// The altered Image
		/// </returns>
		[Filter]
		public CIImage FalseColor ()
		{
			var color0 = new CIColor (new CGColor (255F, 251F, 0F)); // A Yellowish Color
			var color1 = new CIColor (new CGColor (51F, 0F, 255F)); // A Purplish Color
			var falseColor = new CIFalseColor ()
			{
				Image = flower,
				Color0 = color0,
				Color1 = color1
			};
			
			return falseColor.OutputImage;
		}
		
		/// <summary>
		/// Adjusts midtone brightness.
		/// </summary>
		/// <returns>
		/// Alters the image
		/// </returns>
		[Filter]
		public CIImage GammaAdjust ()
		{
			var gammaAdjust = new CIGammaAdjust ()
			{
				Image = flower,
				Power = 3F, // Default value: 0.75
			};
			
			return gammaAdjust.OutputImage;
		}
		#endregion
		
		#region CategoryGradient
		/// <summary>
		/// Generates a gradient that varies from one color to another using a Gaussian distribution.
		/// </summary>
		/// <returns>
		/// The gradient.
		/// </returns>
		[Filter]
		public CIImage GaussianGradient ()
		{
			var centerVector = new CIVector (100, 100); // Default is [150 150]
			var color1 = CIColor.FromRgba (1, 0, 1, 1);
			var color0 = CIColor.FromRgba (0, 1, 1, 1);
				
			var gaussGradient = new CIGaussianGradient ()
			{
				Center = centerVector,
				Color0 = color0,
				Color1 = color1,
				Radius = 280f // Default is 300
			};
			
			return Crop (gaussGradient);
		}
		
		/// <summary>
		/// Generates a gradient that varies along a linear axis between two defined endpoints.
		/// </summary>
		/// <returns>
		/// The gradient.
		/// </returns>
		[Filter]
		public CIImage LinearGradient()
		{
			var point0 = new CIVector(0, 0); // Default [0 0]
			var point1 = new CIVector(250, 250); // Default [200 200]
			var linearGrad = new CILinearGradient()
			{
				Point0 = point0,
				Point1 = point1,
				Color0 = new CIColor (UIColor.Red),
				Color1 = new CIColor (UIColor.Blue)
			};
			
			return Crop (linearGrad);
		}
		
		/// <summary>
		/// Generates a gradient that varies radially between two circles having the same center.
		/// </summary>
		/// <returns>
		/// The gradient.
		/// </returns>
		[Filter]
		public CIImage RadialGradient()
		{
			var center = new CIVector(100, 100); // Default [150 150]
			var radGradient = new CIRadialGradient()
			{
				Center = center,
				Radius0 = 10F, // Default 5
				Radius1 = 150F, // Default 100
				Color0 = new CIColor(new CGColor(0, 255F, 0)), // Green
				Color1 = new CIColor(new CGColor(0, 0, 0)) // Black
			};
			
			return Crop (radGradient);
		}
		#endregion
		
		#region CICategoryGeometryAdjustment
		/// <summary>
		/// Applies a crop to an image.
		/// </summary>
		[Filter]
		public CIImage Crop ()
		{
			var crop = new CICrop () {
				Image = flower,
				Rectangle = new CIVector (0, 0, 300, 300)
			};
			return crop.OutputImage;
		}
		
		/// <summary>
		/// Applies an affine transform to an image
		/// </summary>
		/// <returns>
		/// The Altered Image
		/// </returns>
		[Filter]
		public CIImage AffineTransform ()
		{
			var transform = new CGAffineTransform (1F, .5F, .5F, 1F, 0F, 0F);
			
			var affineTransform = new CIAffineTransform ()
			{
				Image = flower,
				Transform = transform
			};
			
			return affineTransform.OutputImage;
		}
		
		/// <summary>
		/// Adjusts the exposure setting for an image similar to the way you control exposure for a camera when you change the F-stop.
		/// </summary>
		/// <returns>
		/// The altered Image
		/// </returns>
		[Filter]
		public CIImage ExposureAdjust ()
		{
			var exposureAdjust = new CIExposureAdjust ()
			{
				Image = flower,
				EV = 2F // Default value: 0.50 Minimum: 0.00 Maximum: 0.00 Slider minimum: -10.00 Slider maximum: 10.00 Identity: 0.00
			};
			
			return exposureAdjust.OutputImage;
		}
		
		/// <summary>
		/// Rotates the source image by the specified angle in radians.
		/// </summary>
		/// <returns>
		/// The filtered Image
		/// </returns>
		[Filter]
		public CIImage StraightenFilter()
		{
			var straightFilter = new CIStraightenFilter()
			{
				Image = heron,
				Angle = Convert.ToSingle(Math.PI / 4.0) // Change by 45 degrees = pi/4 Radians.
			};
			
			return straightFilter.OutputImage;
		}
		
		
		#endregion
		
		#region CICategoryCompositeOperation
		/// <summary>
		/// Adds color components to achieve a brightening effect.
		/// </summary>
		/// <returns>
		/// The composite Image
		/// </returns>
		[Filter]
		public CIImage AdditionCompositing ()
		{
			var addComp = new CIAdditionCompositing ()
			{
				Image = heron,
				BackgroundImage = clouds,
			};
			
			return addComp.OutputImage;
		}
		
		/// <summary>
		/// Uses the luminance values of the background with the hue and saturation values of the source image.
		/// </summary>
		/// <returns>
		/// The composite image
		/// </returns>
		[Filter]
		public CIImage ColorBlendMode ()
		{
			var colorBlend = new CIColorBlendMode ()
			{
				Image = heron,
				BackgroundImage = clouds
			};
			
			return colorBlend.OutputImage;
		}
		
		/// <summary>
		/// Brightens the background image samples to reflect the source image samples.
		/// </summary>
		/// <returns>
		/// The composite Image
		/// </returns>
		public CIImage ColorDodgeBlendMode ()
		{
			var colorDodgeBlend = new CIColorDodgeBlendMode ()
			{
				Image = heron,
				BackgroundImage = clouds,
			};
			
			return colorDodgeBlend.OutputImage;
		}
		
		/// <summary>
		/// Subtracts either the source image sample color from the background image sample color, or the reverse, depending on which sample has the greater brightness value.
		/// </summary>
		/// <returns>
		/// The composite image.
		/// </returns>
		[Filter]
		public CIImage DifferenceBlendMode ()
		{
			var differenceBlend = new CIDifferenceBlendMode ()
			{
				Image = heron,
				BackgroundImage = clouds
			};
			
			return differenceBlend.OutputImage;
		}
		
		/// <summary>
		/// Produces an effect similar to that produced by the CIDifferenceBlendMode filter but with lower contrast.
		/// </summary>
		/// <returns>
		/// The composite Image
		/// </returns>
		[Filter]
		public CIImage ExclusionBlendMode ()
		{
			var exclusionBlend = new CIExclusionBlendMode ()
			{
				Image = heron,
				BackgroundImage = clouds
			};
			
			return exclusionBlend.OutputImage;
		}
		
		/// <summary>
		/// Either multiplies or screens colors, depending on the source image sample color.
		/// </summary>
		/// <returns>
		/// The composite image
		/// </returns>
		[Filter]
		public CIImage HardLightBlendMode ()
		{
			var hardLightBlend = new CIHardLightBlendMode ()
			{
				Image = heron,
				BackgroundImage = clouds
			};
			
			return hardLightBlend.OutputImage;
		}
		
		/// <summary>
		/// Uses the luminance and saturation values of the background with the hue of the source image.
		/// </summary>
		/// <returns>
		/// The composite Image
		/// </returns>
		[Filter]
		public CIImage HueBlendMode()
		{
			var hueBlend = new CIHueBlendMode()
			{
				Image = heron,
				BackgroundImage = clouds
			};
			
			return hueBlend.OutputImage;
		}
		
		/// <summary>
		/// Creates composite image samples by choosing the lighter samples (either from the source image or the background).
		/// </summary>
		/// <returns>
		/// The composite Image
		/// </returns>
		[Filter]
		public CIImage LightenBlendMode()
		{
			var lightenBlend = new CILightenBlendMode()
			{
				Image = heron,
				BackgroundImage = clouds
			};
			
			return lightenBlend.OutputImage;
		}
		
		/// <summary>
		/// Uses the hue and saturation of the background with the luminance of the source image.
		/// </summary>
		/// <returns>
		/// The composite image
		/// </returns>
		[Filter]
		public CIImage LuminosityBlendMode()
		{
			var luminosityBlend = new CILuminosityBlendMode()
			{
				Image = heron,
				BackgroundImage = clouds
			};
			
			return luminosityBlend.OutputImage;
		}
		
		/// <summary>
		/// Computes the maximum value, by color component, of two input images and creates an output image using the maximum values.
		/// </summary>
		/// <returns>
		/// The composite image.
		/// </returns>
		[Filter]
		public CIImage MaximumCompositing()
		{
			var maxComposite = new CIMaximumCompositing()
			{
				Image = heron,
				BackgroundImage = clouds
			};
			
			return maxComposite.OutputImage;
		}

		/// <summary>
		/// Computes the minimum value, by color component, of two input images and creates an output image using the minimum values.
		/// </summary>
		/// <returns>
		/// The composite image
		/// </returns>
		[Filter]
		public CIImage MinimumCompositing()
		{
			var minComposite = new CIMinimumCompositing()
			{
				Image = heron,
				BackgroundImage = clouds
			};
			
			return minComposite.OutputImage;
		}
		
		/// <summary>
		/// Multiplies the source image samples with the background image samples.
		/// </summary>
		/// <returns>
		/// The composite image
		/// </returns>
		[Filter]
		public CIImage MultiplyBlendMode()
		{
			var multiBlend = new CIMultiplyBlendMode()
			{
				Image = heron,
				BackgroundImage = clouds
			};
			
			return multiBlend.OutputImage;
		}
		
		/// <summary>
		/// Multiplies the color component of two input images and creates an output image using the multiplied values.
		/// </summary>
		/// <returns>
		/// The composite image
		/// </returns>
		[Filter]
		public CIImage MultiplyCompositing()
		{
			var multiComposite = new CIMultiplyCompositing()
			{
				Image = heron,
				BackgroundImage = clouds
			};
			
			return multiComposite.OutputImage;
		}
		
		/// <summary>
		/// Overlaies the blend mode.
		/// </summary>
		/// <returns>
		/// The blend mode.
		/// </returns>
		[Filter]
		public CIImage OverlayBlendMode()
		{
			var overlayBlend = new CIOverlayBlendMode()
			{
				Image = heron,
				BackgroundImage = clouds
			};
			
			return overlayBlend.OutputImage;
		}
		
		/// <summary>
		/// Saturations the blend mode.
		/// </summary>
		/// <returns>
		/// The composite image.
		/// </returns>
		[Filter]
		public CIImage SaturationBlendMode()
		{
			var saturationBlend = new CISaturationBlendMode()
			{
				Image = heron, 
				BackgroundImage = clouds,
			};
			
			return saturationBlend.OutputImage;
		}
		
		/// <summary>
		/// Multiplies the inverse of the source image samples with the inverse of the background image samples.
		/// </summary>
		/// <returns>
		/// The composite image
		/// </returns>
		[Filter]
		public CIImage ScreenBlendMode()
		{
			var screenBlend = new CIScreenBlendMode()
			{
				Image = heron,
				BackgroundImage = clouds
			};
			
			return screenBlend.OutputImage;
		}
	
		/// <summary>
		/// Either darkens or lightens colors, depending on the source image sample color.
		/// </summary>
		/// <returns>
		/// The Composite Image.
		/// </returns>
		[Filter]
		public CIImage SoftLightBlendMode()
		{
			var softLightBlend = new CISoftLightBlendMode()
			{
				Image = heron,
				BackgroundImage = clouds
			};
			
			return softLightBlend.OutputImage;
		}
		
		/// <summary>
		/// Places the source image over the background image, then uses the luminance of the background image to determine what to show.
		/// </summary>
		/// <returns>
		/// The Composite Image
		/// </returns>
		[Filter]
		public CIImage SourceAtopCompositing()
		{
			var sourceAtopComposite = new CISourceAtopCompositing()
			{
				Image = heron,
				BackgroundImage = clouds,
			};
			
			return sourceAtopComposite.OutputImage;
		}
		
		/// <summary>
		/// Uses the second image to define what to leave in the source image, effectively cropping the image.
		/// </summary>
		/// <returns>
		/// The composite image
		/// </returns>
		[Filter]
		public CIImage SourceInCompositing()
		{
			var sourceComposite = new CISourceInCompositing()
			{
				Image = clouds, // This image will be Cropped
				BackgroundImage = heron
			};
			
			return sourceComposite.OutputImage;
		}
		
		/// <summary>
		/// Uses the second image to define what to take out of the first image.
		/// </summary>
		/// <returns>
		/// The composite image
		/// </returns>
		[Filter]
		public CIImage SourceOutCompositing()
		{
			var sourceOutComposite = new CISourceOutCompositing()
			{
				Image = clouds, // This Image will be Cropped
				BackgroundImage = heron
			};
			
			return sourceOutComposite.OutputImage;
		}
		
		/// <summary>
		/// Places the second image over the first.
		/// </summary>
		/// <returns>
		/// The composite Image
		/// </returns>
		[Filter]
		public CIImage SourceOverCompositing()
		{
			var sourceOverComposite = new CISourceOverCompositing()
			{
				Image = heron,
				BackgroundImage = clouds
			};
			
			return sourceOverComposite.OutputImage;
		}
		#endregion
		
		#region CICategoryGenerator
		/// <summary>
		/// Generates a checkerboard pattern.
		/// </summary>
		/// <returns>
		/// An Image of a Checkboard pattern
		/// </returns>
		[Filter]
		public CIImage CheckerboardGenerator ()
		{
			// Color 1 
			var c0 = CIColor.FromRgb (1, 0, 0);
			var c1 = CIColor.FromRgb (0, 1, 0);
			var checker = new CICheckerboardGenerator ()
			{
				Color0 = c0,
				Color1 = c1,
				Center = new CIVector (new float[] { 10 , 10 }), // Default [80 80]
				Sharpness = 1F // Default 1
			};
			
			return Crop (checker);
		}
		
		/// <summary>
		/// Generates a solid color.
		/// </summary>
		/// <returns>
		/// A Solid Color Image
		/// </returns>
		[Filter]
		public CIImage ConstantColorGenerator ()
		{
			var colorGen = new CIConstantColorGenerator ()
			{
				Color = new CIColor (UIColor.Blue)
			};
			
			return Crop (colorGen);
		}
		
		/// <summary>
		/// Generates a stripe pattern.
		/// </summary>
		/// <returns>
		/// The generated pattern.
		/// </returns>
		[Filter]
		public CIImage StripesGenerator()
		{
			var stripeGen = new CIStripesGenerator()
			{
				Center = new CIVector(150, 100), // Default [150 150]
				Color0 = new CIColor (UIColor.Blue),
				Color1 = new CIColor (UIColor.Red),
				Width = 10,
			};
			
			return Crop (stripeGen);
		}
		#endregion
	
		#region CICategoryStylize
		/// <summary>
		/// Adjust the tonal mapping of an image while preserving spatial detail.
		/// </summary>
		/// <returns>
		/// The altered Image
		/// </returns>
		[Filter]
		public CIImage HighlightShadowAdjust ()
		{
			var shadowAdjust = new CIHighlightShadowAdjust ()
			{
				Image = flower,
				HighlightAmount = .75F, // Default is 1
				ShadowAmount = 1.5F // Default is 0
			};
			
			return shadowAdjust.OutputImage;
		}
		#endregion
		#endregion
	}

}

