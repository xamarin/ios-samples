
namespace SpeedySloth
{
    using CoreFoundation;
    using Foundation;
    using HealthKit;
    using System;
    using UIKit;
    using WatchConnectivity;

    public partial class WorkoutViewController : UIViewController, IWCSessionDelegate
    {
        private readonly HKHealthStore healthStore = new HKHealthStore();

        private Action<WCSession> sessionActivationCompletion;

        public WorkoutViewController(IntPtr handle) : base(handle) { }

        public HKWorkoutConfiguration Configuration { get; set; }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            this.StartWatchApp();
        }

        public override void ViewDidDisappear(bool animated)
        {
            this.healthStore.Dispose();
            base.ViewDidDisappear(animated);
        }

        #region Convenience

        private void StartWatchApp()
        {
            if (this.Configuration != null)
            {
                this.GetActiveWCSession((wcSession) =>
                {
                    if (wcSession.ActivationState == WCSessionActivationState.Activated && wcSession.WatchAppInstalled)
                    {
                        this.healthStore.StartWatchApp(this.Configuration, (isSucces, error) =>
                        {
                            if (!isSucces)
                            {
                                Console.WriteLine($"starting watch app failed with error: ({error.Description})");
                            }
                        });
                    }
                });
            }
        }

        public void GetActiveWCSession(Action<WCSession> completion)
        {
            if (WCSession.IsSupported)
            {
                var wcSession = WCSession.DefaultSession;
                wcSession.Delegate = this;

                switch (wcSession.ActivationState)
                {
                    case WCSessionActivationState.NotActivated:
                    case WCSessionActivationState.Inactive:
                        wcSession.ActivateSession();
                        this.sessionActivationCompletion = completion;
                        break;
                    case WCSessionActivationState.Activated:
                        completion(wcSession);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                // ...Alert the user that their iOS device does not support watch connectivity
                throw new NotSupportedException("watch connectivity session nor supported");
            }
        }

        public void UpdateSessionState(string state)
        {
            if (state == "ended")
            {
                DispatchQueue.MainQueue.DispatchAsync(() =>
                {
                    this.DismissModalViewController(true);
                });
            }
            else
            {
                this.WorkoutSessionState.Text = state;
            }
        }

        #endregion

        #region WCSessionDelegate

        [Export("session:activationDidCompleteWithState:error:")]
        public void ActivationDidComplete(WCSession session, WCSessionActivationState activationState, NSError error)
        {
            if (activationState == WCSessionActivationState.Activated)
            {
                var activationCompletion = this.sessionActivationCompletion;
                if (activationCompletion != null)
                {
                    activationCompletion(session);
                    this.sessionActivationCompletion = null;
                }
            }
        }

        [Export("session:didReceiveMessage:")]
        public void DidReceiveMessage(WCSession session, Foundation.NSDictionary<Foundation.NSString, Foundation.NSObject> message)
        {
            NSObject state;
            if (message.TryGetValue(new NSString("State"), out state))
            {
                DispatchQueue.MainQueue.DispatchAsync(() =>
                {
                    var @string = state as NSString;
                    this.UpdateSessionState(@string);
                });
            }
        }

        [Export("sessionDidBecomeInactive:")]
        public void DidBecomeInactive(WCSession session)
        {
        }

        [Export("sessionDidDeactivate:")]
        public void DidDeactivate(WCSession session)
        {
        }

        #endregion
    }
}