using System;
using System.Linq;
using CoreFoundation;
using CoreGraphics;
using CoreVideo;
using Foundation;
using UIKit;
using Vision;

namespace ObjectTracker {
	/// <summary>
	/// Makes Vision requests in "scanning" mode -- looks for rectangles
	/// </summary>
	internal class RectangleScanner : NSObject, IRectangleViewer {
		/// <summary>
		/// Connection to the Vision subsystem
		/// </summary>
		VNDetectRectanglesRequest rectangleRequest;

		/// <summary>
		/// The set of detected rectangles
		/// </summary> 
		VNRectangleObservation [] observations;

		/// <summary>
		/// Display overlay
		/// </summary>
		Overlay overlay;

		internal RectangleScanner (Overlay overlay)
		{
			this.overlay = overlay;

			rectangleRequest = new VNDetectRectanglesRequest (RectanglesDetected);
			rectangleRequest.MaximumObservations = 10;
		}

		/// <summary>
		/// Called by `ViewController.OnFrameCaptured` once per frame with the buffer processed by the image-processing pipeline in 
		/// `VideoCaptureDelegate.DidOutputSampleBuffer`
		/// </summary>
		/// <param name="buffer">The captured video frame.</param>
		public void OnFrameCaptured (CVPixelBuffer buffer)
		{

			BeginInvokeOnMainThread (() => overlay.Message = $"Scanning...");

			// Run the rectangle detector
			var handler = new VNImageRequestHandler (buffer, new NSDictionary ());
			NSError error;
			handler.Perform (new VNRequest [] { rectangleRequest }, out error);
			if (error != null) {
				Console.Error.WriteLine (error);
				BeginInvokeOnMainThread (() => overlay.Message = error.ToString ());
			}
		}

		/// <summary>
		/// Asynchronously called by the Vision subsystem subsequent to `Perform` in `OnFrameCaptured` 
		/// </summary>
		/// <param name="request">The request sent to the Vision subsystem.</param>
		/// <param name="err">If not null, describes an error in Vision.</param>
		private void RectanglesDetected (VNRequest request, NSError err)
		{
			if (err != null) {
				overlay.Message = err.ToString ();
				Console.Error.WriteLine (err);
				return;
			}
			overlay.Clear ();

			observations = request.GetResults<VNRectangleObservation> ();
			overlay.StrokeColor = UIColor.Blue.CGColor;

			//Draw all detected rectangles in blue
			foreach (var o in observations) {
				var quad = new [] { o.TopLeft, o.TopRight, o.BottomRight, o.BottomLeft };
				RectangleDetected (quad);
			}
		}

		private void RectangleDetected (CGPoint [] normalizedQuadrilateral)
		{
			overlay.InvokeOnMainThread (() => {
				// Note conversion from inverted coordinate system!
				var rotatedQuadrilateral = normalizedQuadrilateral.Select (pt => new CGPoint (pt.X, 1.0 - pt.Y)).ToArray ();
				overlay.AddQuad (rotatedQuadrilateral);
			});
		}


		private static bool ObservationContainsPoint (VNRectangleObservation o, CGPoint normalizedPoint)
		{
			// Enhancement: This is actually wrong, since the touch could be within the bounding box but outside the quadrilateral. 
			// For better accuracy, implement the Winding Rule algorithm 
			return o.BoundingBox.Contains (normalizedPoint);
		}

		internal VNRectangleObservation Containing (CGPoint normalizedPoint) => observations.FirstOrDefault (o => ObservationContainsPoint (o, normalizedPoint));
	}
}
