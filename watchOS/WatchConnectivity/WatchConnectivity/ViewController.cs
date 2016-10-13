using System;
using System.Collections.Generic;
using Foundation;
using UIKit;

namespace WatchConnectivity
{
	public partial class ViewController : UIViewController
	{
		protected ViewController(IntPtr handle) : base(handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			// Perform any additional setup after loading the view, typically from a nib.
			WCSessionManager.SharedManager.ApplicationContextUpdated += DidReceiveApplicationContext;

			tableView.Source = new TableSource(new string[] {"🐶", "🐱", "🐼", "🐯", "🦁", "🐷", "🐻", "🐰", "🐨", "🐸", "🐙", "🐵"});
			tableView.Delegate = new TableDelegate(this);
		}

		public override void ViewDidUnload()
		{
			base.ViewDidUnload();
			WCSessionManager.SharedManager.ApplicationContextUpdated -= DidReceiveApplicationContext;
		}

        public void DidReceiveApplicationContext(WCSession session, Dictionary<string, object> applicationContext)
		{
			var message = (string)applicationContext["MessageWatch"];
			if (message != null)
			{
				Console.WriteLine($"Application context update received : {message}");
				InvokeOnMainThread(() =>
				{
					label.Text = $"⌚️ : {message}";
				});
			}
		}

		public void SendEmoji(string emoji)
		{
			WCSessionManager.SharedManager.UpdateApplicationContext(new Dictionary<string, object>() { { "MessagePhone", $"{emoji}" } });
		}
	}

	public class TableSource : UITableViewSource
	{

		string[] TableItems;

		public TableSource(string[] items)
		{
			TableItems = items;
		}

		public override nint RowsInSection(UITableView tableview, nint section)
		{
			return TableItems.Length;
		}

		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell("CommandCell", indexPath) as CommandTableCell;
			cell.label.Text = TableItems[indexPath.Row];
			return cell;
		}
	}

	public class TableDelegate : UITableViewDelegate
	{
		#region Private Variables
		private ViewController Controller;
		#endregion

		#region Constructors
		public TableDelegate()
		{
		}

		public TableDelegate(ViewController controller)
		{
			// Initialize
			this.Controller = controller;
		}
		#endregion

		#region Override Methods
		public override nfloat EstimatedHeight(UITableView tableView, Foundation.NSIndexPath indexPath)
		{
			return 44f;
		}

		public override void RowSelected(UITableView tableView, Foundation.NSIndexPath indexPath)
		{
			var cell = tableView.CellAt(indexPath) as CommandTableCell;
			Controller.SendEmoji(cell.label.Text);
		}
		#endregion
	}
}
