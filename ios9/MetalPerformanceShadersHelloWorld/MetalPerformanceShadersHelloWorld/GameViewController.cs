using CoreAnimation;
using CoreGraphics;
using Foundation;
using Metal;
using MetalKit;
using MetalPerformanceShaders;
using UIKit;
using System;

namespace MetalPerformanceShadersHelloWorld {
	public partial class GameViewController : UIViewController, IMTKViewDelegate, INSCoding {

		MTKView metalView;
		IMTLCommandQueue commandQueue;
		IMTLTexture sourceTexture;

		[Export ("initWithCoder:")]
		public GameViewController (NSCoder coder) : base (coder)
		{
		}

		public override void ViewDidLoad ()
		{
			metalView = (MTKView)View;

			// Set the view to use the default device.
			metalView.Device = MTLDevice.SystemDefault;

			// Make sure the current device supports MetalPerformanceShaders.
			if (metalView.Device == null)
				return;

			bool deviceSupportsMPS = MPSKernel.Supports (metalView.Device);

			if (!deviceSupportsMPS)
				return;

			MetalPerformanceShadersDisabledLabel.Hidden = true;
			SetupView ();
			SetupMetal ();
			LoadAssets ();
		}

		void SetupMetal ()
		{
			// Create a new command queue.
			commandQueue = metalView.Device.CreateCommandQueue ();
		}

		void SetupView ()
		{
			metalView.Delegate = this;

			// Setup the render target, choose values based on your app.
			metalView.DepthStencilPixelFormat = MTLPixelFormat.Depth32Float_Stencil8;

			// Set up pixel format as your input/output texture.
			metalView.ColorPixelFormat = MTLPixelFormat.BGRA8Unorm;

			// Allow to access to currentDrawable.texture write mode.
			metalView.FramebufferOnly = false;
		}

		void LoadAssets ()
		{
			var textureLoader = new MTKTextureLoader (metalView.Device);
			NSUrl url = NSBundle.MainBundle.GetUrlForResource ("AnimalImage", "png");

			NSError error;
			sourceTexture = textureLoader.FromUrl (url, null, out error);
		}

		void Render ()
		{
			// Create a new command buffer for each renderpass to the current drawable.
			IMTLCommandBuffer commandBuffer = commandQueue.CommandBuffer ();

			// Initialize MetalPerformanceShaders gaussianBlur with Sigma = 10.0f.
			var gaussianblur = new MPSImageGaussianBlur (metalView.Device, 10f);

			var drawable = ((CAMetalLayer)metalView.Layer).NextDrawable ();

			// Run MetalPerformanceShader gaussianblur.
			gaussianblur.EncodeToCommandBuffer (commandBuffer, sourceTexture, drawable.Texture);

			// Schedule a present using the current drawable.
			commandBuffer.PresentDrawable (drawable);
			// Finalize command buffer.
			commandBuffer.Commit ();
			commandBuffer.WaitUntilCompleted ();

			// To reuse a CAMetalDrawable object’s texture, you must deallocate the drawable after presenting it.
			drawable.Dispose ();
		}

		public void Draw (MTKView view)
		{
			Render ();
		}

		public void DrawableSizeWillChange (MTKView view, CGSize size)
		{
			// Called whenever view changes orientation or layout is changed
		}
	}
}