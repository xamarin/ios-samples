using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace InitalScreenDemo
{
	partial class ViewController1 : UIViewController
	{
		public ViewController1 (IntPtr handle) : base (handle)
		{
		}

		partial void InitialActionCompleted (UIButton sender)
		{
			aButton.Hidden = true;  
		}

		public override void ViewDidLoad ()
		{
			if (ParentViewController != null){
				aButton.Hidden = true;
			}

		}
	}
}
