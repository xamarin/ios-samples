using ARKit;
using System.ComponentModel.Design.Serialization;
using System;

namespace ScanningAndDetecting3DObjects
{
	internal class ViewControllerSessionDelegate : ARSessionDelegate
	{
		ViewController source;
		ViewControllerApplicationState appState;
		ViewControllerSessionInfo sessionInfo;

		internal ViewControllerSessionDelegate(ViewController source, ViewControllerApplicationState appState, ViewControllerSessionInfo sessionInfo)
		{
			this.source = source;
			this.appState = appState;
			this.sessionInfo = sessionInfo;
		}

		public override void CameraDidChangeTrackingState(ARSession session, ARCamera camera)
		{
			sessionInfo.UpdateSessionInfoLabel(camera);

			switch (camera.TrackingState)
			{
				case ARTrackingState.NotAvailable :
					appState.CurrentState = AppState.NotReady;
					break;
				case ARTrackingState.Limited :
					switch (appState.CurrentState)
					{
						case AppState.StartARSession:
							appState.CurrentState = AppState.NotReady;
							break;
						case AppState.NotReady:
						case AppState.Testing:
							break;
						case AppState.Scanning:
							var scan = source.CurrentScan;
							if (scan != null)
							{
								switch (scan.State)
								{
									case Scan.ScanState.Ready:
										appState.CurrentState = AppState.NotReady;
										break;
									case Scan.ScanState.DefineBoundingBox:
									case Scan.ScanState.Scanning:
									case Scan.ScanState.AdjustingOrigin:
										var reason = camera.TrackingStateReason;
										if (reason == ARTrackingStateReason.Relocalizing)
										{
											// If ARKit is relocalizing we should abort the current scan
											// as this can cause unpredictable distortions of the map.
											Console.WriteLine("Warning: ARKit is relocalizing");

											var title = "Warning: Scan may be broken";
											var message = "A gap in tracking has occurred. It is recommended to restart the scan.";
											var buttonTitle = "Restart Scan";
											source.ShowAlert(title, message, buttonTitle, true, (_) =>
											{
												appState.CurrentState = AppState.NotReady;
											});

										}
										else
										{
											// Suggest the user restart tracking after a while
											source.StartLimitedTrackingTimer();
										}
										break;
								}
							}
							break;
					}
					break;
				case ARTrackingState.Normal :
					source.CancelLimitedTrackingTimer();

					switch (appState.CurrentState)
					{
						case AppState.StartARSession :
						case AppState.NotReady :
							appState.CurrentState = AppState.Scanning;
							break;
						case AppState.Scanning : 
						case AppState.Testing :
							break;


					}
					break;
			}
		}
	}
}