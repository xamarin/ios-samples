using System;
using System.Drawing;
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.SystemConfiguration;
using MonoTouch.CoreFoundation;

public partial class ReachabilityAppDelegate : UIApplicationDelegate {
	public string Hostname = "www.apple.com";
	UIImage imageCarrier, imageWiFi, imageStop;
	NetworkReachability nr;
	UITableView tableView;
	
	public override bool FinishedLaunching (UIApplication app, NSDictionary options)
	{
		imageCarrier = UIImage.FromFile ("WWAN5.png");
		imageWiFi = UIImage.FromFile ("Airport.png");
		imageStop = UIImage.FromFile ("stop-32.png");

		nr = new NetworkReachability ("www.apple.com");
		nr.SetCallback (ReachabilityChanged);
		nr.Schedule (CFRunLoop.Current, CFRunLoop.ModeDefault);
			
		AddTable ();
		//UpdateStatus ();
		//UpdateCarrierWarning ();

		window.MakeKeyAndVisible ();

		return true;
	}

	//
	// Invoked on the main loop when reachability changes
	//
	void ReachabilityChanged (NetworkReachabilityFlags flags)
	{
	}
	
	void AddTable ()
	{
		RectangleF tableFrame = UIScreen.MainScreen.ApplicationFrame;
		
		tableView = new UITableView (tableFrame, UITableViewStyle.Grouped) {
			AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight,
			RowHeight = 44.0f,
			SeparatorStyle = UITableViewCellSeparatorStyle.None,
			SectionHeaderHeight = 28.0f,
			ScrollEnabled = false,
			Delegate = new TableViewDelegate (),
			DataSource = new DataSource (this)
		};
					
		contentView.InsertSubviewBelow (tableView, summaryLabel);
		contentView.BringSubviewToFront (summaryLabel);
		tableView.ReloadData ();
	}
	
	void ReachabilitChanged (NSNotification note)
	{
		Console.WriteLine ("reachability changed");
	}

	void UpdateStatus ()
	{
	}
	
	public override void OnActivated (UIApplication application)
	{
		Console.WriteLine ("OnActivated");
	}

	class TableViewDelegate : UITableViewDelegate {
		public override NSIndexPath WillSelectRow (UITableView view, NSIndexPath index)
		{
			return null;
		}
	}

	class DataSource : UITableViewDataSource {
		ReachabilityAppDelegate parent;
		
		public DataSource (ReachabilityAppDelegate parent)
		{
			this.parent = parent;
		}

		public override int RowsInSection (UITableView view, int section)
		{
			return 1;
		}

		public override int NumberOfSections (UITableView view)
		{
			return 3;
		}

		public override string TitleForHeader (UITableView view, int section)
		{
			switch (section){
			case 0:
				return parent.Hostname;
			case 1:
				return "Access to internet hosts";
			case 2:
				return "Access to Local Bonjour Hosts";
			default:
				return "Unknown";
			}
		}

		static NSString ReachabilityTableCellIdentifier = new NSString ("ReachabilityTableCell");
		
		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell (ReachabilityTableCellIdentifier);
			if (cell == null) {
				cell =  new UITableViewCell (UITableViewCellStyle.Default, ReachabilityTableCellIdentifier);
				var label = cell.TextLabel;
				label.Font = UIFont.SystemFontOfSize (12f);
				label.TextColor = UIColor.DarkGray;
				label.TextAlignment = UITextAlignment.Left;
			}
			
			var row = indexPath.Row;
			switch (indexPath.Section){
			case 0:
				cell.TextLabel.Text = "First";
				break;
			case 1:
				cell.TextLabel.Text = "First";
				break;
			case 2:	
				cell.TextLabel.Text = "First";
				break;
			}
			
			return cell;
		}
	}
}

class Demo {
	static void Main (string [] args)
	{
		UIApplication.Main (args, null, null);
	}		
}

