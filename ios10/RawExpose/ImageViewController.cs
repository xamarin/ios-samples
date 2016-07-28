using System;

using UIKit;
using Foundation;
using GLKit;
using Photos;
using CoreImage;
using CoreGraphics;
using ObjCRuntime;
using OpenGLES;

using OpenTK.Graphics.ES30;
using static OpenTK.Graphics.ES30.GL;
using static CoreGraphics.CGAffineTransform;

namespace RawExpose
{
	// This UIViewController displays an image processed using the CoreImage CIRawFilter in a GLKView.
	// It also allows the user to perform simple edit, like adjusting exposure, temperature and tint.
	public partial class ImageViewController : UIViewController, IGLKViewDelegate
	{
		// Outlet to sliders used to edit the image.
		[Outlet ("exposureSlider")]
		UISlider ExposureSlider { get; set; }

		[Outlet ("tempSlider")]
		UISlider TempSlider { get; set; }

		[Outlet ("tintSlider")]
		UISlider TintSlider { get; set; }

		// View used to display the CoreImage output produced by the CIRawFilter.
		[Outlet("imageView")]
		public GLKView ImageView { get; set; }

		// Asset containing the image to render.
		public PHAsset Asset { get; set; }

		// Original values of temperature and tint from the image.
		float originalTemp;
		float originalTint;

		// CIRawFilter used to process the Raw image
		CIFilter ciRawFilter;

		// Size of the processed image.
		CGSize? imageNativeSize;

		// Context used to render the CIImage to display.
		CIContext ciContext;

		public ImageViewController (IntPtr handle)
			: base (handle)
		{
		}

		// On load, construct the CIRawFilter
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			var asset = Asset;
			if (asset == null)
				return;

			// Setup options to request original image.
			var options = new PHImageRequestOptions {
				Version = PHImageRequestOptionsVersion.Original,
				Synchronous = true
			};

			// Request the image data and UTI type for the image.
			PHImageManager.DefaultManager.RequestImageData (asset, options, (imageData, dataUTI, _, __) => {
				if (imageData == null || dataUTI == null)
					return;

				// Create a CIRawFilter from original image data.
				// UTI type is passed in to provide the CIRawFilter with a hint about the UTI type of the Raw file.
				//var rawOptions = [String (kCGImageSourceTypeIdentifierHint) : dataUTI ]
				var rawOptions = new NSMutableDictionary ();
				var imageIOLibrary = Dlfcn.dlopen ("/System/Library/Frameworks/ImageIO.framework/ImageIO", 0);
				var key = Dlfcn.GetIntPtr (imageIOLibrary, "kCGImageSourceTypeIdentifierHint");
				rawOptions.LowlevelSetObject (dataUTI, key);

				ciRawFilter = CIFilter.CreateRawFilter (imageData, rawOptions);
				if (ciRawFilter == null)
					return;

				// Get the native size of the image produced by the CIRawFilter.
				var sizeValue = ciRawFilter.ValueForKey (Keys.kCIOutputNativeSizeKey) as CIVector;
				if (sizeValue != null)
					imageNativeSize = new CGSize (sizeValue.X, sizeValue.Y);

				// Record the original value of the temperature, and setup the editing slider.
				var tempValue = (NSNumber)ciRawFilter.ValueForKey (Keys.kCIInputNeutralTemperatureKey);
				if (tempValue != null) {
					originalTemp = tempValue.FloatValue;
					TempSlider.SetValue (tempValue.FloatValue, animated: false);
				}

				// Record the original value of the tint, and setup the editing slider.
				var tintValue = (NSNumber)ciRawFilter.ValueForKey (Keys.kCIInputNeutralTintKey);
				if (tintValue != null) {
					originalTint = tintValue.FloatValue;
					TintSlider.SetValue (tintValue.FloatValue, animated: false);
				}
			});

			// Create EAGL context used to render the CIImage produced by the CIRawFilter to display.
			ImageView.Context = new EAGLContext (EAGLRenderingAPI.OpenGLES3);
			ciContext = CIContext.FromContext (ImageView.Context, new CIContextOptions { CIImageFormat = CIImage.FormatRGBAh });
		}

		#region Image edit actions

		// Resets the CIRawFilter to the original values.
		[Action ("resetSettings:")]
		void ResetSettings (UIBarButtonItem sender)
		{
			var filter = ciRawFilter;
			if (filter == null)
				return;

			filter.SetValueForKey (NSNumber.FromInt32 (0), CIFilterInputKey.EV);
			ExposureSlider.SetValue (0, animated: false);

			filter.SetValueForKey (NSNumber.FromFloat (originalTemp), Keys.kCIInputNeutralTemperatureKey);
			TempSlider.SetValue (originalTemp, animated: false);

			filter.SetValueForKey (NSNumber.FromFloat (originalTint), Keys.kCIInputNeutralTintKey);
			TintSlider.SetValue (originalTint, animated: false);

			ImageView.SetNeedsDisplay ();
		}

		// Adjust the exposure of the image
		[Action ("exposureAdjustedWithSender:")]
		void exposureAdjusted (UISlider sender)
		{
			var filter = ciRawFilter;
			if (filter == null)
				return;

			filter.SetValueForKey (NSNumber.FromFloat (sender.Value), CIFilterInputKey.EV);
			ImageView.SetNeedsDisplay ();
		}

		// Adjust the temperature of the image
		[Action("temperatureAdjustedWithSender:")]
		void temperatureAdjusted (UISlider sender)
		{
			var filter = ciRawFilter;
			if (filter == null)
				return;

			filter.SetValueForKey (NSNumber.FromFloat (sender.Value), Keys.kCIInputNeutralTemperatureKey);
			ImageView.SetNeedsDisplay ();
		}

		// Adjust the tint of the image
		[Action ("tintAdjustedWithSender:")]
		void tintAdjusted (UISlider sender)
		{
			var filter = ciRawFilter;
			if (filter == null)
				return;

			ciRawFilter.SetValueForKey (NSNumber.FromFloat (sender.Value), Keys.kCIInputNeutralTintKey);
			ImageView.SetNeedsDisplay ();
		}

		// Update the image when the device is rotated.
		public override void ViewWillTransitionToSize (CGSize toSize, IUIViewControllerTransitionCoordinator coordinator)
		{
			ImageView.SetNeedsDisplay ();
		}

		// Render image to display.
		public void DrawInRect (GLKView view, CGRect rect)
		{
			var context = ciContext;
			if (context == null)
				return;

			var filter = ciRawFilter;
			if (filter == null)
				return;

			var nativeSize = imageNativeSize;
			if (!nativeSize.HasValue)
				return;

			// OpenGLES drawing setup.
			ClearColor (0, 0, 0, 1);
			Clear (ClearBufferMask.ColorBufferBit);

			// Set the blend mode to "source over" so that CI will use that.
			Enable (EnableCap.Blend);
			BlendFunc (BlendingFactorSrc.One, BlendingFactorDest.OneMinusSrcAlpha);

			// Calculate scale to show the image at.
			var scaleTransform = MakeScale (view.ContentScaleFactor, view.ContentScaleFactor);
			var contentScaledRect = CGRectApplyAffineTransform (rect, scaleTransform);
			var scale = (float)Math.Min (contentScaledRect.Width / nativeSize.Value.Width, contentScaledRect.Height / nativeSize.Value.Height);

			// Set scale factor of the CIRawFilter to size it correctly for display.
			filter.SetValueForKey (NSNumber.FromFloat (scale), Keys.kCIInputScaleFactorKey);

			// Calculate rectangle to display image in.
			var displayRect = CGRectApplyAffineTransform (new CGRect (0, 0, nativeSize.Value.Width, nativeSize.Value.Height), MakeScale (scale, scale));

			// Ensure the image is centered.
			displayRect.X = (contentScaledRect.Width - displayRect.Width) / 2;
			displayRect.Y = (contentScaledRect.Height - displayRect.Height) / 2;

			var image = ciRawFilter.OutputImage;
			if (image == null)
				return;

			// Display the image scaled to fit.
			context.DrawImage (image, displayRect, image.Extent);
		}

		#endregion
	}

	// TODO: https://trello.com/c/cKoavtdL
	public static class Keys
	{
		static readonly IntPtr CoreImageLibrary = Dlfcn.dlopen ("/System/Library/Frameworks/CoreImage.framework/CoreImage", 0);

		public static NSString kCIOutputNativeSizeKey =  Dlfcn.GetStringConstant (CoreImageLibrary, "kCIOutputNativeSizeKey");
		public static NSString kCIInputNeutralTemperatureKey = Dlfcn.GetStringConstant (CoreImageLibrary, "kCIInputNeutralTemperatureKey");
		public static NSString kCIInputNeutralTintKey = Dlfcn.GetStringConstant (CoreImageLibrary, "kCIInputNeutralTintKey");
		public static NSString kCIInputScaleFactorKey = Dlfcn.GetStringConstant (CoreImageLibrary, "kCIInputScaleFactorKey");
	}
}
