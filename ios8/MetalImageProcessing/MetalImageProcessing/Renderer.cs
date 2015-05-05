using System;
using System.Runtime.InteropServices;
using System.Threading;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using Metal;
using OpenTK;
using UIKit;

namespace MetalImageProcessing
{
	public class Renderer
	{
		const float interfaceOrientationLandscapeAngle = 35.0f;
		const float interfaceOrientationPortraitAngle = 50.0f;
		const float prespectiveNear = 0.1f;
		const float prespectiveFar = 100.0f;
		const int maxInflightBuffers = 3;

		nuint sampleCount;
		IMTLDevice device;
		MTLPixelFormat depthPixelFormat;
		MTLPixelFormat stencilPixelFormat;
		Quad mpQuad;

		// Interface Orientation
		UIInterfaceOrientation  mnOrientation;
		Semaphore inflightSemaphore;

		// Renderer globals
		IMTLCommandQueue commandQueue;
		IMTLLibrary shaderLibrary;
		IMTLDepthStencilState depthState;

		// Compute ivars
		IMTLComputePipelineState kernel;
		MTLSize workgroupSize;
		MTLSize localCount;

		// textured Quad
		Texture mpInTexture;
		IMTLTexture outTexture;
		IMTLRenderPipelineState pipelineState;

		#pragma warning disable 649
		// Dimensions
		CGSize size;
		#pragma warning restore 649

		// Viewing matrix is derived from an eye point, a reference point
		// indicating the center of the scene, and an up vector.
		Matrix4 lookAt;

		// Translate the object in (x,y,z) space.
		Matrix4 translate;

		// Quad transform buffers
		Matrix4 transform;
		IMTLBuffer transformBuffer;

		public Renderer ()
		{
			// initialize properties
			sampleCount = 1;
			depthPixelFormat = MTLPixelFormat.Depth32Float;
			stencilPixelFormat = MTLPixelFormat.Invalid;

			// find a usable Device
			device = MTLDevice.SystemDefault;

			// create a new command queue
			commandQueue = device.CreateCommandQueue ();

			NSError error;
			shaderLibrary = device.CreateLibrary ("default.metallib", out error);

			// if the shader libary isnt loading, nothing good will happen
			if (shaderLibrary == null)
				throw new Exception ("ERROR: Couldnt create a default shader library");

			inflightSemaphore = new Semaphore (maxInflightBuffers, maxInflightBuffers);
		}

		public void Compute (IMTLCommandBuffer commandBuffer)
		{
			IMTLComputeCommandEncoder computeEncoder = commandBuffer.ComputeCommandEncoder;

			if (computeEncoder == null)
				return;

			computeEncoder.SetComputePipelineState (kernel);
			computeEncoder.SetTexture (mpInTexture.MetalTexture, 0);
			computeEncoder.SetTexture (outTexture, 1);
			computeEncoder.DispatchThreadgroups (localCount, workgroupSize);
			computeEncoder.EndEncoding ();
		}

		public void Encode (IMTLRenderCommandEncoder renderEncoder)
		{
			renderEncoder.PushDebugGroup ("encode quad");
			renderEncoder.SetFrontFacingWinding (MTLWinding.CounterClockwise);
			renderEncoder.SetDepthStencilState (depthState);
			renderEncoder.SetRenderPipelineState (pipelineState);
			renderEncoder.SetVertexBuffer (transformBuffer, 0, 2);
			renderEncoder.SetFragmentTexture (outTexture, 0);

			// Encode quad vertex and texture coordinate buffers
			mpQuad.Encode (renderEncoder);

			// tell the render context we want to draw our primitives
			renderEncoder.DrawPrimitives (MTLPrimitiveType.Triangle, 0, 6, 1);
			renderEncoder.EndEncoding ();
			renderEncoder.PopDebugGroup ();
		}

		public void Reshape (ImageView view)
		{
			// To correctly compute the aspect ration determine the device
			// interface orientation.
			UIInterfaceOrientation orientation = UIApplication.SharedApplication.StatusBarOrientation;

			// Update the quad and linear _transformation matrices, if and
			// only if, the device orientation is changed.
			if (mnOrientation == orientation)
				return;

			float angleInDegrees = GetActualAngle (orientation, view.Layer.Bounds);
			CreateMatrix (angleInDegrees);
			UpdateBuffer ();
		}

		public void Render (ImageView view)
		{
			inflightSemaphore.WaitOne ();
			IMTLCommandBuffer commandBuffer = commandQueue.CommandBuffer ();

			// compute image processing on the (same) drawable texture
			Compute (commandBuffer);
			ICAMetalDrawable drawable = view.GetNextDrawable ();

			// create a render command encoder so we can render into something
			MTLRenderPassDescriptor renderPassDescriptor = view.GetRenderPassDescriptor (drawable);

			if (renderPassDescriptor == null) {
				inflightSemaphore.Release ();
				return;
			}

			// Get a render encoder
			IMTLRenderCommandEncoder renderEncoder = commandBuffer.CreateRenderCommandEncoder (renderPassDescriptor);

			// render textured quad
			Encode (renderEncoder);

			commandBuffer.AddCompletedHandler ((IMTLCommandBuffer buffer) => {
				inflightSemaphore.Release ();
				drawable.Dispose ();
			});

			commandBuffer.PresentDrawable (drawable);
			commandBuffer.Commit ();
		}

		public void Configure (ImageView renderView)
		{
			renderView.DepthPixelFormat = depthPixelFormat;
			renderView.StencilPixelFormat = stencilPixelFormat;
			renderView.SampleCount = sampleCount;

			// we need to set the framebuffer only property of the layer to NO so we
			// can perform compute on the drawable's texture
			var metalLayer = (CAMetalLayer)renderView.Layer;
			metalLayer.FramebufferOnly = false;

			if(!PreparePipelineState ())
				throw new ApplicationException ("ERROR: Failed creating a depth stencil state descriptor!");

			if(!PrepareTexturedQuad ("Default", "jpg"))
				throw new ApplicationException ("ERROR: Failed creating a textured quad!");

			if(!PrepareCompute ())
				throw new ApplicationException ("ERROR: Failed creating a compute stage!");

			if(!PrepareDepthStencilState ())
				throw new ApplicationException ("ERROR: Failed creating a depth stencil state!");

			if(!PrepareTransformBuffer ())
				throw new ApplicationException ("ERROR: Failed creating a transform buffer!");

			// Default orientation is unknown
			mnOrientation = UIInterfaceOrientation.Unknown;

			// Create linear transformation matrices
			PrepareTransforms ();
		}

		bool PreparePipelineState ()
		{
			// get the fragment function from the library
			IMTLFunction fragmentProgram = shaderLibrary.CreateFunction ("texturedQuadFragment");

			if(fragmentProgram == null)
				Console.WriteLine ("ERROR: Couldn't load fragment function from default library");

			// get the vertex function from the library
			IMTLFunction vertexProgram = shaderLibrary.CreateFunction ("texturedQuadVertex");

			if(vertexProgram == null)
				Console.WriteLine ("ERROR: Couldn't load vertex function from default library");

			//  create a pipeline state for the quad
			var quadPipelineStateDescriptor = new MTLRenderPipelineDescriptor {
				DepthAttachmentPixelFormat = depthPixelFormat,
				StencilAttachmentPixelFormat = MTLPixelFormat.Invalid,
				SampleCount = sampleCount,
				VertexFunction = vertexProgram,
				FragmentFunction = fragmentProgram
			};

			quadPipelineStateDescriptor.ColorAttachments[0].PixelFormat = MTLPixelFormat.BGRA8Unorm;

			NSError error;

			pipelineState = device.CreateRenderPipelineState (quadPipelineStateDescriptor, out error);
			if(pipelineState == null) {
				Console.WriteLine ("ERROR: Failed acquiring pipeline state descriptor: %@", error.Description);
				return false;
			}

			return true;
		}

		bool PrepareTexturedQuad (string texStr, string extStr)
		{
			mpInTexture = new Texture (texStr, extStr);
			bool isAcquired = mpInTexture.Finalize (device);
			mpInTexture.MetalTexture.Label = texStr;

			if(!isAcquired) {
				Console.WriteLine ("ERROR: Failed creating an input 2d texture!");
				return false;
			}

			size.Width = mpInTexture.Width;
			size.Height = mpInTexture.Height;

			mpQuad = new Quad (device) {
				Size = size
			};

			return true;
		}

		bool PrepareCompute ()
		{
			NSError error;

			// Create a compute kernel function
			IMTLFunction function = shaderLibrary.CreateFunction ("grayscale");
			if(function == null) {
				Console.WriteLine ("ERROR: Failed creating a new function!");
				return false;
			}

			// Create a compute kernel
			kernel = device.CreateComputePipelineState (function, out error);
			if(kernel == null) {
				Console.WriteLine ("ERROR: Failed creating a compute kernel: %@", error.Description);
				return false;
			}

			MTLTextureDescriptor texDesc = MTLTextureDescriptor.CreateTexture2DDescriptor (MTLPixelFormat.RGBA8Unorm, (nuint)size.Width, (nuint)size.Height, false);
			if(texDesc == null) {
				Console.WriteLine ("ERROR: Failed creating a texture 2d descriptor with RGBA unnormalized pixel format!");
				return false;
			}

			outTexture = device.CreateTexture (texDesc);
			if(outTexture == null) {
				Console.WriteLine ("ERROR: Failed creating an output 2d texture!");
				return false;
			}

			// Set the compute kernel's workgroup size and count
			workgroupSize = new MTLSize (1, 1, 1);
			localCount = new MTLSize ((nint)size.Width, (nint)size.Height, 1);
			return true;
		}

		bool PrepareTransformBuffer ()
		{
			// allocate regions of memory for the constant buffer
			transformBuffer = device.CreateBuffer ((nuint)Marshal.SizeOf<Matrix4> (), MTLResourceOptions.CpuCacheModeDefault);

			if(transformBuffer == null)
				return false;

			transformBuffer.Label = "TransformBuffer";

			return true;
		}

		bool PrepareDepthStencilState ()
		{
			var depthStateDesc = new MTLDepthStencilDescriptor {
				DepthCompareFunction = MTLCompareFunction.Always,
				DepthWriteEnabled = true
			};

			depthState = device.CreateDepthStencilState (depthStateDesc);

			if (depthState == null)
				return false;

			return true;
		}

		void PrepareTransforms ()
		{
			// Create a viewing matrix derived from an eye point, a reference point
			// indicating the center of the scene, and an up vector.
			var eye = Vector3.Zero;
			var center = new Vector3 (0f, 0f, 1f);
			var up = new Vector3 (0f, 1f, 0f);

			lookAt = MathUtils.LookAt (eye, center, up);
			// Translate the object in (x,y,z) space.
			translate = MathUtils.Translate (0.0f, -0.25f, 2.0f);
		}

		float GetActualAngle (UIInterfaceOrientation orientation, CGRect bounds)
		{
			// Update the device orientation
			mnOrientation = orientation;

			// Get the bounds for the current rendering layer
			mpQuad.Bounds = bounds;

			// Based on the device orientation, set the angle in degrees
			// between a plane which passes through the camera position
			// and the top of your screen and another plane which passes
			// through the camera position and the bottom of your screen.
			float dangle = 0.0f;

			switch(mnOrientation)
			{
			case UIInterfaceOrientation.LandscapeLeft:
			case UIInterfaceOrientation.LandscapeRight:
				dangle = interfaceOrientationLandscapeAngle;
				break;
			case UIInterfaceOrientation.Portrait:
			case UIInterfaceOrientation.PortraitUpsideDown:
			default:
				dangle = interfaceOrientationPortraitAngle;
				break;
			}

			return dangle;
		}

		void CreateMatrix (float angle)
		{
			// Describes a tranformation matrix that produces a perspective projection
			float near = prespectiveNear;
			float far = prespectiveFar;
			float rangle = MathUtils.Radians (angle);
			float length = near * (float)Math.Tan (rangle);

			float right = length / (float)mpQuad.Aspect;
			float left = -right;
			float top = length;
			float bottom = -top;

			Matrix4 perspective = MathUtils.FrustrumOc (left, right, bottom, top, near, far);

			// Create a viewing matrix derived from an eye point, a reference point
			// indicating the center of the scene, and an up vector.
			transform = (lookAt * translate).SwapColumnsAndRows ();

			// Create a linear _transformation matrix
			transform = (perspective * transform).SwapColumnsAndRows ();
		}

		void UpdateBuffer ()
		{
			// Update the buffer associated with the linear _transformation matrix
			int rawsize = Marshal.SizeOf <Matrix4> ();
			var rawdata = new byte[rawsize];

			GCHandle pinnedTransform = GCHandle.Alloc (transform, GCHandleType.Pinned);
			IntPtr ptr = pinnedTransform.AddrOfPinnedObject ();
			Marshal.Copy (ptr, rawdata, 0, rawsize);
			pinnedTransform.Free ();

			Marshal.Copy (rawdata, 0, transformBuffer.Contents, rawsize);
		}
	}

}
