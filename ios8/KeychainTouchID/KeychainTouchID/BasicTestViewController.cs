using System;
using System.Collections.Generic;
using CoreFoundation;
using Foundation;
using UIKit;

namespace KeychainTouchID
{
	[Register ("BasicTestViewController")]
	public class BasicTestViewController : UIViewController, IUITableViewDelegate, IUITableViewDataSource
	{
		public List<Test> Tests { get; set; }

		public BasicTestViewController (IntPtr handle) : base (handle)
		{
		}

		public void PrintResult (UITextView textView, string message)
		{
			DispatchQueue.MainQueue.DispatchAsync (() => {
				textView.Text = string.Format ("{0}\n{1}", textView.Text, message);
				textView.ScrollRangeToVisible (new NSRange (0, textView.Text.Length));
			});
		}

		[Export ("numberOfSectionsInTableView:")]
		public int NumberOfSections (UITableView tableView)
		{
			return 1;
		}

		[Export ("tableView:numberOfRowsInSection:")]
		public nint RowsInSection (UITableView tableview, nint section)
		{
			return (nint)Tests.Count;
		}

		[Export ("tableView:titleForHeaderInSection:")]
		public string TitleForHeader (UITableView tableView, int section)
		{
			return Text.SELECT_TEST;
		}

		[Export ("tableView:didSelectRowAtIndexPath:")]
		public void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			var test = GetTestForIndexPath (indexPath);

			if (test.Method != null)
				test.Method ();

			tableView.DeselectRow (indexPath, true);
		}

		[Export ("tableView:cellForRowAtIndexPath:")]
		public UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			string cellIdentifier = Text.TEST_CELL;

			var cell = tableView.DequeueReusableCell (cellIdentifier);
			cell = cell ?? new UITableViewCell (UITableViewCellStyle.Subtitle, cellIdentifier);

			var test = GetTestForIndexPath (indexPath);
			cell.TextLabel.Text = test.Name;
			cell.DetailTextLabel.Text = test.Details;

			return cell;
		}

		Test GetTestForIndexPath (NSIndexPath indexPath)
		{
			if (indexPath.Section > 0 || indexPath.Row >= Tests.Count)
				return null;

			return Tests [indexPath.Row];
		}
	}
}

