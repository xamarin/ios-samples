
using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;

namespace HandlingRotation.Screens.iPad.Method1Autosize {
	public partial class AutosizeScreenPad : UIViewController {
		List<string> tableItems = new List<string> ();

		#region Constructors

		// The IntPtr and initWithCoder constructors are required for controllers that need 
		// to be able to be created from a xib rather than from managed code

		public AutosizeScreenPad (IntPtr handle) : base(handle)
		{
			Initialize ();
		}

		[Export("initWithCoder:")]
		public AutosizeScreenPad (NSCoder coder) : base(coder)
		{
			Initialize ();
		}

		public AutosizeScreenPad () : base("AutosizeScreen", null)
		{
			Initialize ();
		}

		void Initialize ()
		{
		}

		#endregion

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			Title = "Autosizing Controls";
			
			CreateData ();
			
			tblMain.WeakDataSource = this;
		}

		/// <summary>
		/// When the device rotates, the OS calls this method to determine if it should try and rotate the
		/// application and then call WillAnimateRotation
		/// </summary>
		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			// we're passed to orientation that it will rotate to. in our case, we could
			// just return true, but this switch illustrates how you can test for the 
			// different cases
			switch (toInterfaceOrientation)
			{
				case UIInterfaceOrientation.LandscapeLeft:
				case UIInterfaceOrientation.LandscapeRight:
				case UIInterfaceOrientation.Portrait:
				case UIInterfaceOrientation.PortraitUpsideDown:
				default:
					return true;
			}
		}

		protected void CreateData ()
		{
			tableItems.Add ("Radiohead");
			tableItems.Add ("Death Cab for Cutie");
			tableItems.Add ("Earlimart");
			tableItems.Add ("Grandaddy");
			tableItems.Add ("Wonderful");
			tableItems.Add ("Sufjan Stevens");
			tableItems.Add ("Johnny Cash");
			tableItems.Add ("New Order");
			tableItems.Add ("Elliott Smith");
			tableItems.Add ("Elbow");
		}

		[Export("tableView:numberOfRowsInSection:")]
		public int RowsInSection (UITableView tableView, int section)
		{
			Console.WriteLine ("RowsInSection");
			return tableItems.Count;
		}

		[Export("tableView:cellForRowAtIndexPath:")]
		public UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			Console.WriteLine ("GetCell");
			
			// declare vars
			string cellIdentifier = "SimpleCellTemplate";
			
			// try to grab a cell object from the internal queue
			var cell = tableView.DequeueReusableCell (cellIdentifier);
			// if there wasn't any available, just create a new one
			if (cell == null) {
				cell = new UITableViewCell (UITableViewCellStyle.Default, cellIdentifier);
			}
			
			// set the cell properties
			cell.TextLabel.Text = this.tableItems[(int)indexPath.Row];
			
			// return the cell
			return cell;
		}
	}
}