using System;
using Foundation;
using UIKit;
using CoreImage;
using ObjCRuntime;

using System.Collections.Generic;
using MonoTouch.Dialog;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using CoreGraphics;
using System.IO;
using System.Threading.Tasks;

namespace coreimage
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		/// <summary>
		/// "Flower" © 2012 Milica Sekulic, used under a Creative Commons Attribution-ShareAlike license: http://creativecommons.org/licenses/by-sa/3.0/ 
		/// </summary>
		CIImage flower = CIImage.FromCGImage (UIImage.FromFile ("flower.png").CGImage);
		
		/// <summary>
		/// "Sunrise near Atkeison Plateau" © 2012 Charles Atkeison, used under a Creative Commons Attribution-ShareAlike license: http://creativecommons.org/licenses/by-sa/3.0/ 
		/// </summary>
		CIImage clouds = CIImage.FromCGImage (UIImage.FromFile ("clouds.jpg").CGImage);
		
		/// <summary>
		/// "canon" © 2012 cuatrok77 hernandez, used under a Creative Commons Attribution-ShareAlike license: http://creativecommons.org/licenses/by-sa/3.0/ 
		/// </summary>
		CIImage heron = CIImage.FromCGImage (UIImage.FromFile ("heron.jpg").CGImage);

		CIImage xamarin = CIImage.FromCGImage (UIImage.FromFile ("Xamarin.png").CGImage);

		CIImage xamarinAlpha = CIImage.FromCGImage (UIImage.FromFile ("XamarinAlpha.png").CGImage);

		CIImage xamarinCheck = CIImage.FromCGImage (UIImage.FromFile ("XamarinCheck.png").CGImage);

		UIWindow window;

		#region UIApplicationDelegate Methods

		void AddVersionRoot (Section byIOSSection, string versionString, byte majorVersion)
		{
			RootElement iOSRoot = null;

			foreach (string section in sectionList) {
				var query = masterList.Where (fi => fi.MajorVersion == majorVersion && fi.SectionName == section);
				if (query.Any ()) {
					if (iOSRoot == null) {
						iOSRoot = new RootElement (versionString);
						byIOSSection.Add (iOSRoot);
					}

					var catSection = new Section (section);
					iOSRoot.Add (catSection);
					var elementList = query.Select (fi => (Element)new RootElement (fi.Name, (x) => {
						var viewCtrl = Demo (fi.Callback);
						viewCtrl.Title = fi.Name;
						return viewCtrl;
					}));
					catSection.AddAll (elementList);
				}
			}
		}
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			app.StatusBarHidden = true;
			 
			InitList ();

			var root = new RootElement ("Effects");
			var byIOSSection = new Section ("By iOS Version");
			root.Add (byIOSSection);

			AddVersionRoot (byIOSSection, "iOS 7", 7);
			AddVersionRoot (byIOSSection, "iOS 6", 6);
			AddVersionRoot (byIOSSection, "iOS 5", 5);

			var byCategorySection = new Section ("By Category");
			root.Add (byCategorySection);

			foreach (string section in sectionList) {
				var query = masterList.Where (fi => fi.SectionName == section);
				if (query.Any ()) {
					var catRoot = new RootElement (section);
					byCategorySection.Add (catRoot);

					var catSection = new Section ();
					catRoot.Add (catSection);

					var elementList = query.Select (fi => (Element)new RootElement (fi.Name, (x) => {
						var viewCtrl = Demo (fi.Callback);
						viewCtrl.Title = fi.Name;
						return viewCtrl;
					}));
					catSection.AddAll (elementList);
				}
			}

			var testingSection = new Section ("Testing");
			root.Add (testingSection);

			testingSection.Add (new RootElement("Show All Filters", (x) => new VisitFilterViewController (masterList)));

			window = new UIWindow (UIScreen.MainScreen.Bounds) {
				RootViewController = new UINavigationController (new DialogViewController (root))
			};
			window.MakeKeyAndVisible ();

			return true;
		}
		#endregion

		string [] sectionList = new [] {
			"Blur",
			"Color Adjustment",
			"Color Effect",
			"Composite Operation",
			"Distortions",
			"Generators",
			"Geometry Adjust",
			"Gradients",
			"Halftone Effect",
			"Sharpen", 
			"Stylize",
			"Tile Effect",
			"Transition",
		};
		FilterHolder[] masterList;

		void InitList ()
		{
			masterList = new [] {
				// Blur
				new FilterHolder ("GaussianBlur", sectionList[0], 6, typeof (CIGaussianBlur), GaussianBlur),

				// Color Adjustment
				new FilterHolder ("ColorClamp", sectionList[1], 7, typeof (CIColorClamp), ColorClamp),
				new FilterHolder ("ColorControls", sectionList[1], 5, typeof (CIColorControls), ColorControls),
				new FilterHolder ("ColorMatrix", sectionList[1], 5, typeof (CIColorMatrix), ColorMatrix),
				new FilterHolder ("ColorPolynomial", sectionList[1], 7, typeof (CIColorPolynomial), ColorPolynomial),
				new FilterHolder ("ExposureAdjust", sectionList[1], 5, typeof (CIExposureAdjust), ExposureAdjust),
				new FilterHolder ("GammaAdjust", sectionList[1], 5, typeof (CIGammaAdjust), GammaAdjust),
				new FilterHolder ("HueAdjust", sectionList[1], 5, typeof (CIHueAdjust), HueAdjust),
				new FilterHolder ("LinearToSRGBToneCurve", sectionList[1], 7, typeof (CILinearToSRGBToneCurve), LinearToSRGBToneCurve),
				new FilterHolder ("SRGBToneCurveToLinear", sectionList[1], 7, typeof (CISRGBToneCurveToLinear), SRGBToneCurveToLinear),
				new FilterHolder ("TemperatureAndTint", sectionList[1], 5, typeof (CITemperatureAndTint), TemperatureAndTint),
				new FilterHolder ("ToneCurve", sectionList[1], 5, typeof (CIToneCurve), ToneCurve),
				new FilterHolder ("Vibrance", sectionList[1], 5, typeof (CIVibrance), Vibrance),
				new FilterHolder ("WhitePointAdjust", sectionList[1], 5, typeof (CIWhitePointAdjust), WhitePointAdjust),

				// Color Effect
				new FilterHolder ("ColorCrossPolynomial", sectionList[2], 7, typeof (CIColorCrossPolynomial), ColorCrossPolynomial),
				new FilterHolder ("ColorCube", sectionList[2], 5, typeof (CIColorCube), ColorCube),
				new FilterHolder ("ColorCubeWithColorSpace", sectionList[2], 7, typeof (CIColorCubeWithColorSpace), ColorCubeWithColorSpace),
				new FilterHolder ("ColorInvert", sectionList[2], 5, typeof (CIColorInvert), ColorInvert),
				new FilterHolder ("ColorMap", sectionList[2], 6, typeof (CIColorMap), ColorMap),
				new FilterHolder ("ColorMonochrome", sectionList[2], 5, typeof (CIColorMonochrome), ColorMonochrome),
				new FilterHolder ("ColorPosterize", sectionList[2], 6, typeof (CIColorPosterize), ColorPosterize),
				new FilterHolder ("FalseColor", sectionList[2], 5, typeof (CIFalseColor), FalseColor),
				new FilterHolder ("MaskToAlpha", sectionList[2], 6, typeof (CIMaskToAlpha), MaskToAlpha),
				new FilterHolder ("MaximumComponent", sectionList[2], 6, typeof (CIMaximumComponent), MaximumComponent),
				new FilterHolder ("MinimumComponent", sectionList[2], 6, typeof (CIMinimumComponent), MinimumComponent),
				new FilterHolder ("PhotoEffectChrome", sectionList[2], 7, typeof (CIPhotoEffectChrome), PhotoEffectChrome),
				new FilterHolder ("PhotoEffectFade", sectionList[2], 7, typeof (CIPhotoEffectFade), PhotoEffectFade),
				new FilterHolder ("PhotoEffectInstant", sectionList[2], 7, typeof (CIPhotoEffectInstant), PhotoEffectInstant),
				new FilterHolder ("PhotoEffectMono", sectionList[2], 7, typeof (CIPhotoEffectMono), PhotoEffectMono),
				new FilterHolder ("PhotoEffectNoir", sectionList[2], 7, typeof (CIPhotoEffectNoir), PhotoEffectNoir),
				new FilterHolder ("PhotoEffectProcess", sectionList[2], 7, typeof (CIPhotoEffectProcess), PhotoEffectProcess),
				new FilterHolder ("PhotoEffectTonal", sectionList[2], 7, typeof (CIPhotoEffectTonal), PhotoEffectTonal),
				new FilterHolder ("PhotoEffectTransfer", sectionList[2], 7, typeof (CIPhotoEffectTransfer), PhotoEffectTransfer),
				new FilterHolder ("SepiaTone", sectionList[2], 5, typeof (CISepiaTone), SepiaTone),
				new FilterHolder ("Vignette", sectionList[2], 5, typeof (CIVignette), Vignette),
				new FilterHolder ("VignetteEffect", sectionList[2], 7, typeof (CIVignetteEffect), VignetteEffect),

				// Composite Operation
				new FilterHolder ("AdditionCompositing", sectionList[3], 5, typeof (CIAdditionCompositing), AdditionCompositing),
				new FilterHolder ("ColorBlendMode", sectionList[3], 5, typeof (CIColorBlendMode), ColorBlendMode),
				new FilterHolder ("ColorBurnBlendMode", sectionList[3], 5, typeof (CIColorBurnBlendMode), ColorBurnBlendMode),
				new FilterHolder ("ColorDodgeBlendMode", sectionList[3], 5, typeof (CIColorDodgeBlendMode), ColorDodgeBlendMode),
				new FilterHolder ("DarkenBlendMode", sectionList[3], 5, typeof (CIDarkenBlendMode), DarkenBlendMode),
				new FilterHolder ("DifferenceBlendMode", sectionList[3], 5, typeof (CIDifferenceBlendMode), DifferenceBlendMode),
				new FilterHolder ("ExclusionBlendMode", sectionList[3], 5, typeof (CIExclusionBlendMode), ExclusionBlendMode),
				new FilterHolder ("HardLightBlendMode", sectionList[3], 5, typeof (CIHardLightBlendMode), HardLightBlendMode),
				new FilterHolder ("HueBlendMode", sectionList[3], 5, typeof (CIHueBlendMode), HueBlendMode),
				new FilterHolder ("LightenBlendMode", sectionList[3], 5, typeof (CILightenBlendMode), LightenBlendMode),
				new FilterHolder ("LuminosityBlendMode", sectionList[3], 5, typeof (CILuminosityBlendMode), LuminosityBlendMode),
				new FilterHolder ("MaximumCompositing", sectionList[3], 5, typeof (CIMaximumCompositing), MaximumCompositing),
				new FilterHolder ("MinimumCompositing", sectionList[3], 5, typeof (CIMinimumCompositing), MinimumCompositing),
				new FilterHolder ("MultiplyBlendMode", sectionList[3], 5, typeof (CIMultiplyBlendMode), MultiplyBlendMode),
				new FilterHolder ("MultiplyCompositing", sectionList[3], 5, typeof (CIMultiplyCompositing), MultiplyCompositing),
				new FilterHolder ("OverlayBlendMode", sectionList[3], 5, typeof (CIOverlayBlendMode), OverlayBlendMode),
				new FilterHolder ("SaturationBlendMode", sectionList[3], 5, typeof (CISaturationBlendMode), SaturationBlendMode),
				new FilterHolder ("ScreenBlendMode", sectionList[3], 5, typeof (CIScreenBlendMode), ScreenBlendMode),
				new FilterHolder ("SoftLightBlendMode", sectionList[3], 5, typeof (CISoftLightBlendMode), SoftLightBlendMode),
				new FilterHolder ("SourceAtopCompositing", sectionList[3], 5, typeof (CISourceAtopCompositing), SourceAtopCompositing),
				new FilterHolder ("SourceInCompositing", sectionList[3], 5, typeof (CISourceInCompositing), SourceInCompositing),
				new FilterHolder ("SourceOutCompositing", sectionList[3], 5, typeof (CISourceOutCompositing), SourceOutCompositing),
				new FilterHolder ("SourceOverCompositing", sectionList[3], 5, typeof (CISourceOverCompositing), SourceOverCompositing),

				// Distortions
				new FilterHolder ("BumpDistortion", sectionList[4], 6, typeof (CIBumpDistortion), BumpDistortion),
				new FilterHolder ("BumpDistortionLinear", sectionList[4], 6, typeof (CIBumpDistortionLinear), BumpDistortionLinear),
				new FilterHolder ("CircleSplashDistortion", sectionList[4], 6, typeof (CICircleSplashDistortion), CircleSplashDistortion),
				new FilterHolder ("HoleDistortion", sectionList[4], 6, typeof (CIHoleDistortion), HoleDistortion),
				new FilterHolder ("LightTunnel", sectionList[4], 6, typeof (CILightTunnel), LightTunnel),
				new FilterHolder ("PinchDistortion", sectionList[4], 6, typeof (CIPinchDistortion), PinchDistortion),
				new FilterHolder ("TwirlDistortion", sectionList[4], 6, typeof (CITwirlDistortion), TwirlDistortion),
				new FilterHolder ("VortexDistortion", sectionList[4], 6, typeof (CIVortexDistortion), VortexDistortion),

				// Generators
				new FilterHolder ("CheckerboardGenerator", sectionList[5], 5, typeof (CICheckerboardGenerator), CheckerboardGenerator),
				new FilterHolder ("ConstantColorGenerator", sectionList[5], 5, typeof (CIConstantColorGenerator), ConstantColorGenerator),
				new FilterHolder ("QRCodeGenerator", sectionList[5], 7, typeof (CIQRCodeGenerator), QRCodeGenerator),
				new FilterHolder ("RandomGenerator", sectionList[5], 6, typeof (CIRandomGenerator), RandomGenerator),
				new FilterHolder ("StarShineGenerator", sectionList[5], 6, typeof (CIStarShineGenerator), StarShineGenerator),
				new FilterHolder ("StripesGenerator", sectionList[5], 5, typeof (CIStripesGenerator), StripesGenerator),

				// Geometry Adjust
				new FilterHolder ("AffineTransform", sectionList[6], 5, typeof (CIAffineTransform), AffineTransform),
				new FilterHolder ("Crop", sectionList[6], 5, typeof (CICrop), Crop),
				new FilterHolder ("LanczosScaleTransform", sectionList[6], 6, typeof (CILanczosScaleTransform), LanczosScaleTransform),
				new FilterHolder ("PerspectiveTransform", sectionList[6], 6, typeof (CIPerspectiveTransform), PerspectiveTransform),
				new FilterHolder ("PerspectiveTransformWithExtent", sectionList[6], 6, typeof (CIPerspectiveTransformWithExtent), PerspectiveTransformWithExtent),
				new FilterHolder ("StraightenFilter", sectionList[6], 5, typeof (CIStraightenFilter), StraightenFilter),

				// Gradients
				new FilterHolder ("GaussianGradient", sectionList[7], 5, typeof (CIGaussianGradient), GaussianGradient),
				new FilterHolder ("LinearGradient", sectionList[7], 5, typeof (CILinearGradient), LinearGradient),
				new FilterHolder ("RadialGradient", sectionList[7], 5, typeof (CIRadialGradient), RadialGradient),
				new FilterHolder ("SmoothLinearGradient", sectionList[7], 6, typeof (CISmoothLinearGradient), SmoothLinearGradient),

				// Halftone Effect
				new FilterHolder ("CircularScreen", sectionList[8], 6, typeof (CICircularScreen), CircularScreen),
				new FilterHolder ("DotScreen", sectionList[8], 6, typeof (CIDotScreen), DotScreen),
				new FilterHolder ("HatchedScreen", sectionList[8], 6, typeof (CIHatchedScreen), HatchedScreen),
				new FilterHolder ("LineScreen", sectionList[8], 6, typeof (CILineScreen), LineScreen),

				// Sharpen
				new FilterHolder ("SharpenLuminance", sectionList[9], 6, typeof (CISharpenLuminance), SharpenLuminance),
				new FilterHolder ("UnsharpMask", sectionList[9], 6, typeof (CIUnsharpMask), UnsharpMask),

				// Stylize
				new FilterHolder ("BlendWithAlphaMask", sectionList[10], 7, typeof (CIBlendWithAlphaMask), BlendWithAlphaMask),
				new FilterHolder ("BlendWithMask", sectionList[10], 6, typeof (CIBlendWithMask), BlendWithMask),
				new FilterHolder ("Bloom", sectionList[10], 6, typeof (CIBloom), Bloom),
				new FilterHolder ("Convolution3X3", sectionList[10], 7, typeof (CIConvolution3X3), Convolution3X3),
				new FilterHolder ("Convolution5X5", sectionList[10], 7, typeof (CIConvolution5X5), Convolution5X5),
				new FilterHolder ("Convolution9Horizontal", sectionList[10], 7, typeof (CIConvolution9Horizontal), Convolution9Horizontal),
				new FilterHolder ("Convolution9Vertical", sectionList[10], 7, typeof (CIConvolution9Vertical), Convolution9Vertical),
				new FilterHolder ("Gloom", sectionList[10], 6, typeof (CIGloom), Gloom),
				new FilterHolder ("HighlightShadowAdjust", sectionList[10], 5, typeof (CIHighlightShadowAdjust), HighlightShadowAdjust),
				new FilterHolder ("Pixellate", sectionList[10], 6, typeof (CIPixellate), Pixellate),

				// Tile Effect
				new FilterHolder ("AffineClamp", sectionList[11], 6, typeof (CIAffineClamp), AffineClamp),
				new FilterHolder ("AffineTile", sectionList[11], 6, typeof (CIAffineTile), AffineTile),
				new FilterHolder ("EightfoldReflectedTile", sectionList[11], 6, typeof (CIEightfoldReflectedTile), EightfoldReflectedTile),
				new FilterHolder ("FourfoldReflectedTile", sectionList[11], 6, typeof (CIFourfoldReflectedTile), FourfoldReflectedTile),
				new FilterHolder ("FourfoldRotatedTile", sectionList[11], 6, typeof (CIFourfoldRotatedTile), FourfoldRotatedTile),
				new FilterHolder ("FourfoldTranslatedTile", sectionList[11], 6, typeof (CIFourfoldTranslatedTile), FourfoldTranslatedTile),
				new FilterHolder ("GlideReflectedTile", sectionList[11], 6, typeof (CIGlideReflectedTile), GlideReflectedTile),
				new FilterHolder ("PerspectiveTile", sectionList[11], 6, typeof (CIPerspectiveTile), PerspectiveTile),
				new FilterHolder ("SixfoldReflectedTile", sectionList[11], 6, typeof (CISixfoldReflectedTile), SixfoldReflectedTile),
				new FilterHolder ("SixfoldRotatedTile", sectionList[11], 6, typeof (CISixfoldRotatedTile), SixfoldRotatedTile),
				new FilterHolder ("TriangleKaleidoscope", sectionList[11], 6, typeof (CITriangleKaleidoscope), TriangleKaleidoscope),
				new FilterHolder ("TwelvefoldReflectedTile", sectionList[11], 6, typeof (CITwelvefoldReflectedTile), TwelvefoldReflectedTile),

				// Transition
				new FilterHolder ("BarsSwipeTransition", sectionList[12], 6, typeof (CIBarsSwipeTransition), BarsSwipeTransition),
				new FilterHolder ("CopyMachineTransition", sectionList[12], 6, typeof (CICopyMachineTransition), CopyMachineTransition),
				new FilterHolder ("DisintegrateWithMaskTransition", sectionList[12], 6, typeof (CIDisintegrateWithMaskTransition), DisintegrateWithMaskTransition),
				new FilterHolder ("DissolveTransition", sectionList[12], 6, typeof (CIDissolveTransition), DissolveTransition),
				new FilterHolder ("FlashTransition", sectionList[12], 6, typeof (CIFlashTransition), FlashTransition),
				new FilterHolder ("ModTransition", sectionList[12], 6, typeof (CIModTransition), ModTransition),
				new FilterHolder ("SwipeTransition", sectionList[12], 6, typeof (CISwipeTransition), SwipeTransition),
			};  

			int maxVer = 5;
			while (UIDevice.CurrentDevice.CheckSystemVersion (++maxVer, 0));

			masterList = masterList.Where (l => l.MajorVersion < maxVer).ToArray ();
		}           
		
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

		public UIViewController Demo (Func<CIImage> makeDemo)
		{
			var v = new UIViewController ();
			if (UIDevice.CurrentDevice.CheckSystemVersion (7, 0))
				v.EdgesForExtendedLayout = UIRectEdge.None;
			var imageView = new UIImageView (v.View.Bounds);
			imageView.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;
			v.View.AutosizesSubviews = true;
			v.View.AddSubview (imageView);
			
			var output = makeDemo ();
			var context = CIContext.FromOptions (null);
			var result = context.CreateCGImage (output, output.Extent);
			var resultImage = UIImage.FromImage (result);
			imageView.ContentMode = UIViewContentMode.ScaleAspectFit;
			imageView.Image = resultImage;
			return v;
		}

		#endregion
		
		#region Filter Methods

		#region CICategoryBlur

		/// <summary>
		/// Applies a Gaussian blur.
		/// </summary>
		/// <returns>The altered image.</returns>
		CIImage GaussianBlur ()
		{
			var gaussian_blur = new CIGaussianBlur ()
			{
				Image = clouds,
				Radius = 3f,
			};

			return gaussian_blur.OutputImage;
		}

		#endregion


		#region CICategoryColorAdjustment
		/// <summary>
		/// Multiplies source color values and adds a bias factor to each color component.
		/// </summary>
		/// <returns>
		/// The altered Image
		/// </returns>
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
		/// Applies a Color Polynomial to each pixel of of an image.
		/// </summary>
		/// <returns>The altered image.</returns>
		public CIImage ColorPolynomial()
		{
			var color_polynomial = new CIColorPolynomial ()
			{
				Image = flower,
				RedCoefficients = new CIVector   (0, 0, 0,   .4f),
				GreenCoefficients = new CIVector (0, 0, .5f, .8f),
				BlueCoefficients = new CIVector  (0, 0, .5f, 1),
				AlphaCoefficients = new CIVector (0, 1, 1,   1),
			};

			return color_polynomial.OutputImage;
		}

		/// <summary>
		/// Constrains the color values between the range specified.
		/// </summary>
		/// <returns>The altered image.</returns>
		CIImage ColorClamp ()
		{
			var color_clamp = new CIColorClamp ()
			{
				Image = flower,
				InputMinComponents = new CIVector (.1f, 0f, .1f, 0),
				InputMaxComponents = new CIVector (.6f, 1f, .6f, 1),
			};

			return color_clamp.OutputImage;
		}


		/// <summary>
		/// Adjusts saturation, brightness, and contrast values.
		/// </summary>
		/// <returns>
		/// Altered Image
		/// </returns>
		public CIImage ColorControls ()
		{
			var colorCtrls = new CIColorControls ()
			{
				Image = flower,
				Brightness = .5F, // Min: 0 Max: 2
				Saturation = 1.2F, // Min: -1 Max: 1
				Contrast = 3.1F // Min: 0 Max: 4
			};
			
			return colorCtrls.OutputImage;
		}
		
		/// <summary>
		/// Changes the overall hue, or tint, of the source pixels.
		/// </summary>
		/// <returns>
		/// The Altered Image.
		/// </returns>
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
		/// Reduces the brightness around the specified point.
		/// </summary>
		/// <returns>The adjusted image.</returns>
		public CIImage VignetteEffect()
		{
			var vignette_effect = new CIVignetteEffect()
			{
				Image = flower,
				Center = new CIVector (flower.Extent.Width * .3f, flower.Extent.Width * .35f),
				Intensity = .6f,
				Radius = (float) flower.Extent.Width * .20f,
			};

			return vignette_effect.OutputImage;
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
		/// Applies a Sepia Filter to an Image.
		/// </summary>
		/// <returns>
		/// Image with Sepia Filter
		/// </returns>
		public CIImage SepiaTone ()
		{
			var sepia = new CISepiaTone () {
				Image = flower,
				Intensity = .8f
			};
			return sepia.OutputImage;
		}

		/// <summary>
		/// Modifies the source pixels by applying a set of polynomial cross-products.
		/// </summary>
		/// <returns>The cross polynomial.</returns>
		public CIImage ColorCrossPolynomial ()
		{
			var color_cross_polynomial = new CIColorCrossPolynomial () {
				Image = flower,
				RedCoefficients = new CIVector (new nfloat []{1, 0, 0, 0, 0, 0, 0, 0, 0, 0}),
				GreenCoefficients = new CIVector (new nfloat []{0, 1, 0, 0, 0, 0, 0, 0, 0, 0}),
				BlueCoefficients = new CIVector (new nfloat []{1, 0, 1, 0, -20, 0, 0, 0, 0, 0}),
			};
			return color_cross_polynomial.OutputImage;
		}
			
		/// <summary>
		/// Uses a three-dimensional color table to transform the source image pixels.
		/// </summary>
		/// <returns>
		/// The Altered Image
		/// </returns>
		public unsafe CIImage ColorCube ()
		{
			float [] color_cube_data = {
				0, 0, 0, 1,
				.1f, 0, 1, 1,
				0, 1, 0, 1,
				1, 1, 0, 1,
				0, 0, 1, 1,
				1, 0, 1, 1,
				0, 1, 1, 1,
				1, 1, 1, 1
			};

			var byteArray = new byte[color_cube_data.Length * 4];
			Buffer.BlockCopy(color_cube_data, 0, byteArray, 0, byteArray.Length);
			var data = NSData.FromArray (byteArray);

			var color_cube = new CIColorCube ()
			{
				Image = flower,
				CubeDimension = 2,
				CubeData = data
			};

			return color_cube.OutputImage;
		}

		/// <summary>
		/// Modifies the source pixels using a 3D color-table and then maps the result to a color space.
		/// </summary>
		/// <returns>The altered image.</returns>
		public CIImage ColorCubeWithColorSpace ()
		{
			float [] color_cube_data = {
				0, 0, 0, 1,
				.1f, 0, 1, 1,
				0, 1, 0, 1,
				1, 1, 0, 1,
				0, 0, 1, 1,
				1, 0, 1, 1,
				0, 1, 1, 1,
				1, 1, 1, 1
			};

			var byteArray = new byte[color_cube_data.Length * 4];
			Buffer.BlockCopy(color_cube_data, 0, byteArray, 0, byteArray.Length);
			var data = NSData.FromArray (byteArray);

			using (var cs = CGColorSpace.CreateDeviceRGB ()) {
				var cube = new CIColorCubeWithColorSpace () {
					Image = flower,
					CubeDimension = 2,
					CubeData = data,
					ColorSpace = cs
				};
				return cube.OutputImage;
			}
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
		/// Changes colors based on an input gradient image's mapping.
		/// </summary>
		/// <returns>The altered image.</returns>
		CIImage ColorMap ()
		{
			var color_map = new CIColorMap ()
			{
				Image = flower,
				GradientImage = flower
			};

			return color_map.OutputImage;
		}
		
		/// <summary>
		/// Remaps colors so they fall within shades of a single color.
		/// </summary>
		/// <returns>
		/// The Altered Image
		/// </returns>
		public CIImage ColorMonochrome ()
		{
			var inputColor = new CIColor (new CGColor (100F, 0F, 100F)); // Make it Purple R + B = Purple
			var monoChrome = new CIColorMonochrome ()
			{
				Image = flower,
				Color = inputColor,
				Intensity = 1F, // Default 1
			};
			
			return monoChrome.OutputImage;
		}

		/// <summary>
		/// Reduces the number of levels for each color component.
		/// </summary>
		/// <returns>The altered image.</returns>
		CIImage ColorPosterize ()
		{
			var posterize = new CIColorPosterize () {
				Image = flower,
				Levels = 8
			};

			return posterize.OutputImage;
		}
		
		/// <summary>
		/// Maps luminance to a color ramp of two colors.
		/// </summary>
		/// <returns>
		/// The altered Image
		/// </returns>
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
		/// Converts a grayscale image to an alpha mask.
		/// </summary>
		/// <returns>The altered image.</returns>
		CIImage MaskToAlpha ()
		{
			var masktoalpha = new CIMaskToAlpha ()
			{
				Image = heron
			};

			return masktoalpha.OutputImage;
		}

		/// <summary>
		/// Creates a grayscale image from the maximum value of the RGB color values.
		/// </summary>
		/// <returns>The altered image.</returns>
		CIImage MaximumComponent ()
		{
			var maximumcomponent = new CIMaximumComponent ()
			{
				Image = flower
			};

			return maximumcomponent.OutputImage;
		}

		/// <summary>
		/// Creates a grayscale image from the minimum component of the RGB values.
		/// </summary>
		/// <returns>The altered image.</returns>
		CIImage MinimumComponent ()
		{
			var minimumcomponent = new CIMinimumComponent ()
			{
				Image = flower
			};

			return minimumcomponent.OutputImage;
		}

		/// <summary>
		/// Exaggerates color of the image producing a vintage look.
		/// </summary>
		/// <returns>The altered inmage.</returns>
		CIImage PhotoEffectChrome ()
		{
			var photo_effect_chrome = new CIPhotoEffectChrome ()
			{
				Image = flower
			};

			return photo_effect_chrome.OutputImage;
		}

		/// <summary>
		/// Reduces color of the image producing a vintage look.
		/// </summary>
		/// <returns>The altered image.</returns>
		CIImage PhotoEffectFade ()
		{
			var photo_effect_fade = new CIPhotoEffectFade ()
			{
				Image = flower
			};

			return photo_effect_fade.OutputImage;
		}

		/// <summary>
		/// Distorts colors in a style reminiscent of instant film.
		/// </summary>
		/// <returns>The altered image.</returns>
		CIImage PhotoEffectInstant ()
		{
			var photo_effect_instant = new CIPhotoEffectInstant ()
			{
				Image = flower
			};

			return photo_effect_instant.OutputImage;
		}
				
		/// <summary>
		/// Produces a low-contrast black-and-white image.
		/// </summary>
		/// <returns>The altered image.</returns>
		CIImage PhotoEffectMono ()
		{
			var photo_effect_mono = new CIPhotoEffectMono ()
			{
				Image = flower
			};

			return photo_effect_mono.OutputImage;
		}
				
		/// <summary>
		/// Produces a high-contrast black-and-white image.
		/// </summary>
		/// <returns>The altered image.</returns>
		CIImage PhotoEffectNoir ()
		{
			var photo_effect_noir = new CIPhotoEffectNoir ()
			{
				Image = flower
			};

			return photo_effect_noir.OutputImage;
		}
				
		/// <summary>
		/// Produces a vintage look with exagerrated cool colors.
		/// </summary>
		/// <returns>The altered image.</returns>
		CIImage PhotoEffectProcess ()
		{
			var photo_effect_process = new CIPhotoEffectProcess ()
			{
				Image = flower
			};

			return photo_effect_process.OutputImage;
		}
				
		/// <summary>
		/// Produces a black-and-white image with minimal contrast changes.
		/// </summary>
		/// <returns>The altered image.</returns>
		CIImage PhotoEffectTonal ()
		{
			var photo_effect_tonal = new CIPhotoEffectTonal ()
			{
				Image = flower
			};

			return photo_effect_tonal.OutputImage;
		}
				
		/// <summary>
		/// Produces a vintage look with exagerrated warm colors.
		/// </summary>
		/// <returns>The altered image.</returns>
		CIImage PhotoEffectTransfer ()
		{
			var photo_effect_transfer = new CIPhotoEffectTransfer ()
			{
				Image = flower
			};

			return photo_effect_transfer.OutputImage;
		}

		/// <summary>
		/// Adjusts midtone brightness.
		/// </summary>
		/// <returns>
		/// Alters the image
		/// </returns>
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

		/// <summary>
		/// Produces a gradient along a linear axis between two endpoints.
		/// </summary>
		/// <returns>The altered image.</returns>
		CIImage SmoothLinearGradient ()
		{
			var point0 = new CIVector(0, 0); // Default [0 0]
			var point1 = new CIVector(250, 250); // Default [200 200]
			var linearGrad = new CISmoothLinearGradient()
			{
				Point0 = point0,
				Point1 = point1,
				Color0 = new CIColor (UIColor.Red),
				Color1 = new CIColor (UIColor.Blue)
			};

			return Crop (linearGrad);
		}

		#endregion

		#region CICategoryHalftoneEffect

		/// <summary>
		/// Creates a circular bulls-eye-style halftone screen.
		/// </summary>
		/// <returns>The altered image.</returns>
		CIImage CircularScreen ()
		{
			var cilcular_screen = new CICircularScreen () {
				Image = flower
			};

			return cilcular_screen.OutputImage;
		}

		/// <summary>
		/// Creates a halftone dot pattern screen.
		/// </summary>
		/// <returns>The altered image.</returns>
		CIImage DotScreen ()
		{
			var dot_screen = new CIDotScreen () {
				Image = flower
			};

			return dot_screen.OutputImage;
		}

		/// <summary>
		/// Creates a hatched halftone pattern screen.
		/// </summary>
		/// <returns>The altered image.</returns>
		CIImage HatchedScreen ()
		{
			var hatched_screen = new CIHatchedScreen () {
				Image = flower
			};

			return hatched_screen.OutputImage;
		}

		/// <summary>
		/// Simulates a halftone made of lines.
		/// </summary>
		/// <returns>The altered image.</returns>
		CIImage LineScreen ()
		{
			var line_screen = new CILineScreen () {
				Image = flower
			};

			return line_screen.OutputImage;
		}

		#endregion

		#region CICategorySharpen

		/// <summary>
		/// Sharpens the image.
		/// </summary>
		/// <returns>The altered image.</returns>
		CIImage SharpenLuminance ()
		{
			var sharpen = new CISharpenLuminance ()
			{
				Image = heron
			};

			return sharpen.OutputImage;
		}

		/// <summary>
		/// Increases the contrast of edges in the image
		/// </summary>
		/// <returns>The altered image.</returns>
		CIImage UnsharpMask ()
		{
			var unsharp_mask = new CIUnsharpMask ()
			{
				Image = heron
			};

			return unsharp_mask.OutputImage;
		}

		#endregion
		
		#region CICategoryGeometryAdjustment
		/// <summary>
		/// Applies a crop to an image.
		/// </summary>
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
		public CIImage AffineTransform ()
		{
			// Create an AffineTransform to Skew the Image
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
		/// Scales the image using Lanczos resampling.
		/// </summary>
		/// <returns>The altered image.</returns>
		CIImage LanczosScaleTransform ()
		{
			var lanczos_scale_transform = new CILanczosScaleTransform () {
				Image = heron
			};

			return lanczos_scale_transform.OutputImage;
		}

		/// <summary>
		/// Applies a transform the simulates perspective.
		/// </summary>
		/// <returns>The altered image.</returns>
		CIImage PerspectiveTransform ()
		{
			var perspective_transform = new CIPerspectiveTransform () {
				Image = heron
			};

			return perspective_transform.OutputImage;
		}

		/// <summary>
		/// Alters a portion of the total image based on a perspective transform.
		/// </summary>
		/// <returns>The altered image.</returns>
		CIImage PerspectiveTransformWithExtent ()
		{
			var extent = heron.Extent;

			var perspective_transform_with_extent = new CIPerspectiveTransformWithExtent () {
				Image = heron,
				BottomLeft = new CIVector (extent.Left + 70, extent.Top + 20),
				BottomRight = new CIVector (extent.Right - 70, extent.Top - 20),
				TopLeft = new CIVector (extent.Left - 70, extent.Bottom - 20),
				TopRight = new CIVector (extent.Right + 70, extent.Bottom + 20),
				Extent = new CIVector (new nfloat [] {extent.X + 100, extent.Y + 100, extent.Width - 100, extent.Height - 100})
			};

			return perspective_transform_with_extent.OutputImage;
		}
		
		/// <summary>
		/// Rotates the source image by the specified angle in radians.
		/// </summary>
		/// <returns>
		/// The filtered Image
		/// </returns>
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
		/// Darkens the background image samples to reflect the source image samples.
		/// </summary>
		/// <returns>
		/// The composite image
		/// </returns>
		public CIImage ColorBurnBlendMode()
		{
			var colorBurn = new CIColorBurnBlendMode()
			{
				Image = heron,
				BackgroundImage = clouds
			};
			
			return colorBurn.OutputImage;
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
		/// Creates composite image samples by choosing the darker samples (from either the source image or the background).
		/// </summary>
		/// <returns>
		/// The composite Image
		/// </returns>
		public CIImage DarkenBlendMode()
		{
			var darkenBlend = new CIDarkenBlendMode()
			{
				Image = heron,
				BackgroundImage = clouds
			};
			
			return darkenBlend.OutputImage;
		}
		
		/// <summary>
		/// Subtracts either the source image sample color from the background image sample color, or the reverse, depending on which sample has the greater brightness value.
		/// </summary>
		/// <returns>
		/// The composite image.
		/// </returns>
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

		#region CICategoryDistortionEffect
		/// <summary>
		/// Distorts the image around a convex or concave bump.
		/// </summary>
		/// <returns>The altered image.</returns>
		CIImage BumpDistortion ()
		{
			var width = xamarinCheck.Extent.Width;
			var height = xamarinCheck.Extent.Height;

			var bump_distortion = new CIBumpDistortion () {
				Image = xamarinCheck,
				Center = new CIVector (width/2f, height/2f),
				Radius = .4f * (float)height,
				Scale = .5f
			};

			return bump_distortion.OutputImage;
		}

		/// <summary>
		/// Distorts the image around a convex or concave bump.
		/// </summary>
		/// <returns>The altered image.</returns>
		CIImage BumpDistortionLinear ()
		{
			var width = xamarinCheck.Extent.Width;
			var height = xamarinCheck.Extent.Height;

			var bump_distortion_linear = new CIBumpDistortionLinear () {
				Image = xamarinCheck,
				Center = new CIVector (width * .5f, height * .5f),
				Radius = (float) .4f * (float)height,
				Scale = .5f,
				Angle = (float)Math.PI * .5f
			};

			return bump_distortion_linear.OutputImage;
		}

		/// <summary>
		/// Makes the pixels at the circumference of a circle spread out to the boundaries of the image.
		/// </summary>
		/// <returns>The altered image.</returns>
		CIImage CircleSplashDistortion ()
		{
			var distortion = new CICircleSplashDistortion () {
				Image = heron,
			};

			return Crop (distortion);
		}

		/// <summary>
		/// distorts pixels around a circular area.
		/// </summary>
		/// <returns>The altered image.</returns>
		CIImage HoleDistortion ()
		{
			var distortion = new CIHoleDistortion () {
				Image = heron,
				Radius = 85
			};

			return distortion.OutputImage;
		}

		/// <summary>
		/// Creates a spiraling effect.
		/// </summary>
		/// <returns>The altered image.</returns>
		CIImage LightTunnel ()
		{
			var lighttunel = new CILightTunnel () {
				Image = flower
			};

			return Crop (lighttunel);
		}

		/// <summary>
		/// Pinches pixels towards a rectangular area
		/// </summary>
		/// <returns>The altered image.</returns>
		CIImage PinchDistortion ()
		{
			var pinch_distortion = new CIPinchDistortion () {
				Image = flower
			};

			return pinch_distortion.OutputImage;
		}

		/// <summary>
		/// Rotates pixels around a point
		/// </summary>
		/// <returns>The altered image.</returns>
		CIImage TwirlDistortion ()
		{
			var twirl_distortion = new CITwirlDistortion () {
				Image = flower
			};

			return twirl_distortion.OutputImage;
		}

		/// <summary>
		/// Creates a tight spiraling distortion suggestive of a vortex.
		/// </summary>
		/// <returns>The altered image.</returns>
		CIImage VortexDistortion ()
		{
			var vortex_distortion = new CIVortexDistortion () {
				Image = heron
			};

			return vortex_distortion.OutputImage;
		}

		#endregion
		
		#region CICategoryGenerator
		/// <summary>
		/// Generates a checkerboard pattern.
		/// </summary>
		/// <returns>
		/// An Image of a Checkboard pattern
		/// </returns>
		public CIImage CheckerboardGenerator ()
		{
			// Color 1 
			var c0 = CIColor.FromRgb (1, 0, 0);
			var c1 = CIColor.FromRgb (0, 1, 0);
			var checker = new CICheckerboardGenerator ()
			{
				Color0 = c0,
				Color1 = c1,
				Center = new CIVector (new nfloat[] { 10 , 10 }), // Default [80 80]
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
		public CIImage ConstantColorGenerator ()
		{
			var colorGen = new CIConstantColorGenerator ()
			{
				Color = new CIColor (UIColor.Blue)
			};
			
			return Crop (colorGen);
		}

		/// <summary>
		/// Generates a QR code.
		/// </summary>
		/// <returns>An image with the code.</returns>
		CIImage QRCodeGenerator ()
		{
			var qr_code_generator = new CIQRCodeGenerator () 
			{
				Message = NSData.FromString ("http://xamarin.com"),
				CorrectionLevel = "M",
			};

			return qr_code_generator.OutputImage;
		}

		/// <summary>
		/// Randomly colors the pixels of an image.
		/// </summary>
		/// <returns>The generated image.</returns>
		CIImage RandomGenerator ()
		{
			var random = new CIRandomGenerator ();
			return Crop (random);
		}

		/// <summary>
		/// Simulates lens flare.
		/// </summary>
		/// <returns>The image with a star.</returns>
		CIImage StarShineGenerator ()
		{
			var generator = new CIStarShineGenerator () {
				Radius = 20,
			};

			return Crop (generator);
		}
		
		/// <summary>
		/// Generates a stripe pattern.
		/// </summary>
		/// <returns>
		/// The generated pattern.
		/// </returns>
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
		/// Uses a mask image to blend foreground and background images.
		/// </summary>
		/// <returns>The altered image.</returns>
		CIImage BlendWithAlphaMask ()
		{
			var blend_with_alpha_mask = new CIBlendWithAlphaMask () {
				BackgroundImage = clouds,
				Image = flower,
				Mask = xamarinAlpha
			};

			return blend_with_alpha_mask.OutputImage;
		}

		/// <summary>
		/// Uses a grayscale mask to blends its foreground and background images.
		/// </summary>
		/// <returns>The altered image.</returns>
		CIImage BlendWithMask ()
		{
			var blend_with_mask = new CIBlendWithMask () {
				BackgroundImage = clouds,
				Image = flower,
				Mask = xamarin
			};

			return blend_with_mask.OutputImage;
		}

		/// <summary>
		/// Creates an edge-flow effect.
		/// </summary>
		/// <returns>The altered image.</returns>
		CIImage Bloom ()
		{
			var bloom = new CIBloom () {
				Image = flower
			};

			return bloom.OutputImage;
		}

		/// <summary>
		/// Performs a custom 3x3 matrix convolution.
		/// </summary>
		/// <returns>The altered image.</returns>
		CIImage Convolution3X3 ()
		{
			var convolution_3X3 = new CIConvolution3X3 () {
				Image = heron,
				Weights = new CIVector (new nfloat [] {
					0, -1, 0,
					-1, 5, -1,
					0, -1, 0}),
				Bias = 0,
			};

			return convolution_3X3.OutputImage;
		}

		/// <summary>
		/// Performs a custom 5x5 matrix convolution.
		/// </summary>
		/// <returns>The altered image.</returns>
		CIImage Convolution5X5 ()
		{
			var convolution_5X5 = new CIConvolution5X5 () {
				Image = heron,
				Weights = new CIVector (new nfloat [] {
					.5f, 0, 0, 0, 0,
					0, 0, 0, 0, 0,
					0, 0, 0, 0, 0,
					0, 0, 0, 0, 0,
					0, 0, 0, 0, .5f}),
				Bias = 0,
			};

			return convolution_5X5.OutputImage;
		}

		/// <summary>
		/// Performs a horizontal convolution of 9 elements.
		/// </summary>
		/// <returns>The altered image.</returns>
		CIImage Convolution9Horizontal ()
		{
			var convolution_9_horizontal = new CIConvolution9Horizontal () {
				Image = heron,
				Weights = new CIVector (new nfloat [] {1, -1, 1, 0, 1, 0, -1, 1, -1}),
			};

			return convolution_9_horizontal.OutputImage;
		}

		/// <summary>
		/// Performs a vertical convolution of 9 elements.
		/// </summary>
		/// <returns>The altered image.</returns>
		CIImage Convolution9Vertical ()
		{
			var convolution_9_vertical = new CIConvolution9Vertical () {
				Image = heron,
				Weights = new CIVector (new nfloat [] {1, -1, 1, 0, 1, 0, -1, 1, -1}),
			};

			return convolution_9_vertical.OutputImage;
		}

		/// <summary>
		/// Maps color intensity from a linear gamma curve to the sRGB color space.
		/// </summary>
		/// <returns>The altered image.</returns>
		CIImage LinearToSRGBToneCurve ()
		{
			var linear2Srgb_tone_curve = new CILinearToSRGBToneCurve () {
				Image = flower
			};

			return linear2Srgb_tone_curve.OutputImage;
		}

		/// <summary>
		/// Adjusts tone response in sRGB color space and then maps it to a linear gamma curve.
		/// </summary>
		/// <returns>The altered image.</returns>
		CIImage SRGBToneCurveToLinear ()
		{
			var srgb_tone_curve2linear = new CISRGBToneCurveToLinear () {
				Image = flower
			};

			return srgb_tone_curve2linear.OutputImage;
		}

		/// <summary>
		/// Dulls the highlights of the source image.
		/// </summary>
		/// <returns>The altered image.</returns>
		CIImage Gloom ()
		{
			var gloom = new CIGloom () {
				Image = flower
			};

			return gloom.OutputImage;
		}

		/// <summary>
		/// Adjust the tonal mapping of an image while preserving spatial detail.
		/// </summary>
		/// <returns>
		/// The altered Image
		/// </returns>
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

		/// <summary>
		/// Pixelates the original image.
		/// </summary>
		/// <returns>The altered image.</returns>
		CIImage Pixellate ()
		{
			var pixellate = new CIPixellate () {
				Image = flower,
			};

			return pixellate.OutputImage;
		}

		#endregion

		#region CICategoryTileEffect

		/// <summary>
		/// Extends the border pixels to the post-transform boundaries.
		/// </summary>
		/// <returns>The altered image.</returns>
		CIImage AffineClamp ()
		{
			var affine_clamp = new CIAffineClamp () {
				Image = flower
			};

			return Crop (affine_clamp);
		}

		/// <summary>
		/// Tiles the transformed image
		/// </summary>
		/// <returns>The altered image.</returns>
		CIImage AffineTile ()
		{
			var affine_tile = new CIAffineTile () {
				Image = flower
			};

			return Crop (affine_tile);
		}

		/// <summary>
		/// Applies 8-way reflected symmetry.
		/// </summary>
		/// <returns>The altered image.</returns>
		CIImage EightfoldReflectedTile ()
		{
			var tile = new CIEightfoldReflectedTile () {
				Image = flower
			};

			return Crop (tile);
		}

		/// <summary>
		/// Applies 4-way reflected symmetry
		/// </summary>
		/// <returns>The altered image.</returns>
		CIImage FourfoldReflectedTile ()
		{
			var tile = new CIFourfoldReflectedTile () {
				Image = flower
			};

			return Crop (tile);
		}

		/// <summary>
		/// Rotates the source image in 90-degree increments
		/// </summary>
		/// <returns>The altered image.</returns>
		CIImage FourfoldRotatedTile ()
		{
			var tile = new CIFourfoldRotatedTile () {
				Image = flower
			};

			return Crop (tile);
		}

		/// <summary>
		/// Applies four translations to the source image.
		/// </summary>
		/// <returns>The altered image.</returns>
		CIImage FourfoldTranslatedTile ()
		{
			var tile = new CIFourfoldTranslatedTile () {
				Image = flower,
				Center = new CIVector (100, 100),
				Width = 150,
			};

			return Crop (tile);
		}

		/// <summary>
		/// Translates and smears the source image.
		/// </summary>
		/// <returns>The altered image.</returns>
		CIImage GlideReflectedTile ()
		{
			var tile = new CIGlideReflectedTile () {
				Image = flower
			};

			return Crop (tile);
		}

		/// <summary>
		/// Applies a perspective transform and then tiles the result.
		/// </summary>
		/// <returns>The altered image.</returns>
		CIImage PerspectiveTile ()
		{
			var tile = new CIPerspectiveTile () {
				Image = flower
			};

			return Crop (tile);
		}

		/// <summary>
		/// Applies 6-way reflected symmetry.
		/// </summary>
		/// <returns>The altered image.</returns>
		CIImage SixfoldReflectedTile ()
		{
			var tile = new CISixfoldReflectedTile () {
				Image = flower
			};

			return Crop (tile);
		}

		/// <summary>
		/// Rotates the image in 60-degree increments
		/// </summary>
		/// <returns>The altered image.</returns>
		CIImage SixfoldRotatedTile ()
		{
			var tile = new CISixfoldRotatedTile () {
				Image = flower
			};

			return Crop (tile);
		}

		/// <summary>
		/// Creates a kaleidoscopic effect
		/// </summary>
		/// <returns>The altered image.</returns>
		CIImage TriangleKaleidoscope ()
		{
			var triangle_kaleidoscope = new CITriangleKaleidoscope () {
				Image = heron,
				Size = 200,
				Point = new CIVector (heron.Extent.Width * .3f, heron.Extent.Height * .6f),
			};

			return Crop (triangle_kaleidoscope);
		}

		/// <summary>
		/// Applies 12-way reflected symmetry
		/// </summary>
		/// <returns>The altered image.</returns>
		CIImage TwelvefoldReflectedTile ()
		{
			var tile = new CITwelvefoldReflectedTile () {
				Image = flower
			};

			return Crop (tile);
		}

		#endregion

		#region CICategoryTransition

		/// <summary>
		/// Animates a transition by moving a bar over the source image.
		/// </summary>
		/// <returns>The altered image.</returns>
		CIImage BarsSwipeTransition ()
		{
			var transition = new CIBarsSwipeTransition ()
			{
				Image = heron,
				TargetImage = clouds,
				Time = 0.5f
			};

			return transition.OutputImage;
		}

		/// <summary>
		/// Mimics the effect of a photocopier.
		/// </summary>
		/// <returns>The altered image.</returns>
		CIImage CopyMachineTransition ()
		{
			var transition = new CICopyMachineTransition ()
			{
				Image = heron,
				TargetImage = clouds,
				Time = 0.5f
			};

			return transition.OutputImage;
		}

		/// <summary>
		/// Uses a mask to define the transition.
		/// </summary>
		/// <returns>The altered image.</returns>
		CIImage DisintegrateWithMaskTransition ()
		{
			var transition = new CIDisintegrateWithMaskTransition ()
			{
				Image = clouds, 
				TargetImage = flower,
				Mask = xamarinCheck
			};

			return transition.OutputImage;
		}

		/// <summary>
		/// Performs a cross-dissolve.
		/// </summary>
		/// <returns>The altered image.</returns>
		CIImage DissolveTransition ()
		{
			var transition = new CIDissolveTransition ()
			{
				Image = heron,
				TargetImage = clouds,
				Time = 0.5f
			};

			return transition.OutputImage;
		}

		/// <summary>
		/// Presents a starburst-like flash.
		/// </summary>
		/// <returns>The altered image.</returns>
		CIImage FlashTransition ()
		{
			var transition = new CIFlashTransition ()
			{
				Image = heron,
				TargetImage = clouds,
				Time = 0.8f
			};

			return transition.OutputImage;
		}

		/// <summary>
		/// Reveals the background image via a series of irregularly shaped holes.
		/// </summary>
		/// <returns>The altered image.</returns>
		CIImage ModTransition ()
		{
			var transition = new CIModTransition ()
			{
				Image = heron,
				TargetImage = clouds,
				Time = 0.5f
			};

			return transition.OutputImage;
		}

		/// <summary>
		/// Swipes from one image to the other.
		/// </summary>
		/// <returns>The altered image.</returns>
		CIImage SwipeTransition ()
		{
			var swipe_transition = new CISwipeTransition ()
			{
				Image = heron,
				TargetImage = clouds,
				Time = 0.8f
			};

			return swipe_transition.OutputImage;
		}

		#endregion

		#endregion
	}

}

