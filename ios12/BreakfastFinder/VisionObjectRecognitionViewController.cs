using Foundation;
using System;
using UIKit;
using CoreAnimation;
using Vision;
using CoreFoundation;
using System.Linq;
using AVFoundation;
using CoreMedia;
using CoreVideo;
using CoreGraphics;
using CoreText;

namespace BreakfastFinder
{
    public partial class VisionObjectRecognitionViewController : ViewController
    {
        public VisionObjectRecognitionViewController (IntPtr handle) : base (handle) { }
        public override UIView previewView { get => PreviewView; }
        NSDictionary options = new NSDictionary();

        CALayer detectionOverlay = null;

        // Vision parts
        VNRequest[] requests;

        NSError SetupVision()
        {
            // Setup Vision parts
            var visionModel = VNCoreMLModel.FromMLModel(
                new ObjectDetector().model, 
                out NSError error
            );

            if (error != null)
            {
                Console.WriteLine($"Model loading went wrong: {error.LocalizedDescription}");
                return error;
            }

            var objectRecognition = new VNCoreMLRequest(visionModel, (VNRequest request, NSError requestError) =>
            {
                if (requestError == null)
                {
                    DispatchQueue.MainQueue.DispatchAsync(() =>
                    {
                        var results = request.GetResults<VNObservation>();
                        if (results != null)
                        {
                            this.DrawVisionRequestResults(results);
                        }
                    });
                }
                else
                {
                    Console.WriteLine($"{error.LocalizedDescription}");
                }
            });

            this.requests = new VNRequest[] { objectRecognition };

            return error;
        }

        void DrawVisionRequestResults(VNObservation[] results)
        {
            CATransaction.Begin();
            CATransaction.SetValueForKey(NSNumber.FromBoolean(true), CATransaction.DisableActionsKey);
            detectionOverlay.Sublayers = null; // remove all the old recognized objects
            results.OfType<VNRecognizedObjectObservation>()
                   .ToList()
                   .ForEach((VNRecognizedObjectObservation objectObservation) =>
                    {
                        // Select only the label with the highest confidence
                        VNClassificationObservation topLabelObservation = objectObservation.Labels[0];
                        CGRect objectBounds = VNUtils.GetImageRect(
                            objectObservation.BoundingBox,
                            (nuint)bufferSize.Width,
                            (nuint)bufferSize.Height
                        );


                        CALayer shapeLayer = this.CreateRoundedRectLayerWithBounds(objectBounds);
                        CALayer textLayer = this.CreateTextSubLayerInBounds(
                            objectBounds,
                            topLabelObservation.Identifier,
                            topLabelObservation.Confidence
                        );

                        shapeLayer.AddSublayer(textLayer);
                        detectionOverlay.AddSublayer(shapeLayer);
                    });
            this.UpdateLayerGeometry();
            CATransaction.Commit();
        }

        [Export("captureOutput:didOutputSampleBuffer:fromConnection:")]
        public virtual void DidOutputSampleBuffer(AVCaptureOutput captureOutput, CMSampleBuffer sampleBuffer, AVCaptureConnection connection)
        {
            CVPixelBuffer pixelBuffer = null;
            VNImageRequestHandler imageRequestHandler = null;

            try
            {
                pixelBuffer = sampleBuffer.GetImageBuffer() as CVPixelBuffer;
                if (pixelBuffer == null)
                {
                    return;
                }

                var exifOrientation = ExifOrientationFromDeviceOrientation();

                imageRequestHandler = new VNImageRequestHandler(pixelBuffer, exifOrientation, options);
                imageRequestHandler.Perform(this.requests, out NSError error);
                if (error != null)
                {
                    Console.WriteLine($"{error.LocalizedDescription}");
                }
            }
            catch (Exception x)
            {
                Console.WriteLine(x.Message);
            }
            finally
            {
                if (sampleBuffer != null)
                {
                    sampleBuffer.Dispose();
                }

                if (pixelBuffer != null)
                {
                    pixelBuffer.Dispose();
                }

                if (imageRequestHandler != null)
                {
                    imageRequestHandler.Dispose();
                }
            }
        }

        override protected void SetupAVCapture()
        {
            base.SetupAVCapture();

            // setup Vision parts
            SetupLayers();
            UpdateLayerGeometry();
            SetupVision();

            // start the capture
            StartCaptureSession();
        }

        void SetupLayers()
        {
            detectionOverlay = new CALayer(); // container layer that has all the renderings of the observations
            detectionOverlay.Name = "DetectionOverlay";
            detectionOverlay.Bounds = new CGRect(0.0, 0.0, bufferSize.Width, bufferSize.Height);
            detectionOverlay.Position = new CGPoint(rootLayer.Bounds.GetMidX(), rootLayer.Bounds.GetMidY());
            rootLayer.AddSublayer(detectionOverlay);
        }

        void UpdateLayerGeometry()
        {
            //    let bounds = rootLayer.bounds
            var bounds = rootLayer.Bounds;
            nfloat scale;

            nfloat xScale = bounds.Size.Width / bufferSize.Height;
            nfloat yScale = bounds.Size.Height / bufferSize.Width;
            scale = NMath.Max(xScale, yScale);

            if (nfloat.IsInfinity(scale))
            {
                scale = 1.0f;
            }
            CATransaction.Begin();
            CATransaction.SetValueForKey(NSNumber.FromBoolean(true), CATransaction.DisableActionsKey);

            // rotate the layer into screen orientation and scale and mirror
            var affineTransform = CGAffineTransform.MakeScale(scale, -1 * scale);
            affineTransform.Rotate((nfloat)Math.PI / 2.0f);
            detectionOverlay.AffineTransform = affineTransform;

            // center the layer
            detectionOverlay.Position = new CGPoint(bounds.GetMidX(), bounds.GetMidY());

            CATransaction.Commit();
        }

        CATextLayer CreateTextSubLayerInBounds(CGRect bounds, string Identifier, float confidence)
        {
            var textLayer = new CATextLayer();
            textLayer.Name = "Object Label";
            var formattedString = new NSMutableAttributedString($"{Identifier}\nConfidence:  {confidence:F2}");
            var largeFont = UIFont.FromName("Helvetica", 24.0f);
            formattedString.AddAttributes(
                new UIStringAttributes() { Font = largeFont },
                new NSRange(0, Identifier.Length)
            );
            textLayer.AttributedString = formattedString;
            textLayer.Bounds = new CGRect(0, 0, bounds.Size.Height - 10, bounds.Size.Width - 10);
            textLayer.Position = new CGPoint(bounds.GetMidX(), bounds.GetMidY());
            textLayer.ShadowOpacity = 0.7f;
            textLayer.ShadowOffset = new CGSize(2, 2);
            textLayer.ForegroundColor = new CGColor(
                CGColorSpace.CreateDeviceRGB(), 
                new nfloat[] { 0.0f, 0.0f, 0.0f, 1.0f }
            );
            textLayer.ContentsScale = 2.0f; // retina rendering
            // rotate the layer into screen orientation and scale and mirror
            var affineTransform = CGAffineTransform.MakeScale(1.0f, -1.0f);
            affineTransform.Rotate((nfloat)Math.PI / 2.0f);
            textLayer.AffineTransform = affineTransform;
            return textLayer;
        }

        CALayer CreateRoundedRectLayerWithBounds(CGRect bounds)
        {
            var shapeLayer = new CALayer();
            shapeLayer.Bounds = bounds;
            shapeLayer.Position = new CGPoint(bounds.GetMidX(), bounds.GetMidY());
            shapeLayer.Name = "Found Object";
            shapeLayer.BackgroundColor = new CGColor(
                CGColorSpace.CreateDeviceRGB(),
                new nfloat[] { 1.0f, 1.0f, 0.2f, 0.4f }
            );
            shapeLayer.CornerRadius = 7;
            return shapeLayer;
        }
    }
}