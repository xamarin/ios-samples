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