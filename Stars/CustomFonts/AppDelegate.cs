using System;
using System.Collections.Generic;
using System.Linq;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace CustomFonts
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		public override UIWindow Window {
			get;
			set;
		}

		public override bool FinishedLaunching (UIApplication application, NSDictionary launcOptions)
		{
			// Add to our FontLoader fonts that we included in our bundle WITHOUT using the UIAppFonts key in our Info.plist
			// Note that this operation will not instantiate and register the fonts creating a peformance problem during launch.
			//
			// Using the UIAppFonts key in the Info.plist is very convenient way to bundle fonts with your Applicaion. However, it may
			// be unsuitable to many applications (e.g. apps that utilize many fonts because it takes time to register every font
			// at launch time, or the font(s) should not be available until it is actually needed.

			FontLoader.SharedFontLoader.AddFontFilesAndNames (new string [] { "LowVow.ttf", "LowVow-Bold.ttf" }, 
				new string [] { "LowVow", "LowVow-Bold" }, NSBundle.MainBundle.ResourcePath);
			return true;
		}

		static void Main (string[] args)
		{
			UIApplication.Main (args, null, "AppDelegate");
		}
	}
}
