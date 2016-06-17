/* 
 * This is the initial interface controller for the WatchKit app. 
 * It loads the initial table of the app with data and responds to Handoff launching the WatchKit app.
*/

using System;
using System.Collections.Generic;
using System.IO;

using WatchKit;
using Foundation;

using Newtonsoft.Json;

namespace WatchkitExtension
{
	public partial class InterfaceController : WKInterfaceController
	{
		readonly List<Dictionary<string, string>> elementsList;

		public InterfaceController (IntPtr handle) : base (handle)
		{
			var appData = NSBundle.MainBundle.PathForResource ("AppData", "json");
			elementsList = JsonConvert.DeserializeObject<List<Dictionary<string, string>>> (File.ReadAllText (appData));
		}

		public override void Awake (NSObject context)
		{
			LoadTableRows ();
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

		public override void HandleUserActivity (NSDictionary userActivity)
		{
			PushController (userActivity ["controllerName"].ToString (), userActivity ["detailInfo"]);
		}

		public override void DidSelectRow (WKInterfaceTable table, nint rowIndex)
		{
			var rowData = elementsList [(int)rowIndex];

			PushController (rowData ["controllerIdentifier"], (NSObject) null);
		}

		void LoadTableRows ()
		{
			interfaceTable.SetNumberOfRows ((nint)elementsList.Count, "default");

			// Create all of the table rows.
			for (var i = 0; i < elementsList.Count; i++) {
				var elementRow = (ElementRowController)interfaceTable.GetRowController (i);

				var rowData = elementsList [i];
				elementRow.ElementLabel.SetText (rowData ["label"]);
			}
		}
	}
}

