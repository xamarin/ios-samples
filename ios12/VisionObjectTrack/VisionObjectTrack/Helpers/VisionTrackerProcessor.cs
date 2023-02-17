
namespace VisionObjectTrack {
	using AVFoundation;
	using CoreGraphics;
	using CoreVideo;
	using Foundation;
	using System.Collections.Generic;
	using System.Linq;
	using Vision;
	using VisionObjectTrack.Enums;

	public interface IVisionTrackerProcessorDelegate {
		void DisplayFrame (CVPixelBuffer frame, CGAffineTransform transform, IList<TrackedPolyRect> rects);
		void DisplayFrameCounter (int frame);
		void DidFinifshTracking ();
	}

	/// <summary>
	/// Contains the tracker processing logic using Vision.
	/// </summary>
	public class VisionTrackerProcessor {
		private List<VNRectangleObservation> initialRectObservations = new List<VNRectangleObservation> ();

		private readonly AVAsset videoAsset;

		private bool cancelRequested;

		public VisionTrackerProcessor (AVAsset videoAsset)
		{
			this.videoAsset = videoAsset;
		}

		public IVisionTrackerProcessorDelegate Delegate { get; set; }

		public VNRequestTrackingLevel TrackingLevel { get; set; } = VNRequestTrackingLevel.Accurate;

		public List<TrackedPolyRect> ObjectsToTrack { get; set; } = new List<TrackedPolyRect> ();

		/// <summary>
		/// Set Initial Condition
		/// </summary>
		public void ReadAndDisplayFirstFrame (bool performRectanglesDetection, out NSError error)
		{
			var videoReader = VideoReader.Create (this.videoAsset);
			if (videoReader != null) {
				var firstFrame = videoReader.NextFrame ();
				if (firstFrame != null) {
					List<TrackedPolyRect> firstFrameRects = null;
					if (performRectanglesDetection) {
						// Vision Rectangle Detection
						var imageRequestHandler = new VNImageRequestHandler (firstFrame, videoReader.Orientation, new NSMutableDictionary ());

						var rectangleDetectionRequest = new VNDetectRectanglesRequest (null) {
							MinimumAspectRatio = 0.2f,
							MaximumAspectRatio = 1f,
							MinimumSize = 0.1f,
							MaximumObservations = 10
						};

						imageRequestHandler.Perform (new VNRequest [] { rectangleDetectionRequest }, out NSError performError);
						if (performError != null) {
							error = new VisionTrackerProcessorError (VisionTrackerProcessorErrorType.FirstFrameReadFailed);
							return;
						}

						var rectObservations = rectangleDetectionRequest.GetResults<VNRectangleObservation> ();
						if (rectObservations != null && rectObservations.Any ()) {
							this.initialRectObservations = rectObservations.ToList ();
							var detectedRects = new List<TrackedPolyRect> ();
							for (var index = 0; index < this.initialRectObservations.Count; index++) {
								var rectangleObservation = this.initialRectObservations [index];
								var rectColor = TrackedObjectsPalette.Color (index);

								detectedRects.Add (new TrackedPolyRect (rectangleObservation, rectColor));
							}

							firstFrameRects = detectedRects;
						}
					}

					error = null;
					this.Delegate?.DisplayFrame (firstFrame, videoReader.AffineTransform, firstFrameRects);
				} else {
					error = new VisionTrackerProcessorError (VisionTrackerProcessorErrorType.FirstFrameReadFailed);
				}
			} else {
				error = new VisionTrackerProcessorError (VisionTrackerProcessorErrorType.ReaderInitializationFailed);
			}
		}

		/// <summary>
		/// Perform Requests
		/// </summary>
		public void PerformTracking (TrackedObjectType type, out NSError error)
		{
			var videoReader = VideoReader.Create (videoAsset);
			if (videoReader != null) {
				if (videoReader.NextFrame () != null) {
					this.cancelRequested = false;

					// Create initial observations
					var inputObservations = new Dictionary<NSUuid, VNDetectedObjectObservation> ();
					var trackedObjects = new Dictionary<NSUuid, TrackedPolyRect> ();
					switch (type) {
					case TrackedObjectType.Object:
						foreach (var rect in this.ObjectsToTrack) {
							var inputObservation = VNDetectedObjectObservation.FromBoundingBox (rect.BoundingBox);
							inputObservations [inputObservation.Uuid] = inputObservation;
							trackedObjects [inputObservation.Uuid] = rect;
						}
						break;

					case TrackedObjectType.Rectangle:
						foreach (var rectangleObservation in this.initialRectObservations) {
							inputObservations [rectangleObservation.Uuid] = rectangleObservation;
							var rectColor = TrackedObjectsPalette.Color (trackedObjects.Count);
							trackedObjects [rectangleObservation.Uuid] = new TrackedPolyRect (rectangleObservation, rectColor);
						}
						break;
					}

					var requestHandler = new VNSequenceRequestHandler ();
					var frames = 1;
					var trackingFailedForAtLeastOneObject = false;

					CVPixelBuffer frame = null;
					while (true) {
						if (this.cancelRequested || (frame = videoReader.NextFrame ()) == null) {
							break;
						}

						this.Delegate?.DisplayFrameCounter (frames);
						frames += 1;

						var rects = new List<TrackedPolyRect> ();
						var trackingRequests = new List<VNRequest> ();
						foreach (var inputObservation in inputObservations) {
							VNTrackingRequest request = null;
							switch (type) {
							case TrackedObjectType.Object:
								request = new VNTrackObjectRequest (inputObservation.Value);
								break;

							case TrackedObjectType.Rectangle:
								if (inputObservation.Value is VNRectangleObservation rectObservation) {
									request = new VNTrackRectangleRequest (rectObservation);
								} else {
									continue;
								}
								break;
							}

							request.TrackingLevel = this.TrackingLevel;
							trackingRequests.Add (request);
						}

						// Perform array of requests
						requestHandler.Perform (trackingRequests.ToArray (), frame, videoReader.Orientation, out NSError performError);
						trackingFailedForAtLeastOneObject = performError != null;

						foreach (var processedRequest in trackingRequests) {
							var results = processedRequest.GetResults<VNObservation> ();
							if (results == null || !results.Any ()) {
								continue;
							}

							if (results.FirstOrDefault () is VNDetectedObjectObservation observation) {
								// Assume threshold = 0.5f
								var rectStyle = observation.Confidence > 0.5f ? TrackedPolyRectStyle.Solid : TrackedPolyRectStyle.Dashed;
								var knownRect = trackedObjects [observation.Uuid];

								switch (type) {
								case TrackedObjectType.Object:
									rects.Add (new TrackedPolyRect (observation, knownRect.Color, rectStyle));
									break;

								case TrackedObjectType.Rectangle:
									if (observation is VNRectangleObservation rectObservation) {
										rects.Add (new TrackedPolyRect (rectObservation, knownRect.Color, rectStyle));
									}
									break;
								}

								// Initialize inputObservation for the next iteration
								inputObservations [observation.Uuid] = observation;
							}
						}

						// Draw results
						this.Delegate?.DisplayFrame (frame, videoReader.AffineTransform, rects);

						var miliseconds = videoReader.FrameRateInSeconds / 1000;
						System.Threading.Thread.Sleep ((int) miliseconds);
					}

					this.Delegate?.DidFinifshTracking ();

					error = trackingFailedForAtLeastOneObject ? new VisionTrackerProcessorError (VisionTrackerProcessorErrorType.FirstFrameReadFailed)
															  : null;
				} else {
					error = new VisionTrackerProcessorError (VisionTrackerProcessorErrorType.FirstFrameReadFailed);
				}
			} else {
				error = new VisionTrackerProcessorError (VisionTrackerProcessorErrorType.ReaderInitializationFailed);
			}
		}

		public void CancelTracking ()
		{
			this.cancelRequested = true;
		}
	}
}
