using UIKit;
using System.Drawing;
using System;
using System.Collections.Generic;
using Foundation;
using CoreTelephony;

namespace CoreTelephonyDemo
{
	public partial class CoreTelephonyDemoViewController : UITableViewController
	{
		CTTelephonyNetworkInfo networkInfo;
		CTCallCenter callCenter;
		string carrierName;
		CTCall[] calls = new CTCall [0];
		
		public CoreTelephonyDemoViewController ()
			: base ("CoreTelephonyDemoViewController", null)
		{
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			this.Title = "Core Telephony Info";
			this.TableView.AllowsSelection = false;
			this.TableView.DataSource = new TableViewDataSource (this);
			
			networkInfo = new CTTelephonyNetworkInfo ();
			callCenter = new CTCallCenter ();
			callCenter.CallEventHandler += CallEvent;
			carrierName = networkInfo.SubscriberCellularProvider == null ? null : networkInfo.SubscriberCellularProvider.CarrierName;
			networkInfo.CellularProviderUpdatedEventHandler += ProviderUpdatedEvent;
		}
		
		private void ProviderUpdatedEvent (CTCarrier carrier)
		{
			CoreFoundation.DispatchQueue.MainQueue.DispatchSync (() =>
			{
				carrierName = carrier == null ? null : carrier.CarrierName;
				TableView.ReloadData ();
			});
		}
		
		private void CallEvent (CTCall inCTCall)
		{
			CoreFoundation.DispatchQueue.MainQueue.DispatchSync (() =>
			{
				NSSet calls = callCenter.CurrentCalls;
				calls = callCenter.CurrentCalls;
				if (calls == null) {
					this.calls = new CTCall [0];
				} else {
					this.calls = calls.ToArray<CTCall> ();
				}
				Array.Sort (this.calls, (CTCall a, CTCall b) =>
				{
					return string.Compare (a.CallID, b.CallID);
				});
				TableView.ReloadData ();
			});
		}
		
		public override void ViewDidUnload ()
		{
			base.ViewDidUnload ();
			
			// Release any retained subviews of the main view.
			// e.g. myOutlet = null;
			networkInfo.CellularProviderUpdatedEventHandler -= ProviderUpdatedEvent;
			callCenter.CallEventHandler -= CallEvent;
		}
		
		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			return true;
		}
		
		enum SectionIndex {
			CurrentCall = 0,
			CallCenter,
			Carrier,
			Count,
		}
		
		enum SectionRow {
			CurrentCall = 1,
			CallCenter = 1,
			Carrier = 1,
			Count,
		}
		
		class TableViewDataSource : UITableViewDataSource {
			CoreTelephonyDemoViewController controller;
			List<UITableViewCell> table_cells = new List<UITableViewCell> ();
			
			public TableViewDataSource (CoreTelephonyDemoViewController controller)
			{
				this.controller = controller;
			}
			
			#region implemented abstract members of UIKit.UITableViewDataSource
			public override nint RowsInSection (UITableView tableView, nint section)
			{
				switch ((SectionIndex) (int)section) {
				case SectionIndex.CurrentCall:
					return Math.Max (controller.calls.Length, 1);
				case SectionIndex.CallCenter:
					return (int) SectionRow.CallCenter;
				case SectionIndex.Carrier:
					return (int) SectionRow.Carrier;
				default:
					return 1;
				}
			}
			
			string GetCallState (string callState)
			{
				switch (callState) {
				case "CTCallStateDialing": 
					return "Dialing";
				case "CTCallStateIncoming": 
					return "Incoming";
				case "CTCallStateConnected":
					return "Connected";
				case "CTCallStateDisconnected": 
					return "Disconnected";
				default:
					return callState;
				}
			}
			
			public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
			{
				UITableViewCell cell;
				string cellText = string.Empty;
				
				cell = tableView.DequeueReusableCell ("Cell");
				if (cell == null) {
					cell = new UITableViewCell (UITableViewCellStyle.Default, "Cell");
					table_cells.Add (cell);
				}
				
				switch ((SectionIndex) (int)indexPath.Section) {
				case SectionIndex.CurrentCall:
					if (controller.calls.Length > 0) {
						cellText = string.Format ("Call {0}: {1}", indexPath.Row + 1, GetCallState (controller.calls [indexPath.Row].CallState));
					} else {
						cellText = "No calls";
					}
					break;
				case SectionIndex.CallCenter:
					if (controller.calls.Length == 0) {
						cellText = "No calls";
					} else if (controller.calls.Length == 1) {
						cellText = "1 call at Call Center";
					} else {
						cellText = string.Format ("{0} calls at Call Center", controller.calls.Length);
					}
					break;
				case SectionIndex.Carrier:
					if (!string.IsNullOrEmpty (controller.carrierName)) {
						cellText = controller.carrierName;
					} else {
						cellText = "Unknown";
					}
					break;
				}
				
				cell.TextLabel.Text = cellText;
				return cell;
			}
			#endregion
			
			public override nint NumberOfSections (UITableView tableView)
			{
				return (int)SectionIndex.Count;
			}
			
			public override string TitleForHeader (UITableView tableView, nint section)
			{
				switch ((SectionIndex) (int) section) {
				case SectionIndex.CurrentCall: 
					return "Current call";
				case SectionIndex.CallCenter: 
					return "Call center";
				case SectionIndex.Carrier:
					return "Carrier";
				default: 
					return null;
				}
			}
		}
	}
}
