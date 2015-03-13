using System;
using Foundation;
using UIKit;
using System.CodeDom.Compiler;

namespace UIKitEnhancements
{
	public partial class DetailsViewController : UINavigationController
	{
		#region Computed Properties
		/// <summary>
		/// Returns the delegate of the current running application
		/// </summary>
		/// <value>The this app.</value>
		public AppDelegate ThisApp {
			get { return (AppDelegate)UIApplication.SharedApplication.Delegate; }
		}

		/// <summary>
		/// Gets or sets the menu item.
		/// </summary>
		/// <value>The menu item.</value>
		public MenuItem MenuItem { get; set; }
		#endregion

		#region Constructors
		public DetailsViewController (IntPtr handle) : base (handle)
		{
		}
		#endregion

		#region Override Methods
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Connect to Application Delegate
			ThisApp.iPadViewController = this;
		}

		/// <summary>	
		/// Prepares for segue.
		/// </summary>
		/// <param name="segue">Segue.</param>
		/// <param name="sender">Sender.</param>
		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			base.PrepareForSegue (segue, sender);

			// Dismiss previous view before displaying the next view
			// in the sequence.
			if (ViewControllers.Length>0) {
				// Pop everything off the stack back to the Home view
				PopToRootViewController (false);
			}

			// Take action based on the segue type
			switch (segue.Identifier) {
			case "WebSegue":
				var webView = segue.DestinationViewController as WebViewController;
				webView.URL = MenuItem.URL;
				break;
			} 

		}
		#endregion
	}
}
