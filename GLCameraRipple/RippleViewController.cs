using System;
using System.Collections.Generic;
using CoreGraphics;
using OpenTK.Graphics;
using OpenTK.Graphics.ES20;
using GLKit;
using OpenGLES;
using Foundation;
using CoreFoundation;
using CoreMedia;
using UIKit;
using AVFoundation;
using CoreVideo;

namespace GLCameraRipple
{
	public class RippleViewController : GLKViewController {
		DataOutputDelegate dataOutputDelegate;
		EAGLContext context;
		CVOpenGLESTextureCache videoTextureCache;
		AVCaptureSession session;
		GLKView glkView;
		RippleModel ripple;
		int meshFactor;
		CGSize size;

		//
		// OpenGL components
		//
		const int UNIFORM_Y = 0;
		const int UNIFORM_UV = 1;
		const int ATTRIB_VERTEX = 0;
    	const int ATTRIB_TEXCOORD = 1;
		int [] uniforms = new int [2];
		int program;
		int indexVbo, positionVbo, texcoordVbo;
				
		public override void ViewDidLoad ()
		{
			bool isPad = UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad;
			base.ViewDidLoad ();

			context = new EAGLContext (EAGLRenderingAPI.OpenGLES2);
			glkView = (GLKView) View;
			glkView.Context = context;
			glkView.MultipleTouchEnabled = true;
			glkView.DrawInRect += Draw;
			
			PreferredFramesPerSecond = 60;
			size = UIScreen.MainScreen.Bounds.Size.ToSize ();
			View.ContentScaleFactor = UIScreen.MainScreen.Scale;
			
			meshFactor = isPad ? 8 : 4;
			SetupGL ();
			SetupAVCapture (isPad ? AVCaptureSession.PresetiFrame1280x720 : AVCaptureSession.Preset640x480);
		}

		public override UIStatusBarStyle PreferredStatusBarStyle ()
		{
			return UIStatusBarStyle.LightContent;
		}

		void Draw (object sender, GLKViewDrawEventArgs args)
		{
			GL.Clear ((int)All.ColorBufferBit);
			if (ripple != null)
				GL.DrawElements (All.TriangleStrip, ripple.IndexCount, All.UnsignedShort, IntPtr.Zero);
		}
		
		void ProcessTouches (NSSet touches)
		{
			if (ripple == null)
				return;
			
			foreach (UITouch touch in touches.ToArray<UITouch> ())
				ripple.InitiateRippleAtLocation (touch.LocationInView (touch.View));
		}
		
		public override void TouchesBegan (NSSet touches, UIEvent evt)
		{
			ProcessTouches (touches);
		}

		public override void TouchesMoved (NSSet touches, UIEvent evt)
		{
			ProcessTouches (touches);
		}
		
		public override void Update ()
		{
			if (ripple != null){
				ripple.RunSimulation ();
				GL.BufferData (BufferTarget.ArrayBuffer, (IntPtr) ripple.VertexSize, ripple.TexCoords, BufferUsage.DynamicDraw);
			}
		}
		
		public override void ViewDidUnload ()
		{
			base.ViewDidUnload ();
			TeardownAVCapture ();
			TeardownGL ();
			if (EAGLContext.CurrentContext == context)
				EAGLContext.SetCurrentContext (null);
		}
		
		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			// Camera is fixed, only allow portrait
			return (toInterfaceOrientation == UIInterfaceOrientation.Portrait);
		}
		
		void SetupGL ()
		{
			EAGLContext.SetCurrentContext (context);
			if (LoadShaders ()){
				GL.UseProgram (program);
				GL.Uniform1 (uniforms [UNIFORM_Y], 0);
				GL.Uniform1 (uniforms [UNIFORM_UV], 1);
			}
		}
		
		void SetupAVCapture (NSString sessionPreset)
		{
			if ((videoTextureCache = CVOpenGLESTextureCache.FromEAGLContext (context)) == null){
				Console.WriteLine ("Could not create the CoreVideo TextureCache");
				return;
			}
			session = new AVCaptureSession ();
			session.BeginConfiguration ();
			
			// Preset size
			session.SessionPreset = sessionPreset;
			
			// Input device
			var videoDevice = AVCaptureDevice.DefaultDeviceWithMediaType (AVMediaType.Video);
			if (videoDevice == null){
				Console.WriteLine ("No video device");
				return;
			}
			NSError err;
			var input = new AVCaptureDeviceInput (videoDevice, out err);
			if (err != null){
				Console.WriteLine ("Error creating video capture device");
				return;
			}
			session.AddInput (input);
			
			// Create the output device
			var dataOutput = new AVCaptureVideoDataOutput () {
				AlwaysDiscardsLateVideoFrames = true,

				
				// YUV 420, use "BiPlanar" to split the Y and UV planes in two separate blocks of 
				// memory, then we can index 0 to get the Y and 1 for the UV planes in the frame decoding
				//VideoSettings = new AVVideoSettings (CVPixelFormatType.CV420YpCbCr8BiPlanarFullRange)
			};
					
			dataOutputDelegate = new DataOutputDelegate (this);

			// 
			// This dispatches the video frames into the main thread, because the OpenGL
			// code is accessing the data synchronously.
			//
			dataOutput.SetSampleBufferDelegateQueue (dataOutputDelegate, DispatchQueue.MainQueue);
			session.AddOutput (dataOutput);
			session.CommitConfiguration ();
			session.StartRunning ();
		}
		
		void TeardownAVCapture ()
		{
		}
		
		void TeardownGL ()
		{
		}
		
		bool LoadShaders ()
		{
			int vertShader, fragShader;
			
			program = GL.CreateProgram ();
			if (CompileShader (out vertShader, All.VertexShader, "Shaders/Shader.vsh")){
				if (CompileShader (out fragShader, All.FragmentShader, "Shaders/Shader.fsh")){
					// Attach shaders
					GL.AttachShader (program, vertShader);
					GL.AttachShader (program, fragShader);
					
					// Bind attribtue locations
					GL.BindAttribLocation (program, ATTRIB_VERTEX, "position");
					GL.BindAttribLocation (program, ATTRIB_TEXCOORD, "texCoord");

					if (LinkProgram (program)){
						// Get uniform locations
						uniforms [UNIFORM_Y] = GL.GetUniformLocation (program, "SamplerY");
						uniforms [UNIFORM_UV] = GL.GetUniformLocation (program, "SamplerUV");
						
						// Delete these ones, we do not need them anymore
						GL.DeleteShader (vertShader);
						GL.DeleteShader (fragShader);
						return true;
					} else {
						Console.WriteLine ("Failed to link the shader programs");
						GL.DeleteProgram (program);
						program = 0;
					}
				} else
					Console.WriteLine ("Failed to compile fragment shader");
				GL.DeleteShader (vertShader);
			} else 
				Console.WriteLine ("Failed to compile vertex shader");
			GL.DeleteProgram (program);
			return false;
		}
		
		bool CompileShader (out int shader, All type, string path)
		{
			string shaderProgram = System.IO.File.ReadAllText (path);
			int len = shaderProgram.Length, status = 0;
			shader = GL.CreateShader (type);
			
			GL.ShaderSource (shader, 1, new string [] { shaderProgram }, ref len);
			GL.CompileShader (shader);
			GL.GetShader (shader, All.CompileStatus, ref status);
			if (status == 0){
				GL.DeleteShader (shader);
				return false;
			}
			return true;
		}
		
		bool LinkProgram (int program)
		{
			GL.LinkProgram (program);
			int status = 0;
			int len = 0;
			GL.GetProgram (program, ProgramParameter.LinkStatus, out status);
			if (status == 0){
				GL.GetProgram (program, ProgramParameter.InfoLogLength, out len);
				var sb = new System.Text.StringBuilder (len);
				GL.GetProgramInfoLog (program, len, out len, sb);
				Console.WriteLine ("Link error: {0}", sb);
			}
			return status != 0;
		}
		
		unsafe void SetupBuffers ()
		{
			GL.GenBuffers (1, out indexVbo);
			GL.BindBuffer (BufferTarget.ElementArrayBuffer, indexVbo);
			GL.BufferData (BufferTarget.ElementArrayBuffer, (IntPtr) ripple.IndexSize, ripple.Indices, BufferUsage.StaticDraw);
			
			GL.GenBuffers (1, out positionVbo);
			GL.BindBuffer (BufferTarget.ArrayBuffer, positionVbo);
			GL.BufferData (BufferTarget.ArrayBuffer, (IntPtr) ripple.VertexSize, ripple.Vertices, BufferUsage.StaticDraw);
			
			GL.EnableVertexAttribArray (ATTRIB_VERTEX);
			
			GL.VertexAttribPointer (ATTRIB_VERTEX, 2, VertexAttribPointerType.Float, false, 2*sizeof(float), IntPtr.Zero);
			GL.GenBuffers (1, out texcoordVbo);
			GL.BindBuffer (BufferTarget.ArrayBuffer, texcoordVbo);
			GL.BufferData (BufferTarget.ArrayBuffer, (IntPtr) ripple.VertexSize, ripple.TexCoords, BufferUsage.DynamicDraw);
			
			GL.EnableVertexAttribArray (ATTRIB_TEXCOORD);
			GL.VertexAttribPointer (ATTRIB_TEXCOORD, 2, VertexAttribPointerType.Float, false, 2*sizeof (float), IntPtr.Zero);
		}
			  
		void SetupRipple (int width, int height)
		{			
			ripple = new RippleModel (size, meshFactor, 5, new CGSize (width, height));
			SetupBuffers ();
		}
		
		class DataOutputDelegate : AVCaptureVideoDataOutputSampleBufferDelegate {
			CVOpenGLESTexture lumaTexture, chromaTexture;
			RippleViewController container;
			int textureWidth, textureHeight;
			
			public DataOutputDelegate (RippleViewController container)
			{
				this.container = container;
			}
			
			void CleanupTextures ()
			{
				if (lumaTexture != null)
					lumaTexture.Dispose ();
				if (chromaTexture != null)
					chromaTexture.Dispose ();
				container.videoTextureCache.Flush (CVOptionFlags.None);
			}
		
			public override void DidOutputSampleBuffer (AVCaptureOutput captureOutput, CoreMedia.CMSampleBuffer sampleBuffer, AVCaptureConnection connection)
			{
				try {
					using (var pixelBuffer = sampleBuffer.GetImageBuffer () as CVPixelBuffer){	
						int width = (int)pixelBuffer.Width;
						int height = (int)pixelBuffer.Height;
					
						if (container.ripple == null || width != textureWidth || height != textureHeight){
							textureWidth = width;
							textureHeight = height;
							container.SetupRipple (textureWidth, textureHeight);
						}
						CleanupTextures ();
						
						// Y-plane
						GL.ActiveTexture (All.Texture0);
						All re = (All) 0x1903; // GL_RED_EXT, RED component from ARB OpenGL extension
						CVReturn status;
						lumaTexture = container.videoTextureCache.TextureFromImage (pixelBuffer, true, re, textureWidth, textureHeight, re, DataType.UnsignedByte, 0, out status);
						
						if (lumaTexture == null){
							Console.WriteLine ("Error creating luma texture: {0}", status);
							return;
						}
						GL.BindTexture ((All)lumaTexture.Target, lumaTexture.Name);
						GL.TexParameter (All.Texture2D, All.TextureWrapS, (int) All.ClampToEdge);
						GL.TexParameter (All.Texture2D, All.TextureWrapT, (int) All.ClampToEdge);
						
						// UV Plane
						GL.ActiveTexture (All.Texture1);
						re = (All) 0x8227; // GL_RG_EXT, RED GREEN component from ARB OpenGL extension
						chromaTexture = container.videoTextureCache.TextureFromImage (pixelBuffer, true, re, textureWidth/2, textureHeight/2, re, DataType.UnsignedByte, 1, out status);
						
						if (chromaTexture == null){
							Console.WriteLine ("Error creating chroma texture: {0}", status);
							return;
						}
						GL.BindTexture ((All) chromaTexture.Target, chromaTexture.Name);
						GL.TexParameter (All.Texture2D, All.TextureWrapS, (int)All.ClampToEdge);
						GL.TexParameter (All.Texture2D, All.TextureWrapT, (int) All.ClampToEdge);
					}
				} finally {
					sampleBuffer.Dispose ();
				}
			}
		}
	}
}