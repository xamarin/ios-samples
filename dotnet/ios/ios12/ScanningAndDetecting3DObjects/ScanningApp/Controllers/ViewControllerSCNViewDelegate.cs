namespace ScanningAndDetecting3DObjects;

internal class ViewControllerSCNViewDelegate : ARSCNViewDelegate
{

	ViewController source;
	ViewControllerSessionInfo sessionInfo;

	internal ViewControllerSCNViewDelegate (ViewController source, ViewControllerSessionInfo sessionInfo)
	{
		this.source = source;
		this.sessionInfo = sessionInfo;
	}

	public override void Update (ISCNSceneRenderer renderer, double timeInSeconds)
	{
		// Note: Always a super-tricky thing in ARKit : must get rid of the managed reference to the Frame object ASAP.
		using (var frame = source.SessionFrame ())
		{
			if (frame is null)
			{
				return;
			}
			source.CurrentScan?.UpdateOnEveryFrame (frame);
			source.ActiveTestRun?.UpdateOnEveryFrame ();
		}
	}

	public override void DidAddNode (ISCNSceneRenderer renderer, SCNNode node, ARAnchor anchor)
	{
		if (anchor is not null && anchor is ARObjectAnchor)
		{
			var objectAnchor = anchor as ARObjectAnchor;
			if (source.ActiveTestRun is not null && objectAnchor.ReferenceObject == source.ActiveTestRun.ReferenceObject)
			{
				source.ActiveTestRun.SuccessfulDetection (objectAnchor);
				var messageText = $"Object successfully detected from this angle.\n{source.ActiveTestRun.Statistics}";
				sessionInfo.DisplayMessage (messageText, source.ActiveTestRun.ResultDisplayDuration.TotalSeconds);
			}
		}
		else
		{
			if (source.State?.CurrentState == AppState.Scanning && anchor is ARPlaneAnchor)
			{
				var planeAnchor = anchor as ARPlaneAnchor;
				if (planeAnchor is null)
					return;
				source.CurrentScan?.ScannedObject.TryToAlignWithPlanes (new [] { planeAnchor });
			}
		}
	}

	public override void DidUpdateNode (ISCNSceneRenderer renderer, SCNNode node, ARAnchor anchor)
	{
		if (source.State?.CurrentState == AppState.Scanning && anchor is ARPlaneAnchor)
		{
			var planeAnchor = anchor as ARPlaneAnchor;
			if (planeAnchor is null)
				return;
			source.CurrentScan?.ScannedObject.TryToAlignWithPlanes (new [] { planeAnchor });
		}
	}
}
