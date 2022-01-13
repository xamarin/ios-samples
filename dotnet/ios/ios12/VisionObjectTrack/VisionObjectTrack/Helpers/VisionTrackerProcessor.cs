namespace VisionObjectTrack;

public interface IVisionTrackerProcessorDelegate
{
        void DisplayFrame (CVPixelBuffer frame, CGAffineTransform transform, IList<TrackedPolyRect>? rects);
        void DisplayFrameCounter (int frame);
        void DidFinifshTracking ();
}

/// <summary>
/// Contains the tracker processing logic using Vision.
/// </summary>
public class VisionTrackerProcessor
{
        List<VNRectangleObservation> initialRectObservations = new List<VNRectangleObservation> ();

        readonly AVAsset videoAsset;

        bool cancelRequested;

        public VisionTrackerProcessor (AVAsset videoAsset)
        {
                this.videoAsset = videoAsset;
        }

        public IVisionTrackerProcessorDelegate? Delegate { get; set; }

        public VNRequestTrackingLevel TrackingLevel { get; set; } = VNRequestTrackingLevel.Accurate;

        public List<TrackedPolyRect> ObjectsToTrack { get; set; } = new List<TrackedPolyRect> ();

        /// <summary>
        /// Set Initial Condition
        /// </summary>
        public void ReadAndDisplayFirstFrame (bool performRectanglesDetection, out NSError? error)
        {
                var videoReader = VideoReader.Create (videoAsset);
                if (videoReader is null)
		{
                        error = new VisionTrackerProcessorError (VisionTrackerProcessorErrorType.FirstFrameReadFailed);
                        return;
                }
                var firstFrame = videoReader.NextFrame ();
                if (firstFrame is null)
		{
                        error = new VisionTrackerProcessorError (VisionTrackerProcessorErrorType.FirstFrameReadFailed);
                        return;
                }

                List<TrackedPolyRect>? firstFrameRects = null;
                if (performRectanglesDetection)
                {
                        // Vision Rectangle Detection
                        var imageRequestHandler = new VNImageRequestHandler (firstFrame, videoReader.Orientation, new NSMutableDictionary ());

                        var rectangleDetectionRequest = new VNDetectRectanglesRequest (null)
                        {
                                MinimumAspectRatio = 0.2f,
                                MaximumAspectRatio = 1f,
                                MinimumSize = 0.1f,
                                MaximumObservations = 10
                        };

                        imageRequestHandler.Perform (new VNRequest [] { rectangleDetectionRequest }, out NSError performError);
                        if (performError is not null)
                        {
                                error = new VisionTrackerProcessorError (VisionTrackerProcessorErrorType.FirstFrameReadFailed);
                                return;
                        }

                        var rectObservations = rectangleDetectionRequest.GetResults<VNRectangleObservation> ();
                        if (rectObservations is not null && rectObservations.Any ())
                        {
                                initialRectObservations = rectObservations.ToList ();
                                var detectedRects = new List<TrackedPolyRect> ();
                                for (var index = 0; index < initialRectObservations.Count; index++)
                                {
                                        var rectangleObservation = initialRectObservations [index];
                                        var rectColor = TrackedObjectsPalette.Color (index);

                                        detectedRects.Add (new TrackedPolyRect (rectangleObservation, rectColor));
                                }

                                firstFrameRects = detectedRects;
                        }
                }

                error = null;
                Delegate?.DisplayFrame (firstFrame, videoReader.AffineTransform, firstFrameRects);
        }

        /// <summary>
        /// Perform Requests
        /// </summary>
        public void PerformTracking (TrackedObjectType type, out NSError? error)
        {
                var videoReader = VideoReader.Create (videoAsset);
                if (videoReader is not null)
                {
                        if (videoReader.NextFrame () is not null)
                        {
                                cancelRequested = false;

                                // Create initial observations
                                var inputObservations = new Dictionary<NSUuid, VNDetectedObjectObservation> ();
                                var trackedObjects = new Dictionary<NSUuid, TrackedPolyRect> ();
                                switch (type)
                                {
                                        case TrackedObjectType.Object:
                                                foreach (var rect in ObjectsToTrack)
                                                {
                                                        var inputObservation = VNDetectedObjectObservation.FromBoundingBox (rect.BoundingBox);
                                                        inputObservations [inputObservation.Uuid] = inputObservation;
                                                        trackedObjects [inputObservation.Uuid] = rect;
                                                }
                                                break;

                                        case TrackedObjectType.Rectangle:
                                                foreach (var rectangleObservation in initialRectObservations)
                                                {
                                                        inputObservations [rectangleObservation.Uuid] = rectangleObservation;
                                                        var rectColor = TrackedObjectsPalette.Color (trackedObjects.Count);
                                                        trackedObjects [rectangleObservation.Uuid] = new TrackedPolyRect (rectangleObservation, rectColor);
                                                }
                                                break;
                                }

                                var requestHandler = new VNSequenceRequestHandler ();
                                var frames = 1;
                                var trackingFailedForAtLeastOneObject = false;

                                CVPixelBuffer? frame = null;
                                while (true)
                                {
                                        if (cancelRequested || (frame = videoReader.NextFrame ()) is null)
                                        {
                                                break;
                                        }

                                        Delegate?.DisplayFrameCounter (frames);
                                        frames += 1;

                                        var rects = new List<TrackedPolyRect> ();
                                        var trackingRequests = new List<VNRequest> ();
                                        foreach (var inputObservation in inputObservations)
                                        {
                                                VNTrackingRequest? request = null;
                                                switch (type)
                                                {
                                                        case TrackedObjectType.Object:
                                                                request = new VNTrackObjectRequest (inputObservation.Value);
                                                                break;

                                                        case TrackedObjectType.Rectangle:
                                                                if (inputObservation.Value is VNRectangleObservation rectObservation)
                                                                {
                                                                        request = new VNTrackRectangleRequest (rectObservation);
                                                                }
                                                                else
                                                                {
                                                                        continue;
                                                                }
                                                                break;
                                                }

                                                if (request is not null)
                                                {
                                                        request.TrackingLevel = TrackingLevel;
                                                        trackingRequests.Add (request);
                                                }
                                        }

                                        // Perform array of requests
                                        requestHandler.Perform (trackingRequests.ToArray (), frame, videoReader.Orientation, out NSError performError);
                                        trackingFailedForAtLeastOneObject = performError is not null;

                                        foreach (var processedRequest in trackingRequests)
                                        {
                                                var results = processedRequest.GetResults<VNObservation> ();
                                                if (results is null || !results.Any ())
                                                {
                                                        continue;
                                                }

                                                if (results.FirstOrDefault () is VNDetectedObjectObservation observation)
                                                {
                                                        // Assume threshold = 0.5f
                                                        var rectStyle = observation.Confidence > 0.5f ? TrackedPolyRectStyle.Solid : TrackedPolyRectStyle.Dashed;
                                                        var knownRect = trackedObjects [observation.Uuid];

                                                        switch (type)
                                                        {
                                                                case TrackedObjectType.Object:
                                                                        rects.Add (new TrackedPolyRect (observation, knownRect.Color, rectStyle));
                                                                        break;

                                                                case TrackedObjectType.Rectangle:
                                                                        if (observation is VNRectangleObservation rectObservation)
                                                                        {
                                                                                rects.Add (new TrackedPolyRect (rectObservation, knownRect.Color, rectStyle));
                                                                        }
                                                                        break;
                                                        }

                                                        // Initialize inputObservation for the next iteration
                                                        inputObservations [observation.Uuid] = observation;
                                                }
                                        }

                                        // Draw results
                                        Delegate?.DisplayFrame (frame, videoReader.AffineTransform, rects);

                                        var miliseconds = videoReader.FrameRateInSeconds / 1000;
                                        System.Threading.Thread.Sleep ((int)miliseconds);
                                }

                                Delegate?.DidFinifshTracking ();

                                error = trackingFailedForAtLeastOneObject ? new VisionTrackerProcessorError (VisionTrackerProcessorErrorType.FirstFrameReadFailed)
                                                                          : null;
                        }
                        else
                        {
                                error = new VisionTrackerProcessorError (VisionTrackerProcessorErrorType.FirstFrameReadFailed);
                        }
                }
                else
                {
                        error = new VisionTrackerProcessorError (VisionTrackerProcessorErrorType.ReaderInitializationFailed);
                }
        }

        public void CancelTracking ()
        {
                cancelRequested = true;
        }
}
