using System;
using System.CodeDom.Compiler;

using Foundation;
using UIKit;

namespace DesignerWalkthrough
{
	partial class MonkeyController : UIViewController
	{
		public MonkeyController (IntPtr handle) : base (handle)
		{
		}

		partial void Clicked (UIButton sender)
		{
			Console.WriteLine ("Back to Main pressed");
			DismissViewController (true, null);
		}
	}
}
