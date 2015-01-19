using System;
using Foundation;
using OpenGLES;
using CoreGraphics;
using CoreVideo;
using OpenTK.Graphics.ES20;

namespace AVCustomEdit
{
	public class OpenGLRenderer : NSObject
	{
		public int ProgramY = -1;
		public int ProgramUV = -1;
		public CGAffineTransform RenderTransform;
		public CVOpenGLESTextureCache VideoTextureCache;
		public EAGLContext CurrentContext;
		public uint OffscreenBufferHandle;
		public int[] Uniforms = new int[(int)Uniform.Num_Uniforms];

		string vertShaderSource = "attribute vec4 position; \n" +
			"attribute vec2 texCoord; \n" +
				"uniform mat4 renderTransform; \n"+
				"varying vec2 texCoordVarying; \n"+
				"void main() \n"+
				"{ \n"+
				"gl_Position = position * renderTransform; \n"+
				"texCoordVarying = texCoord; \n"+
				"}";
		string fragShaderYSource = "varying highp vec2 texCoordVarying; \n"+
		    " uniform sampler2D SamplerY; \n"+
		     "void main() \n"+
		     "{ \n"+
				"gl_FragColor.r = texture2D(SamplerY, texCoordVarying).r; \n"+
		     "}";
		string fragShaderUVSource ="varying highp vec2 texCoordVarying; \n"+
		    "uniform sampler2D SamplerUV; \n"+
		    "void main() \n"+
		    "{ \n"+
				"gl_FragColor.rg = texture2D(SamplerUV, texCoordVarying).rg; \n"+
		    "}";

		public OpenGLRenderer () : base()
		{
			CurrentContext = new EAGLContext (EAGLRenderingAPI.OpenGLES2);

			EAGLContext.SetCurrentContext (CurrentContext);

			SetupOffScreenRenderContext();
			loadShaders ();

			EAGLContext.SetCurrentContext (null);

		}

		public virtual CVOpenGLESTexture LumaTextureForPixelBuffer (CVPixelBuffer pixelBuffer)
		{
			CVOpenGLESTexture lumaTexture = null;
			CVReturn err;
			if (VideoTextureCache == null) {
				Console.Error.WriteLine ("No video texture cache");
				return lumaTexture;
			}
			// Periodic texture cache flush every frame
			VideoTextureCache.Flush (0);

			// CVOpenGLTextureCacheCreateTextureFromImage will create GL texture optimally from CVPixelBufferRef.
			// UV
			lumaTexture = VideoTextureCache.TextureFromImage (pixelBuffer, true, All.RedExt, (int)pixelBuffer.Width,(int) pixelBuffer.Height, All.RedExt, DataType.UnsignedByte, 0, out err);
			if (lumaTexture == null || err != CVReturn.Success)
				Console.Error.WriteLine ("Error at creating luma texture using CVOpenGLESTextureCacheCreateTextureFromImage: " + err.ToString ());

			return lumaTexture;
		}

public virtual CVOpenGLESTexture ChromaTextureForPixelBuffer (CVPixelBuffer pixelBuffer)
{
	CVOpenGLESTexture chromaTexture = null;
	CVReturn err;
	if (VideoTextureCache == null) {
		Console.Error.WriteLine ("No video texture cache");
		return chromaTexture;
	}
	// Periodic texture cache flush every frame
	VideoTextureCache.Flush (0);

	// CVOpenGLTextureCacheCreateTextureFromImage will create GL texture optimally from CVPixelBufferRef.
	// UV
	var height = pixelBuffer.GetHeightOfPlane (1);
	var width = pixelBuffer.GetWidthOfPlane (1);
			chromaTexture = VideoTextureCache.TextureFromImage (pixelBuffer, true, All.RgExt, (int)width, (int)height, All.RgExt, DataType.UnsignedByte, 1, out err);

	if (chromaTexture == null || err != CVReturn.Success)
		Console.Error.WriteLine ("Error at creating chroma texture using CVOpenGLESTextureCacheCreateTextureFromImage: " + err.ToString ());

	return chromaTexture;
}

		public virtual void RenderPixelBuffer(CVPixelBuffer destinationPixelBuffer, CVPixelBuffer foregroundPixelBuffer, CVPixelBuffer backgroundPixelBuffer, float tween)
		{
			DoesNotRecognizeSelector (new ObjCRuntime.Selector ("_cmd"));
		}

		public void SetupOffScreenRenderContext()
		{
			//-- Create CVOpenGLESTextureCacheRef for optimal CVPixelBufferRef to GLES texture conversion.
			if (VideoTextureCache != null) {
				VideoTextureCache.Dispose ();
				VideoTextureCache = null;
			}

			VideoTextureCache = CVOpenGLESTextureCache.FromEAGLContext (CurrentContext);
			GL.Disable (EnableCap.DepthTest);
			GL.GenFramebuffers (1, out OffscreenBufferHandle);
			GL.BindFramebuffer (FramebufferTarget.Framebuffer, OffscreenBufferHandle);
		}

		// OpenGL ES 2 shader compilation
		bool loadShaders()
		{
			int vertShader = 0, fragShaderY = 0, fragShaderUV = 0;

			// Create the shader program.
			ProgramY = GL.CreateProgram ();
			ProgramUV = GL.CreateProgram ();

			// Create and compile the vertex shader.

			if (!compileShader (ShaderType.VertexShader, vertShaderSource, out vertShader)) {
				Console.Error.WriteLine ("Failed to compile vertex shader");
				return false;
			}

			if(!compileShader (ShaderType.FragmentShader, fragShaderYSource, out fragShaderY)) {
				Console.Error.WriteLine ("Failed to compile Y fragment shader");
				return false;
			}

			if(!compileShader (ShaderType.FragmentShader, fragShaderUVSource, out fragShaderUV)) {
				Console.Error.WriteLine ("Failed to compile UV fragment shader");
				return false;
			}

			GL.AttachShader (ProgramY, vertShader);
			GL.AttachShader (ProgramY, fragShaderY);

			GL.AttachShader (ProgramUV, vertShader);
			GL.AttachShader (ProgramUV, fragShaderUV);

			// Bind attribute locations. This needs to be done prior to linking.
			GL.BindAttribLocation (ProgramY,(int) Attrib.Vertex_Y, "position");
			GL.BindAttribLocation (ProgramY,(int) Attrib.TexCoord_Y, "texCoord");
			GL.BindAttribLocation (ProgramUV,(int) Attrib.Vertex_UV, "position");
			GL.BindAttribLocation (ProgramUV, (int)Attrib.TexCoord_UV, "texCoord");

			// Link the program.
			if (!linkProgram (ProgramY) || !this.linkProgram (ProgramUV)) {
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
			Uniforms [(int)Uniform.Y] = GL.GetUniformLocation (ProgramY, "SamplerY");
			Uniforms [(int)Uniform.UV] = GL.GetUniformLocation (ProgramUV, "SamplerUV");
			Uniforms [(int)Uniform.Render_Transform_Y] = GL.GetUniformLocation (ProgramY, "renderTransform");
			Uniforms [(int)Uniform.Render_Transform_UV] = GL.GetUniformLocation (ProgramUV, "renderTransform");

			//Release vertex and fragment shaders.
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

		bool compileShader(ShaderType type, string sourceString, out int shader)
		{
			if (string.IsNullOrEmpty (sourceString)) {
				Console.Error.WriteLine ("Failed to load vertex shader: Empty source string");
				shader = 0;
				return false;
			}

			int status;
			shader = GL.CreateShader (type);
			GL.ShaderSource (shader, sourceString);
			GL.CompileShader (shader);

#if DEBUG
			int logLength;
			GL.GetShader (shader, ShaderParameter.InfoLogLength, out logLength);
			if (logLength > 0) {
				var log = GL.GetShaderInfoLog (shader);
				Console.WriteLine ("Shader compile log: {0}", log);
			}
#endif

			GL.GetShader (shader, ShaderParameter.CompileStatus, out status);
			if (status == 0) {
				GL.DeleteShader (shader);
				return false;
			}

			return true;
		}

		bool linkProgram(int program)
		{
			int status = 0;
			GL.LinkProgram (program);

#if DEBUG
			int logLength = 0;
			GL.GetProgram(program, ProgramParameter.InfoLogLength, out logLength);
			if (logLength > 0) {
				System.Text.StringBuilder log = new System.Text.StringBuilder(logLength);
				GL.GetProgramInfoLog (program, logLength, out logLength, log);
				Console.WriteLine("Program link log: " + log.ToString());
				log.Clear ();
			}
#endif

			GL.GetProgram (program, ProgramParameter.LinkStatus, out status);
			if (status == 0)
				return false;

			return true;
		}

		bool validateProgram(int program)
		{
			int logLength = 0;
			int status = 0;
			GL.ValidateProgram (program);

			GL.GetProgram (program, ProgramParameter.InfoLogLength, out logLength);
			if (logLength > 0) {
				System.Text.StringBuilder log = new System.Text.StringBuilder(logLength);
				GL.GetProgramInfoLog (program, logLength, out logLength, log);
				Console.WriteLine("Program validate log: " + log.ToString());
				log.Clear ();
			}

			GL.GetProgram (program, ProgramParameter.ValidateStatus, out status);
			if (status == 0)
				return false;

			return true;
		}
	}

	public enum Uniform
	{
		Y,
		UV,
		Render_Transform_Y,
		Render_Transform_UV,
		Num_Uniforms
	}

	public enum Attrib{
		Vertex_Y,
		TexCoord_Y,
		Vertex_UV,
		TexCoord_UV,
		Num_Attributes
	}

}

