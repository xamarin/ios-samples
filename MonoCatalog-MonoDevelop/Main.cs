
using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace MonoTouch {
	
	class Application
	{
		static void Main (string [] args)
		{
			// It will load the main UI as specified in the
			// Info.plist file (MainWindow.nib)
			UIApplication.Main (args, null, null);
		}
	}
}