using System;
using UIKit;
using ElizaCore;
using Foundation;
using Intents;
using System.Threading;
using UserNotifications;

namespace ElizaChat {
	public partial class ViewController : UIViewController {
		#region AppDelegate Access
		public AppDelegate ThisApp {
			get { return (AppDelegate) UIApplication.SharedApplication.Delegate; }
		}
		#endregion

		#region Computed Properties
		public ElizaMain Eliza { get; set; } = new ElizaMain ();
		#endregion

		#region Constructors
		protected ViewController (IntPtr handle) : base (handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}
		#endregion

		#region Public Methods
		public void AskQuestion (string question, bool sendNotification)
		{
			// Anything to process?
			if (question == "") return;

			// Add question to history
			ChatHistory.Text += string.Format ("\nHuman: {0}", question);

			// Clear input
			ChatInput.Text = "";

			// Get response
			var response = Eliza.ProcessInput (question);

			// Add Eliza's response to history
			ChatHistory.Text += string.Format ("\nEliza: {0}\n", response);

			// Scroll output to bottom
			var bottom = new NSRange (ChatHistory.Text.Length - 1, 1);
			ChatHistory.ScrollRangeToVisible (bottom);

			// Sending a user notification as well
			if (sendNotification) {
				var content = new UNMutableNotificationContent ();
				content.Title = "ElizaChat Question";
				content.Subtitle = question;
				content.Body = response;

				var trigger = UNTimeIntervalNotificationTrigger.CreateTrigger (5, false);

				var requestID = "ElizaQuestion";
				var request = UNNotificationRequest.FromIdentifier (requestID, content, trigger);

				UNUserNotificationCenter.Current.AddNotificationRequest (request, (err) => {
					if (err != null) {
						Console.WriteLine ("Error: {0}", err);
					}
				});
			}
		}
		#endregion

		#region Override Methods
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Register with app delegate
			ThisApp.Controller = this;

			// Do we have access to Siri?
			if (INPreferences.SiriAuthorizationStatus == INSiriAuthorizationStatus.Authorized) {
				// Yes, update Siri's vocabulary
				new Thread (() => {
					Thread.CurrentThread.IsBackground = true;
					ThisApp.AddressBook.UpdateUserSpecificVocabulary ();
				}).Start ();
			}

			// Observe keyboard events
			UIKeyboard.Notifications.ObserveWillShow ((sender, e) => {
				// Move content out of way
				UIView.BeginAnimations ("keyboard");
				UIView.SetAnimationDuration (0.3f);
				ContainerBottomConstraint.Constant = e.FrameEnd.Size.Height;
				UIView.CommitAnimations ();
			});

			UIKeyboard.Notifications.ObserveWillHide ((sender, e) => {
				// Move content back to original position
				UIView.BeginAnimations ("keyboard");
				UIView.SetAnimationDuration (0.3f);
				ContainerBottomConstraint.Constant = 0;
				UIView.CommitAnimations ();
			});

			// Wireup events
			ChatInput.ShouldBeginEditing = delegate (UITextField field)
			{
				//Placeholder
				return true;
			};

			ChatInput.ShouldReturn = delegate (UITextField field)
			{
				field.ResignFirstResponder ();
				AskQuestion (field.Text, false);
				return true;
			};

			ChatInput.ShouldEndEditing = delegate (UITextField field)
			{
				AskQuestion (field.Text, false);
				return true;
			};

		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}
		#endregion
	}
}
