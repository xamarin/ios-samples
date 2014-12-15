using System;
using Metal;
using OpenTK;
using System.Threading;
using Foundation;
using System.Runtime.InteropServices;
using CoreAnimation;

namespace MetalBasic3D
{
	public class Renderer : IGameViewController, IGameView
	{
		const int max_inflight_buffers = 3;
		const int max_bytes_per_frame = 1024 * 1024;

		Vector4 box1AmbientColor = new Vector4 (0.18f, 0.24f, 0.8f, 1.0f);
		Vector4 box1DiffuseColor = new Vector4 (0.4f, 0.4f, 1.0f, 1.0f);

		Vector4 box2AmbientColor = new Vector4 (0.8f, 0.24f, 0.1f, 1.0f);
		Vector4 box2DiffuseColor = new Vector4 (0.8f, 0.4f, 0.4f, 1.0f);

		float FOVY = 65.0f;
		Vector3 eye = new Vector3 (0.0f, 0.0f, 0.0f);
		Vector3 center = new Vector3 (0.0f, 0.0f, 1.0f);
		Vector3 up = new Vector3 (0.0f, 1.0f, 0.0f);
		float rotation;

		const float width = 1f;
		const float height = 1f;
		const float depth = 1f;

		float[] cubeVertexData = new float [216] {
			width, -height, depth,   0.0f, -1.0f,  0.0f,
			-width, -height, depth,   0.0f, -1.0f, 0.0f,
			-width, -height, -depth,   0.0f, -1.0f,  0.0f,
			width, -height, -depth,  0.0f, -1.0f,  0.0f,
			width, -height, depth,   0.0f, -1.0f,  0.0f,
			-width, -height, -depth,   0.0f, -1.0f,  0.0f,

			width, height, depth,    1.0f, 0.0f,  0.0f,
			width, -height, depth,   1.0f,  0.0f,  0.0f,
			width, -height, -depth,  1.0f,  0.0f,  0.0f,
			width, height, -depth,   1.0f, 0.0f,  0.0f,
			width, height, depth,    1.0f, 0.0f,  0.0f,
			width, -height, -depth,  1.0f,  0.0f,  0.0f,

			-width, height, depth,    0.0f, 1.0f,  0.0f,
			width, height, depth,    0.0f, 1.0f,  0.0f,
			width, height, -depth,   0.0f, 1.0f,  0.0f,
			-width, height, -depth,   0.0f, 1.0f,  0.0f,
			-width, height, depth,    0.0f, 1.0f,  0.0f,
			width, height, -depth,   0.0f, 1.0f,  0.0f,

			-width, -height, depth,  -1.0f,  0.0f, 0.0f,
			-width, height, depth,   -1.0f, 0.0f,  0.0f,
			-width, height, -depth,  -1.0f, 0.0f,  0.0f,
			-width, -height, -depth,  -1.0f,  0.0f,  0.0f,
			-width, -height, depth,  -1.0f,  0.0f, 0.0f,
			-width, height, -depth,  -1.0f, 0.0f,  0.0f,

			width, height,  depth,  0.0f, 0.0f,  1.0f,
			-width, height,  depth,  0.0f, 0.0f,  1.0f,
			-width, -height, depth,   0.0f,  0.0f, 1.0f,
			-width, -height, depth,   0.0f,  0.0f, 1.0f,
			width, -height, depth,   0.0f,  0.0f,  1.0f,
			width, height,  depth,  0.0f, 0.0f,  1.0f,

			width, -height, -depth,  0.0f,  0.0f, -1.0f,
			-width, -height, -depth,   0.0f,  0.0f, -1.0f,
			-width, height, -depth,  0.0f, 0.0f, -1.0f,
			width, height, -depth,  0.0f, 0.0f, -1.0f,
			width, -height, -depth,  0.0f,  0.0f, -1.0f,
			-width, height, -depth,  0.0f, 0.0f, -1.0f
		};

		MTLPixelFormat depthPixelFormat;
		MTLPixelFormat stencilPixelFormat;
		nuint sampleCount;
		int constantDataBufferIndex;

		IMTLDevice device;
		IMTLCommandQueue commandQueue;
		IMTLLibrary defaultLibrary;

		Semaphore inflightSemaphore;
		IMTLBuffer[] dynamicConstantBuffer;

		// render stage
		IMTLRenderPipelineState pipelineState;
		IMTLBuffer vertexBuffer;
		IMTLDepthStencilState depthState;

		// global transform data
		Matrix4 projectionMatrix;
		Matrix4 viewMatrix;

		Uniforms[] constantBuffer;
		int multiplier;

		public IMTLDevice Device { get; private set; }

		public Renderer ()
		{
			sampleCount = 4;
			depthPixelFormat = MTLPixelFormat.Depth32Float;
			stencilPixelFormat = MTLPixelFormat.Invalid;

			// find a usable Device
			device = MTLDevice.SystemDefault;

			// create a new command queue
			commandQueue = device.CreateCommandQueue ();

			NSError error;
			defaultLibrary = device.CreateLibrary ("default.metallib", out error);

			// if the shader libary isnt loading, nothing good will happen
			if (defaultLibrary == null)
				throw new Exception ("ERROR: Couldnt create a default shader library");

			constantDataBufferIndex = 0;
			inflightSemaphore = new Semaphore (max_inflight_buffers, max_inflight_buffers);

			constantBuffer = new Uniforms[2];
			constantBuffer [0].ambientColor = box1AmbientColor;
			constantBuffer [0].diffuseColor = box1DiffuseColor;

			constantBuffer [1].ambientColor = box2AmbientColor;
			constantBuffer [1].diffuseColor = box2DiffuseColor;

			multiplier = 1;
		}

		public void RenderViewControllerUpdate (GameViewController gameViewController)
		{
			Matrix4 baseModel = CreateMatrixFromTranslation (0.0f, 0.0f, 5.0f) * CreateMatrixFromRotation (rotation, 0.0f, 1.0f, 0.0f);
			Matrix4 baseMv = viewMatrix * baseModel;
			Matrix4 modelViewMatrix = CreateMatrixFromTranslation (0.0f, 0.0f, 1.5f) * CreateMatrixFromRotation (rotation, 1.0f, 1.0f, 1.0f);
			modelViewMatrix = baseMv * modelViewMatrix;

			constantBuffer [0].normalMatrix = Matrix4.Invert (Matrix4.Transpose (modelViewMatrix));
			constantBuffer [0].modelviewProjectionMatrix = Matrix4.Transpose (projectionMatrix * modelViewMatrix);

			modelViewMatrix = CreateMatrixFromTranslation (0.0f, 0.0f, -1.5f);
			modelViewMatrix = modelViewMatrix * CreateMatrixFromRotation (rotation, 1.0f, 1.0f, 1.0f);
			modelViewMatrix = baseMv * modelViewMatrix;

			constantBuffer [1].normalMatrix = Matrix4.Invert (Matrix4.Transpose (modelViewMatrix));
			constantBuffer [1].modelviewProjectionMatrix = Matrix4.Transpose (projectionMatrix * modelViewMatrix);

			if (constantBuffer [1].ambientColor.Y >= 0.8) {
				multiplier = -1;
				constantBuffer [1].ambientColor.Y = 0.79f;
			} else if (constantBuffer [1].ambientColor.Y <= 0.2) {
				multiplier = 1;
				constantBuffer [1].ambientColor.Y = 0.21f;
			} else {
				constantBuffer [1].ambientColor.Y += multiplier * 0.01f;
			}

			rotation += (float)gameViewController.TimeSinceLastDraw;
		}

		public void RenderViewController (GameViewController gameViewController, bool value)
		{
			// timer is suspended/resumed
			// Can do any non-rendering related background work here when suspended
		}

		public void Reshape (GameView view)
		{
			var aspect = (float)(view.Bounds.Size.Width / view.Bounds.Size.Height);
			projectionMatrix = CreateMatrixFromPerspective (FOVY, aspect, 0.1f, 100.0f);
			viewMatrix = LookAt (eye, center, up);
		}

		public void Render (GameView view)
		{
			inflightSemaphore.WaitOne ();

			PrepareToDraw ();

			IMTLCommandBuffer commandBuffer = commandQueue.CommandBuffer ();
			ICAMetalDrawable drawable = view.GetNextDrawable ();
			MTLRenderPassDescriptor renderPassDescriptor = view.GetRenderPassDescriptor (drawable);

			if (renderPassDescriptor == null)
				Console.WriteLine ("ERROR: Failed to get render pass descriptor from view!");

			IMTLRenderCommandEncoder renderEncoder = commandBuffer.CreateRenderCommandEncoder (renderPassDescriptor);
			renderEncoder.SetDepthStencilState (depthState);

			RenderBox (renderEncoder, view, 0, "Box1");
			RenderBox (renderEncoder, view, (nuint)Marshal.SizeOf<Uniforms> (), "Box2");
			renderEncoder.EndEncoding ();

			commandBuffer.AddCompletedHandler ((IMTLCommandBuffer buffer) => {
				drawable.Dispose ();
				inflightSemaphore.Release ();
			});

			commandBuffer.PresentDrawable (drawable);
			commandBuffer.Commit ();

			constantDataBufferIndex = (constantDataBufferIndex + 1) % max_inflight_buffers;
		}

		public void Configure (GameView view)
		{
			view.DepthPixelFormat = depthPixelFormat;
			view.StencilPixelFormat = stencilPixelFormat;
			view.SampleCount = sampleCount;

			dynamicConstantBuffer = new IMTLBuffer[max_inflight_buffers];

			// allocate one region of memory for the constant buffer
			for (int i = 0; i < max_inflight_buffers; i++) {
				dynamicConstantBuffer [i] = device.CreateBuffer (max_bytes_per_frame, MTLResourceOptions.CpuCacheModeDefault);
				dynamicConstantBuffer [i].Label = string.Format ("ConstantBuffer{0}", i);
			}

			// load the fragment program into the library
			IMTLFunction fragmentProgram = defaultLibrary.CreateFunction ("lighting_fragment");
			if (fragmentProgram == null)
				Console.WriteLine ("ERROR: Couldnt load fragment function from default library");

			// load the vertex program into the library
			IMTLFunction vertexProgram = defaultLibrary.CreateFunction ("lighting_vertex");
			if (vertexProgram == null)
				Console.WriteLine ("ERROR: Couldnt load vertex function from default library");

			// setup the vertex buffers
			vertexBuffer = device.CreateBuffer<float> (cubeVertexData, MTLResourceOptions.CpuCacheModeDefault);
			vertexBuffer.Label = "Vertices";

			//  create a reusable pipeline state
			var pipelineStateDescriptor = new MTLRenderPipelineDescriptor {
				Label = "MyPipeline",
				SampleCount = sampleCount,
				VertexFunction = vertexProgram,
				FragmentFunction = fragmentProgram,
				DepthAttachmentPixelFormat = depthPixelFormat
			};

			pipelineStateDescriptor.ColorAttachments [0].PixelFormat = MTLPixelFormat.BGRA8Unorm;

			NSError error;
			pipelineState = device.CreateRenderPipelineState (pipelineStateDescriptor, out error);

			var depthStateDesc = new MTLDepthStencilDescriptor {
				DepthCompareFunction = MTLCompareFunction.Less,
				DepthWriteEnabled = true
			};

			depthState = device.CreateDepthStencilState (depthStateDesc);
		}

		void PrepareToDraw ()
		{
			int rawsize = 2 * Marshal.SizeOf <Uniforms> ();
			byte[] rawdata = new byte[rawsize];

			GCHandle pinnedArray = GCHandle.Alloc (constantBuffer, GCHandleType.Pinned);
			IntPtr ptr = pinnedArray.AddrOfPinnedObject ();
			Marshal.Copy (ptr, rawdata, 0, rawsize);
			pinnedArray.Free ();

			Marshal.Copy (rawdata, 0, dynamicConstantBuffer [constantDataBufferIndex].Contents, rawsize);
			Marshal.Copy (rawdata, 0, IntPtr.Add (dynamicConstantBuffer [constantDataBufferIndex].Contents, rawsize), rawsize);
		}

		void RenderBox (IMTLRenderCommandEncoder renderEncoder, GameView view, nuint offset, string name)
		{
			renderEncoder.PushDebugGroup (name);
			//  set context state
			renderEncoder.SetRenderPipelineState (pipelineState);
			renderEncoder.SetVertexBuffer (vertexBuffer, 0, 0);
			renderEncoder.SetVertexBuffer (dynamicConstantBuffer [constantDataBufferIndex], offset, 1);

			// tell the render context we want to draw our primitives
			renderEncoder.DrawPrimitives (MTLPrimitiveType.Triangle, 0, 36, 1);
			renderEncoder.PopDebugGroup ();
		}

		Matrix4 CreateMatrixFromPerspective (float fovY, float aspect, float near, float far)
		{
			float yscale = 1.0f / (float)Math.Tan (fovY * 0.5f);
			float xscale = yscale / aspect;
			float zScale = far / (far - near);

			var m = new Matrix4 {
				Row0 = new Vector4 (xscale, 0.0f, 0.0f, 0.0f),
				Row1 = new Vector4 (0.0f, yscale, 0.0f, 0.0f),
				Row2 = new Vector4 (0.0f, 0.0f, zScale, -near * zScale),
				Row3 = new Vector4 (0.0f, 0.0f, 1.0f, 0.0f)
			};

			return m;
		}

		Matrix4 CreateMatrixFromTranslation (float x, float y, float z)
		{
			var m = Matrix4.Identity;
			m.Row0.W = x;
			m.Row1.W = y;
			m.Row2.W = z;
			m.Row3.W = 1.0f;
			return m;
		}

		Matrix4 CreateMatrixFromRotation (float radians, float x, float y, float z)
		{
			Vector3 v = Vector3.Normalize (new Vector3 (x, y, z));
			var cos = (float)Math.Cos (radians);
			var sin = (float)Math.Sin (radians);
			float cosp = 1.0f - cos;

			var m = new Matrix4 {
				Row0 = new Vector4 (cos + cosp * v.X * v.X, cosp * v.X * v.Y - v.Z * sin, cosp * v.X * v.Z + v.Y * sin, 0.0f),
				Row1 = new Vector4 (cosp * v.X * v.Y + v.Z * sin, cos + cosp * v.Y * v.Y, cosp * v.Y * v.Z - v.X * sin, 0.0f),
				Row2 = new Vector4 (cosp * v.X * v.Z - v.Y * sin, cosp * v.Y * v.Z + v.X * sin, cos + cosp * v.Z * v.Z, 0.0f),
				Row3 = new Vector4 (0.0f, 0.0f, 0.0f, 1.0f)
			};

			return m;
		}

		Matrix4 LookAt (Vector3 eye, Vector3 center, Vector3 up)
		{
			Vector3 zAxis = Vector3.Normalize (center - eye);
			Vector3 xAxis = Vector3.Normalize (Vector3.Cross (up, zAxis));
			Vector3 yAxis = Vector3.Cross (zAxis, xAxis);

			var P = new Vector4 (xAxis.X, yAxis.X, zAxis.X, 0.0f);
			var Q = new Vector4 (xAxis.Y, yAxis.Y, zAxis.Y, 0.0f);
			var R = new Vector4 (xAxis.Z, yAxis.Z, zAxis.Z, 0.0f);
			var S = new Vector4 (Vector3.Dot (xAxis, eye), Vector3.Dot (yAxis, eye), Vector3.Dot (zAxis, eye), 1.0f);

			var result = new Matrix4 (P, Q, R, S);
			return Matrix4.Transpose (result);
		}
	}
}

