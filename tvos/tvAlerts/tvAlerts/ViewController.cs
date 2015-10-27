using System;
using Foundation;
using UIKit;

namespace MySingleView
{
	public partial class ViewController : UIViewController
	{
		#region Constructors
		public ViewController (IntPtr handle) : base (handle)
		{
		}
		#endregion

		#region Override Methods
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			// Perform any additional setup after loading the view, typically from a nib.
		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}
		#endregion

		#region Custom Actions
		partial void DisplayDestructiveAlert (Foundation.NSObject sender) {
			// User helper class to present alert
			AlertViewController.PresentDestructiveAlert("A Short Title is Best","The message should be a short, complete sentence.","Delete",this, (ok) => {
				Console.WriteLine("Destructive Alert: The user selected {0}",ok);
			});
		}

		partial void DisplayOkCancelAlert (Foundation.NSObject sender) {
			// User helper class to present alert
			AlertViewController.PresentOKCancelAlert("A Short Title is Best","The message should be a short, complete sentence.",this, (ok) => {
				Console.WriteLine("OK/Cancel Alert: The user selected {0}",ok);
			});
		}

		partial void DisplaySimpleAlert (Foundation.NSObject sender) {
			// User helper class to present alert
			AlertViewController.PresentOKAlert("A Short Title is Best","The message should be a short, complete sentence.",this);
		}

		partial void DisplayTextInputAlert (Foundation.NSObject sender) {
			// User helper class to present alert
			AlertViewController.PresentTextInputAlert("A Short Title is Best","The message should be a short, complete sentence.","placeholder", "", this, (ok, text) => {
				Console.WriteLine("Text Input Alert: The user selected {0} and entered `{1}`",ok,text);
			});
		}
		#endregion
	}
}


