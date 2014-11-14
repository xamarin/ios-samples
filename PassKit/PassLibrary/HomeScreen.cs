using System;
using CoreGraphics;
using System.Collections.Generic;
using UIKit;
using Foundation;
using System.IO;
using PassKit;

/*
 NOTE: Be sure to check the Entitlements.plist - you must enter your TeamID with the 
 PassTypeIDs that you have created on the iOS Provisioning Portal, and ensure that your
 Provisioning Profiles for testing/deploying this app have PassKit enabled.
 */

namespace PassLibrary {

	/// <summary>
	/// Lists the passes associated with our Team ID.
	/// Has buttons to 'test' adding and updating a card.
	/// </summary>
	public class HomeScreen : UIViewController {
		UITableView table;
		UILabel passLibraryAvailableLabel;
		UIButton addPassButton, replacePassButton, refreshButton;

		PKPassLibrary library;

		NSObject noteCenter;

		public HomeScreen ()
		{	
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			View.BackgroundColor = UIColor.White;

			table = new UITableView (new CGRect (0, 110, View.Bounds.Width, View.Bounds.Height - 120));
			table.AutoresizingMask = UIViewAutoresizing.All;

			refreshButton = UIButton.FromType (UIButtonType.RoundedRect);
			refreshButton.SetTitle ("Refresh", UIControlState.Normal);
			refreshButton.TouchUpInside += HandleRefreshTouchUpInside;

			addPassButton = UIButton.FromType (UIButtonType.RoundedRect);

			addPassButton.SetTitle ("Add", UIControlState.Normal);
			addPassButton.TouchUpInside += HandleAddTouchUpInside;

			replacePassButton = UIButton.FromType (UIButtonType.RoundedRect);

			replacePassButton.SetTitle ("Replace", UIControlState.Normal);
			replacePassButton.TouchUpInside += HandleReplaceTouchUpInside;

			passLibraryAvailableLabel = new UILabel ();
			passLibraryAvailableLabel.Text = "Pass Library Available: " + PKPassLibrary.IsAvailable.ToString ();

			if (PKPassLibrary.IsAvailable) {
				library = new PKPassLibrary ();
				Console.WriteLine ("library.GetPasses");
				var passes = library.GetPasses ();
				table.Source = new TableSource (passes, library);

				// Notification for changes to the library!
				noteCenter = NSNotificationCenter.DefaultCenter.AddObserver (PKPassLibrary.DidChangeNotification, (not) => {
					BeginInvokeOnMainThread (() => {
						new UIAlertView("Pass Library Changed"
						                , "Notification Received", null, "OK", null).Show();
						// refresh the list
						var passlist = library.GetPasses ();
						table.Source = new TableSource (passlist, library);
						table.ReloadData ();
					});
				}, library);  // IMPORTANT: must pass the library in 
			} else {
				Console.WriteLine ("No Pass Kit - must be an iPad");
				addPassButton.SetTitleColor (UIColor.LightGray, UIControlState.Disabled);
				addPassButton.Enabled = false;
			}

			if (UIDevice.CurrentDevice.CheckSystemVersion (7, 0)) {
				refreshButton.Frame = new CGRect (230, 114, 80, 40);
				addPassButton.Frame = new CGRect (10, 114, 80, 40);
				replacePassButton.Frame = new CGRect (100, 114, 80, 40);
				passLibraryAvailableLabel.Frame = new CGRect (10, 69, 300, 40);
			} else {
				refreshButton.Frame = new CGRect (230, 50, 80, 40);
				addPassButton.Frame = new CGRect (10, 50, 80, 40);
				replacePassButton.Frame = new CGRect (100, 50, 80, 40);
				passLibraryAvailableLabel.Frame = new CGRect (10, 5, 300, 40);
			}

			Add (table);
			Add (passLibraryAvailableLabel);
			Add (refreshButton);
			Add (addPassButton);
			Add (replacePassButton);
		}

		/// <summary>
		/// Proves that they return the passes in non-deterministic order,
		/// and that developers should sort them manually
		/// </summary>
		void HandleRefreshTouchUpInside (object sender, EventArgs e)
		{
			Console.WriteLine ("HandleRefreshTouchUpInside");
			var passes = library.GetPasses ();
			table.Source = new TableSource (passes, library);
			table.ReloadData ();
		}
		void HandleAddTouchUpInside (object sender, EventArgs e)
		{
			if (PKPassLibrary.IsAvailable) {

				var documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.Personal); // Documents folder
				var newFilePath = Path.Combine (documentsPath, "CouponBanana2.pkpass");
				var builtInPassPath = Path.Combine (System.Environment.CurrentDirectory, "CouponBanana2.pkpass");
				if (!System.IO.File.Exists(newFilePath))
					System.IO.File.Copy (builtInPassPath, newFilePath);

				NSData nsdata;
				using ( FileStream oStream = File.Open (newFilePath, FileMode.Open ) ) {
					nsdata = NSData.FromStream ( oStream );
				}

				var err = new NSError(new NSString("42"), -42);
				var newPass = new PKPass(nsdata,out err);

				bool alreadyExists = library.Contains (newPass);

				if (alreadyExists) {
					new UIAlertView(newPass.LocalizedDescription + " Tapped"
					                , "Already exists", null, "OK", null).Show();
				} else {
//					new UIAlertView(newPass.LocalizedDescription + " Tapped"
//					                , "Isn't in Pass Library", null, "OK, add it", null).Show();

					var pkapvc = new PKAddPassesViewController(newPass);
					NavigationController.PresentModalViewController (pkapvc, true);
				}
			}
		}

		/// <summary>
		/// PKLibrary.Replace() doesn't require user to agreee (no UI)
		/// BUT only works if the pass already exists
		/// </summary>
		void HandleReplaceTouchUpInside (object sender, EventArgs e)
		{
			if (PKPassLibrary.IsAvailable) {
			
				var documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.Personal); // Documents folder
				var newFilePath = Path.Combine (documentsPath, "CouponBanana2.pkpass");
				var builtInPassPath = Path.Combine (System.Environment.CurrentDirectory, "CouponBanana2.pkpass");
				if (!System.IO.File.Exists(newFilePath))
					System.IO.File.Copy (builtInPassPath, newFilePath);
				
				NSData nsdata;
				using ( FileStream oStream = File.Open (newFilePath, FileMode.Open ) ) {
					nsdata = NSData.FromStream ( oStream );
				}
				
				var err = new NSError(new NSString("42"), -42);
				var newPass = new PKPass(nsdata,out err);


				bool alreadyExists = library.Contains (newPass);

				if (alreadyExists) {
					library.Replace (newPass);
					new UIAlertView(newPass.LocalizedDescription + " replaced!"
					                , "your choice if you offer UI when you update", null, "OK", null).Show();
				} else
					new UIAlertView(newPass.LocalizedDescription + " doesn't exit"
					                , "Can't *replace* if the pass isn't already in library", null, "OK", null).Show();
			}
		}
	}
}