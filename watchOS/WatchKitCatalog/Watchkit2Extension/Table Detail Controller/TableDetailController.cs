/*
 * This controller displays a table with rows. This controller demonstrates how to insert rows after the intial set of rows has been added and displayed.
*/

using System;

using WatchKit;
using Foundation;

namespace WatchkitExtension
{
	public partial class TableDetailController : WKInterfaceController
	{
		string[] cityNames;

		public TableDetailController ()
		{
		}

		public override void Awake (NSObject context)
		{
			LoadTableData ();
		}

		public override void WillActivate ()
		{
			// This method is called when the controller is about to be visible to the wearer.
			Console.WriteLine ("{0} will activate", this);
		}

		public override void DidDeactivate ()
		{
			// This method is called when the controller is no longer visible.
			Console.WriteLine ("{0} did deactivate", this);
		}

		void LoadTableData ()
		{
			cityNames = new [] { "Cupertino", "Sunnyvale", "Campbell", "Morgan Hill", "Mountain View" };

			interfaceTable.SetNumberOfRows (cityNames.Length, "default");

			for (var i = 0; i < cityNames.Length; i++) {
				var row = (TableRowController)interfaceTable.GetRowController (i);
				row.RowLabel.SetText (cityNames [i]);
			}
		}

		public override void DidSelectRow (WKInterfaceTable table, nint rowIndex)
		{
			var newCityNames = new [] { "Saratoga", "San Jose" };

			var newCityIndexes = NSIndexSet.FromNSRange (new NSRange (rowIndex + 1, newCityNames.Length));

			// Insert new rows into the table.
			interfaceTable.InsertRows (newCityIndexes, "default");

			// Update the rows that were just inserted with the appropriate data.
			var newCityNumber = 0;
			newCityIndexes.EnumerateIndexes ((nuint idx, ref bool stop) => {
				var newCityName = newCityNames [newCityNumber];
				var row = (TableRowController)interfaceTable.GetRowController ((nint) idx);
				row.RowLabel.SetText (newCityName);
				newCityNumber++;
			});
		}
	}
}