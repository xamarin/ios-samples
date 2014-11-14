using System;
using OpenTK.Platform.iPhoneOS;
using Foundation;
using ObjCRuntime;
using CoreAnimation;
using OpenTK;
using OpenGLES;
using OpenTK.Graphics.ES11;

namespace OpenGLESSampleGameView
{
	public partial class EAGLView : iPhoneOSGameView 
	{

		[Export ("layerClass")]
		static Class LayerClass()
		{
			return iPhoneOSGameView.GetLayerClass ();
		}
	
		[Export ("initWithCoder:")]
		public EAGLView (NSCoder coder) : base (coder)
		{
			LayerRetainsBacking = false;
			LayerColorFormat    = EAGLColorFormat.RGBA8;
			ContextRenderingApi = EAGLRenderingAPI.OpenGLES1;
		}
	
		protected override void ConfigureLayer(CAEAGLLayer eaglLayer)
		{
			eaglLayer.Opaque = true;
		}
	
		protected override void OnRenderFrame(FrameEventArgs e)
		{
			base.OnRenderFrame (e);
				
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
	
			MakeCurrent();
			GL.Viewport (0, 0, Size.Width, Size.Height);
	
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
	
			SwapBuffers();
		}
	}
}