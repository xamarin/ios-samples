using System;
using UIKit;

namespace Example_Touch {
	public class Application {
		public static void Main (string [] args)
		{
			try {
				UIApplication.Main (args, null, "AppDelegate");
			} catch (Exception e) {
				Console.WriteLine (e.ToString ());
			}
		}
	}
}
