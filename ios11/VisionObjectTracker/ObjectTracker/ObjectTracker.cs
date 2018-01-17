using System;
using System.Linq;
using CoreGraphics;
using CoreVideo;
using Foundation;
using UIKit;
using Vision;

namespace ObjectTracker
{
	/// <summary>
	/// Handles tracking an object over a sequence of frames (i.e., video-based tracking)
	/// </summary>
	internal class ObjectTracker : NSObject, IRectangleViewer
	{
		/// <summary>
		/// Drawing layer.
		/// </summary>
		Overlay overlay;

		/// <summary>
		/// Maintains a sequence (video) request connection to the Vision subsystem
		/// </summary>
		VNSequenceRequestHandler trackingHandler;

		/// <summary>
		/// The tracked object.
		/// </summary>
		VNDetectedObjectObservation trackedRectangle;

		internal ObjectTracker(Overlay overlay)
		{
			this.overlay = overlay;
			trackingHandler = new VNSequenceRequestHandler();
		}

		/// <summary>
		/// Track the specified observation. 
		/// </summary>
		internal void Track(VNDetectedObjectObservation observation) => trackedRectangle = observation;

		/// <summary>
		/// Called by `ViewController.OnFrameCaptured` once per frame with the buffer processed by the image-processing pipeline in 
		/// `VideoCaptureDelegate.DidOutputSampleBuffer`
		/// </summary>
		/// <param name="buffer">The captured video frame.</param>
		public void OnFrameCaptured(CVPixelBuffer buffer)
		{
			// Run the tracker
			var request = new VNTrackObjectRequest(trackedRectangle, ObjectTracked);
			request.TrackingLevel = VNRequestTrackingLevel.Accurate;
			NSError error;
			var requests = new[] { request };
			overlay.InvokeOnMainThread(() => overlay.Clear());
			trackingHandler.Perform(requests, buffer, out error);
			if (error != null)
			{
				InvokeOnMainThread(() => overlay.Message = error.ToString());
			}
		}

		/// <summary>
		/// Draws a quadrilateral on the overlay
		/// </summary>
		/// <param name="normalizedQuadrilateral">Normalized quadrilateral.</param>
		private void ObjectTracked(CGPoint[] normalizedQuadrilateral)
		{
			overlay.InvokeOnMainThread(() =>
			{
				var rotatedQuadrilateral = normalizedQuadrilateral.Select(pt => new CGPoint(pt.X, 1.0 - pt.Y)).ToArray();
				overlay.AddQuad(rotatedQuadrilateral);
			});
		}

		/// <summary>
		/// Asynchronously called by the Vision subsystem subsequent to `Perform` in `OnFrameCaptured` 
		/// </summary>
		/// <param name="request">The request sent to the Vision subsystem.</param>
		/// <param name="err">If not null, describes an error in Vision.</param>
		private void ObjectTracked(VNRequest request, NSError err)
		{
			if (err != null)
			{
				Console.Error.WriteLine(err);
				InvokeOnMainThread(() => overlay.Message = err.ToString());
				return;
			}

			InvokeOnMainThread(() =>
			{
				overlay.Clear();
				overlay.StrokeColor = UIColor.Green.CGColor;
				var observations = request.GetResults<VNDetectedObjectObservation>();
				var o = observations.FirstOrDefault();
				if (o != null)
				{
					// o is a succesfully tracked object, so draw it on the `overlay`
					overlay.Message = "Locked";
					if (o.Confidence < 0.5)
					{
						overlay.StrokeColor = UIColor.Red.CGColor;
					}
					if (o.Confidence < 0.8)
					{
						overlay.StrokeColor = UIColor.Yellow.CGColor;
					}

					var quad = new[] {
						new CGPoint(o.BoundingBox.Left, o.BoundingBox.Top),
						new CGPoint(o.BoundingBox.Right, o.BoundingBox.Top),
						new CGPoint(o.BoundingBox.Right, o.BoundingBox.Bottom),
						new CGPoint(o.BoundingBox.Left, o.BoundingBox.Bottom)
					};
					ObjectTracked(quad);
					trackedRectangle = o;
				}
			});
		}
	}
}