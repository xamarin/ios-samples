using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;
using CoreGraphics;

namespace print
{
	public class Application
	{
		static void Main (string[] args)
		{
			UIApplication.Main (args);
		}
	}
	
	public partial class AppDelegate : UIApplicationDelegate
	{
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			window.MakeKeyAndVisible ();
			var button = UIButton.FromType (UIButtonType.RoundedRect);
			button.Frame = new CGRect (100, 100, 120, 60);
			button.SetTitle ("Print", UIControlState.Normal);
			button.TouchDown += delegate {
				Print ();
			};
			window.AddSubview (button);
			return true;
		}

		void Print ()
		{
			var printInfo = UIPrintInfo.PrintInfo;
			printInfo.OutputType = UIPrintInfoOutputType.General;
			printInfo.JobName = "My first Print Job";
			
			var textFormatter = new UISimpleTextPrintFormatter ("Once upon a time...") {
				StartPage = 0,
				ContentInsets = new UIEdgeInsets (72, 72, 72, 72),
				MaximumContentWidth = 6 * 72,
			};
			
			var printer = UIPrintInteractionController.SharedPrintController;
			printer.PrintInfo = printInfo;
			printer.PrintFormatter = textFormatter;
			printer.ShowsPageRange = true;
			printer.Present (true, (handler, completed, err) => {
				if (!completed && err != null){
					Console.WriteLine ("error");
				}
			});
		}
		
		public override void OnActivated (UIApplication application)
		{
		}
	}
}

