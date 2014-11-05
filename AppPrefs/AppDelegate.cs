using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;

namespace AppPrefs
{
	/// <summary>
	/// The UIApplicationDelegate for the application. This class is responsible for launching the 
	/// User Interface of the application, as well as listening (and optionally responding) to 
	/// application events from iOS.
	/// </summary>
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		// class-level declarations
		NSObject observer;

		public override UIWindow Window { get; set; }

		public override void FinishedLaunching (UIApplication application)
		{
			observer = NSNotificationCenter.DefaultCenter.AddObserver ((NSString)"NSUserDefaultsDidChangeNotification", DefaultsChanged);
			DefaultsChanged (null);
		}
		
		/// <summary>
		/// This method is called when the application is about to terminate. Save data, if needed. 
		/// </summary>
		/// <seealso cref="DidEnterBackground"/>
		public override void WillTerminate (UIApplication application)
		{
			if (observer != null) {
				NSNotificationCenter.DefaultCenter.RemoveObserver (observer);
				observer = null;
			}
		}
		
		void DefaultsChanged (NSNotification obj)
		{
			Settings.SetupByPreferences ();
		}
	}
}
