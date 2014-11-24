using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;
using CoreGraphics;

namespace PhotoFilter
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		UIWindow window;
		UILabel note;

		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			// check is it 64bit or 32bit
			Console.WriteLine (IntPtr.Size);

			// Main app do nothing
			// Go to Photo > Edit then choose PhotoFilter to start app extension

			window = new UIWindow (UIScreen.MainScreen.Bounds);
			window.BackgroundColor = UIColor.White;

			note = new UILabel ();
			note.Text = "Note that the app in this sample only serves as a host for the extension. To use the sample extension, edit a photo or video using the Photos app, and tap the extension icon";
			note.Lines = 0;
			note.LineBreakMode = UILineBreakMode.WordWrap;
			var frame = note.Frame;
			note.Frame = new CGRect (0, 0, UIScreen.MainScreen.Bounds.Width * 0.75f, 0);
			note.SizeToFit ();

			window.AddSubview (note);
			note.Center = window.Center;

			window.MakeKeyAndVisible ();
			return true;
		}
	}
}

