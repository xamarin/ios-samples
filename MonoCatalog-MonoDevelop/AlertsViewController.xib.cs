//
// Alerts sample in C#
//

using System;
using UIKit;
using Foundation;


namespace MonoCatalog {
	
	public partial class AlertsViewController : UITableViewController {
	
		// Load our definition from the NIB file
		public AlertsViewController () : base ("AlertsViewController", null)
		{
		}
	
		struct AlertSample {
			public string Title, Label, Source;
	
			public AlertSample (string t, string l, string s)
			{
				Title = t;
				Label = l;
				Source = s;
			}
		}
	
		static AlertSample [] samples;
	
		static AlertsViewController ()
		{
			samples = new AlertSample [] {
				new AlertSample ("UIActionSheet", "Show simple", "alert.cs: DialogSimpleAction ()"),
				new AlertSample ("UIActionSheet", "Show OK Cancel", "alert.cs: DialogOkCancelAction ()"),
				new AlertSample ("UIActionSheet", "Show Customized", "alert.cs: DialogOtherAction ()"),
				new AlertSample ("UIAlertView", "Show simple", "alert.cs: AlertSimpleAction ()"),
				new AlertSample ("UIAlertView", "Show OK Cancel", "alert.cs: AlertOkCancelAction ()"),
				new AlertSample ("UIAlertView", "Show Customized ", "alert.cs: AlertOtherAction ()"),
			};
		}
			
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			Title = "Alerts";
	
			TableView.DataSource = new DataSource ();
			TableView.Delegate = new TableDelegate (this);
		}
	
		void DialogSimpleAction ()
		{
			var actionSheet = new UIActionSheet ("UIActionSheet <title>", null, null, "OK", null){
				Style = UIActionSheetStyle.Default
			};
			actionSheet.Clicked += delegate (object sender, UIButtonEventArgs args){
				Console.WriteLine ("Clicked on item {0}", args.ButtonIndex);
			};
	
			actionSheet.ShowInView (View);
		}
	
		void DialogOkCancelAction ()
		{
			var actionSheet = new UIActionSheet ("UIActionSheet <title>", null, "Cancel", "OK", null){
				Style = UIActionSheetStyle.Default
			};
			actionSheet.Clicked += delegate (object sender, UIButtonEventArgs args){
				Console.WriteLine ("Clicked on item {0}", args.ButtonIndex);
			};
	
			actionSheet.ShowInView (View);
		}
	
		void DialogOtherAction ()
		{
			var actionSheet = new UIActionSheet ("UIActionSheet <title>", null, "Cancel", "OK", "Other1"){
				Style = UIActionSheetStyle.Default
			};
			actionSheet.Clicked += delegate (object sender, UIButtonEventArgs args){
				Console.WriteLine ("Clicked on item {0}", args.ButtonIndex);
			};
	
			actionSheet.ShowInView (View);
		}
		
		void AlertSimpleAction ()
		{
			using (var alert = new UIAlertView ("UIAlertView", "<Alert Message>", null, "OK", null))
				alert.Show ();
		}
	
		void AlertOkCancelAction ()
		{
			using (var alert = new UIAlertView ("UIAlertView", "<Alert Message>", null, "Cancel", "OK"))
			       alert.Show ();
		}
	
		void AlertOtherAction ()
		{
			using (var alert = new UIAlertView ("UIAlertView", "<Alert Message>", null, "Cancel", "Button1", "Button2"))
			       alert.Show ();
			
		}
	       
		#region Delegates for the table
		class DataSource : UITableViewDataSource {
			static NSString kDisplayCell_ID = new NSString ("AlertCellID");
			static NSString kSourceCell_ID = new NSString ("SourceCellID");
			
			public override nint NumberOfSections (UITableView tableView)
			{
				return samples.Length;
			}
	
			public override string TitleForHeader (UITableView tableView, nint section)
			{
				return samples [section].Title;
			}
	
			public override nint RowsInSection (UITableView tableView, nint section)
			{
				return 2;
			}
	
			public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
			{
				UITableViewCell cell;
	
				if (indexPath.Row == 0){
					cell = tableView.DequeueReusableCell (kDisplayCell_ID);
					if (cell == null)
						cell = new UITableViewCell (UITableViewCellStyle.Default, kDisplayCell_ID);
					cell.TextLabel.Text = samples [indexPath.Section].Label;
				} else {
					cell = tableView.DequeueReusableCell (kSourceCell_ID);
					if (cell == null){
						cell = new UITableViewCell (UITableViewCellStyle.Default, kSourceCell_ID){
							SelectionStyle = UITableViewCellSelectionStyle.None
						};
						var label = cell.TextLabel;
						label.Opaque = false;
						label.TextAlignment = UITextAlignment.Center;
						label.TextColor = UIColor.Gray;
						label.Lines = 2;
						label.Font = UIFont.SystemFontOfSize (12f);
					}
					cell.TextLabel.Text = samples [indexPath.Section].Source;
				}
	
				return cell;
			}
		}
	
		class TableDelegate : UITableViewDelegate {
			AlertsViewController avc;
			
			public TableDelegate (AlertsViewController avc)
			{
				this.avc = avc;
			}
				
			public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
			{
				return indexPath.Row == 0 ? 50f : 22f;
			}
	
			public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
			{
				// deselect current row
				tableView.DeselectRow (tableView.IndexPathForSelectedRow, true);
	
				if (indexPath.Row == 0){
					switch (indexPath.Section){
					case 0:
						avc.DialogSimpleAction ();
						break;
					case 1:
						avc.DialogOkCancelAction ();
						break;
						
					case 2:
						avc.DialogOtherAction ();
						break;
						
					case 3:
						avc.AlertSimpleAction ();
						break;
						
					case 4:
						avc.AlertOkCancelAction ();
						break;
						
					case 5:
						avc.AlertOtherAction ();
						break;
					}
				}
			}
		}
		#endregion
	}
}