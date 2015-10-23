using System;
using CoreGraphics;
using OpenGLES;
using OpenTK.Graphics.ES20;

namespace AVCustomEdit
{
	public class DiagonalWipeRenderer : OpenGLRenderer
	{
		const int ForegroundTrack = 0;
		const int BackgroundTrack = 1;

		CGPoint diagonalEnd1;
		CGPoint	diagonalEnd2;

		void quadVertexCoordinates (ref float[] vertexCoordinates, int trackID, float tween)
		{
			/*
			 diagonalEnd1 and diagonalEnd2 represent the endpoints of a line which partitions the frame on screen into the two parts.

			 diagonalEnd1
			 ------------X-----------
			 |			 			|
			 |			  			X diagonalEnd2
			 |						|
			 |						|
			 ------------------------

			 The below conditionals, use the tween factor as a measure to determine the size of the foreground and background quads.

			 */

			if (tween <= 0.5f) { // The expectation here is that in half the timeRange of the transition we reach the diagonal of the frame
				diagonalEnd1 = new CGPoint (1f - tween * 4f, -1f);
				diagonalEnd2 = new CGPoint (1f, -1f + tween * 4f);

				vertexCoordinates [6] = (float)diagonalEnd2.X;
				vertexCoordinates [7] = (float)diagonalEnd2.Y;
				vertexCoordinates [8] = (float)diagonalEnd1.X;
				vertexCoordinates [9] = (float)diagonalEnd1.Y;
			} else if (tween > 0.5f && tween < 1f) {
				if (trackID == ForegroundTrack) {
					diagonalEnd1 = new CGPoint (-1f, -1 + (tween - 0.5f) * 4f);
					diagonalEnd2 = new CGPoint (1f - (tween - 0.5f) * 4f, 1f);

					vertexCoordinates [2] = (float)diagonalEnd2.X;
					vertexCoordinates [3] = (float)diagonalEnd2.Y;
					vertexCoordinates [4] = (float)diagonalEnd1.X;
					vertexCoordinates [5] = (float)diagonalEnd1.Y;
					vertexCoordinates [6] = (float)diagonalEnd1.X;
					vertexCoordinates [7] = (float)diagonalEnd1.Y;
					vertexCoordinates [8] = (float)diagonalEnd1.X;
					vertexCoordinates [9] = (float)diagonalEnd1.Y;
				} else if (trackID == BackgroundTrack) {
					vertexCoordinates [4] = 1f;
					vertexCoordinates [5] = 1f;
					vertexCoordinates [6] = -1f;
					vertexCoordinates [7] = -1f;
				}
			} else if (tween >= 1f) {
				diagonalEnd1 = new CGPoint (1f, -1f);
				diagonalEnd2 = new CGPoint (1f, -1f);
			}
		}

		public override void RenderPixelBuffer (CoreVideo.CVPixelBuffer destinationPixelBuffer, CoreVideo.CVPixelBuffer foregroundPixelBuffer, CoreVideo.CVPixelBuffer backgroundPixelBuffer, float tween)
		{
			EAGLContext.SetCurrentContext (CurrentContext);

			if (foregroundPixelBuffer == null && backgroundPixelBuffer == null)
				return;

			var foregroundLumaTexture = LumaTextureForPixelBuffer (foregroundPixelBuffer);
			var foregroundChromaTexture = ChromaTextureForPixelBuffer (foregroundPixelBuffer);

			var backgroundLumaTexture = LumaTextureForPixelBuffer (backgroundPixelBuffer);
			var backgroundChromaTexture = ChromaTextureForPixelBuffer (backgroundPixelBuffer);

			var destLumaTexture = LumaTextureForPixelBuffer (destinationPixelBuffer);
			var destChromaTexture = ChromaTextureForPixelBuffer (destinationPixelBuffer);

			GL.UseProgram (ProgramY);

			// Set the render transformq
			float[] preferredRenderTransform = {
				(float)RenderTransform.xx,(float) RenderTransform.xy, (float)RenderTransform.x0, 0.0f,
				(float)RenderTransform.yx, (float)RenderTransform.yy, (float)RenderTransform.y0, 0.0f,
				0.0f, 				0.0f, 				1.0f, 				0.0f,
				0.0f,				0.0f,				0.0f,				1.0f
			};

			GL.UniformMatrix4 (Uniforms [(int)Uniform.Y], 1, false, preferredRenderTransform);

			GL.BindFramebuffer (FramebufferTarget.Framebuffer, (int)OffscreenBufferHandle);

			GL.Viewport (0, 0, (int)destinationPixelBuffer.GetWidthOfPlane (0), (int)destinationPixelBuffer.GetHeightOfPlane (0));

			// Y planes of foreground and background frame are used to render the Y plane of the destination frame
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

			GL.FramebufferTexture2D (FramebufferTarget.Framebuffer, FramebufferSlot.ColorAttachment0,
				destLumaTexture.Target, destLumaTexture.Name, 0);

			if (GL.CheckFramebufferStatus (FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete) {
				Console.WriteLine ("Failed to make complete frmaebuffer object: {0}", GL.CheckFramebufferStatus (FramebufferTarget.Framebuffer));

				foregroundLumaTexture.Dispose ();
				foregroundChromaTexture.Dispose ();
				backgroundLumaTexture.Dispose ();
				backgroundChromaTexture.Dispose ();
				destLumaTexture.Dispose ();
				destChromaTexture.Dispose ();

				// Periodic texture cache flush every frame
				VideoTextureCache.Flush (CoreVideo.CVOptionFlags.None);

				EAGLContext.SetCurrentContext (null);
			}

			GL.ClearColor (0f, 0f, 0f, 1f);
			GL.Clear (ClearBufferMask.ColorBufferBit);

			float[] quadVertexData1 = {
				-1.0f, 1.0f,
				1.0f, 1.0f,
				-1.0f, -1.0f,
				1.0f, -1.0f,
				1.0f, -1.0f,
			};
			// Compute the vertex data for the foreground frame at this instructionLerp
			quadVertexCoordinates (ref quadVertexData1, ForegroundTrack, tween);

			// texture data varies from 0 -> 1, whereas vertex data varies from -1 -> 1
			float[] quadTextureData1 = {
				0.5f + quadVertexData1[0] / 2f, 0.5f + quadVertexData1[1] / 2f,
				0.5f + quadVertexData1[2] / 2f, 0.5f + quadVertexData1[3] / 2f,
				0.5f + quadVertexData1[4] / 2f, 0.5f + quadVertexData1[5] / 2f,
				0.5f + quadVertexData1[6] / 2f, 0.5f + quadVertexData1[7] / 2f,
				0.5f + quadVertexData1[8] / 2f, 0.5f + quadVertexData1[9] / 2f,
			};

			GL.Uniform1 (Uniforms [(int)Uniform.Y], 0f);

			GL.VertexAttribPointer<float> ((int)Attrib.Vertex_Y, 2, VertexAttribPointerType.Float, false, 0, quadVertexData1);
			GL.EnableVertexAttribArray ((int)Attrib.Vertex_Y);

			GL.VertexAttribPointer<float> ((int)Attrib.TexCoord_Y, 2, VertexAttribPointerType.Float, false, 0, quadTextureData1);
			GL.EnableVertexAttribArray ((int)Attrib.TexCoord_Y);

			GL.DrawArrays (BeginMode.TriangleStrip, 0, 5);

			float[] quadVertexData2 = {
				(float)diagonalEnd2.X, (float)diagonalEnd2.Y,
				(float)diagonalEnd1.X, (float)diagonalEnd1.Y,
				1.0f, -1.0f,
				1.0f, -1.0f,
				1.0f, -1.0f,
			};

			quadVertexCoordinates (ref quadVertexData2, BackgroundTrack, tween);

			float[] quadTextureData2 = {
				0.5f + quadVertexData2[0] / 2f, 0.5f + quadVertexData2[1] / 2f,
				0.5f + quadVertexData2[2] / 2f, 0.5f + quadVertexData2[3] / 2f,
				0.5f + quadVertexData2[4] / 2f, 0.5f + quadVertexData2[5] / 2f,
				0.5f + quadVertexData2[6] / 2f, 0.5f + quadVertexData2[7] / 2f,
				0.5f + quadVertexData2[8] / 2f, 0.5f + quadVertexData2[9] / 2f,
			};

			GL.Uniform1 (Uniforms [(int)Uniform.Y], 1);

			GL.VertexAttribPointer<float> ((int)Attrib.Vertex_Y, 2, VertexAttribPointerType.Float, false, 0, quadVertexData2);
			GL.EnableVertexAttribArray ((int)Attrib.Vertex_Y);

			GL.VertexAttribPointer<float> ((int)Attrib.TexCoord_Y, 2, VertexAttribPointerType.Float, false, 0, quadTextureData2);
			GL.EnableVertexAttribArray ((int)Attrib.TexCoord_Y);

			// Draw the background frame
			GL.DrawArrays (BeginMode.TriangleStrip, 0, 5);

			// Perform similar operations as above for the UV plane
			GL.UseProgram (ProgramUV);

			GL.UniformMatrix4 (Uniforms [(int)Uniform.Render_Transform_UV], 1, false, preferredRenderTransform);

			// UV planes of foreground and background frame are used to render the UV plane of the destination frame
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

			GL.Viewport (0, 0,(int) destinationPixelBuffer.GetWidthOfPlane (1), (int)destinationPixelBuffer.GetHeightOfPlane (1));

			// Attach the destination texture as a color attachment to the off screen frame buffer
			GL.FramebufferTexture2D (FramebufferTarget.Framebuffer, FramebufferSlot.ColorAttachment0, destChromaTexture.Target,
				destChromaTexture.Name, 0);

			if (GL.CheckFramebufferStatus (FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete) {
				Console.WriteLine ("Failed to make complete framebuffer object: {0}",
					GL.CheckFramebufferStatus (FramebufferTarget.Framebuffer));

				foregroundLumaTexture.Dispose ();
				foregroundChromaTexture.Dispose ();
				backgroundLumaTexture.Dispose ();
				backgroundChromaTexture.Dispose ();
				destLumaTexture.Dispose ();
				destChromaTexture.Dispose ();

				// Periodic texture cache flush every frame
				VideoTextureCache.Flush (CoreVideo.CVOptionFlags.None);

				EAGLContext.SetCurrentContext (null);
			}

			GL.ClearColor (0f, 0f, 0f, 1f);
			GL.Clear (ClearBufferMask.ColorBufferBit);

			GL.Uniform1 (Uniforms [(int)Uniform.UV], 2);

			GL.VertexAttribPointer<float> ((int)Attrib.Vertex_UV, 2, VertexAttribPointerType.Float, false, 0, quadVertexData1);
			GL.EnableVertexAttribArray ((int)Attrib.Vertex_UV);

			GL.VertexAttribPointer<float> ((int)Attrib.TexCoord_UV, 2, VertexAttribPointerType.Float, false, 0, quadTextureData1);
			GL.EnableVertexAttribArray ((int)Attrib.TexCoord_UV);

			GL.DrawArrays (BeginMode.TriangleStrip, 0, 5);

			GL.Uniform1 (Uniforms [(int)Uniform.UV], 3);

			GL.VertexAttribPointer<float> ((int)Attrib.Vertex_UV, 2, VertexAttribPointerType.Float, false, 0, quadVertexData2);
			GL.EnableVertexAttribArray ((int)Attrib.Vertex_UV);

			GL.VertexAttribPointer<float> ((int)Attrib.TexCoord_UV, 2, VertexAttribPointerType.Float, false, 0, quadTextureData2);
			GL.EnableVertexAttribArray ((int)Attrib.TexCoord_UV);

			GL.DrawArrays (BeginMode.TriangleStrip, 0, 5);

			GL.Flush ();

			foregroundLumaTexture.Dispose ();
			foregroundChromaTexture.Dispose ();
			backgroundLumaTexture.Dispose ();
			backgroundChromaTexture.Dispose ();
			destLumaTexture.Dispose ();
			destChromaTexture.Dispose ();

			// Periodic texture cache flush every frame
			VideoTextureCache.Flush (CoreVideo.CVOptionFlags.None);

			EAGLContext.SetCurrentContext (null);
		}
	}
}
