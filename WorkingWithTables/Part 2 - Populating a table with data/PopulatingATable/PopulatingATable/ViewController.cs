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
	}
}