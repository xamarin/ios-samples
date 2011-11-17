using System;
using MonoTouch.UIKit;
using OpenTK.Platform.iPhoneOS;
using OpenTK.Graphics.ES11;
using System.Drawing;
using MonoTouch.ObjCRuntime;
using MonoTouch.CoreAnimation;
using OpenTK.Graphics;
using MonoTouch.Foundation;
using MonoTouch.OpenGLES;

namespace LowLevelGLPaint
{
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
}

