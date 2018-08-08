using System;
using ARKit;
using Foundation;
using CoreFoundation;

namespace ScanningAndDetecting3DObjects
{
	internal class ViewControllerSessionInfo
	{
		ViewController source;
		DateTime startTimeOfLastMessage;
		double expirationTimeOfLastMessage;
		NSTimer messageExpirationTimer;

		internal ViewControllerSessionInfo(ViewController source)
		{
			this.source = source;
		}


		internal void UpdateSessionInfoLabel(ARCamera camera)
		{
			// Update the UI to provide feedback on the state of the AR experience

			var message = "";
			var stateString = source.State.CurrentState == AppState.Testing ? "Detecting" : "Scanning";

			switch (camera.TrackingState)
			{
				case ARTrackingState.NotAvailable :
					message = $"{stateString} not possible : {camera.PresentationString()}";
					startTimeOfLastMessage = DateTime.Now;
					expirationTimeOfLastMessage = 3.0;
					break;
				case ARTrackingState.Limited :
					message = $"{stateString} might not work : {camera.PresentationString()}";
					startTimeOfLastMessage = DateTime.Now;
					expirationTimeOfLastMessage = 3.0;
					break;
				default :
					// No feedback needed when tracking is normal.
					// Defer clearing the info label if the last message hasn't reached its expiration time.
					var now = DateTime.Now;
					if ((now - startTimeOfLastMessage).TotalSeconds < expirationTimeOfLastMessage)
					{
						var timeToKeepLastMessageOnScreen = expirationTimeOfLastMessage - (now - startTimeOfLastMessage).TotalSeconds;
						StartMessageExpirationTimer(timeToKeepLastMessageOnScreen);
					}
					else
					{
						// Otherwise hide the info label immediately
						source.ShowSessionInfo("", false);
					}
					//Note return not break
					return;
			}

			source.ShowSessionInfo(message, true);
		}

		internal void DisplayMessage(string message, double expirationTime)
		{
			startTimeOfLastMessage = DateTime.Now;
			expirationTimeOfLastMessage = expirationTime;
			DispatchQueue.MainQueue.DispatchAsync(() =>
			{
				source.ShowSessionInfo(message, true);
				StartMessageExpirationTimer(expirationTime);
			});
		}

		private void StartMessageExpirationTimer(double duration)
		{
			CancelMessageExpirationTimer();

			messageExpirationTimer = NSTimer.CreateScheduledTimer(duration, (timer) =>
			{
				CancelMessageExpirationTimer();
				source.ShowSessionInfo("", false);

				startTimeOfLastMessage = default(DateTime);
				expirationTimeOfLastMessage = default(double);
			});
		}

		internal void CancelMessageExpirationTimer()
		{
			messageExpirationTimer?.Invalidate();
			messageExpirationTimer?.Dispose();
			messageExpirationTimer = null;
		}
	}
}