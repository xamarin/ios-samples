using System;
using System.IO;

using AVFoundation;
using CoreAnimation;
using CoreGraphics;
using CoreVideo;
using Foundation;
using OpenGLES;
using OpenTK.Graphics.ES20;
using UIKit;

namespace VideoTimeLine
{
	public class EAGLLayer : CAEAGLLayer
	{
		public enum UniformIndex {
			Y = 0,
			UV,
			RotationAngle,
			ColorConversionMatrix,
			NumUniforms
		}

		public enum AttributeIndex {
			Vertex = 0,
			TextureCoordinates,
			NumAttributes
		}

		// BT.601, which is the standard for SDTV.
		static float[] colorConversion601 = {
			1.164f, 1.164f, 1.164f,
			0.0f, -0.392f, 2.017f,
			1.596f, -0.813f, 0.0f
		};

		// BT.709, which is the standard for HDTV.
		static float[] colorConversion709 = {
			1.164f, 1.164f, 1.164f,
			0.0f, -0.213f, 2.112f,
			1.793f, -0.533f, 0.0f
		};

		EAGLContext context;
		CVOpenGLESTexture lumaTexture;
		CVOpenGLESTexture chromaTexture;
		CVOpenGLESTextureCache videoTextureCache;

		int backingWidth;
		int backingHeight;
		uint frameBufferHandle;
		uint colorBufferHandle;

		float[] preferredConversion;
		int[] uniforms = new int[(int)UniformIndex.NumUniforms];

		static string fragmentShaderSource;

		static string FragmentShaderSource {
			get {
				if (fragmentShaderSource == null) {
					using (var fragShaderURL = NSBundle.MainBundle.GetUrlForResource ("Shader", "fsh"))
						fragmentShaderSource = File.ReadAllText (fragShaderURL.Path);
				}
				return fragmentShaderSource;
			}
		}

		static string vertexShaderSource;

		static string VertexShaderSource {
			get {
				if (vertexShaderSource == null) {
					using (var vertShaderURL = NSBundle.MainBundle.GetUrlForResource ("Shader", "vsh"))
						vertexShaderSource = File.ReadAllText (vertShaderURL.Path);
				}
				return vertexShaderSource;
			}
		}

		public CGSize PresentationRect { get; set; }

		public string TimeCode { get; set; }

		public CVPixelBuffer PixelBufferContents { get; private set; }

		public int Program { get; private set; }

		public EAGLLayer ()
		{
			Opaque = true;
			DrawableProperties = NSDictionary.FromObjectsAndKeys (
				new object[] { NSNumber.FromBoolean (false), EAGLColorFormat.RGBA8 },
				new object[] { EAGLDrawableProperty.RetainedBacking, EAGLDrawableProperty.ColorFormat }
			);

			context = new EAGLContext (EAGLRenderingAPI.OpenGLES2);

			if (context == null || !EAGLContext.SetCurrentContext (context))
				throw new Exception ("Can't set EAGL context");

			preferredConversion = colorConversion709;
		}

		bool disposed;

		protected override void Dispose (bool disposing)
		{
			if (disposed)
				return;

			if (disposing) {
				CleanUpTextures ();

				if (videoTextureCache != null) {
					videoTextureCache.Flush (CVOptionFlags.None);
					videoTextureCache.Dispose ();
				}

				if (context != null)
					context.Dispose ();
			}

			GL.DeleteFramebuffers (1, ref frameBufferHandle);
			GL.DeleteRenderbuffers (1, ref colorBufferHandle);
			disposed = true;

			base.Dispose (disposing);
		}

		public void SetupGL ()
		{
			EAGLContext.SetCurrentContext (context);
			SetupBuffers ();
			LoadShaders ();

			GL.UseProgram (Program);

			// 0 and 1 are the texture IDs of lumaTexture and chromaTexture respectively.
			GL.Uniform1 (uniforms [(int)UniformIndex.Y], 0);
			GL.Uniform1 (uniforms [(int)UniformIndex.UV], 1);
			GL.Uniform1 (uniforms [(int)UniformIndex.RotationAngle], 0);
			GL.UniformMatrix3 (uniforms [(int)UniformIndex.ColorConversionMatrix], 1, false, preferredConversion);

			if (videoTextureCache != null)
				return;

			videoTextureCache = CVOpenGLESTextureCache.FromEAGLContext (context);
			if (videoTextureCache == null)
				Console.WriteLine ("Error at CVOpenGLESTextureCache.FromEAGLContext");
		}

		public void DisplayPixelBuffer (CVPixelBuffer pixelBuffer)
		{
			DrawTextInCorner (pixelBuffer);
			CVReturn error;

			if (pixelBuffer != null) {
				int frameWidth = (int)pixelBuffer.Width;
				int frameHeight = (int)pixelBuffer.Height;

				if (videoTextureCache == null) {
					Console.WriteLine ("No video texture cache");
					return;
				}

				CleanUpTextures ();
				CVAttachmentMode attachmentMode;
				var colorAttachments = pixelBuffer.GetAttachment <NSString> (CVImageBuffer.YCbCrMatrixKey, out attachmentMode);

				if (colorAttachments == CVImageBuffer.YCbCrMatrix_ITU_R_601_4)
					preferredConversion = colorConversion601;
				else
					preferredConversion = colorConversion709;

				GL.ActiveTexture (TextureUnit.Texture0);
				lumaTexture = videoTextureCache.TextureFromImage (pixelBuffer, true, All.RedExt, frameWidth, frameHeight,
					All.RedExt, DataType.UnsignedByte, 0, out error);

				if (lumaTexture == null)
					Console.WriteLine ("Error at CVOpenGLESTextureCach.TextureFromImage");

				GL.BindTexture (lumaTexture.Target, lumaTexture.Name);
				GL.TexParameter (TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Linear);
				GL.TexParameter (TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Linear);
				GL.TexParameter (TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)All.ClampToEdge);
				GL.TexParameter (TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)All.ClampToEdge);

				GL.ActiveTexture (TextureUnit.Texture1);
				chromaTexture = videoTextureCache.TextureFromImage (pixelBuffer, true, All.RgExt, frameWidth / 2, frameHeight / 2,
					All.RgExt, DataType.UnsignedByte, 1, out error);

				if (chromaTexture == null)
					Console.WriteLine ("Error at CVOpenGLESTextureCach.TextureFromImage");

				GL.BindTexture (chromaTexture.Target, chromaTexture.Name);
				GL.TexParameter (TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Linear);
				GL.TexParameter (TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Linear);
				GL.TexParameter (TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)All.ClampToEdge);
				GL.TexParameter (TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)All.ClampToEdge);

				GL.BindFramebuffer (FramebufferTarget.Framebuffer, frameBufferHandle);
				GL.Viewport (0, 0, backingWidth, backingHeight);
			}

			GL.ClearColor (0f, 0f, 0f, 1f);
			GL.Clear (ClearBufferMask.ColorBufferBit);

			GL.UseProgram (Program);
			GL.Uniform1 (uniforms [(int)UniformIndex.RotationAngle], 0f);
			GL.UniformMatrix3 (uniforms [(int)UniformIndex.ColorConversionMatrix], 1, false, preferredConversion);

			CGRect vertexSamplingRect = AVUtilities.WithAspectRatio (Bounds, PresentationRect);

			var normalizedSamplingSize = new CGSize (0f, 0f);
			var cropScaleAmount = new CGSize (vertexSamplingRect.Width / Bounds.Width, vertexSamplingRect.Height / Bounds.Height);

			if (cropScaleAmount.Width > cropScaleAmount.Height) {
				normalizedSamplingSize.Width = 1f;
				normalizedSamplingSize.Height = cropScaleAmount.Height / cropScaleAmount.Width;
			} else {
				normalizedSamplingSize.Width = 1f;
				normalizedSamplingSize.Height = cropScaleAmount.Width / cropScaleAmount.Height;
			}

			float[] quadVertexData = {
				-1f * (float)normalizedSamplingSize.Width, -1f * (float)normalizedSamplingSize.Height,
				(float)normalizedSamplingSize.Width, -1f * (float)normalizedSamplingSize.Height,
				-1f * (float)normalizedSamplingSize.Width, (float)normalizedSamplingSize.Height,
				(float)normalizedSamplingSize.Width, (float)normalizedSamplingSize.Height,
			};

			GL.VertexAttribPointer ((int)AttributeIndex.Vertex, 2, VertexAttribPointerType.Float, false, 0, quadVertexData);
			GL.EnableVertexAttribArray ((int)AttributeIndex.Vertex);

			var textureSamplingRect = new CGRect (0, 0, 1, 1);
			float[] quadTextureData = {
				(float)textureSamplingRect.GetMinX (), (float)textureSamplingRect.GetMaxY (),
				(float)textureSamplingRect.GetMaxX (), (float)textureSamplingRect.GetMaxY (),
				(float)textureSamplingRect.GetMinX (), (float)textureSamplingRect.GetMinY (),
				(float)textureSamplingRect.GetMaxX (), (float)textureSamplingRect.GetMinY ()
			};

			GL.VertexAttribPointer ((int)AttributeIndex.TextureCoordinates, 2, VertexAttribPointerType.Float, false, 0, quadTextureData);
			GL.EnableVertexAttribArray ((int)AttributeIndex.TextureCoordinates);

			GL.DrawArrays (BeginMode.TriangleStrip, 0, 4);
			GL.BindRenderbuffer (RenderbufferTarget.Renderbuffer, colorBufferHandle);
			context.PresentRenderBuffer ((int)RenderbufferTarget.Renderbuffer);
		}

		void CleanUpTextures ()
		{
			if (lumaTexture != null) {
				lumaTexture.Dispose ();
				lumaTexture = null;
			}

			if (chromaTexture != null) {
				chromaTexture.Dispose ();
				chromaTexture = null;
			}
		}

		void SetupBuffers ()
		{
			GL.Disable (EnableCap.DepthTest);

			GL.EnableVertexAttribArray ((int)AttributeIndex.Vertex);
			GL.VertexAttribPointer ((int)AttributeIndex.Vertex, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);

			GL.EnableVertexAttribArray ((int)AttributeIndex.TextureCoordinates);
			GL.VertexAttribPointer ((int)AttributeIndex.TextureCoordinates, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);

			GL.GenFramebuffers (1, out frameBufferHandle);
			GL.BindFramebuffer (FramebufferTarget.Framebuffer, frameBufferHandle);

			GL.GenRenderbuffers (1, out colorBufferHandle);
			GL.BindRenderbuffer (RenderbufferTarget.Renderbuffer, colorBufferHandle);

			context.RenderBufferStorage ((int)RenderbufferTarget.Renderbuffer, this);
			GL.GetRenderbufferParameter (RenderbufferTarget.Renderbuffer, RenderbufferParameterName.RenderbufferWidth, out backingWidth); 
			GL.GetRenderbufferParameter (RenderbufferTarget.Renderbuffer, RenderbufferParameterName.RenderbufferHeight, out backingHeight); 

			GL.FramebufferRenderbuffer (FramebufferTarget.Framebuffer, FramebufferSlot.ColorAttachment0, RenderbufferTarget.Renderbuffer, colorBufferHandle);

			if (GL.CheckFramebufferStatus (FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
				Console.WriteLine ("Failed to make complete framebuffer object");
		}

		bool LoadShaders ()
		{
			int vertShader, fragShader;
			Program = GL.CreateProgram ();

			// Create and compile the vertex shader.
			if (!CompileShader (ShaderType.VertexShader, out vertShader)) {
				Console.WriteLine ("Failed to compile vertex shader");
				return false;
			}

			// Create and compile fragment shader.
			if (!CompileShader (ShaderType.FragmentShader, out fragShader)) {
				Console.WriteLine ("Failed to compile fragment shader");
				return false;
			}

			GL.AttachShader (Program, vertShader);
			GL.AttachShader (Program, fragShader);

			GL.BindAttribLocation (Program, (int)AttributeIndex.Vertex, "position");
			GL.BindAttribLocation (Program, (int)AttributeIndex.TextureCoordinates, "texCoord");

			GL.LinkProgram (Program);
#if DEBUG
			string log;
			GL.GetProgramInfoLog (Program, out log);
			if (!string.IsNullOrEmpty (log))
				Console.WriteLine ("Program compile log: {0}", log);
#endif
			int status;
			GL.GetProgram (Program, ProgramParameter.LinkStatus, out status);
			bool ok = (status != 0);
			if (ok) {
				uniforms [(int) UniformIndex.Y] = GL.GetUniformLocation (Program, "SamplerY");
				uniforms [(int) UniformIndex.UV] = GL.GetUniformLocation (Program, "SamplerUV");
				uniforms [(int) UniformIndex.RotationAngle] = GL.GetUniformLocation (Program, "preferredRotation");
				uniforms [(int) UniformIndex.ColorConversionMatrix] = GL.GetUniformLocation (Program, "colorConversionMatrix");
			}
			if (vertShader != 0) {
				GL.DetachShader (Program, vertShader);
				GL.DeleteShader (vertShader);
			}
			if (fragShader != 0) {
				GL.DetachShader (Program, fragShader);
				GL.DeleteShader (fragShader);
			}
			if (!ok) {
				GL.DeleteProgram (Program);
				Program = 0;
			}
			return ok;
		}

		bool CompileShader (ShaderType shaderType, out int shader)
		{
			shader = 0;
			string source = (shaderType == ShaderType.FragmentShader) ? FragmentShaderSource : VertexShaderSource;
			if (source == null) {
				Console.WriteLine ("Failed to load {0}", shaderType);
				return false;
			}
				
			shader = GL.CreateShader (shaderType);
			GL.ShaderSource (shader, source);
			GL.CompileShader (shader);

			string log;
			GL.GetShaderInfoLog (shader, out log);
			if (!string.IsNullOrEmpty (log))
				Console.WriteLine ("Shader compile log: {0}", log);

			int parameters;
			GL.GetShader (shader, ShaderParameter.CompileStatus, out parameters);

			return true;
		}

		void DrawTextInCorner (CVPixelBuffer pixelBuffer)
		{
			var textLayer = new CATextLayer ();

			const float textLayerWidth = 100f;
			const float textLayerHeight = 50f;

			if (AffineTransform.xx == -1.0f && AffineTransform.yy == -1.0f) {
				textLayer.AffineTransform = AffineTransform;
			} else if (AffineTransform.xy == 1.0f && AffineTransform.yx == -1f) {
				textLayer.AffineTransform = new CGAffineTransform (
					AffineTransform.xx * -1f, AffineTransform.xy * -1f,
					AffineTransform.yx * -1f, AffineTransform.yy * -1f,
					AffineTransform.x0, AffineTransform.y0
                );
			}

			textLayer.Frame = new CGRect (Bounds.Size.Width - textLayerWidth, 0f, textLayerWidth, textLayerHeight);
			textLayer.String = TimeCode;
			textLayer.BackgroundColor = UIColor.Black.CGColor;

			AddSublayer (textLayer);
		}
	}
}