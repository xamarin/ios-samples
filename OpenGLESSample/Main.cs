using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;
using OpenTK.Platform;
using OpenGLES;

namespace OpenGLESSample
{
	public class Application
	{
		static void Main (string[] args)
		{
			using (var c = Utilities.CreateGraphicsContext(EAGLRenderingAPI.OpenGLES1)) {
		
				UIApplication.Main (args);
	
			}
		}
	}
	
	// The name AppDelegate is referenced in the MainWindow.xib file.
	public partial class OpenGLESSampleAppDelegate : UIApplicationDelegate
	{
		// This method is invoked when the application has loaded its UI and its ready to run
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			// If you have defined a view, add it here:
			// window.AddSubview (navigationController.View);
			
			glView.AnimationInterval = 1.0 / 60.0;
			glView.StartAnimation();

			
			window.MakeKeyAndVisible ();
	
			return true;
		}
	
		public override void OnResignActivation (UIApplication application)
		{
			glView.AnimationInterval = 1.0 / 5.0;
		}
		
		// This method is required in iPhoneOS 3.0
		public override void OnActivated (UIApplication application)
		{
			glView.AnimationInterval = 1.0 / 60.0;
		}
	}
}

