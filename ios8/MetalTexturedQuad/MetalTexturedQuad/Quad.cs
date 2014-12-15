using System;
using System.Runtime.InteropServices;
using CoreGraphics;
using Foundation;
using Metal;
using OpenTK;

namespace MetalTexturedQuad
{
	public class Quad : NSObject
	{
		// textured Quad
		IMTLBuffer vertexBuffer;
		IMTLBuffer texCoordBuffer;

		// Scale
		Vector2 scale;

		// Dimensions
		CGRect bounds;

		readonly Vector4[] quadVertices = new Vector4 [] {
			new Vector4 (-1f, -1f, 0f, 1f),
			new Vector4 (1f, -1f, 0f, 1f),
			new Vector4 (-1f, 1f, 0f, 1f),

			new Vector4 (1f, -1f, 0f, 1f),
			new Vector4 (-1f, 1f, 0f, 1f),
			new Vector4 (1f, 1f, 0f, 1f)
		};

		readonly Vector2[] quadTexCoords = new Vector2 [] {
			new Vector2 (0f, 0f),
			new Vector2 (1f, 0f),
			new Vector2 (0f, 1f),

			new Vector2 (1f, 0f),
			new Vector2 (0f, 1f),
			new Vector2 (1f, 1f)
		};

		// Indices
		nuint vertexIndex;
		nuint texCoordIndex;

		public CGSize Size { get; set; }

		public nfloat Aspect  { get; set; }

		public CGRect Bounds {
			get {
				return bounds;
			}

			set {
				bounds = value;
				UpdateBounds ();
			}
		}

		// Designated initializer
		public Quad (IMTLDevice device)
		{
			if (device == null)
				throw new Exception ("ERROR: Invalid device!");

			vertexBuffer = device.CreateBuffer<Vector4> (quadVertices, MTLResourceOptions.CpuCacheModeDefault);
			if (vertexBuffer == null)
				Console.WriteLine ("ERROR: Failed creating a vertex buffer for a quad!");
			vertexBuffer.Label = "quad vertices";

			texCoordBuffer = device.CreateBuffer<Vector2> (quadTexCoords, MTLResourceOptions.CpuCacheModeDefault);
			if (texCoordBuffer == null)
				Console.WriteLine ("ERROR: Failed creating a 2d texture coordinate buffer!");
			texCoordBuffer.Label = "quad texcoords";

			vertexIndex = 0;
			texCoordIndex = 1;
			Aspect = 1f;

			scale = Vector2.One;
		}

		// Encoder
		public void Encode (IMTLRenderCommandEncoder renderEncoder)
		{
			renderEncoder.SetVertexBuffer (vertexBuffer, 0, vertexIndex);
			renderEncoder.SetVertexBuffer (texCoordBuffer, 0, texCoordIndex);
		}

		void UpdateBounds ()
		{
			Aspect = bounds.Size.Width / bounds.Size.Height;

			float aspect = 1.0f / (float)Aspect;
			Vector2 localScale = Vector2.Zero;

			localScale.X = (float)(aspect * Size.Width / Bounds.Size.Width);
			localScale.Y = (float)(Size.Height / Bounds.Size.Height);

			// Did the scaling factor change
			bool newScale = (scale.X != localScale.X) || (scale.Y != localScale.Y);

			// Set the (x,y) bounds of the quad
			if (newScale) {

				// Update the scaling factor
				scale = localScale;

				var vertices = new Vector4[quadVertices.Length];
				IntPtr contentPtr = vertexBuffer.Contents;
				for (int i = 0; i < vertices.Length; i++) {
					vertices [i] = (Vector4)Marshal.PtrToStructure (contentPtr, typeof(Vector4));
					contentPtr += Marshal.SizeOf<Vector4> ();
				}

				// First triangle
				vertices [0].X = -scale.X;
				vertices [0].Y = -scale.Y;

				vertices [1].X = scale.X;
				vertices [1].Y = -scale.Y;

				vertices [2].X = -scale.X;
				vertices [2].Y = scale.Y;

				// Second triangle
				vertices [3].X = scale.X;
				vertices [3].Y = -scale.Y;

				vertices [4].X = -scale.X;
				vertices [4].Y = scale.Y;

				vertices [5].X = scale.X;
				vertices [5].Y = scale.Y;

				int rawsize = quadVertices.Length * Marshal.SizeOf <Vector4> ();
				var rawdata = new byte[rawsize];
				GCHandle pinnedTransform = GCHandle.Alloc (vertices, GCHandleType.Pinned);
				IntPtr ptr = pinnedTransform.AddrOfPinnedObject ();
				Marshal.Copy (ptr, rawdata, 0, rawsize);
				pinnedTransform.Free ();

				Marshal.Copy (rawdata, 0, vertexBuffer.Contents, rawsize);
			}
		}
	}
}

