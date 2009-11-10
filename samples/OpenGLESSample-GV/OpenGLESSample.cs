using System;
using System.Drawing;
using MonoTouch.CoreAnimation;
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
using MonoTouch.OpenGLES;
using MonoTouch.UIKit;
using MonoTouch.ObjCRuntime;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.ES11;
using OpenTK.Platform;
using OpenTK.Platform.iPhoneOS;

public class EAGLView : iPhoneOSGameView {

	[Export ("layerClass")]
	static Class LayerClass()
	{
		return iPhoneOSGameView.GetLayerClass ();
	}

	[Export ("initWithCoder:")]
	public EAGLView (NSCoder coder)
		: base (coder)
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

[Register]
public class OpenGLESSampleAppDelegate : UIApplicationDelegate {

	[Connect]
	public EAGLView glView {
		get {return (EAGLView) GetNativeField ("glView");}
		set {SetNativeField("glView", value);}
	}
	
	[Connect]
	public UIWindow window {
		get {return (UIWindow) GetNativeField ("window");}
		set {SetNativeField("window", value);}
	}

	public override void FinishedLaunching (UIApplication app)
	{
		glView.Run(60.0);
	}

	public override void OnResignActivation (UIApplication app)
	{
		glView.Stop();
		glView.Run(5.0);
	}

	public override void OnActivated (UIApplication app)
	{
		glView.Stop();
		glView.Run(60.0);
	}

}

class Demo {
	static void Main (string [] args)
	{
		Console.WriteLine ("Launching");
		UIApplication.Main (args);
		Console.WriteLine ("Returning from Main, this sucks");
	}
}
