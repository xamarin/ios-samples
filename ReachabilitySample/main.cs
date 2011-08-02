using System;
using System.Drawing;
using System.Net;
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.SystemConfiguration;
using MonoTouch.CoreFoundation;

namespace reachability {
	public partial class ReachabilityAppDelegate : UIApplicationDelegate {
	    UITableView tableView;
		NetworkStatus remoteHostStatus, internetStatus, localWifiStatus;
		
	    public override bool FinishedLaunching (UIApplication app, NSDictionary options)
	    {	
	        AddTable ();
	        UpdateStatus ();
	        //UpdateCarrierWarning ();
	
	        window.MakeKeyAndVisible ();
	
	        return true;
	    }
	
	    //
	    // Invoked on the main loop when reachability changes
	    //
	    void ReachabilityChanged (NetworkReachabilityFlags flags)
	    {
			UpdateStatus ();
	    }
				
	    void UpdateStatus ()
	    {
			remoteHostStatus = Reachability.RemoteHostStatus ();
			internetStatus = Reachability.InternetConnectionStatus ();
			localWifiStatus = Reachability.LocalWifiConnectionStatus ();
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
				
						Source = new DataSource (this),
	            };
	                                    
	            contentView.InsertSubviewBelow (tableView, summaryLabel);
	            contentView.BringSubviewToFront (summaryLabel);
	            tableView.ReloadData ();
	    }
    
	    public override void OnActivated (UIApplication application)
	    {
			Console.WriteLine ("OnActivated");
	    }
	
	    class DataSource : UITableViewSource {
	        ReachabilityAppDelegate parent;
        	    UIImage imageCarrier, imageWiFi, imageStop;

			public DataSource (ReachabilityAppDelegate parent)
	        {
		        imageCarrier = UIImage.FromFile ("WWAN5.png");
		        imageWiFi = UIImage.FromFile ("Airport.png");
		        imageStop = UIImage.FromFile ("stop-32.png");

                this.parent = parent;
	        }
	
	        public override NSIndexPath WillSelectRow (UITableView view, NSIndexPath index)
	        {
	                return null;
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
	            		return Reachability.HostName;
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
				string text = "";
				UIImage image = null;
	            switch (indexPath.Section){
	            case 0:
	                	    switch (parent.remoteHostStatus){
						case NetworkStatus.NotReachable:
							text = "Cannot connect to remote host";
							image = imageStop;
							break;
						case NetworkStatus.ReachableViaCarrierDataNetwork:
							text = "Reachable via data carrier network";
							image = imageCarrier;
							break;
						case NetworkStatus.ReachableViaWiFiNetwork:
							text = "Reachable via WiFi network";
							image = imageWiFi;
							break;
						}
	                    break;
	            case 1:
	                	    switch (parent.internetStatus){
						case NetworkStatus.NotReachable:
							text = "Access not available";
							image = imageStop;
							break;
						case NetworkStatus.ReachableViaCarrierDataNetwork:
							text = "Avaialble via data carrier network";
							image = imageCarrier;
							break;
						case NetworkStatus.ReachableViaWiFiNetwork:
							text = "Available via WiFi network";
							image = imageWiFi;
							break;
						}
	                    break;
	            case 2: 
	                	    switch (parent.localWifiStatus){
						case NetworkStatus.NotReachable:
							text = "Access not available";
							image = imageStop;
							break;
						case NetworkStatus.ReachableViaWiFiNetwork:
							text = "Available via WiFi network";
							image = imageWiFi;
							break;
						}
	                    break;
	            }
	            cell.TextLabel.Text = text;
				cell.ImageView.Image = image;
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
}
