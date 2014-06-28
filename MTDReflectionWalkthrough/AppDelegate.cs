using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;
using MonoTouch.Dialog;

namespace MTDReflectionWalkthrough
{
	[Register ("AppDelegate")]
    public partial class AppDelegate : UIApplicationDelegate
	{
		UIWindow window;
		UINavigationController nav;
	
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			
			window = new UIWindow (UIScreen.MainScreen.Bounds);
            
			var expense = new Expense ();
			var bctx = new BindingContext (null, expense, "Create a task");
			var dvc = new DialogViewController (bctx.Root);
			
			nav = new UINavigationController(dvc);
			    
			window.RootViewController = nav;
			window.MakeKeyAndVisible ();
            
			return true;
		}
	}
}
