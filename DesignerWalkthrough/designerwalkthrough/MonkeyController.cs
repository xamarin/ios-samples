using System;
using Foundation;
using UIKit;
using System.CodeDom.Compiler;

namespace DesignerWalkthrough
{
	partial class MonkeyController : UIViewController
	{
		public MonkeyController (IntPtr handle) : base (handle)
		{
		}

		partial void Clicked (UIButton sender)
		{
			Console.WriteLine("Back to Main pressed");
			DismissViewController(true, null);
		}

	}
}
