using System;

using UIKit;
using System.Collections.Generic;
using Foundation;

namespace PopulatingATable
{
	public partial class ViewController : UIViewController
	{
		UITableView table;
		TableSource source;

		public ViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			table = new UITableView (View.Bounds);
			table.AutoresizingMask = UIViewAutoresizing.All;
			CreateTableItems();
			Add (table);

//			TableViewDelegate tableDelegate = new TableViewDelegate (this);
//
//			table.Delegate = tableDelegate;

		}


		void CreateTableItems ()
		{
			var tableItems = new string[] {
				"Vegetables",
				"Fruits",
				"Flower Buds",
				"Legumes",
				"Bulbs",
				"Tubers"
			};

			source = new TableSource (tableItems);
			source.Selected += (sender, e) => {
				var alert = UIAlertController.Create ("Row Selected", e.Content, UIAlertControllerStyle.Alert);
				alert.AddAction (UIAlertAction.Create ("Ok", UIAlertActionStyle.Default, null));

				PresentViewController(alert, true, null);
			};

			table.Source = source;
		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}

//		class TableViewDelegate : NSObject, IUITableViewDelegate{
//
//			ViewController parent ;
//
//			public TableViewDelegate(ViewController parent){
//				this.parent = parent;
//			}
//
//			[Export ("tableView:didSelectRowAtIndexPath:")]
//			public void RowSelected (UITableView tableView, NSIndexPath indexPath)
//			{
//				var tableSource = (TableSource)parent.table.Source;
//				UIAlertController okAlertController = UIAlertController.Create ("Row Selected", tableSource.GetTableItem(indexPath), UIAlertControllerStyle.Alert);
//				okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
//				parent.PresentViewController (okAlertController, true, null);
//				tableView.DeselectRow (indexPath, true);
//			}
//
//		}
	}
}

