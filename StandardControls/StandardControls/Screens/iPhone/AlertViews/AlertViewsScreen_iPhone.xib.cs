
using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Example_StandardControls.Controls;
using System.Threading;

namespace Example_StandardControls.Screens.iPhone.AlertViews
{
	public partial class AlertViewsScreen_iPhone : UIViewController
	{
		/// <summary>
		/// This is here to keep a reference to an alert after the method that creates it
		/// completes. unlike in windows, the .show() method is not blocking (with thread 
		/// magic that still keeps the UI unblocked), so after show() returns, the method
		/// will complete and the reference to the alert (and more importantly, the alert
		/// delegate will get garbage collected).
		/// </summary>
		UIAlertView alert;
		
		#region Constructors

		// The IntPtr and initWithCoder constructors are required for controllers that need 
		// to be able to be created from a xib rather than from managed code

		public AlertViewsScreen_iPhone (IntPtr handle) : base(handle)
		{
			Initialize ();
		}

		[Export("initWithCoder:")]
		public AlertViewsScreen_iPhone (NSCoder coder) : base(coder)
		{
			Initialize ();
		}

		public AlertViewsScreen_iPhone () : base("AlertViewsScreen_iPhone", null)
		{
			Initialize ();
		}

		void Initialize ()
		{
		}
		
		#endregion
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			Title = "Alert Views";
			
			btnSimpleAlert.TouchUpInside += HandleBtnSimpleAlertTouchUpInside;
			btnCustomButtons.TouchUpInside += HandleBtnCustomButtonsTouchUpInside;
			btnCustomButtonsWithDelegate.TouchUpInside += HandleBtnCustomButtonsWithDelegateTouchUpInside;
			btnCustomAlert.TouchUpInside += HandleBtnCustomAlertTouchUpInside;
		}
		
		#region -= simple alert =-

		/// <summary>
		/// Runs when the simple alert button is clicked. launches a very simple alert 
		/// that presents an "OK" button, does not use a delegate
		/// </summary>
		protected void HandleBtnSimpleAlertTouchUpInside (object sender, EventArgs e)
		{
			UIAlertView alert = new UIAlertView () { 
				Title = "alert title", Message = "this is a simple alert"
			};
			alert.AddButton("OK");
			alert.Show ();

			// can also use the constructor
			//UIAlertView alert = new UIAlertView ("alert title", "this is a simple alert.", null
			//	, "OK", null);
			//alert.Show();
		}
		
		#endregion

		#region -= custom buttons alert =-
		
		/// <summary>
		/// 
		/// </summary>
		protected void HandleBtnCustomButtonsTouchUpInside (object sender, EventArgs e)
		{
			// create an alert and add more buttons
			alert = new UIAlertView () {
				Title = "custom buttons alert", 
				Message = "this alert has custom buttons"
			};
			alert.AddButton("custom button 1");
			alert.AddButton("custom button 2");
			// last button added is the 'cancel' button (index of '2')
			alert.AddButton("OK");
			
			// or via the constructor (note, in this method, the "ok" button will have an index of '0'):
			//alert = new UIAlertView (
			//	"custom buttons alert", "this alert has custom buttons"
			//	, null, "ok", new string[] { "custom button 1", "custom button 2" });
			
			// wire up a handler for the click event
			alert.Clicked += delegate(object a, UIButtonEventArgs b) {
				Console.WriteLine ("Button " + b.ButtonIndex.ToString () + " clicked"); };
			alert.Show ();
		}
		
		#endregion

		#region -= custom buttons with delegate alert =-
		
		/// <summary>
		/// Runs when the Custom Buttons alert button is clicked. launches an alert with 
		/// additional buttons added
		/// </summary>
		protected void HandleBtnCustomButtonsWithDelegateTouchUpInside (object sender, EventArgs e)
		{
			string[] otherButtons = { "custom button 1", "custom button 2" };
			
			alert = new UIAlertView ("custom buttons alert", "this alert has custom buttons",
						 new CustomButtonsAlertDelegate (), "ok", otherButtons);
			alert.Show ();
		}
		
		/// <summary>
		/// This is our custom buttons alert delegate.
		/// </summary>
		protected class CustomButtonsAlertDelegate : UIAlertViewDelegate
		{
			public CustomButtonsAlertDelegate () : base() {	}
		
			public override void Canceled (UIAlertView alertView)
			{
				Console.WriteLine ("Alert Cancelled");
			}
		
			/// <summary>
			/// Runs when any of the custom buttons on the alert are clicked
			/// </summary>
			public override void Clicked (UIAlertView alertview, int buttonIndex)
			{
				Console.WriteLine ("Button " + buttonIndex.ToString () + " clicked");
			}
			
			/// <summary>
			/// Runs right after clicked, and before Dismissed
			/// </summary>
			public override void WillDismiss (UIAlertView alertView, int buttonIndex)
			{
				Console.WriteLine ("Alert will dismiss, button " + buttonIndex.ToString ());
			}
		
			/// <summary>
			/// Runs after Clicked
			/// </summary>
			public override void Dismissed (UIAlertView alertView, int buttonIndex)
			{
				Console.WriteLine ("Alert Dismissed, button " + buttonIndex.ToString ());
			}
		}
		
		#endregion

		#region -= custom alert =-
		
		/// <summary>
		/// runs when the custom alert button is pressed. shows the alert and then
		/// kicks off a secondary thread that spins for 5 seconds and then closes 
		/// the alert
		/// </summary>
		protected void HandleBtnCustomAlertTouchUpInside (object sender, EventArgs e)
		{
			alert = new ActivityIndicatorAlertView ();
			(alert as ActivityIndicatorAlertView).Message = "performing stuff";
			alert.Show ();
			
			Thread longRunningProc = new Thread (delegate() {
				LongRunningProcess (5);
			});
			longRunningProc.Start ();
		}
		
		/// <summary>
		/// spins a thread for the specified amount of time and then closes the 
		/// custom alert. used to simulate a long-running process
		/// </summary>
		protected void LongRunningProcess (int seconds)
		{
			Thread.Sleep (seconds * 1000);
			(alert as ActivityIndicatorAlertView).Hide (true);
		}
		
		#endregion
	}
}

