using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;

namespace Working_with_images {
	public class Application {
		/// <summary>
		/// This is the main entry point of the application.
		/// </summary>
		static void Main (string [] args)
		{
			// if you want to use a different Application Delegate class from "AppDelegate"
			// you can specify it here.
			UIApplication.Main (args, null, "AppDelegate");
		}
	}
}
