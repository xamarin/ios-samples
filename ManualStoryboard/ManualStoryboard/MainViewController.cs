using System;
using Foundation;
using UIKit;
using System.CodeDom.Compiler;

namespace ManualStoryboard
{
	public partial class MainViewController : UIViewController
	{
		UIViewController pinkViewController;

		public MainViewController (IntPtr handle) : base (handle)
		{

		}

		public override void AwakeFromNib ()
		{
			// Called when loaded from xib or storyboard.

			this.Initialize ();
		}

		public void Initialize(){

			var myStoryboard = AppDelegate.Storyboard;
			//Instatiating View Controller with Storyboard ID 'PinkViewController'
			pinkViewController = myStoryboard.InstantiateViewController ("PinkViewController") as PinkViewController;
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			//When we push the button, we will push the pinkViewController onto our current Navigation Stack
			PinkButton.TouchUpInside += (o, e) => {
				this.NavigationController.PushViewController (pinkViewController, true);
			};
		}

	}
}
