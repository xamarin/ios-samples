using System;
using CoreGraphics;

using Foundation;
using AVFoundation;
using CoreFoundation;
using CoreVideo;
using CoreMedia;

namespace AVCustomEdit
{
	public class CrossDissolveCompositor : CustomVideoCompositor
	{
		public CrossDissolveCompositor (IntPtr handle) : base (handle)
		{
			oglRender = new CrossDissolveRenderer ();
		}

		public CrossDissolveCompositor(AVVideoComposition videoComposition) : base(videoComposition)
		{
			oglRender = new CrossDissolveRenderer ();
		}
	}

	public class DiagonalWipeCompositor : CustomVideoCompositor
	{
		public DiagonalWipeCompositor (IntPtr handle) : base (handle)
		{
			oglRender = new DiagonalWipeRenderer ();
		}

		public DiagonalWipeCompositor(AVVideoComposition videoComposition) : base(videoComposition)
		{
			oglRender = new DiagonalWipeRenderer ();
		}
	}

	public class CustomVideoCompositor : AVVideoCompositing
	{
		bool shouldCancelAllRequests;
		bool renderContextDidChange;
		DispatchQueue renderingQueue;
		DispatchQueue renderContextQueue;
		AVVideoCompositionRenderContext renderContext;
		//CVPixelBuffer previousBuffer;
		public OpenGLRenderer oglRender;

		public CustomVideoCompositor (IntPtr handle) : base (handle)
		{
			renderingQueue = new DispatchQueue ("com.apple.aplcustomvideocompositor.renderingqueue");
			renderContextQueue = new DispatchQueue ("com.apple.aplcustomvideocompositor.rendercontextqueue");
			renderContextDidChange = false;
		}

		public CustomVideoCompositor (AVVideoComposition videoComposition)
		{
			renderingQueue = new DispatchQueue ("com.apple.aplcustomvideocompositor.renderingqueue");
			renderContextQueue = new DispatchQueue ("com.apple.aplcustomvideocompositor.rendercontextqueue");
			renderContextDidChange = false;
		}

		public override NSDictionary SourcePixelBufferAttributes ()
		{
			return new NSDictionary (CVPixelBuffer.PixelFormatTypeKey, CVPixelFormatType.CV420YpCbCr8BiPlanarVideoRange,
				CVPixelBuffer.OpenGLESCompatibilityKey, true);
		}

		public override NSDictionary RequiredPixelBufferAttributesForRenderContext ()
		{
			return new NSDictionary (CVPixelBuffer.PixelFormatTypeKey, CVPixelFormatType.CV420YpCbCr8BiPlanarVideoRange,
				CVPixelBuffer.OpenGLESCompatibilityKey, true);
		}

		public override void RenderContextChanged (AVVideoCompositionRenderContext newRenderContext)
		{
			renderContextQueue.DispatchSync (() => {
				renderContext = newRenderContext;
				renderContextDidChange = true;
			});
		}

		public override void StartVideoCompositionRequest (AVAsynchronousVideoCompositionRequest asyncVideoCompositionRequest)
		{
			renderingQueue.DispatchAsync (() => {
				if(shouldCancelAllRequests)
					asyncVideoCompositionRequest.FinishCancelledRequest();
				else
				{
					NSError error;
					CVPixelBuffer resultPixels = newRenderedPixelBufferForRequest( asyncVideoCompositionRequest, out error);
					if(resultPixels != null){
						asyncVideoCompositionRequest.FinishWithComposedVideoFrame(resultPixels);
						resultPixels.Dispose();
					}
					else{
						asyncVideoCompositionRequest.FinishWithError(error);
					}

				}
			});
		}

		public override void CancelAllPendingVideoCompositionRequests ()
		{
			shouldCancelAllRequests = true;
			renderingQueue.DispatchAsync (() => {
				shouldCancelAllRequests = false;
			});
		}

		//Utilities methods

		static double FactorForTimeInRange( CMTime time, CMTimeRange range)
		{
			CMTime elapsed = CMTime.Subtract (time, range.Start);
			return elapsed.Seconds / range.Duration.Seconds;
		}

		CVPixelBuffer newRenderedPixelBufferForRequest (AVAsynchronousVideoCompositionRequest request, out NSError error )
		{
			CVPixelBuffer dstPixels;
			float tweenFactor =(float) FactorForTimeInRange (request.CompositionTime, request.VideoCompositionInstruction.TimeRange);

			var currentInstruction = (CustomVideoCompositionInstruction)request.VideoCompositionInstruction;

			CVPixelBuffer foregroundSourceBuffer = request.SourceFrameByTrackID (currentInstruction.ForegroundTrackID);
			CVPixelBuffer backgroundSourceBuffer = request.SourceFrameByTrackID (currentInstruction.BackgroundTrackID);

			dstPixels = renderContext.CreatePixelBuffer ();

			if (renderContextDidChange) {
				var renderSize = renderContext.Size;
				var destinationSize = new CGSize (dstPixels.Width, dstPixels.Height);
				var renderContextTransform = new CGAffineTransform (renderSize.Width / 2, 0, 0, renderSize.Height / 2, renderSize.Width / 2, renderSize.Height / 2);
				var destinationTransform = new CGAffineTransform (2 / destinationSize.Width, 0, 0, 2 / destinationSize.Height, -1, -1);
				var normalizedRenderTransform = CGAffineTransform.Multiply( CGAffineTransform.Multiply(renderContextTransform, renderContext.RenderTransform), destinationTransform);
				oglRender.RenderTransform = normalizedRenderTransform;

				renderContextDidChange = false;
			}

			oglRender.RenderPixelBuffer (dstPixels, foregroundSourceBuffer, backgroundSourceBuffer, tweenFactor);

			error = null;
			return dstPixels;
		}
	}
}

