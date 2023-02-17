using AVFoundation;
using CoreMedia;
using CoreVideo;
using OpenTK.Graphics.ES20;
using System;

namespace GLCameraRipple.Helpers {
	class DataOutputDelegate : AVCaptureVideoDataOutputSampleBufferDelegate {
		private WeakReference<ViewController> viewControllerReference;

		private CVOpenGLESTexture lumaTexture, chromaTexture;

		private int textureWidth, textureHeight;

		public DataOutputDelegate (ViewController container)
		{
			this.viewControllerReference = new WeakReference<ViewController> (container);
		}

		public override void DidOutputSampleBuffer (AVCaptureOutput captureOutput, CMSampleBuffer sampleBuffer, AVCaptureConnection connection)
		{
			try {
				using (var pixelBuffer = sampleBuffer.GetImageBuffer () as CVPixelBuffer) {
					int width = (int) pixelBuffer.Width;
					int height = (int) pixelBuffer.Height;

					if (this.viewControllerReference.TryGetTarget (out var container)) {
						if (container.Ripple == null || width != this.textureWidth || height != this.textureHeight) {
							this.textureWidth = width;
							this.textureHeight = height;
							container.SetupRipple (this.textureWidth, this.textureHeight);
						}

						this.CleanupTextures ();

						// Y-plane
						GL.ActiveTexture (TextureUnit.Texture0);
						All re = (All) 0x1903; // GL_RED_EXT, RED component from ARB OpenGL extension

						this.lumaTexture = container.VideoTextureCache.TextureFromImage (pixelBuffer, true, re, this.textureWidth, this.textureHeight, re, DataType.UnsignedByte, 0, out CVReturn status);
						if (this.lumaTexture == null) {
							Console.WriteLine ("Error creating luma texture: {0}", status);
							return;
						}

						GL.BindTexture (this.lumaTexture.Target, this.lumaTexture.Name);
						GL.TexParameter (TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int) All.ClampToEdge);
						GL.TexParameter (TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int) All.ClampToEdge);

						// UV Plane
						GL.ActiveTexture (TextureUnit.Texture1);
						re = (All) 0x8227; // GL_RG_EXT, RED GREEN component from ARB OpenGL extension
						this.chromaTexture = container.VideoTextureCache.TextureFromImage (pixelBuffer, true, re, this.textureWidth / 2, this.textureHeight / 2, re, DataType.UnsignedByte, 1, out status);
						if (this.chromaTexture == null) {
							Console.WriteLine ("Error creating chroma texture: {0}", status);
							return;
						}

						GL.BindTexture (this.chromaTexture.Target, this.chromaTexture.Name);
						GL.TexParameter (TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int) All.ClampToEdge);
						GL.TexParameter (TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int) All.ClampToEdge);
					}
				}
			} finally {
				sampleBuffer.Dispose ();
			}
		}

		private void CleanupTextures ()
		{
			if (this.lumaTexture != null) {
				this.lumaTexture.Dispose ();
				this.lumaTexture = null;
			}

			if (this.chromaTexture != null) {
				this.chromaTexture.Dispose ();
				this.chromaTexture = null;
			}

			if (this.viewControllerReference.TryGetTarget (out var container)) {
				container.VideoTextureCache.Flush (CVOptionFlags.None);
			}
		}
	}
}
