using System;
using CoreGraphics;
using Foundation;
using UIKit;
using System.CodeDom.Compiler;

namespace HomeKitIntro
{
	partial class AddHomeViewController : UIViewController
	{
		#region Computed Properties
		/// <summary>
		/// Returns the delegate of the current running application
		/// </summary>
		/// <value>The this app.</value>
		public AppDelegate ThisApp {
			get { return (AppDelegate)UIApplication.SharedApplication.Delegate; }
		}
		#endregion

		#region Constructors
		public AddHomeViewController (IntPtr handle) : base (handle)
		{
		}
		#endregion

		#region Private Methods
		/// <summary>
		/// Moves the view away from the keyboard
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		private void MoveTo (float x, float y){
			View.Frame = new CGRect (0, y, View.Frame.Width, View.Frame.Height);
		}

		/// <summary>
		/// Adds the home.
		/// </summary>
		private void AddNewHome() {
			// Has a name been entered?
			if (HomeName.Text=="" ) {
				// No, inform the user that they must create a home first
				AlertView.PresentOKAlert("Home Name Error","You must enter a name for the home before it can be created.",this);
				return;
			}

			// Add new home to HomeKit
			ThisApp.HomeManager.AddHome(HomeName.Text,(home,error) =>{
				// Did an error occur
				if (error!=null) {
					// Yes, inform user
					AlertView.PresentOKAlert("Add Home Error",string.Format("Error adding {0}: {1}",HomeName.Text,error.LocalizedDescription),this);
					return;
				}

				// Make the primary house
				ThisApp.HomeManager.UpdatePrimaryHome(home,(err) => {
					// Error?
					if (err!=null) {
						// Inform user of error
						AlertView.PresentOKAlert("Add Home Error",string.Format("Unable to make this the primary home: {0}",err.LocalizedDescription),this);
						return ;
					}
				});

				// Close the window when the home is created
				DismissViewController(true,null);
			});
		}
		#endregion

		#region Override Methods
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			HomeName.ShouldBeginEditing= delegate(UITextField field){
				//Placeholder
				UIView.BeginAnimations("keyboard");
				UIView.SetAnimationDuration(0.3f);
				this.MoveTo(0,-170);
				UIView.CommitAnimations();
				return true;
			};
			HomeName.ShouldReturn = delegate (UITextField field){
				field.ResignFirstResponder ();
				UIView.BeginAnimations("keyboard");
				UIView.SetAnimationDuration(0.3f);
				this.MoveTo(0,0);
				UIView.CommitAnimations();

				return true;
			};
			HomeName.ShouldEndEditing= delegate (UITextField field){
				UIView.BeginAnimations("keyboard");
				UIView.SetAnimationDuration(0.3f);
				this.MoveTo(0,0);
				UIView.CommitAnimations();
				AddNewHome();
				return true;
			};

			// Wireup buttons
			AddHome.TouchUpInside += (sender, e) => {
				// Add the new home
				AddNewHome();
			};
		}
		#endregion
	}
}
