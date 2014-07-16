using System;
using UIKit;
using CoreAnimation;
using OpenTK.Platform.iPhoneOS;
using Foundation;
using OpenTK.Graphics;
using OpenGLES;
using OpenTK.Graphics.ES11;
using ObjCRuntime;

namespace OpenGLESSample
{
	public partial class EAGLView : UIView 
	{
		int BackingWidth;
		int BackingHeight;
		iPhoneOSGraphicsContext Context;
		uint ViewRenderBuffer, ViewFrameBuffer;
		uint DepthRenderBuffer;
		NSTimer AnimationTimer;
		internal double AnimationInterval;
	
		const bool UseDepthBuffer = false;
	
		[Export ("layerClass")]
		public static Class LayerClass ()
		{
			return new Class (typeof (CAEAGLLayer));
		}
	
		[Export ("initWithCoder:")]
		public EAGLView (NSCoder coder) : base (coder)
		{
			CAEAGLLayer eaglLayer = (CAEAGLLayer) Layer;
			eaglLayer.Opaque = true;
			eaglLayer.DrawableProperties = NSDictionary.FromObjectsAndKeys (
				new NSObject []{NSNumber.FromBoolean(false),          EAGLColorFormat.RGBA8},
				new NSObject []{EAGLDrawableProperty.RetainedBacking, EAGLDrawableProperty.ColorFormat}
			);
			Context = (iPhoneOSGraphicsContext) ((IGraphicsContextInternal) GraphicsContext.CurrentContext).Implementation;
			
			Context.MakeCurrent(null);
			AnimationInterval = 1.0 / 60.0;
		}
	
		void DrawView ()
		{
			float[] squareVertices = {
				-0.5f, -0.5f,
				0.5f, -0.5f,
				-0.5f, 0.5f, 
				0.5f, 0.5f,
			};
			byte[] squareColors = {
				255, 255,   0, 255,
				0,   255, 255, 255,
				0,     0,    0,  0,
				255,   0,  255, 255,
			};
	
			Context.MakeCurrent(null);
			GL.Oes.BindFramebuffer (All.FramebufferOes, ViewFrameBuffer);
			GL.Viewport (0, 0, BackingWidth, BackingHeight);
	
			GL.MatrixMode (All.Projection);
			GL.LoadIdentity ();
			GL.Ortho (-1.0f, 1.0f, -1.5f, 1.5f, -1.0f, 1.0f);
			GL.MatrixMode (All.Modelview);
			GL.Rotate (3.0f, 0.0f, 0.0f, 1.0f);
			
			GL.ClearColor (0.5f, 0.5f, 0.5f, 1.0f);
			GL.Clear ((uint) All.ColorBufferBit);
	
			GL.VertexPointer (2, All.Float, 0, squareVertices);
			GL.EnableClientState (All.VertexArray);
			GL.ColorPointer (4, All.UnsignedByte, 0, squareColors);
			GL.EnableClientState (All.ColorArray);
			
			GL.DrawArrays (All.TriangleStrip, 0, 4);
	
			GL.Oes.BindRenderbuffer (All.RenderbufferOes, ViewRenderBuffer);
			Context.EAGLContext.PresentRenderBuffer ((uint) All.RenderbufferOes);
		}
	
		public override void LayoutSubviews ()
		{
			Context.MakeCurrent(null);
			DestroyFrameBuffer ();
			CreateFrameBuffer ();
			DrawView ();
		}
		
		bool CreateFrameBuffer ()
		{
			GL.Oes.GenFramebuffers (1, out ViewFrameBuffer);
			GL.Oes.GenRenderbuffers (1, out ViewRenderBuffer);
	
			GL.Oes.BindFramebuffer (All.FramebufferOes, ViewFrameBuffer);
			GL.Oes.BindRenderbuffer (All.RenderbufferOes, ViewRenderBuffer);
			Context.EAGLContext.RenderBufferStorage ((uint) All.RenderbufferOes, (CAEAGLLayer) Layer);
			GL.Oes.FramebufferRenderbuffer (All.FramebufferOes,
				All.ColorAttachment0Oes,
				All.RenderbufferOes,
				ViewRenderBuffer);
	
			GL.Oes.GetRenderbufferParameter (All.RenderbufferOes, All.RenderbufferWidthOes, out BackingWidth);
			GL.Oes.GetRenderbufferParameter (All.RenderbufferOes, All.RenderbufferHeightOes, out BackingHeight);
	
			if (UseDepthBuffer) {
				GL.Oes.GenRenderbuffers (1, out DepthRenderBuffer);
				GL.Oes.BindRenderbuffer (All.RenderbufferOes, DepthRenderBuffer);
				GL.Oes.RenderbufferStorage (All.RenderbufferOes, All.DepthComponent16Oes, BackingWidth, BackingHeight);
				GL.Oes.FramebufferRenderbuffer (All.FramebufferOes, All.DepthAttachmentOes, All.RenderbufferOes, DepthRenderBuffer);
			}
			if (GL.Oes.CheckFramebufferStatus (All.FramebufferOes) != All.FramebufferCompleteOes) {
				Console.Error.WriteLine("failed to make complete framebuffer object {0}",
					GL.Oes.CheckFramebufferStatus (All.FramebufferOes));
			}
			return true;
		}
	
		void DestroyFrameBuffer ()
		{
			GL.Oes.DeleteFramebuffers (1, ref ViewFrameBuffer);
			ViewFrameBuffer = 0;
			GL.Oes.DeleteRenderbuffers (1, ref ViewRenderBuffer);
			ViewRenderBuffer = 0;
	
			if (DepthRenderBuffer != 0) {
				GL.Oes.DeleteRenderbuffers (1, ref DepthRenderBuffer);
				DepthRenderBuffer = 0;
			}
		}
	
		public void StartAnimation ()
		{
			AnimationTimer = NSTimer.CreateRepeatingScheduledTimer (TimeSpan.FromSeconds (AnimationInterval), (d) => DrawView ());
		}
	
		public void StopAnimation ()
		{
			AnimationTimer = null;
		}
	
		public void SetAnimationTimer (NSTimer timer)
		{
			AnimationTimer.Invalidate ();
			AnimationTimer = timer;
		}
	
		public void SetAnimationInterval (double interval)
		{
			AnimationInterval = interval;
			if (AnimationTimer != null) {
				StopAnimation ();
				StartAnimation ();
			}
		}
	}
}

