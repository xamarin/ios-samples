using AVFoundation;
using CoreFoundation;
using CoreGraphics;
using CoreMedia;
using CoreVideo;
using Foundation;
using System;

namespace AVCustomEdit
{
    public class CrossDissolveCompositor : CustomVideoCompositor
    {
        public CrossDissolveCompositor(IntPtr handle) : base(handle)
        {
            Render = new CrossDissolveRenderer();
        }

        public CrossDissolveCompositor(AVVideoComposition videoComposition) : base(videoComposition)
        {
            Render = new CrossDissolveRenderer();
        }
    }

    public class DiagonalWipeCompositor : CustomVideoCompositor
    {
        public DiagonalWipeCompositor(IntPtr handle) : base(handle)
        {
            Render = new DiagonalWipeRenderer();
        }

        public DiagonalWipeCompositor(AVVideoComposition videoComposition) : base(videoComposition)
        {
            Render = new DiagonalWipeRenderer();
        }
    }

    public class CustomVideoCompositor : AVVideoCompositing
    {
        private bool shouldCancelAllRequests;
        private bool renderContextDidChange;
        private DispatchQueue renderingQueue;
        private DispatchQueue renderContextQueue;
        private AVVideoCompositionRenderContext renderContext;
        //private CVPixelBuffer previousBuffer;

        public CustomVideoCompositor(IntPtr handle) : base(handle)
        {
            renderingQueue = new DispatchQueue("com.apple.aplcustomvideocompositor.renderingqueue");
            renderContextQueue = new DispatchQueue("com.apple.aplcustomvideocompositor.rendercontextqueue");
            //previousBuffer = null;
            renderContextDidChange = false;
        }

        public CustomVideoCompositor(AVVideoComposition videoComposition)
        {
            renderingQueue = new DispatchQueue("com.apple.aplcustomvideocompositor.renderingqueue");
            renderContextQueue = new DispatchQueue("com.apple.aplcustomvideocompositor.rendercontextqueue");
            //previousBuffer = null;
            renderContextDidChange = false;
        }

        public OpenGLRenderer Render { get; set; }

        public override NSDictionary SourcePixelBufferAttributes()
        {
            return new NSDictionary(CVPixelBuffer.PixelFormatTypeKey, CVPixelFormatType.CV420YpCbCr8BiPlanarVideoRange,
                                    CVPixelBuffer.OpenGLESCompatibilityKey, true);
        }

        public override NSDictionary RequiredPixelBufferAttributesForRenderContext()
        {
            return new NSDictionary(CVPixelBuffer.PixelFormatTypeKey, CVPixelFormatType.CV420YpCbCr8BiPlanarVideoRange,
                                    CVPixelBuffer.OpenGLESCompatibilityKey, true);
        }

        public override void RenderContextChanged(AVVideoCompositionRenderContext newRenderContext)
        {
            renderContextQueue.DispatchSync(() => 
            {
                renderContext = newRenderContext;
                renderContextDidChange = true;
            });
        }

        public override void StartVideoCompositionRequest(AVAsynchronousVideoCompositionRequest request)
        {
            renderingQueue.DispatchAsync(() => 
            {
                // Check if all pending requests have been cancelled
                if (shouldCancelAllRequests)
                {
                    request.FinishCancelledRequest();
                }
                else
                {
                    // Get the next rendererd pixel buffer
                    var resultPixels = NewRenderedPixelBufferForRequest(request, out NSError error);
                    if (resultPixels != null)
                    {
                        // The resulting pixelbuffer from OpenGL renderer is passed along to the request
                        request.FinishWithComposedVideoFrame(resultPixels);
                        resultPixels.Dispose();
                        resultPixels = null;
                    }
                    else
                    {
                        request.FinishWithError(error);
                    }
                }
            });
        }

        public override void CancelAllPendingVideoCompositionRequests()
        {
            // pending requests will call finishCancelledRequest, those already rendering will call finishWithComposedVideoFrame
            shouldCancelAllRequests = true;

            renderingQueue.DispatchAsync(() =>
            {
                // start accepting requests again
                shouldCancelAllRequests = false;
            });
        }

        #region Utilities methods

        private static double FactorForTimeInRange(CMTime time, CMTimeRange range)
        {
            var elapsed = CMTime.Subtract(time, range.Start);
            return elapsed.Seconds / range.Duration.Seconds;
        }

        private CVPixelBuffer NewRenderedPixelBufferForRequest(AVAsynchronousVideoCompositionRequest request, out NSError error)
        {
            CVPixelBuffer dstPixels;

            // tweenFactor indicates how far within that timeRange are we rendering this frame. This is normalized to vary between 0.0 and 1.0.
            // 0.0 indicates the time at first frame in that videoComposition timeRange
            // 1.0 indicates the time at last frame in that videoComposition timeRange
            var tweenFactor = (float)FactorForTimeInRange(request.CompositionTime, request.VideoCompositionInstruction.TimeRange);

            var currentInstruction = request.VideoCompositionInstruction as CustomVideoCompositionInstruction;

            // Source pixel buffers are used as inputs while rendering the transition
            var foregroundSourceBuffer = request.SourceFrameByTrackID(currentInstruction.ForegroundTrackId);
            var backgroundSourceBuffer = request.SourceFrameByTrackID(currentInstruction.BackgroundTrackId);

            // Destination pixel buffer into which we render the output
            dstPixels = renderContext.CreatePixelBuffer();

            // Recompute normalized render transform everytime the render context changes
            if (renderContextDidChange)
            {
                // The renderTransform returned by the renderContext is in X: [0, w] and Y: [0, h] coordinate system
                // But since in this sample we render using OpenGLES which has its coordinate system between [-1, 1] we compute a normalized transform
                var renderSize = renderContext.Size;
                var destinationSize = new CGSize(dstPixels.Width, dstPixels.Height);
                var renderContextTransform = new CGAffineTransform(renderSize.Width / 2, 0, 0, renderSize.Height / 2, renderSize.Width / 2, renderSize.Height / 2);
                var destinationTransform = new CGAffineTransform(2 / destinationSize.Width, 0, 0, 2 / destinationSize.Height, -1, -1); 
                var normalizedRenderTransform = CGAffineTransform.Multiply(CGAffineTransform.Multiply(renderContextTransform, renderContext.RenderTransform), destinationTransform);
                Render.RenderTransform = normalizedRenderTransform;

                renderContextDidChange = false;
            }

            Render.RenderPixelBuffer(dstPixels, foregroundSourceBuffer, backgroundSourceBuffer, tweenFactor);

            error = null;
            return dstPixels;
        }
    }

    #endregion
}