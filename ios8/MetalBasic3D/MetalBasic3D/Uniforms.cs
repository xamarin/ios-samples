using System;
using Metal;
using OpenTK;
using System.Threading;

namespace MetalBasic3D
{
	struct Uniforms
	{
		public Matrix4 modelviewProjectionMatrix;
		public Matrix4 normalMatrix;
		public Vector4 ambientColor;
		public Vector4 diffuseColor;
	}
}

