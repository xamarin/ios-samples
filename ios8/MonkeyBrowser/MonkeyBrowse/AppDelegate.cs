using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;

namespace MonkeyBrowse
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the
	// User Interface of the application, as well as listening (and optionally responding) to
	// application events from iOS.
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		#region Public Constants
		public NSString UserActivityTab1 = new NSString ("com.xamarin.monkeybrowser.tab1");
		public NSString UserActivityTab2 = new NSString ("com.xamarin.monkeybrowser.tab2");
		public NSString UserActivityTab3 = new NSString ("com.xamarin.monkeybrowser.tab3");
		public NSString UserActivityTab4 = new NSString ("com.xamarin.monkeybrowser.tab4");
		#endregion

		#region Computed Properties
		public override UIWindow Window { get; set;}
		public FirstViewController Tab1 { get; set; }
		public SecondViewController Tab2 { get; set;}
		public ThirdViewController Tab3 { get; set; }
		public FourthViewController Tab4 { get; set; }
		#endregion

		#region Override Methods
		/// <summary>
		/// The application is getting warning that a Handoff continuation is about occur.
		/// </summary>
		/// <returns><c>true</c>, if continue user activity was willed, <c>false</c> otherwise.</returns>
		/// <param name="application">Application.</param>
		/// <param name="userActivityType">User activity type.</param>
		public override bool WillContinueUserActivity (UIApplication application, string userActivityType)
		{
			// Report Activity
			Console.WriteLine ("Will Continue Activity: {0}", userActivityType);

			// Take action based on the user activity type
			switch (userActivityType) {
			case "com.xamarin.monkeybrowser.tab1":
				// Inform view that it's going to be modified
				Tab1.PreparingToHandoff ();
				break;
			case "com.xamarin.monkeybrowser.tab2":
				// Inform view that it's going to be modified
				Tab2.PreparingToHandoff ();
				break;
			case "com.xamarin.monkeybrowser.tab3":
				// Inform view that it's going to be modified
				Tab3.PreparingToHandoff ();
				break;
			case "com.xamarin.monkeybrowser.tab4":
				// Inform view that it's going to be modified
				Tab4.PreparingToHandoff ();
				break;
			}

			// Inform system we handled this
			return true;
		}

		/// <returns>To be added.</returns>
		/// <summary>
		/// Continues the user activity.
		/// </summary>
		/// <param name="application">Application.</param>
		/// <param name="userActivity">User activity.</param>
		/// /// <param name="completionHandler">Completion Handler.</param>
		public override bool ContinueUserActivity (UIApplication application, NSUserActivity userActivity, UIApplicationRestorationHandler completionHandler)
		{

			// Report Activity
			Console.WriteLine ("Continuing User Activity: {0}", userActivity.ToString());

			// Get input and output streams from the Activity
			userActivity.GetContinuationStreams ((NSInputStream arg1, NSOutputStream arg2, NSError arg3) => {
				// Send required data via the streams
				// ...
			});

			// Take action based on the Activity type
			switch (userActivity.ActivityType) {
			case "com.xamarin.monkeybrowser.tab1":
				// Preform handoff
				Tab1.PerformHandoff (userActivity);
				completionHandler (new NSObject[]{Tab1});
				break;
			case "com.xamarin.monkeybrowser.tab2":
				// Preform handoff
				Tab2.PerformHandoff (userActivity);
				completionHandler (new NSObject[]{Tab2});
				break;
			case "com.xamarin.monkeybrowser.tab3":
				// Preform handoff
				Tab3.PerformHandoff (userActivity);
				completionHandler (new NSObject[]{Tab3});
				break;
			case "com.xamarin.monkeybrowser.tab4":
				// Preform handoff
				Tab4.PerformHandoff (userActivity);
				completionHandler (new NSObject[]{Tab4});
				break;
			}

			// Inform system we handled this
			return true;
		}

		/// <summary>
		/// Dids the fail to continue user activitiy.
		/// </summary>
		/// <param name="application">Application.</param>
		/// <param name="userActivityType">User activity type.</param>
		public override void DidFailToContinueUserActivitiy (UIApplication application, string userActivityType, NSError error)
		{
			// Log information about the failure
			Console.WriteLine ("User Activity {0} failed to continue. Error: {1}", userActivityType, error.LocalizedDescription);
		}

		// This method is invoked when the application is about to move from active to inactive state.
		// OpenGL applications should use this method to pause.
		public override void OnResignActivation (UIApplication application)
		{
		}
		
		// This method should be used to release shared resources and it should store the application state.
		// If your application supports background exection this method is called instead of WillTerminate
		// when the user quits.
		public override void DidEnterBackground (UIApplication application)
		{
		}
		
		// This method is called as part of the transiton from background to active state.
		public override void WillEnterForeground (UIApplication application)
		{
		}
		
		// This method is called when the application is about to terminate. Save data, if needed.
		public override void WillTerminate (UIApplication application)
		{
		}
		#endregion
	}
}

