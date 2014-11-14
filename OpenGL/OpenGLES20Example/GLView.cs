using System;
using Foundation;
using UIKit;
using OpenGLES;
using OpenTK.Graphics.ES20;
using CoreAnimation;
using ObjCRuntime;

namespace OpenGLES20Example
{
	public class GLView : UIView
	{
		int backingWidth;
		int backingHeight;
		uint frameBuffer; 
		uint renderBuffer;
		uint depthBuffer;

		int animationFrameInterval;
		public int AnimationFrameInterval {
			get { return animationFrameInterval; }
			set {
				if (value >= 1) {
					animationFrameInterval = value;

					if (animating) {
						StopAnimation ();
						StartAnimation ();
					}
				}
			}
		}

		bool animating;
		EAGLContext context;
		CADisplayLink displayLink;

		public GLViewController Controller;

		[Export ("layerClass")]
		public static Class LayerClass ()
		{
			return new Class (typeof (CAEAGLLayer));
		}

		public GLView () : base ()
		{
			CAEAGLLayer eaglLayer = (CAEAGLLayer)Layer;
			eaglLayer.Opaque = true;

			context = new EAGLContext (EAGLRenderingAPI.OpenGLES2);

			if (context == null || !EAGLContext.SetCurrentContext (context))
				return;

			animating = false;
			AnimationFrameInterval = 2;
		}

		void createBuffers ()
		{
			GL.GenFramebuffers (1, out frameBuffer);
			GL.GenRenderbuffers (1, out renderBuffer);
			GL.BindFramebuffer (FramebufferTarget.Framebuffer, frameBuffer);
			GL.BindRenderbuffer (RenderbufferTarget.Renderbuffer, renderBuffer);
			context.RenderBufferStorage ((uint) All.Renderbuffer, (CAEAGLLayer) Layer);
			GL.FramebufferRenderbuffer (FramebufferTarget.Framebuffer, FramebufferSlot.ColorAttachment0, RenderbufferTarget.Renderbuffer, renderBuffer);
			GL.GetRenderbufferParameter (RenderbufferTarget.Renderbuffer, RenderbufferParameterName.RenderbufferWidth, out backingWidth);
			GL.GetRenderbufferParameter (RenderbufferTarget.Renderbuffer, RenderbufferParameterName.RenderbufferHeight, out backingHeight);

			GL.GenRenderbuffers (1, out depthBuffer);
			GL.BindRenderbuffer (RenderbufferTarget.Renderbuffer, depthBuffer);
			GL.RenderbufferStorage (RenderbufferTarget.Renderbuffer, RenderbufferInternalFormat.DepthComponent16, backingWidth, backingHeight);
			GL.FramebufferRenderbuffer (FramebufferTarget.Framebuffer, FramebufferSlot.DepthAttachment, RenderbufferTarget.Renderbuffer, depthBuffer);
		}

		void destroyBuffers ()
		{
			GL.DeleteFramebuffers (1, ref frameBuffer);
			frameBuffer = 0;
			GL.DeleteRenderbuffers (1, ref renderBuffer);
			renderBuffer = 0;
			GL.DeleteRenderbuffers (1, ref depthBuffer);
			depthBuffer = 0;
		}

		void drawView ()
		{
			GL.BindFramebuffer (FramebufferTarget.Framebuffer, frameBuffer);

			Controller.Draw ();

			GL.BindRenderbuffer (RenderbufferTarget.Renderbuffer, renderBuffer);
			context.PresentRenderBuffer ((uint) RenderbufferTarget.Renderbuffer);
		}

		public override void LayoutSubviews ()
		{
			EAGLContext.SetCurrentContext (context);

			destroyBuffers ();
			createBuffers ();
			drawView ();

			GL.BindRenderbuffer (RenderbufferTarget.Renderbuffer, renderBuffer);

			GL.GetRenderbufferParameter (RenderbufferTarget.Renderbuffer, RenderbufferParameterName.RenderbufferWidth, out backingWidth);
			GL.GetRenderbufferParameter (RenderbufferTarget.Renderbuffer, RenderbufferParameterName.RenderbufferHeight, out backingHeight);

			if (GL.CheckFramebufferStatus (FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
				Console.WriteLine (String.Format ("Failed to make complete framebuffer object {0}",
				                                  GL.CheckFramebufferStatus (FramebufferTarget.Framebuffer).ToString ()));

			GL.Viewport (0, 0, backingWidth, backingHeight);

			Controller.Setup ();
		}

		public void StartAnimation ()
		{
			if (!animating) {
				displayLink = CADisplayLink.Create (drawView);
				displayLink.FrameInterval = AnimationFrameInterval;
				displayLink.AddToRunLoop (NSRunLoop.Current, NSRunLoop.NSDefaultRunLoopMode);

				animating = true;
			}
		}

		public void StopAnimation ()
		{
			if (animating) {
				displayLink.Invalidate ();
				displayLink = null;

				animating = false;
			}
		}
	}
}

