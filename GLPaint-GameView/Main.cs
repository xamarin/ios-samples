#define LINQ
using System;
using System.Collections.Generic;
using System.Linq;
using Foundation	;
using UIKit;
using CoreGraphics;
using OpenTK.Graphics.ES11;

namespace GLPaintGameView
{
	public class Application
	{
		static void Main (string[] args)
		{
			UIApplication.Main (args);
		}
	}
	
	// The name AppDelegate is referenced in the MainWindow.xib file.
	public partial class AppDelegate : UIApplicationDelegate
	{
		const int PaletteHeight = 30;
		const int PaletteSize = 5;
		const int AccelerometerFrequency = 25;
		const float FilteringFactor = 0.1f;
		const float EraseAccelerationThreshold = 2.0f;

		static readonly TimeSpan MinEraseInterval = TimeSpan.FromSeconds (0.5);

		const float LeftMarginPadding = 10.0f;
		const float TopMarginPadding = 10.0f;
		const float RightMarginPadding = 10.0f;

		double[] myAccelerometer = new double [3];
		SoundEffect erasingSound = new SoundEffect (NSBundle.MainBundle.PathForResource ("Erase", "caf"));
		SoundEffect selectSound  = new SoundEffect (NSBundle.MainBundle.PathForResource ("Select", "caf"));
		DateTime lastTime;

		PaintingView drawingView;
		
		// This method is invoked when the application has loaded its UI and its ready to run
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			CGRect rect = UIScreen.MainScreen.ApplicationFrame;

			window.BackgroundColor = UIColor.Black;

			//Create the OpenGL drawing view and add it to the window
			drawingView = new PaintingView (new CGRect (rect.Location, rect.Size));
			window.AddSubview (drawingView);

			// Create a segmented control so that the user can choose the brush color.
			var images = new[] {
				UIImage.FromFile ("Images/Red.png"),
				UIImage.FromFile ("Images/Yellow.png"),
				UIImage.FromFile ("Images/Green.png"),
				UIImage.FromFile ("Images/Blue.png"),
				UIImage.FromFile ("Images/Purple.png")
			};
			if (UIDevice.CurrentDevice.CheckSystemVersion (7, 0)) {
				// we want the original colors, which is not the default iOS7 behaviour, so we need to
				// replace them with ones having the right UIImageRenderingMode
				for (int i = 0; i < images.Length; i++)
					images [i] = images [i].ImageWithRenderingMode (UIImageRenderingMode.AlwaysOriginal);
			}
			var segmentedControl = new UISegmentedControl (images);

			// Compute a rectangle that is positioned correctly for the segmented control you'll use as a brush color palette
			var frame = new CGRect (rect.X + LeftMarginPadding, rect.Height - PaletteHeight - TopMarginPadding,
				rect.Width - (LeftMarginPadding + RightMarginPadding), PaletteHeight);
			segmentedControl.Frame = frame;
			// When the user chooses a color, the method changeBrushColor: is called.
			segmentedControl.ValueChanged += ChangeBrushColor;
			// Make sure the color of the color complements the black background
			segmentedControl.TintColor = UIColor.DarkGray;
			// Set the third color (index values start at 0)
			segmentedControl.SelectedSegment = 2;

			// Add the control to the window
			window.AddSubview (segmentedControl);
			// Now that the control is added, you can release it
			// [segmentedControl release];

			float r, g, b;
			// Define a starting color
			HslToRgb (2.0f / PaletteSize, PaintingView.Saturation, PaintingView.Luminosity, out r, out g, out b);
			// Set the color using OpenGL
			GL.Color4 (r, g, b, PaintingView.BrushOpacity);

			
			// Look in the Info.plist file and you'll see the status bar is hidden
			// Set the style to black so it matches the background of the application
			app.SetStatusBarStyle (UIStatusBarStyle.Default, false);
			// Now show the status bar, but animate to the style.
			app.SetStatusBarHidden (false, true);

			//Configure and enable the accelerometer
			UIAccelerometer.SharedAccelerometer.UpdateInterval = 1.0f / AccelerometerFrequency;
			UIAccelerometer.SharedAccelerometer.Acceleration += OnAccelerated;
			
			//Show the window
			window.MakeKeyAndVisible ();
	
			return true;
		}
	
		// This method is required in iPhoneOS 3.0
		public override void OnActivated (UIApplication application)
		{
		}
		
		static void HslToRgb (float h, float s, float l, out float r, out float g, out float b)
		{
			// Check for saturation. If there isn't any just return the luminance value for each, which results in gray.
			if (s == 0.0) {
				r = l;
				g = l;
				b = l;
				return;
			}

			// Test for luminance and compute temporary values based on luminance and saturation
			float temp2;
			if (l < 0.5)
				temp2 = l * (1.0f + s);
			else
				temp2 = l + s - l * s;
			float temp1 = 2.0f * l - temp2;

			// Compute intermediate values based on hue
			float[] temp = {
				h + 1.0f / 3.0f,
				h,
				h - 1.0f / 3.0f,
			};
			for (int i = 0; i < temp.Length; ++i) {
				if (temp [i] < 0.0f)
					temp [i] += 1.0f;
				if (temp [i] > 1.0f)
					temp [i] -= 1.0f;

				if (6.0f * temp [i] < 1.0f)
					temp [i] = temp1 + (temp2 - temp1) * 6.0f * temp [i];
				else {
					if (2.0f * temp [i] < 1.0f)
						temp [i] = temp2;
					else {
						if (3.0f * temp [i] < 2.0f)
							temp [i] = temp1 + (temp2 - temp1) * ((2.0f / 3.0f) - temp [i]) * 6.0f;
						else
							temp [i] = temp1;
					}
				}
			}
			r = temp [0];
			g = temp [1];
			b = temp [2];
		}
		
		private void ChangeBrushColor (object sender, EventArgs e)
		{
			selectSound.Play ();

			float r, g, b;
			HslToRgb (((UISegmentedControl) sender).SelectedSegment / (float) PaletteSize,
					PaintingView.Saturation, PaintingView.Luminosity,
					out r, out g, out b);
			GL.Color4 (r, g, b, PaintingView.BrushOpacity);
		}

		private void OnAccelerated (object sender, UIAccelerometerEventArgs e)
		{
#if LINQ
			myAccelerometer = new[]{e.Acceleration.X, e.Acceleration.Y, e.Acceleration.Z}
				.Select((v, i) => v * FilteringFactor + myAccelerometer [i] * (1.0f - FilteringFactor))
				.ToArray ();
#else
			myAccelerometer [0] = e.Acceleration.X * FilteringFactor + myAccelerometer [0] * (1.0 - FilteringFactor);
			myAccelerometer [1] = e.Acceleration.Y * FilteringFactor + myAccelerometer [1] * (1.0 - FilteringFactor);
			myAccelerometer [2] = e.Acceleration.Z * FilteringFactor + myAccelerometer [2] * (1.0 - FilteringFactor);
#endif

			// Odd; ObjC always uses myAccelerometer[0], while 
			// I'd expect myAccelerometer[0 .. 2]
			var x = e.Acceleration.X - myAccelerometer [0];
			var y = e.Acceleration.Y - myAccelerometer [0];
			var z = e.Acceleration.Z - myAccelerometer [0];

			var length = Math.Sqrt (x * x + y * y + z * z);
			if (length >= EraseAccelerationThreshold && DateTime.Now > lastTime + MinEraseInterval) {
				erasingSound.Play ();
				drawingView.Erase ();
				lastTime = DateTime.Now;
			}
		}
	}
}

