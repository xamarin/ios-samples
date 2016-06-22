using System;

using UIKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreFoundation;
using System.Collections.Generic;
using System.Linq;

namespace CloudKitAtlas
{
	public partial class ResultsViewController : ResultOrErrorViewController, IUITableViewDelegate, IUITableViewDataSource, IUIScrollViewDelegate
	{
		[Outlet]
		public TableView TableView { get; set; }

		[Outlet]
		public NSLayoutConstraint ToolbarHeightConstraint { get; set; }

		[Outlet]
		public UIToolbar Toolbar { get; set; }

		public CodeSample CodeSample { get; set; }
		public Results Results { get; set; } = new Results (); // TODO: could be get only (readonly)?

		string selectedAttributeValue;
		readonly UIActivityIndicatorView activityIndicator = new UIActivityIndicatorView (new CGRect (0, 0, 20, 20));

		public override bool CanBecomeFirstResponder {
			get {
				return true;
			}
		}

		public ResultsViewController (IntPtr handle)
			: base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			if (Results.Items.Count == 1) {
				var result = Results.Items [0];
				NavigationItem.Title = result.SummaryField ?? "Result";
			} else {
				NavigationItem.Title = "Result";
			}

			activityIndicator.HidesWhenStopped = true;
			ToggleToolbar ();
		}

		public override void ViewDidAppear (bool animated)
		{
			var codeSample = CodeSample as MarkNotificationsReadSample;
			codeSample?.Cache?.MarkAsRead ();
		}

		void ToggleToolbar ()
		{
			if (ToolbarHeightConstraint == null)
				return;

			if (Results.MoreComing) {
				Toolbar.Hidden = false;
				ToolbarHeightConstraint.Constant = 44;
			} else {
				Toolbar.Hidden = true;
				ToolbarHeightConstraint.Constant = 0;
			}
		}

		#region Table view data source

		[Export ("numberOfSectionsInTableView:")]
		public nint NumberOfSections (UITableView tableView)
		{
			bool showAsList = (Results.Items.Count == 0 || Results.ShowAsList);
			return showAsList ? 1 : Results.Items [0].AttributeList.Count;
		}

		public nint RowsInSection (UITableView tableView, nint section)
		{
			return (Results.Items.Count == 0 || Results.ShowAsList)
					? Results.Items.Count
					: Results.Items [0].AttributeList [(int)section].Attributes.Count;
		}

		public UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			if (Results.ShowAsList) {
				var resultCell = (ResultTableViewCell)tableView.DequeueReusableCell ("ResultCell", indexPath);
				var result = Results.Items [indexPath.Row];
				resultCell.ResultLabel.Text = result.SummaryField ?? string.Empty;
				resultCell.ChangeLabelWidthConstraint.Constant = 15;

				if (Results.Added.Contains (indexPath.Row))
					resultCell.ChangeLabel.Text = "A";
				else if (Results.Deleted.Contains (indexPath.Row))
					resultCell.ChangeLabel.Text = "D";
				else if (Results.Modified.Contains (indexPath.Row))
					resultCell.ChangeLabel.Text = "M";
				else
					resultCell.ChangeLabelWidthConstraint.Constant = 0;
				return resultCell;
			}

			var attribute = Results.Items [0].AttributeList [indexPath.Section].Attributes [indexPath.Row];
			var value = attribute.Value;
			if (value == null) {
				var attribCell = (AttributeKeyTableViewCell)tableView.DequeueReusableCell ("AttributeKeyCell", indexPath);
				attribCell.AttributeKey.Text = attribute.Key;
				return attribCell;
			}

			if (attribute.Image != null) {
				var imgCell = (ImageTableViewCell)tableView.DequeueReusableCell ("ImageCell", indexPath);
				imgCell.AttributeKey.Text = attribute.Key;
				imgCell.AttributeValue.Text = string.IsNullOrWhiteSpace (value) ? "-" : value;
				imgCell.AssetImage.Image = attribute.Image;
				return imgCell;
			}

			var cellIdentifier = attribute.IsNested ? "NestedAttributeCell" : "AttributeCell";
			var cell = (AttributeTableViewCell)tableView.DequeueReusableCell (cellIdentifier, indexPath);
			cell.AttributeKey.Text = attribute.Key;
			cell.AttributeValue.Text = string.IsNullOrWhiteSpace (value) ? "-" : value;

			return cell;
		}

		[Export ("tableView:titleForHeaderInSection:")]
		public string TitleForHeader (UITableView tableView, nint section)
		{
			var codeSample = CodeSample;
			if (codeSample == null)
				return string.Empty;

			if (Results.ShowAsList)
				return codeSample.ListHeading;

			var result = Results.Items [0];
			return result.AttributeList [(int)section].Title;
		}

		[Export ("tableView:heightForRowAtIndexPath:")]
		public nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
		{
			if (Results.Items.Count > 0 && !Results.ShowAsList) {
				var attribute = Results.Items [0].AttributeList [indexPath.Section].Attributes [indexPath.Row];
				if (attribute.Image != null)
					return 200;
			}
			return tableView.RowHeight;

		}

		#endregion

		#region Responder

		public override bool BecomeFirstResponder ()
		{
			return base.BecomeFirstResponder ();
		}

		public override bool CanPerform (Selector action, NSObject withSender)
		{
			return action.Name == "copyAttributeToClipboard";
		}

		#endregion

		#region Actions

		[Action ("handleLongPress:")]
		void HandleLongPress (UILongPressGestureRecognizer sender)
		{
			if (sender.State != UIGestureRecognizerState.Ended)
				return;

			var point = sender.LocationInView (TableView);
			var indexPath = TableView.IndexPathForRowAtPoint (point);

			var attributeCell = TableView.CellAt (indexPath) as AttributeTableViewCell;
			var attributeValue = attributeCell?.AttributeValue;
			if (attributeValue == null)
				return;

			BecomeFirstResponder ();
			selectedAttributeValue = attributeValue.Text ?? string.Empty;

			var menuController = UIMenuController.SharedMenuController;
			menuController.SetTargetRect (attributeValue.Frame, attributeCell);
			menuController.MenuItems = new UIMenuItem [] {
				new UIMenuItem ("Copy attribute value", new Selector ("copyAttributeToClipboard:"))
			};
			menuController.SetMenuVisible (true, true);
		}


		[Action ("copyAttributeToClipboard")]
		void CopyAttributeToClipboard ()
		{
			var value = selectedAttributeValue;
			if (value != null) {
				var pasteBoard = UIPasteboard.General;
				pasteBoard.String = value;
			}
		}

		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			if (segue.Identifier != "DrillDown")
				return;

			var resultsViewController = segue.DestinationViewController as ResultsViewController;
			if (resultsViewController == null)
				return;

			var indexPath = TableView.IndexPathForSelectedRow;
			if (indexPath == null)
				return;

			var result = Results.Items [indexPath.Row];
			resultsViewController.Results = new Results (new IResult [] { result });
			resultsViewController.CodeSample = CodeSample;
			resultsViewController.IsDrilldown = true;
		
			TableView.DeselectRow (indexPath, false);
		}

		[Action ("loadMoreResults:")]
		public async void loadMoreResults (UIBarButtonItem sender)
		{
			sender.Enabled = false;
			NavigationItem.RightBarButtonItem = new UIBarButtonItem (activityIndicator);
			activityIndicator.StartAnimating ();

			var codeSample = CodeSample;
			if (codeSample != null) {
				try {
					var results = await codeSample.Run ();
					Results = results;
					DispatchQueue.MainQueue.DispatchAsync (() => {
						var indexPaths = new List<NSIndexPath> ();

						foreach (var index in results.Added.OrderBy (i => i))
							indexPaths.Add (NSIndexPath.FromRowSection (index, 0));
						if (indexPaths.Count > 0)
							TableView.InsertRows (indexPaths.ToArray (), UITableViewRowAnimation.Automatic);

						indexPaths.Clear ();

						foreach (var index in results.Deleted.Union (results.Modified).OrderBy (i => i))
							indexPaths.Add (NSIndexPath.FromRowSection (index, 0));
						if (indexPaths.Count > 0)
							TableView.ReloadRows (indexPaths.ToArray (), UITableViewRowAnimation.Automatic);
						NavigationItem.RightBarButtonItem = DoneButton;

						activityIndicator.StopAnimating ();

						if (Results.MoreComing) {
							sender.Enabled = true;
						} else {
							UIView.Animate (0.4, () => {
								ToggleToolbar ();
								View.LayoutIfNeeded ();
							});
						}
					});
				} catch (Exception ex) {
					Console.WriteLine (ex);
					throw ex;
				}
			}
		}


		#endregion
	}
}
