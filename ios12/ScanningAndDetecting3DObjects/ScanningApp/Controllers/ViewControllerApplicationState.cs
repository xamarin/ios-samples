using Foundation;
using System;
using ARKit;
using CoreFoundation;
namespace ScanningAndDetecting3DObjects
{

	enum AppState
	{
 		StartARSession,
		NotReady,
		Scanning,
		Testing
	}

	internal class ViewControllerApplicationState : NSObject
	{
		internal static readonly NSString ApplicationStateChangedNotificationName = new NSString("ApplicationStateChanged");
		internal static readonly NSString AppStateUserInfoKey = new NSString("AppState");

		// Type of UserInfo[AppStateUserInfoKey] : ViewControllerApplicationState;
		private NSNotification StateChangedNotification(AppState state)
		{
			NSDictionary userInfo = NSDictionary.FromObjectAndKey(new SimpleBox<AppState>(state), AppStateUserInfoKey);
			return NSNotification.FromName(ApplicationStateChangedNotificationName, this, userInfo);
		}

		private ViewController source;
		private AppState internalState;

		internal AppState CurrentState { 
			get => internalState;
			set  
			{
				// 1. Check that the preconditions for the state change are met.
				var newState = value;
				switch(value)
				{
					case AppState.StartARSession : break;
					case AppState.NotReady :
						// Immediately switch to Ready if tracking state is normal
						var camera = source.CurrentFrameCamera();
						if (camera != null)
						{
							switch (camera.TrackingState)
							{
								case ARTrackingState.Normal :
									newState = AppState.Scanning;
									break;
								default :
									break;
							}
						}
						else 
						{
							newState = AppState.StartARSession;
						}
						break;
					case AppState.Scanning :
						// Immediately switch to NotReady if tracking state is not normal
						var cameraS = source.CurrentFrameCamera();
						if (cameraS != null)
						{
							switch (cameraS.TrackingState)
							{
								case ARTrackingState.Normal : break;
								default :
									newState = AppState.NotReady;
									break;
							}
						}
						else
						{
							newState = AppState.StartARSession;
						}
						break;
					case AppState.Testing :
						var scan = source.CurrentScan;
						if (scan == null || ! scan.BoundingBoxExists)
						{
							Console.WriteLine("Error: Scan is not ready to be tested");
							//Note: return, not break
							return;
						}
						break;
				}

				// 2. Apply changes as needed per state
				internalState = newState;

				switch(newState)
				{
					case AppState.StartARSession :
						Console.WriteLine("State : Starting ARSession");
						source.EnterStateStartARSession();
						break;
					case AppState.NotReady :
						Console.WriteLine("State : Not ready to scan");
						source.EnterStateNotReady();
						break;
					case AppState.Scanning :
						Console.WriteLine("State : Scanning");
						source.EnterStateScanning();
						break;
					case AppState.Testing :
						Console.WriteLine("State : Testing");
						source.EnterStateTesting();
						break;
				}

				NSNotificationCenter.DefaultCenter.PostNotification(StateChangedNotification(newState));
			}
		}

		internal ViewControllerApplicationState(ViewController source)
		{
			this.source = source;
			this.CurrentState = AppState.StartARSession;
		}

		internal void SwitchToNextState()
		{
			switch (CurrentState)
			{
				case AppState.StartARSession :
					CurrentState = AppState.NotReady;
					break;
				case AppState.NotReady :
					CurrentState = AppState.Scanning;
					break;
				case AppState.Scanning :
					var scan = source.CurrentScan; 
					if (scan != null)
					{
						switch (scan.State)
						{
							case Scan.ScanState.Ready :
								scan.State = Scan.ScanState.DefineBoundingBox;
								break;
							case Scan.ScanState.DefineBoundingBox :
								scan.State = Scan.ScanState.Scanning;
								break;
							case Scan.ScanState.Scanning :
								scan.State = Scan.ScanState.AdjustingOrigin;
								break;
							case Scan.ScanState.AdjustingOrigin :
								CurrentState = AppState.Testing;
								break;
						}
					}
					break;
				case AppState.Testing :
					// Testing is the last state, show the share sheet at the end.
					source.CreateAndShareReferenceObject();
					break;
			}
		}

		internal void SwitchToPreviousState()
		{
			switch (CurrentState)
			{
				case AppState.StartARSession : break;
				case AppState.NotReady :
					CurrentState = AppState.StartARSession;
					break;
				case AppState.Scanning :
					var scan = source.CurrentScan;
					if (scan != null)
					{
						switch (scan.State)
						{
							case Scan.ScanState.Ready :
								source.RestartButtonTapped(this, new EventArgs());
								break;
							case Scan.ScanState.DefineBoundingBox :
								scan.State = Scan.ScanState.Ready;
								break;
							case Scan.ScanState.Scanning :
								scan.State = Scan.ScanState.DefineBoundingBox;
								break;
							case Scan.ScanState.AdjustingOrigin :
								scan.State = Scan.ScanState.Scanning;
								break;
						}
					}
					break;
				case AppState.Testing :
					CurrentState = AppState.Scanning;
					var scanT = source.CurrentScan;
					if (scanT != null)
					{
						scanT.State = Scan.ScanState.AdjustingOrigin;
					}
					break;
			}
		}


		internal void BoundingBoxWasCreated(NSNotification notification)
		{
			if (source.CurrentScan == null)
			{
				return;
			}
			if (source.CurrentScan.State == Scan.ScanState.DefineBoundingBox)
			{
				DispatchQueue.MainQueue.DispatchAsync(() =>
				{
					source.EnableNextButton(true);
				});
			}
		}

		internal void GhostBoundingBoxWasRemoved(NSNotification notification)
		{
			if (source.CurrentScan == null)
			{
				return;
			}
			if (source.CurrentScan.State == Scan.ScanState.Ready)
			{
				DispatchQueue.MainQueue.DispatchAsync(() =>
				{
					source.EnableNextButton(false);
					source.DisplayInstruction(new Message("Point at a nearby object to scan."));
				});
			}
		}

		internal void GhostBoundingBoxWasCreated(NSNotification notification)
		{
			if (source.CurrentScan == null)
			{
				return;
			}
			if (source.CurrentScan.State == Scan.ScanState.Ready)
			{
				DispatchQueue.MainQueue.DispatchAsync(() =>
				{
					source.EnableNextButton(true);
					source.DisplayInstruction(new Message("Tap 'Next' to create an approximate bounding box around the object you want to scan"));
				});
			}
		}

		internal void ScanningStateChanged(NSNotification notification)
		{
			// Guard condition is complex, so break it down into multiple steps
			if (CurrentState != AppState.Scanning)
			{
				return;
			}
			var notificationObject = notification.Object as Scan;
			if (notificationObject == null)
			{
				return;
			}
			if (notificationObject !=  source.CurrentScan)
			{
				return;
			}
			var scanState = notification.UserInfo[Scan.StateUserInfoKey] as SimpleBox<Scan.ScanState>;
			if (scanState == null)
			{
				return;
			}

			DispatchQueue.MainQueue.DispatchAsync(() =>
			{
				switch(scanState.Value)
				{
					case Scan.ScanState.Ready :
						Console.WriteLine("State: Ready to scan");
						source.EnterStateScanReady();
						break;
					case Scan.ScanState.DefineBoundingBox :
						Console.WriteLine("State : Define bounding box");
						source.EnterStateDefineBoundingBox();
						break;
					case Scan.ScanState.Scanning :
						source.EnterStateScanningContinue();
						break;
					case Scan.ScanState.AdjustingOrigin :
						Console.WriteLine("State : Adjusting origin");
						source.EnterStateAdjustingOrigin();
						break;
				}
			});
		}

	}
}
