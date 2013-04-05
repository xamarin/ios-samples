using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.CoreImage;
using MonoTouch.ObjCRuntime;

using System.Collections.Generic;
using MonoTouch.Dialog;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using MonoTouch.CoreGraphics;
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
		UIWindow window;

		#region UIApplicationDelegate Methods
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			var root = new RootElement ("Effects") {
				new Section () {
					(Element)new RootElement ("Color Adjustment"){
						new Section () {
							(Element)new RootElement ("ColorControls", (x) => Demo (ColorControls)),
							(Element)new RootElement ("ColorMatrix", (x) => Demo (ColorMatrix)),
							(Element)new RootElement ("ExposureAdjust", (x) => Demo (ExposureAdjust)),
							(Element)new RootElement ("GammaAdjust", (x) => Demo (GammaAdjust)),
							(Element)new RootElement ("HueAdjust", (x) => Demo (HueAdjust)),
							(Element)new RootElement ("TemperatureAndTint", (x) => Demo (TemperatureAndTint)),
							(Element)new RootElement ("ToneCurve", (x) => Demo (ToneCurve)),
							(Element)new RootElement ("Vibrance", (x) => Demo (Vibrance)),
							(Element)new RootElement ("WhitePointAdjust", (x) => Demo(WhitePointAdjust))
						}
					},
					(Element)new RootElement ("Color Effect"){
						new Section () {
							(Element)new RootElement ("ColorCube", (x) => Demo (ColorCube)),
							(Element)new RootElement ("ColorInvert", (x) => Demo (ColorInvert)),
							(Element)new RootElement ("ColorMap", l => Demo (ColorMap)),
							(Element)new RootElement ("ColorMonochrome", (x) => Demo (ColorMonochrome)),
							(Element)new RootElement ("ColorPosterize", l => Demo (ColorPosterize)),
							(Element)new RootElement ("FalseColor", (x) => Demo (FalseColor)),
							(Element)new RootElement ("MaskToAlpha", l => Demo (MaskToAlpha)),
							(Element)new RootElement ("MaximumComponent", l => Demo (MaximumComponent)),
							(Element)new RootElement ("MinimumComponent", l => Demo (MinimumComponent)),
							(Element)new RootElement ("SepiaTone", (x) => Demo (SepiaTone)),
							(Element)new RootElement ("Vignete", l => Demo (Vignette))
						}
					},
					(Element)new RootElement ("Composite Operation"){
						new Section () {
							(Element)new RootElement ("AdditionCompositing", (x) => Demo (AdditionCompositing)),
							(Element)new RootElement ("ColorBlendMode", (x) => Demo (ColorBlendMode)),
							(Element)new RootElement ("ColorBurnBlendMode", (x) => Demo (ColorBurnBlendMode)),
							(Element)new RootElement ("ColorDodgeBlendMode", (x) => Demo (ColorDodgeBlendMode)),
							(Element)new RootElement ("DarkenBlendMode", (x) => Demo (DarkenBlendMode)),
							(Element)new RootElement ("DifferenceBlendMode", (x) => Demo (DifferenceBlendMode)),
							(Element)new RootElement ("ExclusionBlendMode", (x) => Demo (ExclusionBlendMode)),
							(Element)new RootElement ("HardLightBlendMode", (x) => Demo (HardLightBlendMode)),
							(Element)new RootElement ("HueBlendMode", (x) => Demo (HueBlendMode)),
							(Element)new RootElement ("LightenBlendMode", (x) => Demo (LightenBlendMode)),
							(Element)new RootElement ("LuminosityBlendMode", (x) => Demo (LuminosityBlendMode)),
							(Element)new RootElement ("MaximumCompositing", (x) => Demo (MaximumCompositing)),
							(Element)new RootElement ("MinimumCompositing", (x) => Demo (MinimumCompositing)),
							(Element)new RootElement ("MultiplyBlendMode", (x) => Demo (MultiplyBlendMode)),
							(Element)new RootElement ("MultiplyCompositing", (x) => Demo (MultiplyCompositing)),
							(Element)new RootElement ("OverlayBlendMode", (x) => Demo (OverlayBlendMode)),
							(Element)new RootElement ("SaturationBlendMode", (x) => Demo (SaturationBlendMode)),
							(Element)new RootElement ("ScreenBlendMode", (x) => Demo (ScreenBlendMode)),
							(Element)new RootElement ("SoftLightBlendMode", (x) => Demo (SoftLightBlendMode)),
							(Element)new RootElement ("SourceAtopCompositing", (x) => Demo (SourceAtopCompositing)),
							(Element)new RootElement ("SourceInCompositing", (x) => Demo(SourceInCompositing)),
							(Element)new RootElement ("SourceOutCompositing", (x) => Demo(SourceOutCompositing)),
							(Element)new RootElement ("SourceOverCompositing", (x) => Demo (SourceOverCompositing)),
						}
					},
					(Element)new RootElement ("Distortions"){
						new Section () {
							(Element)new RootElement ("CircleSplashDistortion", l => Demo (CircleSplashDistortion)),
							(Element)new RootElement ("HoleDistortion", l => Demo (HoleDistortion)),
							(Element)new RootElement ("LightTunnel", l => Demo (LightTunel)),
							(Element)new RootElement ("PinchDistortion", l => Demo (PinchDistortion)),
							(Element)new RootElement ("TwirlDistortion", l => Demo (TwirlDistortion)),
							(Element)new RootElement ("VortexDistortion", l => Demo (VortexDistortion))
						}
					},
					(Element)new RootElement ("Generators"){
						new Section () {
							(Element)new RootElement ("CheckerboardGenerator", (x) => Demo (CheckerboardGenerator)),
							(Element)new RootElement ("ConstantColorGenerator", (x) => Demo (ConstantColorGenerator)),
							(Element)new RootElement ("RandomGenerator", l => Demo (RandomGenerator)),
							(Element)new RootElement ("StarShineGenerator", l => Demo (StarShineGenerator)),
							(Element)new RootElement ("StripesGenerator", (x) => Demo (StripesGenerator)),
						}
					},
					(Element)new RootElement ("Geometry Adjust"){
						new Section () {
							(Element)new RootElement ("AffineTransform", (x) => Demo (AffineTransform)),
							(Element)new RootElement ("Crop", (x) => Demo (Crop)),
							(Element)new RootElement ("LanczosScaleTransform", l => Demo (LanczosScaleTransform)),
							(Element)new RootElement ("PerspectiveTransform", l => Demo (PerspectiveTransform)),
							(Element)new RootElement ("StraightenFilter", (x) => Demo (StraightenFilter)),
						}
					},
					(Element)new RootElement ("Gradients"){
						new Section () {
							(Element)new RootElement ("GaussianGradient", (x) => Demo (GaussianGradient)),
							(Element)new RootElement ("LinearGradient", (x) =>Demo(LinearGradient)),
							(Element)new RootElement ("RadialGradient", (x) => Demo (RadialGradient)),
						}
					},
					(Element)new RootElement ("Halftone Effect") {
						new Section () {
							(Element)new RootElement ("CircularScreen", l => Demo (CircularScreen)),
							(Element)new RootElement ("DotScreen", l => Demo (DotScreen)),
							(Element)new RootElement ("HatchedScreen", l => Demo (HatchedScreen)),
							(Element)new RootElement ("LineScreen", l => Demo (LineScreen))
						}
					},
					(Element)new RootElement ("Sharpen") {
						new Section () {
							(Element)new RootElement ("SharpenLuminance", l => Demo (SharpenLuminance)),
							(Element)new RootElement ("UnsharpMask", l => Demo (UnsharpMask)),
						}
					},
					(Element)new RootElement ("Stylize"){
						new Section () {
							(Element)new RootElement ("BlendWithMask", l => Demo (BlendWithMask)),
							(Element)new RootElement ("Bloom", l => Demo (Bloom)),
							(Element)new RootElement ("Gloom", l => Demo (Gloom)),
							(Element)new RootElement ("HighlightShadowAdjust", (x) => Demo (HighlightShadowAdjust)),
							(Element)new RootElement ("Pixellate", l => Demo (Pixellate)),
						}
					},
					(Element)new RootElement ("Tile Effect"){
						new Section () {
							(Element)new RootElement ("AffineClamp", l => Demo (AffineClamp)),
							(Element)new RootElement ("AffineTile", l => Demo (AffineTile)),
							(Element)new RootElement ("EightfoldReflectedTile", l => Demo (EightfoldReflectedTile)),
							(Element)new RootElement ("FourfoldReflectedTile", l => Demo (FourfoldReflectedTile)),
							(Element)new RootElement ("FourfoldRotatedTile", l => Demo (FourfoldRotatedTile)),
							(Element)new RootElement ("GlideReflectedTile", l => Demo (GlideReflectedTile)),
							(Element)new RootElement ("PerspectiveTile", l => Demo (PerspectiveTile)),
							(Element)new RootElement ("SixfoldReflectedTile", l => Demo (SixfoldReflectedTile)),
							(Element)new RootElement ("SixfoldRotatedTile", l => Demo (SixfoldRotatedTile)),
							(Element)new RootElement ("TriangleKaleidoscope", l => Demo (TriangleKaleidoscope)),
							(Element)new RootElement ("TwelvefoldReflectedTile", l => Demo (TwelvefoldReflectedTile))
						}
					},
					(Element)new RootElement ("Transition"){
						new Section () {
							(Element)new RootElement ("BarsSwipeTransition", l => Demo (BarsSwipeTransition)),
							(Element)new RootElement ("CopyMachineTransition", l => Demo (CopyMachineTransition)),
							(Element)new RootElement ("DissolveTransition", l => Demo (DissolveTransition)),
							(Element)new RootElement ("FlashTransition", l => Demo (FlashTransition)),
							(Element)new RootElement ("ModTransition", l => Demo (ModTransition)),
							(Element)new RootElement ("SwipeTransition", l => Demo (SwipeTransition))
						}
					},
#if DEBUG
					(Element)new RootElement("Rebase Test Images", (x) => RebaseTestImages()),
					(Element)new RootElement("Test Filters", (x) => TestView())
#endif
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

		public UIViewController Demo (Func<CIImage> makeDemo)
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
		
		public UIViewController TestView()
		{
			// Create a view with a Button to run the tests and a Text View for the Results.
			var vc = new UIViewController();
			
			var btn = new UIButton(new RectangleF(0, 0, 100, 40));
			btn.SetTitle("Run Tests", UIControlState.Normal);
			btn.BackgroundColor = UIColor.White;
			btn.SetTitleColor(UIColor.Black, UIControlState.Normal);
			
			var textResults = new UITextView(new RectangleF(0, 50, vc.View.Bounds.Width, 40));
			textResults.Text = "";
			textResults.Hidden = true;
			
			// Setup Test here
			btn.TouchUpInside += (sender, e) => 
			{
				Console.WriteLine("Running Tests...");
				textResults.Text = "Tests Running";
			 	btn.Enabled = false;
				
				textResults.SetNeedsDisplay();
				btn.SetNeedsDisplay();
				
				try	
				{	
					var results = RunTests();
					
					var failed = results.Where(r => !r.Pass).ToList();
					
					textResults.Text = failed.Count == 0 ? "All Filters Passed" : "These filters failed " + string.Join(Environment.NewLine, failed.Select (r => r.FilterName));
					textResults.Hidden = false;
				}
				finally
				{
					btn.Enabled = true;
				}
			};
			
			vc.View.AddSubviews(btn, textResults);
			
			return vc;
		}
		
		struct TestResult
		{
			public bool Pass {get; set;}
			public string FilterName {get; set;}
		}
		
	    List<TestResult> RunTests()
		{
			var filters = CreateFilterDictionary();
			var resultList = new List<TestResult>();
			
			foreach(var filter in filters)
			{
				Console.WriteLine("Testing Filter " + filter.Key);
				
				// Run the Filter
				var view = filter.Value();
				
				// Capture the Ouput as an Image
				var image = CaptureView(view.View);
				var cgImage = image.CGImage;
				
				// Get the Image data for the current Image
				byte[] testRawData = new byte[cgImage.Width * cgImage.Height * 4];
				var testColorSpace = CGColorSpace.CreateDeviceRGB();
			
				var testCGContext = new CGBitmapContext(testRawData, cgImage.Width, cgImage.Height, 8, cgImage.BytesPerRow, testColorSpace, CGImageAlphaInfo.PremultipliedLast);
				testCGContext.DrawImage(new RectangleF(0, 0, cgImage.Width, cgImage.Height), cgImage);
				
				// Get the base image
				var baseImage = GetTestImage(filter.Key);
				
				// Get the image data for the base image.
				byte[] baseRawData = new byte[baseImage.Width * baseImage.Height * 4];
				var baseColorSpace = CGColorSpace.CreateDeviceRGB();
				
				var baseCGContext = new CGBitmapContext(baseRawData, baseImage.Width, baseImage.Height, 8, baseImage.BytesPerRow, baseColorSpace, CGImageAlphaInfo.PremultipliedLast);
				baseCGContext.DrawImage(new RectangleF(0, 0, baseImage.Width, baseImage.Height), baseImage);
				
				// Compare each Pixel
				bool wasMismatch = false;
				for(var i = 0; i < baseRawData.Length; i++)
				{
					if(testRawData[i] != baseRawData[i])
					{
						wasMismatch = true;
						resultList.Add(new TestResult() { FilterName = filter.Key, Pass = false});
						break;
					}
				}
				
				if(!wasMismatch)
					resultList.Add(new TestResult() { FilterName = filter.Key, Pass = true});
			}
			
			return resultList;
		}
		
		CGImage GetTestImage(string imageName)
		{
			var image = UIImage.FromFile(ImagePath(imageName));
			return image.CGImage;
		}
		
		private UIViewController RebaseTestImages()
		{	
			var filterList = CreateFilterDictionary();
			UIViewController view = new UIViewController();
			
			var txtBounds = view.View.Bounds;
			var boundsHeight = 20;
			txtBounds.Height = boundsHeight;

			var text = new UITextView(txtBounds){ Text = "Rebasing Images" };
			text.Hidden = true;
			view.View.AddSubview(text);
			
			var btnBounds = view.View.Bounds;
			btnBounds.Y = boundsHeight;
			btnBounds.Height = boundsHeight;
			
			var btn = new UIButton(btnBounds);
			btn.BackgroundColor = UIColor.Black;
			btn.SetTitleColor(UIColor.White, UIControlState.Normal);
			btn.SetTitle("Rebase", UIControlState.Normal);
			
			btn.TouchUpInside += (sender, e) => {
				Console.WriteLine("Rebasing Images");
				text.Text = "Rebasing Images"; 
				text.Hidden = false;
				
				// Foreach ViewController, Display it, take a screen shot and then, save it
				foreach(var filter in filterList)
				{
					Console.WriteLine("Rebasing Filter " + filter.Key);
					view = filter.Value();
					
					// Display the Filter
					//window.RootViewController  = view;
					
					// Get a screenshot
					var image = CaptureView(view.View);
					
					// Save the Screenshot.
					NSError err;
					var directory = ImageDirectory();
					var fileName = ImagePath(filter.Key);
					
					if(!Directory.Exists(directory))
						Directory.CreateDirectory(directory);
					
					if (Runtime.Arch == Arch.SIMULATOR) {
						if(File.Exists(fileName))
							File.Delete(fileName);
					
						image.AsPNG().Save(fileName, NSDataWritingOptions.FileProtectionNone, out err);
					
						if(err != null)
							Console.WriteLine("Could not write image File. " + Environment.NewLine + err.LocalizedDescription);
					}
				}

				if (Runtime.Arch == Arch.DEVICE)
					Console.WriteLine ("File cannnot be updated on device (only on the simulator)");

				text.Text = "Done";	
			};
			
			view.View.AddSubview(btn);
			return view;
		}
		
		string ImagePath(string imageName)
		{	
			var fileName = Path.Combine (ImageDirectory(), imageName);
			fileName = Path.ChangeExtension(fileName, ".png");
			
			return fileName;
		}
		
		string ImageDirectory()
		{
			var directory = Path.Combine(Directory.GetCurrentDirectory(), "TestImages");
			return directory;
		}
		
		UIImage CaptureView (UIView view)
		{
			RectangleF rect = UIScreen.MainScreen.Bounds;
			UIGraphics.BeginImageContext(rect.Size);
			
			CGContext context = UIGraphics.GetCurrentContext();
			view.Layer.RenderInContext(context);
			UIImage img = UIGraphics.GetImageFromCurrentImageContext();
			
			UIGraphics.EndImageContext();	
			return img;
		}
		
		private Dictionary<string, Func<UIViewController>> CreateFilterDictionary()
		{
			var dictionary = new Dictionary<string, Func<UIViewController>>()
			{
				{ "ColorControls", () => Demo (ColorControls) },
				{ "ColorMatrix", () => Demo (ColorMatrix) },
				{ "ExposureAdjust", () => Demo (ExposureAdjust) },
				{ "GammaAdjust", () => Demo (GammaAdjust) },
				{ "HueAdjust", () => Demo (HueAdjust) },
				{ "TemperatureAndTint", () => Demo (TemperatureAndTint) },
				{ "ToneCurve", () => Demo (ToneCurve) },
				{ "Vibrance", () => Demo (Vibrance) },
				{ "WhitePointAdjust", () => Demo(WhitePointAdjust) },
				//{ "ColorCube", () => Demo (ColorCube) },
				{ "ColorInvert", () => Demo (ColorInvert) },
				{ "ColorMonochrome", () => Demo (ColorMonochrome) },
				{ "FalseColor", () => Demo (FalseColor) },
				{ "SepiaTone", () => Demo (SepiaTone) },
				{ "AdditionCompositing", () => Demo (AdditionCompositing)},
				{ "ColorBlendMode", () => Demo (ColorBlendMode)},
				{ "ColorBurnBlendMode", () => Demo (ColorBurnBlendMode)},
				{ "ColorDodgeBlendMode", () => Demo (ColorDodgeBlendMode)},
				{ "DarkenBlendMode", () => Demo (DarkenBlendMode)},
				{ "DifferenceBlendMode", () => Demo (DifferenceBlendMode)},
				{ "ExclusionBlendMode", () => Demo (ExclusionBlendMode)},
				{ "HardLightBlendMode", () => Demo (HardLightBlendMode)},
				{ "HueBlendMode", () => Demo (HueBlendMode)},
				{ "LightenBlendMode", () => Demo (LightenBlendMode)},
				{ "LuminosityBlendMode", () => Demo (LuminosityBlendMode)},
				{ "MaximumCompositing", () => Demo (MaximumCompositing)},
				{ "MinimumCompositing", () => Demo (MinimumCompositing)},
				{ "MultiplyCompositing", () => Demo (MultiplyCompositing)},
				{ "MultiplyBlendMode", () => Demo (MultiplyBlendMode)},
				{ "OverlayBlendMode", () => Demo (OverlayBlendMode)},
				{ "SaturationBlendMode", () => Demo (SaturationBlendMode)},
				{ "ScreenBlendMode", () => Demo (ScreenBlendMode)},
				{ "SoftLightBlendMode", () => Demo (SoftLightBlendMode)},
				{ "SourceAtopCompositing", () => Demo (SourceAtopCompositing)},
				{ "SourceInCompositing", () => Demo(SourceInCompositing)},
				{ "SourceOutCompositing", () => Demo(SourceOutCompositing)},
				{ "SourceOverCompositing", () => Demo (SourceOverCompositing)},
				{ "CheckerboardGenerator", () => Demo (CheckerboardGenerator) },
				{ "ConstantColorGenerator", () => Demo (ConstantColorGenerator) },
				{ "StripesGenerator", () => Demo (StripesGenerator) },
				{ "AffineTransform", () => Demo (AffineTransform) },
				{ "Crop", () => Demo (Crop) },
				{ "StraightenFilter", () => Demo (StraightenFilter) },
				{ "GaussianGradient", () => Demo (GaussianGradient) },
				{ "LinearGradient", () =>Demo(LinearGradient) },
				{ "RadialGradient", () => Demo (RadialGradient) },
				{ "HighlightShadowAdjust", () => Demo (HighlightShadowAdjust) },
				{ "Vignette", () => Demo (Vignette) },

				//{ "RandomGenerator", () => Demo (RandomGenerator) },
				{ "StarShineGenerator", () => Demo (StarShineGenerator) },
				{ "LanczosScaleTransform", () => Demo (LanczosScaleTransform) },
				{ "PerspectiveTransform", () => Demo (PerspectiveTransform) },
				{ "CircularScreen", () => Demo (CircularScreen) },
				{ "DotScreen", () => Demo (DotScreen) },
				{ "HatchedScreen", () => Demo (HatchedScreen) },
				{ "LineScreen", () => Demo (LineScreen) },
				{ "SharpenLuminance", () => Demo (SharpenLuminance) },
				{ "UnsharpMask", () => Demo (UnsharpMask) },
				{ "BlendWithMask", () => Demo (BlendWithMask) },
				{ "Bloom", () => Demo (Bloom) },
				{ "Gloom", () => Demo (Gloom) },
				{ "Pixellate", () => Demo (Pixellate) },
				{ "AffineClamp", () => Demo (AffineClamp) },
				{ "AffineTile", () => Demo (AffineTile) },
				{ "EightfoldReflectedTile", () => Demo (EightfoldReflectedTile) },
				{ "FourfoldReflectedTile", () => Demo (FourfoldReflectedTile) },
				{ "FourfoldRotatedTile", () => Demo (FourfoldRotatedTile) },
				{ "GlideReflectedTile", () => Demo (GlideReflectedTile) },
				{ "PerspectiveTile", () => Demo (PerspectiveTile) },
				{ "SixfoldReflectedTile", () => Demo (SixfoldReflectedTile) },
				{ "SixfoldRotatedTile", () => Demo (SixfoldRotatedTile) },
				{ "TriangleKaleidoscope", () => Demo (TriangleKaleidoscope) },
				{ "TwelvefoldReflectedTile", () => Demo (TwelvefoldReflectedTile) },
				{ "BarsSwipeTransition", () => Demo (BarsSwipeTransition) },
				{ "CopyMachineTransition", () => Demo (CopyMachineTransition) },
				{ "DissolveTransition", () => Demo (DissolveTransition) },
				{ "FlashTransition", () => Demo (FlashTransition) },
				{ "ModTransition", () => Demo (ModTransition) },
				{ "SwipeTransition", () => Demo (SwipeTransition) }
			};
			
			return dictionary;
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
		public unsafe CIImage ColorCube ()
		{
			const uint size = 64;
			var data = generateCubeData(size);

			var cube = new CIColorCube ()
			{
				Image = flower,
				CubeDimension = size,
				CubeData = data
			};

			return cube.OutputImage;
		}
		#region Generate Cube Data -- for ColorCube() example
		/// <summary>
		/// Generates the cube data. Based on Objective-C example at 
		/// https://github.com/vhbit/ColorCubeSample
		/// Many thanks to the original author!
		/// </summary>
		/// <remarks>
		/// Original author grants permission to use this sample code
		/// https://github.com/vhbit/ColorCubeSample/issues/1
		/// </remarks>			
		NSData generateCubeData (uint size)
		{
			var data = new NSData ();
			
			float minHueAngle = 0.1f; // modify to see different result
			float maxHueAngle = 0.4f; // modify to see different result
			float centerHueAngle = minHueAngle + (maxHueAngle - minHueAngle)/2.0f;
			float destCenterHueAngle = 1.0f/4.0f; // modify to see different result

			byte [] cubeData = new byte[size*size*size*4];
			float [] rgb = new float[3], hsv = new float[3], newRGB = new float[3];
			
			uint offset = 0;
			
			for (int z = 0; z < size; z++)
			{
				rgb[2] = (float) ((double) z) / size; // blue value
				for (int y = 0; y < size; y++)
				{
					rgb[1] = (float) ((double) y) / size; // green value
					for (int x = 0; x < size; x++)
					{
						rgb[0] = (float) ((double) x) / size; // red value
						rgbToHSV(rgb, ref hsv);
						
						if (hsv[0] < minHueAngle || hsv[0] > maxHueAngle)
							newRGB = rgb;
						else
						{
							hsv[0] = destCenterHueAngle + (centerHueAngle - hsv[0]);
							hsvToRGB(hsv, ref newRGB);
						}
						
						cubeData[offset]   = (byte) (newRGB[0] * 255);
						cubeData[offset+1] = (byte) (newRGB[1] * 255);
						cubeData[offset+2] = (byte) (newRGB[2] * 255);
						cubeData[offset+3] = (byte) 255; //1.0;
						
						offset += 4;
					}
				}
			}
			return NSData.FromArray(cubeData);
		}
		/// <summary>
		/// https://github.com/vhbit/ColorCubeSample
		/// </summary>
		void rgbToHSV(float[] rgb, ref float[] hsv)
		{
			float min, max, delta;
			float r = rgb[0], g = rgb[1], b = rgb[2];
			
			min = Math.Min( r, Math.Min( g, b ));
			max = Math.Max( r, Math.Max( g, b ));
			hsv[2] = max;               // v
			delta = max - min;
			if( max != 0 )
				hsv[1] = delta / max;       // s
			else {
				// r = g = b = 0        // s = 0, v is undefined
				hsv[1] = 0;
				hsv[0] = -1;
				return;
			}
			if( r == max )
				hsv[0] = ( g - b ) / delta;     // between yellow & magenta
			else if( g == max )
				hsv[0] = 2 + ( b - r ) / delta; // between cyan & yellow
			else
				hsv[0] = 4 + ( r - g ) / delta; // between magenta & cyan
			hsv[0] *= 60;               // degrees
			if( hsv[0] < 0 )
				hsv[0] += 360;
			hsv[0] /= 360.0f;
		}
		/// <summary>
		/// https://github.com/vhbit/ColorCubeSample
		/// </summary>
		void hsvToRGB(float[] hsv, ref float[] rgb)
		{
			float C = hsv[2] * hsv[1];
			float HS = (float) hsv[0] * 6.0f;
			float X = (float)C * (1.0f - Math.Abs((HS % 2.0f) - 1.0f));
			
			if (HS >= 0 && HS < 1)
			{
				rgb[0] = C;
				rgb[1] = X;
				rgb[2] = 0;
			}
			else if (HS >= 1 && HS < 2)
			{
				rgb[0] = X;
				rgb[1] = C;
				rgb[2] = 0;
			}
			else if (HS >= 2 && HS < 3)
			{
				rgb[0] = 0;
				rgb[1] = C;
				rgb[2] = X;
			}
			else if (HS >= 3 && HS < 4)
			{
				rgb[0] = 0;
				rgb[1] = X;
				rgb[2] = C;
			}
			else if (HS >= 4 && HS < 5)
			{
				rgb[0] = X;
				rgb[1] = 0;
				rgb[2] = C;
			}
			else if (HS >= 5 && HS < 6)
			{
				rgb[0] = C;
				rgb[1] = 0;
				rgb[2] = X;
			}
			else {
				rgb[0] = 0.0f;
				rgb[1] = 0.0f;
				rgb[2] = 0.0f;
			}
			
			
			float m = hsv[2] - C;
			rgb[0] += m;
			rgb[1] += m;
			rgb[2] += m;
		}
		#endregion

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

		CIImage ColorMap ()
		{
			var map = new CIColorMap ()
			{
				Image = flower,
				GradientImage = flower
			};

			return map.OutputImage;
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
				Intensity = 1F, // Default 1
			};
			
			return monoChrome.OutputImage;
		}

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

		CIImage MaskToAlpha ()
		{
			var masktoalpha = new CIMaskToAlpha ()
			{
				Image = heron
			};

			return masktoalpha.OutputImage;
		}

		CIImage MaximumComponent ()
		{
			var maximumcomponent = new CIMaximumComponent ()
			{
				Image = flower
			};

			return maximumcomponent.OutputImage;
		}

		CIImage MinimumComponent ()
		{
			var minimumcomponent = new CIMinimumComponent ()
			{
				Image = flower
			};

			return minimumcomponent.OutputImage;
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

		#region CICategoryHalftoneEffect

		CIImage CircularScreen ()
		{
			var cilcular_screen = new CICircularScreen () {
				Image = flower
			};

			return cilcular_screen.OutputImage;
		}

		CIImage DotScreen ()
		{
			var dot_screen = new CIDotScreen () {
				Image = flower
			};

			return dot_screen.OutputImage;
		}

		CIImage HatchedScreen ()
		{
			var hatched_screen = new CIHatchedScreen () {
				Image = flower
			};

			return hatched_screen.OutputImage;
		}

		CIImage LineScreen ()
		{
			var line_screen = new CILineScreen () {
				Image = flower
			};

			return line_screen.OutputImage;
		}

		#endregion

		#region CICategorySharpen

		CIImage SharpenLuminance ()
		{
			var sharpen = new CISharpenLuminance ()
			{
				Image = heron
			};

			return sharpen.OutputImage;
		}

		CIImage UnsharpMask ()
		{
			var sharpen = new CIUnsharpMask ()
			{
				Image = heron
			};

			return sharpen.OutputImage;
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

		CIImage LanczosScaleTransform ()
		{
			var lanczos_scale_transform = new CILanczosScaleTransform () {
				Image = heron
			};

			return lanczos_scale_transform.OutputImage;
		}

		CIImage PerspectiveTransform ()
		{
			var perspective_transform = new CIPerspectiveTransform () {
				Image = heron
			};

			return perspective_transform.OutputImage;
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
		/// Darkens the background image samples to reflect the source image samples.
		/// </summary>
		/// <returns>
		/// The composite image
		/// </returns>
		[Filter]
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
		[Filter]
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

		#region CICategoryDistortionEffect

		CIImage CircleSplashDistortion ()
		{
			var distortion = new CICircleSplashDistortion () {
				Image = heron,
			};

			return Crop (distortion);
		}

		CIImage HoleDistortion ()
		{
			var distortion = new CIHoleDistortion () {
				Image = heron,
				Radius = 85
			};

			return distortion.OutputImage;
		}

		CIImage LightTunel ()
		{
			var lighttunel = new CILightTunnel () {
				Image = flower
			};

			return Crop (lighttunel);
		}

		CIImage PinchDistortion ()
		{
			var pinchdistortion = new CIPinchDistortion () {
				Image = flower
			};

			return pinchdistortion.OutputImage;
		}

		CIImage TwirlDistortion ()
		{
			var twirldistortion = new CITwirlDistortion () {
				Image = flower
			};

			return twirldistortion.OutputImage;
		}

		CIImage VortexDistortion ()
		{
			var vortexdistortion = new CIVortexDistortion () {
				Image = heron
			};

			return vortexdistortion.OutputImage;
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

		CIImage RandomGenerator ()
		{
			var random = new CIRandomGenerator ();
			return Crop (random);
		}

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

		CIImage BlendWithMask ()
		{
			var blend_with_mask = new CIBlendWithMask () {
				Image = heron,
				BackgroundImage = clouds,
				Mask = RandomGenerator ()
			};

			return blend_with_mask.OutputImage;
		}

		CIImage Bloom ()
		{
			var bloom = new CIBloom () {
				Image = flower
			};

			return bloom.OutputImage;
		}

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

		CIImage Pixellate ()
		{
			var pixellate = new CIGloom () {
				Image = flower
			};

			return pixellate.OutputImage;
		}

		#endregion

		#region CICategoryTileEffect

		CIImage AffineClamp ()
		{
			var affine_clamp = new CIAffineClamp () {
				Image = flower
			};

			return Crop (affine_clamp);
		}

		CIImage AffineTile ()
		{
			var affine_tile = new CIAffineTile () {
				Image = flower
			};

			return Crop (affine_tile);
		}

		CIImage EightfoldReflectedTile ()
		{
			var tile = new CIEightfoldReflectedTile () {
				Image = flower
			};

			return Crop (tile);
		}

		CIImage FourfoldReflectedTile ()
		{
			var tile = new CIFourfoldReflectedTile () {
				Image = flower
			};

			return Crop (tile);
		}

		CIImage FourfoldRotatedTile ()
		{
			var tile = new CIFourfoldRotatedTile () {
				Image = flower
			};

			return Crop (tile);
		}

		CIImage GlideReflectedTile ()
		{
			var tile = new CIGlideReflectedTile () {
				Image = flower
			};

			return Crop (tile);
		}

		CIImage PerspectiveTile ()
		{
			var tile = new CIPerspectiveTile () {
				Image = flower
			};

			return Crop (tile);
		}

		CIImage SixfoldReflectedTile ()
		{
			var tile = new CISixfoldReflectedTile () {
				Image = flower
			};

			return Crop (tile);
		}

		CIImage SixfoldRotatedTile ()
		{
			var tile = new CISixfoldRotatedTile () {
				Image = flower
			};

			return Crop (tile);
		}

		CIImage TriangleKaleidoscope ()
		{
			var kaleidoscope = new CITriangleKaleidoscope () {
				Image = heron
			};

			return Crop (kaleidoscope);
		}

		CIImage TwelvefoldReflectedTile ()
		{
			var tile = new CITwelvefoldReflectedTile () {
				Image = flower
			};

			return Crop (tile);
		}

		#endregion

		#region CICategoryTransition

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

		CIImage SwipeTransition ()
		{
			var transition = new CISwipeTransition ()
			{
				Image = heron,
				TargetImage = clouds,
				Time = 0.8f
			};

			return transition.OutputImage;
		}

		#endregion

		#endregion
	}

}

