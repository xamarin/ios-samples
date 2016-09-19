using System;

using WatchKit;
using Foundation;
using System.Collections.Generic;

namespace WatchTables.OnWatchExtension
{
	public partial class InterfaceController : WKInterfaceController
	{
		List<string> rows = new List<string>();

		protected InterfaceController(IntPtr handle) : base(handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}

		public override void Awake(NSObject context)
		{
			base.Awake(context);

			// Configure interface objects here.
			Console.WriteLine("{0} awake with context", this);

			rows.Add("row1");
			rows.Add("row2");
			rows.Add("row3");
			rows.Add("row4");
			rows.Add("row5");
		}

		public override void WillActivate()
		{
			// This method is called when the watch view controller is about to be visible to the user.
			Console.WriteLine("{0} will activate", this);

			LoadTableRows();
		}

		public override void DidDeactivate()
		{
			// This method is called when the watch view controller is no longer visible to the user.
			Console.WriteLine("{0} did deactivate", this);
		}

		public override NSObject GetContextForSegue(string segueIdentifier, WKInterfaceTable table, nint rowIndex)
		{
			// retrieved in DetailController.Awake()
			return new NSString(rows[(int)rowIndex]);
		}

		void LoadTableRows()
		{
			MyTable.SetNumberOfRows((nint)rows.Count, "default");
			//MyTable.SetRowTypes (new [] {"default", "type1", "type2", "default", "default"});
			// Create all of the table rows.
			for (var i = 0; i < rows.Count; i++)
			{
				var elementRow = (RowController)MyTable.GetRowController(i);

				elementRow.MyLabel.SetText(rows[i]);
			}
		}

		public override void DidSelectRow(WKInterfaceTable table, nint rowIndex)
		{
			var rowData = rows[(int)rowIndex];
			Console.WriteLine("Row selected:" + rowData);
		}
	}
}
