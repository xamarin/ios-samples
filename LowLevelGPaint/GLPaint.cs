#define LINQ
using System;
using System.Drawing;
#if LINQ
using System.Linq;
#endif
using System.Runtime.InteropServices;
using MonoTouch.AudioToolbox;
using MonoTouch.CoreAnimation;
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
using MonoTouch.OpenGLES;
using MonoTouch.UIKit;
using MonoTouch.ObjCRuntime;
using OpenTK.Graphics;
using OpenTK.Graphics.ES11;
using OpenTK.Platform;
using OpenTK.Platform.iPhoneOS;

namespace MonoTouch.Samples.GLPaint {

	[Register]
	public class EAGLView : UIView {
		All _format;
		All _depthFormat;
		bool _autoResize;
		iPhoneOSGraphicsContext _context;
		uint _framebuffer;
		uint _renderbuffer;
		uint _depthbuffer;
		SizeF _size;
		bool _hasBeenCurrent;

		[Export ("layerClass")]
		public static Class LayerClass ()
		{
			return new Class (typeof (CAEAGLLayer));
		}

		public EAGLView (RectangleF frame)
			: this (frame, All.Rgb565Oes, 0, false)
		{
		}

		public EAGLView (RectangleF frame, All format)
			: this (frame, format, 0, false)
		{
		}

		public EAGLView (RectangleF frame, All format, All depth, bool retained) : base (frame)
		{
			CAEAGLLayer eaglLayer = (CAEAGLLayer) Layer;
			eaglLayer.DrawableProperties = NSDictionary.FromObjectsAndKeys (
				new NSObject [] {NSNumber.FromBoolean (true),           EAGLColorFormat.RGBA8},
				new NSObject [] {EAGLDrawableProperty.RetainedBacking,  EAGLDrawableProperty.ColorFormat}
			);
			_format = format;
			_depthFormat = depth;

			_context = (iPhoneOSGraphicsContext) ((IGraphicsContextInternal) GraphicsContext.CurrentContext).Implementation;
			CreateSurface ();
		}

		protected override void Dispose (bool disposing)
		{
			DestroySurface ();
			_context.Dispose();
			_context = null;
		}

		void CreateSurface ()
		{
			CAEAGLLayer eaglLayer = (CAEAGLLayer) Layer;
			if (!_context.IsCurrent)
				_context.MakeCurrent(null);

			var newSize = eaglLayer.Bounds.Size;
			newSize.Width  = (float) Math.Round (newSize.Width);
			newSize.Height = (float) Math.Round (newSize.Height);

			int oldRenderbuffer = 0, oldFramebuffer = 0;
			GL.GetInteger (All.RenderbufferBindingOes, ref oldRenderbuffer);
			GL.GetInteger (All.FramebufferBindingOes, ref oldFramebuffer);

			GL.Oes.GenRenderbuffers (1, ref _renderbuffer);
			GL.Oes.BindRenderbuffer (All.RenderbufferOes, _renderbuffer);

			if (!_context.EAGLContext.RenderBufferStorage ((uint) All.RenderbufferOes, eaglLayer)) {
				GL.Oes.DeleteRenderbuffers (1, ref _renderbuffer);
				GL.Oes.BindRenderbuffer (All.RenderbufferBindingOes, (uint) oldRenderbuffer);
				throw new InvalidOperationException ("Error with RenderbufferStorage()!");
			}

			GL.Oes.GenFramebuffers (1, ref _framebuffer);
			GL.Oes.BindFramebuffer (All.FramebufferOes, _framebuffer);
			GL.Oes.FramebufferRenderbuffer (All.FramebufferOes, All.ColorAttachment0Oes, All.RenderbufferOes, _renderbuffer);
			if (_depthFormat != 0) {
				GL.Oes.GenRenderbuffers (1, ref _depthbuffer);
				GL.Oes.BindFramebuffer (All.RenderbufferOes, _depthbuffer);
				GL.Oes.RenderbufferStorage (All.RenderbufferOes, _depthFormat, (int) newSize.Width, (int) newSize.Height);
				GL.Oes.FramebufferRenderbuffer (All.FramebufferOes, All.DepthAttachmentOes, All.RenderbufferOes, _depthbuffer);
			}
			_size = newSize;
			if (!_hasBeenCurrent) {
				GL.Viewport (0, 0, (int) newSize.Width, (int) newSize.Height);
				GL.Scissor (0, 0, (int) newSize.Width, (int) newSize.Height);
				_hasBeenCurrent = true;
			}
			else
				GL.Oes.BindFramebuffer (All.FramebufferOes, (uint) oldFramebuffer);
			GL.Oes.BindRenderbuffer (All.RenderbufferOes, (uint) oldRenderbuffer);

			Action<EAGLView> a = OnResized;
			if (a != null)
				a (this);
		}

		void DestroySurface ()
		{
			EAGLContext oldContext = EAGLContext.CurrentContext;

			if (!_context.IsCurrent)
				_context.MakeCurrent(null);

			if (_depthFormat != 0) {
				GL.Oes.DeleteRenderbuffers (1, ref _depthbuffer);
				_depthbuffer = 0;
			}

			GL.Oes.DeleteRenderbuffers (1, ref _renderbuffer);
			_renderbuffer = 0;

			GL.Oes.DeleteFramebuffers (1, ref _framebuffer);
			_framebuffer = 0;

			EAGLContext.SetCurrentContext (oldContext);
		}

		public override void LayoutSubviews ()
		{
			var bounds = Bounds;
			if (_autoResize && ((float) Math.Round (bounds.Width) != _size.Width) ||
					((float) Math.Round (bounds.Height) != _size.Height)) {
				DestroySurface ();
				CreateSurface ();
			}
		}

		void SetAutoResizesEaglSurface (bool resize)
		{
			_autoResize = resize;
			if (_autoResize)
				LayoutSubviews ();
		}

		public void SetCurrentContext ()
		{
			_context.MakeCurrent(null);
		}

		public bool IsCurrentContext {
			get {return _context.IsCurrent;}
		}

		public void ClearCurrentContext ()
		{
			if (!EAGLContext.SetCurrentContext (null))
				Console.WriteLine ("Failed to clear current context!");
		}

		public void SwapBuffers ()
		{
			EAGLContext oldContext = EAGLContext.CurrentContext;

			if (!_context.IsCurrent)
				_context.MakeCurrent(null);

			int oldRenderbuffer = 0;
			GL.GetInteger (All.RenderbufferBindingOes, ref oldRenderbuffer);
			GL.Oes.BindRenderbuffer (All.RenderbufferOes, _renderbuffer);

			if (!_context.EAGLContext.PresentRenderBuffer ((uint) All.RenderbufferOes))
				Console.WriteLine ("Failed to swap renderbuffer!");

			EAGLContext.SetCurrentContext (oldContext);
		}

		public PointF ConvertPointFromViewToSurface (PointF point)
		{
			var bounds = Bounds;
			return new PointF ((point.X - bounds.X) / bounds.Width * _size.Width,
					(point.Y - bounds.Y) / bounds.Height * _size.Height);
		}

		public RectangleF ConvertRectFromViewToSurface (RectangleF rect)
		{
			var bounds = Bounds;
			return new RectangleF (
					(rect.X - bounds.X) / bounds.Width * _size.Width,
					(rect.Y - bounds.Y) / bounds.Height * _size.Height,
					rect.Width / bounds.Width * _size.Width,
					rect.Height / bounds.Height * _size.Height);
		}

		public event Action<EAGLView> OnResized;
	}

	[Register]
	class PaintingView : EAGLView {

		public const float BrushOpacity = 1.0f / 3.0f;
		public const int BrushPixelStep = 3;
		public const int BrushScale = 2;
		public const float Luminosity = 0.75f;
		public const float Saturation = 1.0f;

		uint brushTexture, drawingTexture, drawingFramebuffer;
		bool firstTouch;

		PointF Location;
		PointF PreviousLocation;
		
		[Export ("layerClass")]
		public static Class LayerClass ()
		{
			return new Class (typeof (CAEAGLLayer));
		}

		public PaintingView (RectangleF frame)
			: base (frame, All.Rgb565Oes, 0, true)
		{
			SetCurrentContext ();
			var brushImage = UIImage.FromFile ("Particle.png").CGImage;
			var width = brushImage.Width;
			var height = brushImage.Height;
			if (brushImage != null) {
				IntPtr brushData = Marshal.AllocHGlobal (width * height * 4);
				if (brushData == IntPtr.Zero)
					throw new OutOfMemoryException ();
				try {
					using (var brushContext = new CGBitmapContext (brushData,
							width, width, 8, width * 4, brushImage.ColorSpace, CGImageAlphaInfo.PremultipliedLast)) {
						brushContext.DrawImage (new RectangleF (0.0f, 0.0f, (float) width, (float) height), brushImage);
					}

					GL.GenTextures (1, ref brushTexture);
					GL.BindTexture (All.Texture2D, brushTexture);
					GL.TexImage2D (All.Texture2D, 0, (int) All.Rgba, width, height, 0, All.Rgba, All.UnsignedByte, brushData);
				}
				finally {
					Marshal.FreeHGlobal (brushData);
				}
				GL.TexParameter (All.Texture2D, All.TextureMinFilter, (int) All.Linear);
				GL.Enable (All.Texture2D);
				GL.BlendFunc (All.SrcAlpha, All.One);
				GL.Enable (All.Blend);
			}
			GL.Disable (All.Dither);
			GL.MatrixMode (All.Projection);
			GL.Ortho (0, frame.Width, 0, frame.Height, -1, 1);
			GL.MatrixMode (All.Modelview);
			GL.Enable (All.Texture2D);
			GL.EnableClientState (All.VertexArray);
			GL.Enable (All.Blend);
			GL.BlendFunc (All.SrcAlpha, All.One);
			GL.Enable (All.PointSpriteOes);
			GL.TexEnv (All.PointSpriteOes, All.CoordReplaceOes, (float) All.True);
			GL.PointSize (width / BrushScale);

			Erase ();

			PerformSelector (new Selector ("playback"), null, 0.2f);
		}

		~PaintingView ()
		{
			Dispose (false);
		}

		protected override void Dispose (bool disposing)
		{
			GL.Oes.DeleteFramebuffers (1, ref drawingFramebuffer);
			GL.DeleteTextures (1, ref drawingTexture);
		}

		public void Erase ()
		{
			GL.Clear ((uint) All.ColorBufferBit);

			SwapBuffers ();
		}

		float[] vertexBuffer;
		int vertexMax = 64;

		private void RenderLineFromPoint (PointF start, PointF end)
		{
			int vertexCount = 0;
			if (vertexBuffer == null) {
				vertexBuffer = new float [vertexMax * 2];
			}
			var count = Math.Max (Math.Ceiling (Math.Sqrt ((end.X - start.X) * (end.X - start.X) + (end.Y - start.Y) * (end.Y - start.Y)) / BrushPixelStep),
					1);
			for (int i = 0; i < count; ++i, ++vertexCount) {
				if (vertexCount == vertexMax) {
					vertexMax *= 2;
					Array.Resize (ref vertexBuffer, vertexMax * 2);
				}
				vertexBuffer [2 * vertexCount + 0] = start.X + (end.X - start.X) * (float) i / (float) count;
				vertexBuffer [2 * vertexCount + 1] = start.Y + (end.Y - start.Y) * (float) i / (float) count;
			}
			GL.VertexPointer (2, All.Float, 0, vertexBuffer);
			GL.DrawArrays (All.Points, 0, vertexCount);

			SwapBuffers ();
		}

		int dataofs = 0;

		[Export ("playback")]
		void Playback ()
		{
			PointF [] points = ShakeMe.Data [dataofs];

			for (int i = 0; i < points.Length - 1; i++)
				RenderLineFromPoint (points [i], points [i + 1]);

			if (dataofs < ShakeMe.Data.Count - 1) {
				dataofs ++;
				PerformSelector (new Selector ("playback"), null, 0.01f);
			}
		}

		public override void TouchesBegan (MonoTouch.Foundation.NSSet touches, MonoTouch.UIKit.UIEvent e)
		{
			var bounds = Bounds;
			var touch = (UITouch) e.TouchesForView (this).AnyObject;
			firstTouch = true;
			Location = touch.LocationInView (this);
			Location.Y = bounds.Height - Location.Y;
		}

		public override void TouchesMoved (MonoTouch.Foundation.NSSet touches, MonoTouch.UIKit.UIEvent e)
		{
			var bounds = Bounds;
			var touch = (UITouch) e.TouchesForView (this).AnyObject;

			if (firstTouch) {
				firstTouch = false;
				PreviousLocation = touch.PreviousLocationInView (this);
				PreviousLocation.Y = bounds.Height - PreviousLocation.Y;
			}
			else {
				Location = touch.LocationInView (this);
				Location.Y = bounds.Height - Location.Y;
				PreviousLocation = touch.PreviousLocationInView (this);
				PreviousLocation.Y = bounds.Height - PreviousLocation.Y;
			}
			RenderLineFromPoint (PreviousLocation, Location);
		}

		public override void TouchesEnded (MonoTouch.Foundation.NSSet touches, MonoTouch.UIKit.UIEvent e)
		{
			var bounds = Bounds;
			var touch = (UITouch) e.TouchesForView (this).AnyObject;
			if (firstTouch) {
				firstTouch = false;
				PreviousLocation = touch.PreviousLocationInView (this);
				PreviousLocation.Y = bounds.Height - PreviousLocation.Y;
				RenderLineFromPoint (PreviousLocation, Location);
			}
		}

		public override void TouchesCancelled (MonoTouch.Foundation.NSSet touches, MonoTouch.UIKit.UIEvent e)
		{
		}
	}

	[Register]
	class SoundEffect : NSObject {
		SystemSound sound;

		public SoundEffect (string path)
		{
			sound = SystemSound.FromFile (new NSUrl (path, false));
		}

		protected override void Dispose (bool disposing)
		{
			((IDisposable) sound).Dispose ();
			sound = null;
		}

		public void Play ()
		{
			sound.PlaySystemSound ();
		}
	}

	[Register ("AppDelegate")]
	class AppDelegate : UIApplicationDelegate {

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
		UIWindow window;

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

		public override void FinishedLaunching (UIApplication app)
		{
			RectangleF rect = UIScreen.MainScreen.ApplicationFrame;

			//Create a full-screen window
			window = new UIWindow (UIScreen.MainScreen.Bounds);
			window.BackgroundColor = UIColor.Black;

			//Create the OpenGL drawing view and add it to the window
			drawingView = new PaintingView (new RectangleF (rect.Location, rect.Size));
			window.AddSubview (drawingView);

			// Create a segmented control so that the user can choose the brush color.
			UISegmentedControl segmentedControl = new UISegmentedControl (new[]{
					UIImage.FromFile ("Red.png"),
					UIImage.FromFile ("Yellow.png"),
					UIImage.FromFile ("Green.png"),
					UIImage.FromFile ("Blue.png"),
					UIImage.FromFile ("Purple.png"),
			});

			// Compute a rectangle that is positioned correctly for the segmented control you'll use as a brush color palette
			RectangleF frame = new RectangleF (rect.X + LeftMarginPadding, rect.Height - PaletteHeight - TopMarginPadding,
				rect.Width - (LeftMarginPadding + RightMarginPadding), PaletteHeight);
			segmentedControl.Frame = frame;
			// When the user chooses a color, the method changeBrushColor: is called.
			segmentedControl.ValueChanged += ChangeBrushColor;
			segmentedControl.ControlStyle = UISegmentedControlStyle.Bar;
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

			//Show the window
			window.MakeKeyAndVisible ();
			// Look in the Info.plist file and you'll see the status bar is hidden
			// Set the style to black so it matches the background of the application
			app.SetStatusBarStyle (UIStatusBarStyle.BlackTranslucent, false);
			// Now show the status bar, but animate to the style.
			app.SetStatusBarHidden (false, true);

			//Configure and enable the accelerometer
			UIAccelerometer.SharedAccelerometer.UpdateInterval = 1.0f / AccelerometerFrequency;
			UIAccelerometer.SharedAccelerometer.Acceleration += OnAccelerated;
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

		public override void OnResignActivation (UIApplication app)
		{
		}

		public override void OnActivated (UIApplication app)
		{
		}

		static void Main (string [] args)
		{
			using (var c = Utilities.CreateGraphicsContext(EAGLRenderingAPI.OpenGLES1)) {
				Console.WriteLine ("Launching");
				UIApplication.Main (args, null, "AppDelegate");
				Console.WriteLine ("Returning from Main, this sucks");
			}
		}
	}
}
