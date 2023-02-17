using System;
using UIKit;

namespace tvText {
	/// <summary>
	/// This View Controller controls the Text tab that is used to enter a User ID
	/// and password.
	/// </summary>
	public partial class FirstViewController : UIViewController {
		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="T:tvText.FirstViewController"/> class.
		/// </summary>
		/// <param name="handle">Handle.</param>
		public FirstViewController (IntPtr handle) : base (handle)
		{
		}
		#endregion

		#region Override Methods
		/// <summary>
		/// Called after the view is loaded into memory from the Storyboard file.
		/// </summary>
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			// Perform any additional setup after loading the view, typically from a nib.
		}

		/// <summary>
		/// Called if the View Controller received a low memory warning.
		/// </summary>
		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}
		#endregion

		#region Actions
		/// <summary>
		/// Logins the button primary action triggered.
		/// </summary>
		/// <param name="sender">Sender.</param>
		partial void LoginButton_PrimaryActionTriggered (UIButton sender)
		{
			Console.WriteLine ("User ID {0} and Password {1}", UserId.Text, Password.Text);
		}
		#endregion
	}
}

