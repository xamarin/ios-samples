using System;
using MonoTouch.UIKit;
using OpenTK.Platform.iPhoneOS;
using MonoTouch.Foundation;
using MonoTouch.CoreAnimation;
using OpenTK.Graphics;
using MonoTouch.OpenGLES;
using OpenTK.Graphics.ES20;
using MonoTouch.ObjCRuntime;
using MonoTouch.CoreVideo;
using System.Drawing;
using MonoTouch.GLKit;
using System.IO;

namespace RosyWriter
{
	public class RosyWriterPreviewWindow: UIView
	{
		EAGLContext Context;
		CVOpenGLESTextureCache videoTextureCache;
		uint FrameBuffer, ColorBuffer;
		int renderBufferWidth, renderBufferHeight;
		internal double AnimationInterval;
		const bool UseDepthBuffer = false;
		
		// Open GL Stuff
		const int UNIFORM_Y = 0;
		const int UNIFORM_UV = 1;
		const int ATTRIB_VERTEX = 0;
		const int ATTRIB_TEXCOORD = 1;
		int glProgram;
		
		[Export ("initWithFrame:")]
		public RosyWriterPreviewWindow (RectangleF frame) : base(frame)
		{
			// Use 2x scale factor on Retina dispalys.
			ContentScaleFactor = UIScreen.MainScreen.Scale;
			
			// Initialize OpenGL ES 2
			CAEAGLLayer eagleLayer = (CAEAGLLayer)Layer;
			eagleLayer.Opaque = true;
			eagleLayer.DrawableProperties = NSDictionary.FromObjectsAndKeys (
				new object[] { NSNumber.FromBoolean (false), EAGLColorFormat.RGBA8  },
				new object[] { EAGLDrawableProperty.RetainedBacking, EAGLDrawableProperty.ColorFormat }
			);
			
			Context = new EAGLContext (EAGLRenderingAPI.OpenGLES2);
			
			if (!EAGLContext.SetCurrentContext (Context))
				throw new ApplicationException ("Could not set EAGLContext");
		}
	
		[Export ("layerClass")]
		public static Class LayerClass ()
		{
			return new Class (typeof(CAEAGLLayer));
		}
		
		#region Setup
		bool CreateFrameBuffer ()
		{
			bool success = true;
					
			GL.Disable (EnableCap.DepthTest);
					
			GL.GenFramebuffers (1, out FrameBuffer);
			GL.BindFramebuffer (FramebufferTarget.Framebuffer, FrameBuffer);
					
			GL.GenRenderbuffers (1, out ColorBuffer);
			GL.BindRenderbuffer (RenderbufferTarget.Renderbuffer, ColorBuffer);
					
			Context.RenderBufferStorage ((uint)All.Renderbuffer, (CAEAGLLayer)Layer);
					
			GL.GetRenderbufferParameter (RenderbufferTarget.Renderbuffer, RenderbufferParameterName.RenderbufferWidth, out renderBufferWidth);
			GL.GetRenderbufferParameter (RenderbufferTarget.Renderbuffer, RenderbufferParameterName.RenderbufferHeight, out renderBufferHeight);
			
			GL.FramebufferRenderbuffer (FramebufferTarget.Framebuffer, FramebufferSlot.ColorAttachment0, RenderbufferTarget.Renderbuffer, ColorBuffer);
					
			if (GL.CheckFramebufferStatus (FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete) {
				Console.WriteLine ("Failure with framebuffer generation");
				success = false;
			}
					
			// Create a new CVOpenGLESTexture Cache
			videoTextureCache = CVOpenGLESTextureCache.FromEAGLContext (Context);			

			glProgram = CreateProgram ();
			
			if (glProgram == 0)
				success = false;					
			
			return success;
		}
		
		int CreateProgram ()
		{
			// Create shader program
			int program = GL.CreateProgram ();
			
			// Create and Compile Vertex Shader
			int vertShader = 0;
			int fragShader = 0;
			bool success = true;
			success = success && CompileShader (out vertShader, ShaderType.VertexShader, "Shaders/passThrough.vsh");
			
			// Create and Compile fragment shader
			success = success && CompileShader (out fragShader, ShaderType.FragmentShader, "Shaders/passThrough.fsh");
			
			// Attach Vertext Shader
			GL.AttachShader (program, vertShader);
			
			// Attach fragment shader
			GL.AttachShader (program, fragShader);
			
			// Bind attribute locations
			GL.BindAttribLocation (program, ATTRIB_VERTEX, "position");
			GL.BindAttribLocation (program, ATTRIB_TEXCOORD, "textureCoordinate");
			
			// Link program
			success = success && LinkProgram (program);
			if (success) {
				// Delete these ones, we do not need them anymore
				GL.DeleteShader (vertShader);
				GL.DeleteShader (fragShader);
			} else {
				Console.WriteLine ("Failed to compile and link the shader programs");
				GL.DeleteProgram (program);
				program = 0;
			}
			
			return program;
		}
		
		bool LinkProgram (int program)
		{
			GL.LinkProgram (program);
			
			int status = 0;
			int len = 0;
			
			GL.GetProgram (program, ProgramParameter.LinkStatus, out status);
			
			if (status == 0) {
				GL.GetProgram (program, ProgramParameter.InfoLogLength, out len);
				var sb = new System.Text.StringBuilder (len);
				GL.GetProgramInfoLog (program, len, out len, sb);
				Console.WriteLine ("Link error: {0}", sb);
			}
			return status != 0;
		}
		
		bool CompileShader (out int shader, ShaderType type, string path)
		{
			string shaderProgram = System.IO.File.ReadAllText (path);
			int len = shaderProgram.Length, status = 0;
			shader = GL.CreateShader (type);

			GL.ShaderSource (shader, 1, new string [] { shaderProgram }, ref len);
			GL.CompileShader (shader);
			GL.GetShader (shader, ShaderParameter.CompileStatus, out status);
			
			if (status == 0) {
				GL.DeleteShader (shader);
				return false;
			}
			return true;
		}
		#endregion
		
		#region Rendering
		public void DisplayPixelBuffer (CVImageBuffer imageBuffer)
		{
			// First check to make sure we have a FrameBuffer to write to.
			if (FrameBuffer == 0) {
				var success = CreateFrameBuffer ();
				if (!success) {
					Console.WriteLine ("Problem initializing OpenGL buffers.");
					return;
				}
			}
			
			if (videoTextureCache == null) {
				Console.WriteLine ("Video Texture Cache not initialized");
				return;
			}
			
			var pixelBuffer = imageBuffer as CVPixelBuffer;
			if (pixelBuffer == null) {
				Console.WriteLine ("Could not get Pixel Buffer from Image Buffer");
				return;
			}
			
			// Create a CVOpenGLESTexture from the CVImageBuffer
			var frameWidth = pixelBuffer.Width;
			var frameHeight = pixelBuffer.Height;
			CVReturn ret;
			using (var texture =  videoTextureCache.TextureFromImage(imageBuffer, true, All.Rgba, frameWidth, frameHeight, All.Bgra, DataType.UnsignedByte, 0, out ret)) {
				if (texture == null || ret != CVReturn.Success) {
					Console.WriteLine ("Could not create Texture from Texture Cache");
					return;
				}
				GL.BindTexture (texture.Target, texture.Name);
			
				// Set texture parameters
				GL.TexParameter (TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Linear);
				GL.TexParameter (TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Linear);
				GL.TexParameter (TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)All.ClampToEdge);
				GL.TexParameter (TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)All.ClampToEdge);
			
				GL.BindFramebuffer (FramebufferTarget.Framebuffer, FrameBuffer);
			
				// Set the view port to the entire view
				GL.Viewport (0, 0, renderBufferWidth, renderBufferHeight);
			
				var squareVerticies = new float[,] {
					 { -1.0F, -1.0F},
					 { 1.0F, -1.0F },
					 { -1.0F, 1.0F },
					 { 1.0F, 1.0F }
				};
				
				// The texture verticies are setup such that we flip the texture vertically.
				// This is so that our top left origin buffers match OpenGL's bottom left texture coordinate system.
				var textureSamplingRect = TextureSamplingRectForCroppingTextureWithAspectRatio (new SizeF (frameWidth, frameHeight), this.Bounds.Size);
				var textureVertices = new float[,]
				{
					{textureSamplingRect.Left, textureSamplingRect.Bottom},
					{textureSamplingRect.Right, textureSamplingRect.Bottom},
					{textureSamplingRect.Left, textureSamplingRect.Top},
					{textureSamplingRect.Right, textureSamplingRect.Top}
				};
				
				// Draw the texture on the screen with OpenGL ES 2
				RenderWithSquareVerticies (squareVerticies, textureVertices);
			
				GL.BindTexture (texture.Target, texture.Name);
			
				// Flush the CVOpenGLESTexture cache and release the texture
				videoTextureCache.Flush (CVOptionFlags.None);
			}
		}
		
		RectangleF TextureSamplingRectForCroppingTextureWithAspectRatio (SizeF textureAspectRatio, SizeF croppingAspectRatio)
		{
			RectangleF normalizedSamplingRect;
			var cropScaleAmount = new SizeF (croppingAspectRatio.Width / textureAspectRatio.Width, croppingAspectRatio.Height / textureAspectRatio.Height);
			var maxScale = Math.Max (cropScaleAmount.Width, cropScaleAmount.Height);
			
			var scaledTextureSize = new SizeF (textureAspectRatio.Width * maxScale, textureAspectRatio.Height * maxScale);
			
			float width, height;
			if (cropScaleAmount.Height > cropScaleAmount.Width) {
				width = croppingAspectRatio.Width / scaledTextureSize.Width;
				height = 1.0F;
				normalizedSamplingRect = new RectangleF (0, 0, width, height);				
			} else {
				height = croppingAspectRatio.Height / scaledTextureSize.Height;
				width = 1.0F;
				normalizedSamplingRect = new RectangleF (0, 0, height, width);
			}
			
			// Center crop
			normalizedSamplingRect.X = (1.0F - normalizedSamplingRect.Size.Width) / 2.0F;
			normalizedSamplingRect.Y = (1.0F - normalizedSamplingRect.Size.Height) / 2.0F;
			
			return normalizedSamplingRect;
		}
		
		void RenderWithSquareVerticies (float[,] squareVerticies, float[,] textureVerticies)
		{
			// Use Shader Program
			GL.UseProgram (glProgram);
			
			// Update attribute values
			GL.VertexAttribPointer (ATTRIB_VERTEX, 2, VertexAttribPointerType.Float, false, 0, squareVerticies);
			GL.EnableVertexAttribArray (ATTRIB_VERTEX);
			
			GL.VertexAttribPointer (ATTRIB_TEXCOORD, 2, VertexAttribPointerType.Float, false, 0, textureVerticies);
			GL.EnableVertexAttribArray (ATTRIB_TEXCOORD);
			
			// Validate program before drawing. (For Debugging purposes)
#if DEBUG
			GL.ValidateProgram (glProgram);
#endif
			GL.DrawArrays (BeginMode.TriangleStrip, 0, 4);
			
			// Present
			GL.BindRenderbuffer (RenderbufferTarget.Renderbuffer, ColorBuffer);
			Context.PresentRenderBuffer ((uint)All.Renderbuffer);
		}
		#endregion		
	}
}

