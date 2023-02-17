using CoreGraphics;
using CoreVideo;
using Foundation;
using OpenGLES;
using OpenTK.Graphics.ES20;
using System;
using System.Text;

namespace AVCustomEdit {
	public class OpenGLRenderer : NSObject {
		private const string VertShaderSource = @"attribute vec4 position; 
                                                  attribute vec2 texCoord;
                                                  uniform mat4 renderTransform;
                                                  varying vec2 texCoordVarying;
                                                  void main()
                                                  {
                                                     gl_Position = position * renderTransform;
                                                     texCoordVarying = texCoord;
                                                  }";

		private const string FragShaderYSource = @"varying highp vec2 texCoordVarying;
                                                   uniform sampler2D SamplerY;
                                                   void main()
                                                   {
                                                      gl_FragColor.r = texture2D(SamplerY, texCoordVarying).r;
                                                   }";

		private const string FragShaderUVSource = @"varying highp vec2 texCoordVarying;
                                                    uniform sampler2D SamplerUV;
                                                    void main()
                                                    {
                                                       gl_FragColor.rg = texture2D(SamplerUV, texCoordVarying).rg;
                                                    }";

		public uint OffscreenBufferHandle;

		public OpenGLRenderer ()
		{
			CurrentContext = new EAGLContext (EAGLRenderingAPI.OpenGLES2);
			EAGLContext.SetCurrentContext (CurrentContext);

			SetupOffScreenRenderContext ();
			LoadShaders ();

			EAGLContext.SetCurrentContext (null);
		}

		public int ProgramY { get; set; } = -1;
		public int ProgramUV { get; set; } = -1;
		public CGAffineTransform RenderTransform { get; set; }
		public CVOpenGLESTextureCache _videoTextureCache { get; set; }
		public EAGLContext CurrentContext { get; set; }
		public int [] Uniforms { get; set; } = new int [(int) Uniform.Num_Uniforms];

		public virtual void RenderPixelBuffer (CVPixelBuffer destinationPixelBuffer, CVPixelBuffer foregroundPixelBuffer, CVPixelBuffer backgroundPixelBuffer, float tween)
		{
			DoesNotRecognizeSelector (new ObjCRuntime.Selector ("_cmd"));
		}

		public void SetupOffScreenRenderContext ()
		{
			//-- Create CVOpenGLESTextureCacheRef for optimal CVPixelBufferRef to GLES texture conversion.
			if (_videoTextureCache != null) {
				_videoTextureCache.Dispose ();
				_videoTextureCache = null;
			}

			_videoTextureCache = CVOpenGLESTextureCache.FromEAGLContext (CurrentContext);

			GL.Disable (EnableCap.DepthTest);
			GL.GenFramebuffers (1, out OffscreenBufferHandle);
			GL.BindFramebuffer (FramebufferTarget.Framebuffer, OffscreenBufferHandle);
		}

		public virtual CVOpenGLESTexture LumaTextureForPixelBuffer (CVPixelBuffer pixelBuffer)
		{
			CVOpenGLESTexture lumaTexture = null;
			if (_videoTextureCache == null) {
				Console.Error.WriteLine ("No video texture cache");
				goto bail;
			}

			// Periodic texture cache flush every frame
			_videoTextureCache.Flush (0);

			// CVOpenGLTextureCacheCreateTextureFromImage will create GL texture optimally from CVPixelBufferRef.
			// Y
			lumaTexture = _videoTextureCache.TextureFromImage (pixelBuffer,
															  true,
															  All.RedExt,
															  (int) pixelBuffer.Width,
															  (int) pixelBuffer.Height,
															  All.RedExt,
															  DataType.UnsignedByte,
															  0,
															  out CVReturn error);

			if (lumaTexture == null || error != CVReturn.Success) {
				Console.Error.WriteLine ($"Error at creating luma texture using CVOpenGLESTextureCacheCreateTextureFromImage: {error}");
			}

		bail:
			return lumaTexture;
		}

		public virtual CVOpenGLESTexture ChromaTextureForPixelBuffer (CVPixelBuffer pixelBuffer)
		{
			CVOpenGLESTexture chromaTexture = null;
			if (_videoTextureCache == null) {
				Console.Error.WriteLine ("No video texture cache");
				goto bail;
			}

			// Periodic texture cache flush every frame
			_videoTextureCache.Flush (0);

			// CVOpenGLTextureCacheCreateTextureFromImage will create GL texture optimally from CVPixelBufferRef.
			// UV
			var height = pixelBuffer.GetHeightOfPlane (1);
			var width = pixelBuffer.GetWidthOfPlane (1);
			chromaTexture = _videoTextureCache.TextureFromImage (pixelBuffer,
																true,
																All.RgExt,
																(int) width,
																(int) height,
																All.RgExt,
																DataType.UnsignedByte,
																1,
																out CVReturn error);

			if (chromaTexture == null || error != CVReturn.Success) {
				Console.Error.WriteLine ($"Error at creating chroma texture using CVOpenGLESTextureCacheCreateTextureFromImage: {error}");
			}

		bail:
			return chromaTexture;
		}

		#region OpenGL ES 2 shader compilation

		private bool LoadShaders ()
		{
			int vertShader, fragShaderY, fragShaderUV;

			// Create the shader program.
			ProgramY = GL.CreateProgram ();
			ProgramUV = GL.CreateProgram ();

			// Create and compile the vertex shader.
			if (!CompileShader (ShaderType.VertexShader, VertShaderSource, out vertShader)) {
				Console.Error.WriteLine ("Failed to compile vertex shader");
				return false;
			}

			if (!CompileShader (ShaderType.FragmentShader, FragShaderYSource, out fragShaderY)) {
				Console.Error.WriteLine ("Failed to compile Y fragment shader");
				return false;
			}

			if (!CompileShader (ShaderType.FragmentShader, FragShaderUVSource, out fragShaderUV)) {
				Console.Error.WriteLine ("Failed to compile UV fragment shader");
				return false;
			}

			// Attach vertex shader to programY.
			GL.AttachShader (ProgramY, vertShader);

			// Attach fragment shader to programY.
			GL.AttachShader (ProgramY, fragShaderY);

			// Attach vertex shader to programUV.
			GL.AttachShader (ProgramUV, vertShader);

			// Attach fragment shader to programUV.
			GL.AttachShader (ProgramUV, fragShaderUV);

			// Bind attribute locations. This needs to be done prior to linking.
			GL.BindAttribLocation (ProgramY, (int) Attrib.Vertex_Y, "position");
			GL.BindAttribLocation (ProgramY, (int) Attrib.TexCoord_Y, "texCoord");
			GL.BindAttribLocation (ProgramUV, (int) Attrib.Vertex_UV, "position");
			GL.BindAttribLocation (ProgramUV, (int) Attrib.TexCoord_UV, "texCoord");

			// Link the program.
			if (!LinkProgram (ProgramY) || !LinkProgram (ProgramUV)) {
				Console.Error.WriteLine ("Failed to link program");
				if (vertShader != 0) {
					GL.DeleteShader (vertShader);
					vertShader = 0;
				}

				if (fragShaderY != 0) {
					GL.DeleteShader (fragShaderY);
					fragShaderY = 0;
				}

				if (fragShaderUV != 0) {
					GL.DeleteShader (fragShaderUV);
					fragShaderUV = 0;
				}

				if (ProgramY != 0) {
					GL.DeleteProgram (ProgramY);
					ProgramY = 0;
				}

				if (ProgramUV != 0) {
					GL.DeleteProgram (ProgramUV);
					ProgramUV = 0;
				}

				return false;
			}

			// Get uniform locations.
			Uniforms [(int) Uniform.Y] = GL.GetUniformLocation (ProgramY, "SamplerY");
			Uniforms [(int) Uniform.UV] = GL.GetUniformLocation (ProgramUV, "SamplerUV");
			Uniforms [(int) Uniform.Render_Transform_Y] = GL.GetUniformLocation (ProgramY, "renderTransform");
			Uniforms [(int) Uniform.Render_Transform_UV] = GL.GetUniformLocation (ProgramUV, "renderTransform");

			// Release vertex and fragment shaders.
			if (vertShader != 0) {
				GL.DetachShader (ProgramY, vertShader);
				GL.DetachShader (ProgramUV, vertShader);
				GL.DeleteShader (vertShader);
			}

			if (fragShaderY != 0) {
				GL.DetachShader (ProgramY, fragShaderY);
				GL.DeleteShader (fragShaderY);
			}

			if (fragShaderUV != 0) {
				GL.DetachShader (ProgramUV, fragShaderUV);
				GL.DeleteShader (fragShaderUV);
			}

			return true;
		}

		private static bool CompileShader (ShaderType type, string sourceString, out int shader)
		{
			if (string.IsNullOrEmpty (sourceString)) {
				Console.Error.WriteLine ("Failed to load vertex shader: Empty source string");
				shader = 0;
				return false;
			}

			shader = GL.CreateShader (type);
			GL.ShaderSource (shader, sourceString);
			GL.CompileShader (shader);

#if DEBUG
            GL.GetShader(shader, ShaderParameter.InfoLogLength, out int logLength);
            if (logLength > 0)
            {
                var log = GL.GetShaderInfoLog(shader);
                Console.WriteLine("Shader compile log: {0}", log);
            }
#endif

			GL.GetShader (shader, ShaderParameter.CompileStatus, out int status);
			if (status == 0) {
				GL.DeleteShader (shader);
				return false;
			}

			return true;
		}

		private static bool LinkProgram (int program)
		{
			GL.LinkProgram (program);

#if DEBUG
            GL.GetProgram(program, ProgramParameter.InfoLogLength, out int logLength);
            if (logLength > 0)
            {
                var log = new StringBuilder(logLength);
                GL.GetProgramInfoLog(program, logLength, out logLength, log);
                Console.WriteLine($"Program link log: {log}");
                log.Clear();
            }
#endif

			GL.GetProgram (program, ProgramParameter.LinkStatus, out int status);
			return status != 0;
		}
	}

	#endregion

	public enum Uniform {
		Y,
		UV,
		Render_Transform_Y,
		Render_Transform_UV,
		Num_Uniforms
	}

	public enum Attrib {
		Vertex_Y,
		TexCoord_Y,
		Vertex_UV,
		TexCoord_UV,
		Num_Attributes
	}
}
