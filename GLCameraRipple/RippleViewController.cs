using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.ES20;
using MonoTouch.GLKit;
using MonoTouch.OpenGLES;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.AVFoundation;
using MonoTouch.CoreVideo;
using System.IO;

namespace GLCameraRipple
{
	public class RippleViewController : GLKViewController {
		EAGLContext context;
		SizeF size;
		int meshFactor;
		NSString sessionPreset;
		CVOpenGLESTexture lumaTexture, chromaTexture;
		CVOpenGLESTextureCache videoTextureCache;
		
		//
		// OpenGL components
		//
		const int UNIFORM_Y = 0;
		const int UNIFORM_UV = 1;
		const int ATTRIB_VERTEX = 0;
    	const int ATTRIB_TEXCOORD = 1;

		int [] uniforms = new int [2];
		int program;
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			context = new EAGLContext (EAGLRenderingAPI.OpenGLES2);
			((GLKView)View).Context = context;
			PreferredFramesPerSecond = 60;
			size = UIScreen.MainScreen.Bounds.Size;
			View.ContentScaleFactor = UIScreen.MainScreen.Scale;
			
			if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad){
				meshFactor = 8;
				sessionPreset = AVCaptureSession.PresetiFrame1280x720;
			} else {
				meshFactor = 4;
				sessionPreset = AVCaptureSession.Preset640x480;
			}
			SetupGL ();
			SetupAVCapture ();
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
		
		void CleanupTextures ()
		{
			if (lumaTexture != null){
				lumaTexture.Dispose ();
				lumaTexture = null;
			}
			if (chromaTexture != null){
				chromaTexture.Dispose ();
				chromaTexture = null;
			}
			videoTextureCache.Flush (CVOptionFlags.None);
		}
		
		void SetupGL ()
		{
			EAGLContext.SetCurrentContext (context);
			LoadShaders ();
			GL.UseProgram (program);
			GL.Uniform1 (uniforms [UNIFORM_Y], 0);
			GL.Uniform1 (uniforms [UNIFORM_UV], 1);
		}
		
		void SetupAVCapture ()
		{
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
			if (!CompileShader (out vertShader, All.VertexShader, "Shaders/Shader.vsh")){
				Console.WriteLine ("Failed to compile vertex shader");
				return false;
			}
			if (!CompileShader (out fragShader, All.FragmentShader, "Shaders/Shader.fsh")){
				Console.WriteLine ("Failed to compile fragment shader");
				return false;
			}
				
			// Attach shaders
			GL.AttachShader (program, vertShader);
			GL.AttachShader (program, fragShader);
			
			// Bind attribtue locations
			GL.BindAttribLocation (program, ATTRIB_VERTEX, "position");
			GL.BindAttribLocation (program, ATTRIB_TEXCOORD, "texCoord");
			
			// Link the program
			if (!LinkProgram (program)){
				Console.WriteLine ("Failed to link the shader programs");
				GL.DeleteShader (vertShader);
				GL.DeleteShader (fragShader);
				GL.DeleteProgram (program);
				return false;
			}
			
			// Get uniform locations
			uniforms [UNIFORM_Y] = GL.GetUniformLocation (program, "SamplerY");
			uniforms [UNIFORM_UV] = GL.GetUniformLocation (program, "SampleUV");
			GL.DeleteShader (vertShader);
			GL.DeleteShader (fragShader);
			return true;
		}
		
		bool CompileShader (out int shader, All type, string path)
		{
			string shaderProgram = File.ReadAllText (path);
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
			GL.GetProgram (program, All.LinkStatus, ref status);
			return status != 0;
		}
	}
}

