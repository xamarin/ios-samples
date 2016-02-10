using System;

using UIKit;

namespace MediaCapture {
	public class Application {
		static void Main (string[] args)
		{
			try {
				// register an event handler for unhandled exceptions.  This little trick handles cases where something has gone so wrong that
				// you can't even show an error dialog.
				AppDomain.CurrentDomain.UnhandledException += logUnhandledException;
				UIApplication.Main (args, null, "AppDelegate");
			} catch (Exception ex) {
				Utilities.LogUnhandledException (ex);
			}
		}

		static void logUnhandledException (object sender, UnhandledExceptionEventArgs e)
		{
			// log the unhandled exception.  It will be shown to the user the next time the app runs.
			Utilities.LogUnhandledException (e.ExceptionObject as Exception);
		}

	}
}
