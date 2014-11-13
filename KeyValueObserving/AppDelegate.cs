//
// kvo: Shows how to observe property changes on an object
//
using System;
using CoreGraphics;

using Foundation;
using UIKit;

namespace kvo
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		UINavigationController topController;
		UIWindow window;
		UILabel label;
		
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			topController = new UINavigationController ();
			topController.View.BackgroundColor = UIColor.Gray;
			
			label = new UILabel (ComputeLabelRect ()) {
				Text = "Rotate the phone to observe the `bounds' change",
				LineBreakMode = UILineBreakMode.WordWrap,
				Lines = 20,
				BackgroundColor = UIColor.LightGray
			};
			topController.View.AddSubview (label);
			
			window = new UIWindow (UIScreen.MainScreen.Bounds) {
				RootViewController = topController
			};
			window.MakeKeyAndVisible ();
			
			// 
			// Observe changes to the topController's View property, in this case, we want to
			// observe changes to the "bounds" property (this is the name of the Objective-C
			// property).
			//
			// The changes observed are delivered to "this", which is expected to override
			// the "ObserveValue" method.
			//
			topController.View.AddObserver (observer: this, 
			                                keyPath:  new NSString ("bounds"), 
			                                options:  NSKeyValueObservingOptions.New, 
			                                context:  IntPtr.Zero);
			return true;
		}
		
		// 
		// Override the ObserveValue method on the class that you designate as the observer
		// 
		public override void ObserveValue (NSString keyPath, NSObject ofObject, NSDictionary change, IntPtr context)
		{
			var str = String.Format ("The {0} property on {1}, the change is: {2}", keyPath, ofObject, change.Description);
			Console.WriteLine (str);
			label.Text = str;
			
			label.Frame = ComputeLabelRect ();
		}
		
		CGRect ComputeLabelRect ()
		{
			var rect = topController.View.Bounds;
			rect.Inflate (-20, -80);

			return rect;
		}
		
		static void Main (string[] args)
		{
			UIApplication.Main (args, null, "AppDelegate");
		}
	}
}

