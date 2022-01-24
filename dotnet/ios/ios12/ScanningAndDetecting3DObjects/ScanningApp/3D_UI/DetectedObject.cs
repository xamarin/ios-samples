namespace ScanningAndDetecting3DObjects;

internal class DetectedObject : SCNNode
{
	// How long, in seconds, this visualization is displayed after an update

	NSTimer? detectedObjectVisualizationTimer;

	DetectedPointCloud pointCloudVisualization;

	DetectedBoundingBox? boundingBox;

	SCNNode originVis;
	SCNNode? customModel;

	ARReferenceObject referenceObject;

	internal DetectedObject (ARReferenceObject referenceObject) : base ()
	{
		this.referenceObject = referenceObject;

		pointCloudVisualization = new DetectedPointCloud (referenceObject.RawFeaturePoints, referenceObject.Center, referenceObject.Extent);
		var scene = SCNScene.FromFile ("axes.scn", "art.scnassets", new NSDictionary ());
		if (scene is not null)
		{
			originVis = new SCNNode ();
			foreach (var child in scene.RootNode.ChildNodes)
			{
				originVis.AddChildNode (child);
			}
		}
		else
		{
			originVis = new SCNNode ();
			Console.WriteLine ("Error: Coordinate system visualization missing.");
		}

		AddChildNode (pointCloudVisualization);
		Hidden = true;

		Set3DModel (ViewController.Instance?.ModelUrl);
	}

	internal void Set3DModel (NSUrl? modelUrl)
	{
		var url = modelUrl;
		SCNNode? model = null;
		if (url is not null)
		{
			model = Utilities.Load3DModel (url);
		}
		if (url is not null && model is not null)
		{
			customModel?.RemoveFromParentNode ();
			customModel?.Dispose ();
			customModel = null;
			originVis.RemoveFromParentNode ();
			ViewController.Instance?.SceneView?.Prepare (model, () =>
			{
				AddChildNode (model);
				return true;
			});
			customModel = model;
			pointCloudVisualization.Hidden = true;
			if (boundingBox is not null)
			{
				boundingBox.Hidden = true;
			}
		}
		else
		{
			customModel?.RemoveFromParentNode ();
			customModel?.Dispose ();
			customModel = null;
			AddChildNode (originVis);
			pointCloudVisualization.Hidden = false;
			if (boundingBox is not null)
			{
				boundingBox.Hidden = false;
			}
		}
	}

	internal TimeSpan DisplayDuration { get; set; } = new TimeSpan ((long)(1.5 * TimeSpan.TicksPerSecond));

	internal void UpdateVisualization (NMatrix4 transform, ARPointCloud currentPointCloud)
	{
		// Update the transform
		Transform = transform.ToSCNMatrix4 ();

		// Update the point cloud visualization
		UpdatePointCloud (currentPointCloud);

		if (boundingBox is null)
		{
			var scale = referenceObject.Scale.X;
			var newBoundingBox = new DetectedBoundingBox (referenceObject.RawFeaturePoints.Points, scale);
			newBoundingBox.Hidden = customModel is not null;
			AddChildNode (newBoundingBox);
			boundingBox = newBoundingBox;
		}

		// This visualization should only display for displayDuration seconds on every update
		detectedObjectVisualizationTimer?.Invalidate ();
		Hidden = false;
		detectedObjectVisualizationTimer = NSTimer.CreateScheduledTimer (DisplayDuration, (_) => Hidden = true);
	}

	internal void UpdatePointCloud (ARPointCloud currentPointCloud)
	{
		pointCloudVisualization.UpdateVisualization (currentPointCloud);
	}
}
