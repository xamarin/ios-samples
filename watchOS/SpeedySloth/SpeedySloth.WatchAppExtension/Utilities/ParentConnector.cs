
namespace SpeedySloth.WatchAppExtension {
	using Foundation;
	using System.Collections.Generic;
	using WatchConnectivity;

	/// <summary>
	/// Utilities class to encapsulate messaging with parent application on iPhone
	/// </summary>
	public class ParentConnector : WCSessionDelegate {
		private readonly List<string> statesToSend = new List<string> ();

		private WCSession session;

		public void Send (string state)
		{
			if (this.session != null) {
				if (this.session.Reachable) {
					this.SendMessage (state);
				}
			} else {
				if (WCSession.DefaultSession.ActivationState == WCSessionActivationState.Activated) {
					// if here is an old delegate - dispose it
					if (WCSession.DefaultSession.Delegate != null) {
						WCSession.DefaultSession.Delegate.Dispose ();
						WCSession.DefaultSession.Delegate = null;
					}

					WCSession.DefaultSession.Delegate = this;
					this.session = WCSession.DefaultSession;
					this.Send (state);
				} else {
					WCSession.DefaultSession.Delegate = this;
					WCSession.DefaultSession.ActivateSession ();
					this.statesToSend.Add (state);
				}
			}
		}

		protected override void Dispose (bool disposing)
		{
			base.Dispose (disposing);
			this.statesToSend.Clear ();

			if (this.session != null) {
				this.session.Dispose ();
				this.session = null;
			}
		}

		public override void ActivationDidComplete (WCSession session, WCSessionActivationState activationState, NSError error)
		{
			if (activationState == WCSessionActivationState.Activated) {
				this.session = session;
				this.SendPending ();
			}
		}

		private void SendPending ()
		{
			if (this.session != null && this.session.Reachable) {
				foreach (var state in this.statesToSend) {
					this.SendMessage (state);
				}

				this.statesToSend.Clear ();
			}
		}

		private void SendMessage (string state)
		{
			var data = new NSDictionary<NSString, NSObject> (new NSString ("State"), NSObject.FromObject (state));
			this.session.SendMessage (data, null, null);
		}
	}
}
