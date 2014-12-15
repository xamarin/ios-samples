using System;
using Foundation;
using OpenGLES;
using CoreGraphics;
using CoreVideo;
using OpenTK.Graphics.ES20;

namespace AVCustomEdit
{
	public class CrossDissolveRenderer : OpenGLRenderer
	{
		public CrossDissolveRenderer () : base()
		{
		}

		public override void RenderPixelBuffer (CoreVideo.CVPixelBuffer destinationPixelBuffer, CoreVideo.CVPixelBuffer foregroundPixelBuffer, CoreVideo.CVPixelBuffer backgroundPixelBuffer, float tween)
		{
			EAGLContext.SetCurrentContext (CurrentContext);
			if (foregroundPixelBuffer != null || backgroundPixelBuffer != null) {
				CVOpenGLESTexture foregroundLumaTexture = LumaTextureForPixelBuffer (foregroundPixelBuffer);
				CVOpenGLESTexture foregroundChromaTexture = ChromaTextureForPixelBuffer (foregroundPixelBuffer);

				CVOpenGLESTexture backgroundLumaTexture = LumaTextureForPixelBuffer (backgroundPixelBuffer);
				CVOpenGLESTexture backgroundChromaTexture = ChromaTextureForPixelBuffer (backgroundPixelBuffer);

				CVOpenGLESTexture destLumaTexture = LumaTextureForPixelBuffer (destinationPixelBuffer);
				CVOpenGLESTexture destChromaTexture = ChromaTextureForPixelBuffer (destinationPixelBuffer);

				GL.UseProgram (ProgramY);

				// Set the render transform
				float[] preferredRenderTransform = {
					(float)RenderTransform.xx, (float)RenderTransform.xy, (float)RenderTransform.x0, 0.0f,
					(float)RenderTransform.yx, (float)RenderTransform.yy,(float)RenderTransform.y0, 0.0f,
					0.0f, 				0.0f, 				1.0f, 				0.0f,
					0.0f, 				0.0f, 				0.0f, 				1.0f,
				};

				GL.UniformMatrix4 (Uniforms [(int)Uniform.Render_Transform_Y], 1, false, preferredRenderTransform);

				GL.BindFramebuffer (FramebufferTarget.Framebuffer, OffscreenBufferHandle);
				GL.Viewport (0, 0, (int)destinationPixelBuffer.Width, (int)destinationPixelBuffer.Height);

				GL.ActiveTexture (TextureUnit.Texture0);
				GL.BindTexture (foregroundLumaTexture.Target, foregroundLumaTexture.Name);
				GL.TexParameter (TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Linear);
				GL.TexParameter (TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Linear);
				GL.TexParameter (TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)All.ClampToEdge);
				GL.TexParameter (TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)All.ClampToEdge);

				GL.ActiveTexture (TextureUnit.Texture1);
				GL.BindTexture (backgroundLumaTexture.Target, backgroundLumaTexture.Name);
				GL.TexParameter (TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Linear);
				GL.TexParameter (TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Linear);
				GL.TexParameter (TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)All.ClampToEdge);
				GL.TexParameter (TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)All.ClampToEdge);

				// Attach the destination texture as a color attachment to the off screen frame buffer
				GL.FramebufferTexture2D (FramebufferTarget.Framebuffer, FramebufferSlot.ColorAttachment0, destLumaTexture.Target, destLumaTexture.Name, 0);

				if (GL.CheckFramebufferStatus (FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete) {
					Console.Error.WriteLine ("Failed to make complete framebuffer object " + GL.CheckFramebufferStatus (FramebufferTarget.Framebuffer));
					foregroundLumaTexture.Dispose ();
					foregroundChromaTexture.Dispose ();
					backgroundLumaTexture.Dispose ();
					backgroundChromaTexture.Dispose ();
					destLumaTexture.Dispose ();
					destChromaTexture.Dispose ();
					VideoTextureCache.Flush (0);
					EAGLContext.SetCurrentContext (null);
					return;
				}

				GL.ClearColor (0.0f, 0.0f, 0.0f, 1.0f);
				GL.Clear (ClearBufferMask.ColorBufferBit);

				float[] quadVertexData1 = {
					-1.0f, 1.0f,
					1.0f, 1.0f,
					-1.0f, -1.0f,
					1.0f, -1.0f,
				};
				// texture data varies from 0 -> 1, whereas vertex data varies from -1 -> 1
				float[] quadTextureData1 = {
					0.5f + quadVertexData1 [0] / 2, 0.5f + quadVertexData1 [1] / 2,
					0.5f + quadVertexData1 [2] / 2, 0.5f + quadVertexData1 [3] / 2,
					0.5f + quadVertexData1 [4] / 2, 0.5f + quadVertexData1 [5] / 2,
					0.5f + quadVertexData1 [6] / 2, 0.5f + quadVertexData1 [7] / 2,
				};

				GL.Uniform1 (Uniforms [(int)Uniform.Y], 0);

				GL.VertexAttribPointer ((int)Attrib.Vertex_Y, 2, VertexAttribPointerType.Float, false, 0, quadVertexData1);
				GL.EnableVertexAttribArray ((int)Attrib.Vertex_Y);

				GL.VertexAttribPointer ((int)Attrib.TexCoord_Y, 2, VertexAttribPointerType.Float, false, 0, quadTextureData1);
				GL.EnableVertexAttribArray ((int)Attrib.TexCoord_Y);

				// Blend function to draw the foreground frame
				GL.Enable (EnableCap.Blend);
				GL.BlendFunc (BlendingFactorSrc.One, BlendingFactorDest.Zero);

				// Draw the foreground frame
				GL.DrawArrays (BeginMode.TriangleStrip, 0, 4);

				GL.Uniform1 (Uniforms [(int)Uniform.Y], 1);

				GL.VertexAttribPointer ((int)Attrib.Vertex_Y, 2, VertexAttribPointerType.Float, false, 0, quadVertexData1);
				GL.EnableVertexAttribArray ((int)Attrib.Vertex_Y);

				GL.VertexAttribPointer ((int)Attrib.TexCoord_Y, 2, VertexAttribPointerType.Float, false, 0, quadTextureData1);
				GL.EnableVertexAttribArray ((int)Attrib.TexCoord_Y);

				// Blend function to draw the background frame
				GL.BlendColor (0, 0, 0, tween);
				GL.BlendFunc (BlendingFactorSrc.ConstantAlpha, BlendingFactorDest.OneMinusConstantAlpha);

				// Draw the background frame
				GL.DrawArrays (BeginMode.TriangleStrip, 0, 4);

				// Perform similar operations as above for the UV plane
				GL.UseProgram (ProgramUV);

				GL.UniformMatrix4 (Uniforms [(int)Uniform.Render_Transform_UV], 1, false, preferredRenderTransform);

				GL.ActiveTexture (TextureUnit.Texture2);
				GL.BindTexture (foregroundChromaTexture.Target, foregroundChromaTexture.Name);
				GL.TexParameter (TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Linear);
				GL.TexParameter (TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Linear);
				GL.TexParameter (TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)All.ClampToEdge);
				GL.TexParameter (TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)All.ClampToEdge);

				GL.ActiveTexture (TextureUnit.Texture3);
				GL.BindTexture (backgroundChromaTexture.Target, backgroundChromaTexture.Name);
				GL.TexParameter (TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Linear);
				GL.TexParameter (TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Linear);
				GL.TexParameter (TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)All.ClampToEdge);
				GL.TexParameter (TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)All.ClampToEdge);

				GL.Viewport (0, 0, (int)destinationPixelBuffer.Width, (int)destinationPixelBuffer.Height);

				// Attach the destination texture as a color attachment to the off screen frame buffer
				GL.FramebufferTexture2D (FramebufferTarget.Framebuffer, FramebufferSlot.ColorAttachment0, destChromaTexture.Target, destChromaTexture.Name, 0);

				if (GL.CheckFramebufferStatus (FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete) {
					Console.Error.WriteLine ("Failed to make complete framebuffer object " + GL.CheckFramebufferStatus (FramebufferTarget.Framebuffer));
					foregroundLumaTexture.Dispose ();
					foregroundChromaTexture.Dispose ();
					backgroundLumaTexture.Dispose ();
					backgroundChromaTexture.Dispose ();
					destLumaTexture.Dispose ();
					destChromaTexture.Dispose ();
					this.VideoTextureCache.Flush (0);
					EAGLContext.SetCurrentContext (null);
					return;
				}

				GL.ClearColor (0.0f, 0.0f, 0.0f, 1.0f);
				GL.Clear (ClearBufferMask.ColorBufferBit);

				GL.Uniform1 (Uniforms [(int)Uniform.UV], 2);

				GL.VertexAttribPointer ((int)Attrib.Vertex_UV, 2, VertexAttribPointerType.Float, false, 0, quadVertexData1);
				GL.EnableVertexAttribArray ((int)Attrib.Vertex_UV);

				GL.VertexAttribPointer ((int)Attrib.TexCoord_UV, 2, VertexAttribPointerType.Float, false, 0, quadTextureData1);
				GL.EnableVertexAttribArray ((int)Attrib.TexCoord_UV);

				// Blend function to draw the foreground frame
				GL.BlendFunc (BlendingFactorSrc.One, BlendingFactorDest.Zero);

				// Draw the foreground frame
				GL.DrawArrays (BeginMode.TriangleStrip, 0, 4);

				GL.Uniform1 (Uniforms [(int)Uniform.UV], 3);

				GL.VertexAttribPointer ((int)Attrib.Vertex_UV, 2, VertexAttribPointerType.Float, false, 0, quadVertexData1);
				GL.EnableVertexAttribArray ((int)Attrib.Vertex_UV);

				GL.VertexAttribPointer ((int)Attrib.TexCoord_UV, 2, VertexAttribPointerType.Float, false, 0, quadTextureData1);
				GL.EnableVertexAttribArray ((int)Attrib.TexCoord_UV);

				// Blend function to draw the background frame
				GL.BlendColor (0, 0, 0, tween);
				GL.BlendFunc (BlendingFactorSrc.ConstantAlpha, BlendingFactorDest.OneMinusConstantAlpha);

				// Draw the background frame
				GL.DrawArrays (BeginMode.TriangleStrip, 0, 4);

				GL.Flush ();

				foregroundLumaTexture.Dispose ();
				foregroundChromaTexture.Dispose ();
				backgroundLumaTexture.Dispose ();
				backgroundChromaTexture.Dispose ();
				destLumaTexture.Dispose ();
				destChromaTexture.Dispose ();
				this.VideoTextureCache.Flush (0);
				EAGLContext.SetCurrentContext (null);
			}
		}
	}
}

