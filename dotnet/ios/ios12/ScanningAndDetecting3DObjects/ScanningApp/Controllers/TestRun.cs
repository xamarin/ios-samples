namespace ScanningAndDetecting3DObjects;

// Manages the process of testing detection after scanning an object
internal class TestRun : NSObject, IDisposable
{
	ViewControllerSessionInfo sessionInfo;
	ARSCNView sceneView;
	int detections = 0;
	double lastDetectionDelayInSeconds = 0.0;
	double averageDetectionDelayInSeconds = 0.0;
	DateTime lastDetectionStartTime;

	NSTimer? noDetectionTimer;

	internal DetectedObject? DetectedObject { get; private set; }
	internal ARReferenceObject? ReferenceObject { get; private set; }
	internal UIImage? PreviewImage { get; private set; }

	internal TestRun (ViewControllerSessionInfo sessionInfo, ARSCNView sceneView)
	{
		this.sessionInfo = sessionInfo;
		this.sceneView = sceneView;

		StartNoDetectionTimer ();
	}


	internal TimeSpan ResultDisplayDuration
	{
		get
		{
			// The recommended display duration for detection results
			// is the average time it takes to detect it, plus 200ms buffer
			return new TimeSpan ((long)(TimeSpan.TicksPerSecond * (averageDetectionDelayInSeconds + 0.2)));
		}
	}

	internal string Statistics
	{
		get
		{
			var lastDelay = $"{lastDetectionDelayInSeconds * 1000:F0}";
			var avgDelay = $"{averageDetectionDelayInSeconds * 1000:F0}";
			return $"Detected after {lastDelay} ms. Avg: {avgDelay}";
		}
	}

	internal void SetReferenceObject (ARReferenceObject obj, UIImage screenshot)
	{
		ReferenceObject = obj;
		PreviewImage = screenshot;
		detections = 0;
		lastDetectionDelayInSeconds = 0;
		averageDetectionDelayInSeconds = 0;

		DetectedObject = new DetectedObject (ReferenceObject);
		sceneView.Scene.RootNode.AddChildNode (DetectedObject);

		lastDetectionStartTime = DateTime.Now;

		var config = new ARWorldTrackingConfiguration ();
		config.DetectionObjects = new NSSet<ARReferenceObject> (new [] { ReferenceObject });
		// ARSessionRunOptions.None should be coming in a future binding. In the meantime, the following cast
		sceneView.Session.Run (config, (ARSessionRunOptions)0);
	}

	internal void SuccessfulDetection (ARObjectAnchor objectAnchor)
	{
		// Compute the time it took to detect this object & the average
		lastDetectionDelayInSeconds = (DateTime.Now - lastDetectionStartTime).TotalSeconds;
		++detections;
		averageDetectionDelayInSeconds = (averageDetectionDelayInSeconds * (1.0 * detections - 1) + lastDetectionDelayInSeconds) / (1.0 * detections);

		// Update the detected object's display duration
		DetectedObject!.DisplayDuration = ResultDisplayDuration;

		// Immediately remove the anchor from the session again to force a re-detection.
		lastDetectionStartTime = DateTime.Now;
		sceneView.Session.RemoveAnchor (objectAnchor);

		if (sceneView.Session.CurrentFrame?.RawFeaturePoints is not null)
		{
			var currentPointCloud = sceneView.Session.CurrentFrame?.RawFeaturePoints;
			DetectedObject?.UpdateVisualization (objectAnchor.Transform, currentPointCloud);
		}

		StartNoDetectionTimer ();
	}

	internal void UpdateOnEveryFrame ()
	{
		if (DetectedObject is not null)
		{
			if (sceneView.Session.CurrentFrame?.RawFeaturePoints is not null)
			{
				DetectedObject.UpdatePointCloud (sceneView.Session.CurrentFrame.RawFeaturePoints);
			}
		}
	}

	void StartNoDetectionTimer ()
	{
		CancelNoDetectionTimer ();
		noDetectionTimer = NSTimer.CreateScheduledTimer (5.0, (timer) =>
		{
			CancelNoDetectionTimer ();
			sessionInfo.DisplayMessage ("Unable to detect the object. Please point the device at the scanned object or rescan.", 5.0);
		});
	}

	void CancelNoDetectionTimer ()
	{
		noDetectionTimer?.Invalidate ();
		noDetectionTimer?.Dispose ();
		noDetectionTimer = null;
	}

	internal new void Dispose ()
	{
		Dispose (true);
		GC.SuppressFinalize (this);
	}

	protected override void Dispose (bool disposing)
	{
		if (disposing)
		{
			DetectedObject?.RemoveFromParentNode ();

			if (sceneView.Session.Configuration as ARWorldTrackingConfiguration is not null)
			{
				// Make sure we switch back to an object scanning configuration & no longer
				// try to detect the object.
				var configuration = new ARObjectScanningConfiguration ();
				configuration.PlaneDetection = ARPlaneDetection.Horizontal;
				sceneView.Session.Run (configuration, ARSessionRunOptions.ResetTracking);
			}
		}
	}
}
