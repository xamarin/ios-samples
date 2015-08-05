using System;
using Xamarin.Controls;
using UIKit;

namespace HelloComponents
{
	public partial class ViewController : UIViewController
	{
		public ViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			AlertCenter.Default.PostMessage ("Knock knock!", "Who's there?");
			AlertCenter.Default.PostMessage ("Interrupting cow.", "Interrupting cow who?",
				UIImage.FromFile ("cow.png"), delegate {
				Console.WriteLine ("Moo!");
			});
		}
	}
}

