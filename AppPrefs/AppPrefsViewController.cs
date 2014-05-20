using UIKit;
using System.Drawing;
using System;
using Foundation;

namespace AppPrefs
{
	public partial class AppPrefsViewController : UIViewController
	{
		NSObject observer;
		
		public AppPrefsViewController (IntPtr handle) : base (handle)
		{
		}
		
		#region View lifecycle
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			TableView.Source = new MyUITableViewSource ();
			observer = NSNotificationCenter.DefaultCenter.AddObserver ((NSString)"NSUserDefaultsDidChangeNotification", UpdateSettings);

		}

		
		public override void ViewDidUnload ()
		{
			base.ViewDidUnload ();
			
			if (observer != null) {
				NSNotificationCenter.DefaultCenter.RemoveObserver (observer);
				observer = null;
			}
		}
		
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			UpdateSettings (null);
		}
		#endregion
		
		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			// Return true for supported orientations
			return (toInterfaceOrientation != UIInterfaceOrientation.PortraitUpsideDown);
		}
		
		void UpdateSettings (NSNotification obj)
		{
			// set table view background color
			switch (Settings.BackgroundColor) {
				case BackgroundColors.Black:
					this.TableView.BackgroundColor = UIColor.Black;
					break;
				case BackgroundColors.White:
					this.TableView.BackgroundColor = UIColor.White;
					break;
				case BackgroundColors.Blue:
					this.TableView.BackgroundColor = UIColor.Blue;
					break;
				case BackgroundColors.Pattern:
					this.TableView.BackgroundColor = UIColor.GroupTableViewBackgroundColor;
					break;
			}
			
			TableView.ReloadData ();
		}
		
		#region UITableViewDataSource
		class MyUITableViewSource : UITableViewSource
		{
			public override nint NumberOfSections (UITableView tableView)
			{
				return 1;
			}
			
			public override nint RowsInSection (UITableView tableview, nint section)
			{
				return 1;
			}
			
			public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
			{
				var kCellIdentifier = new NSString ("MyIdentifier");
				var cell = tableView.DequeueReusableCell (kCellIdentifier);
				if (cell == null) {
					cell = new UITableViewCell (UITableViewCellStyle.Default, kCellIdentifier);
					cell.SelectionStyle = UITableViewCellSelectionStyle.None;
				}
				
				// Get the user settings from the app delegate.
				var firstNameStr = Settings.FirstName;
				var lastNameStr = Settings.LastName;
				cell.TextLabel.Text = firstNameStr + " " + lastNameStr;
				cell.BackgroundColor = UIView.Appearance.BackgroundColor;
				
				switch (Settings.TextColor) {
				case TextColors.Blue:
					cell.TextLabel.TextColor = UIColor.Blue;
					break;
				case TextColors.Green:
					cell.TextLabel.TextColor = UIColor.Green;
					break;
				case TextColors.Red:
					cell.TextLabel.TextColor = UIColor.Red;
					break;
				}
				return cell;
			}
		}
		#endregion

	}
}
