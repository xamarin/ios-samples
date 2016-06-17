using System;
using System.Collections.Generic;

using Foundation;
using HomeKit;
using UIKit;

namespace HomeKitCatalog
{
	partial class HomeListViewController : HMCatalogViewController, IHMHomeManagerDelegate
	{
		public static readonly NSString HomeCell = (NSString)"HomeCell";
		public static readonly NSString ShowHomeSegue = (NSString)"Show Home";

		readonly List<HMHome> homes = new List<HMHome>();
		public List<HMHome> Homes {
			get {
				return homes;
			}
		}

		protected HMHomeManager HomeManager {
			get {
				return HomeStore.HomeManager;
			}
		}

		[Export ("initWithCoder:")]
		public HomeListViewController (NSCoder coder)
			: base (coder)
		{
		}

		public HomeListViewController (IntPtr handle)
			: base (handle)
		{
		}

		public HomeListViewController ()
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			TableView.EstimatedRowHeight = 44;
			TableView.RowHeight = UITableView.AutomaticDimension;
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			ResetHomeList ();
		}

		protected override void RegisterAsDelegate ()
		{
			HomeManager.Delegate = this;
			foreach (var home in Homes)
				home.Delegate = this;
		}

		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			base.PrepareForSegue (segue, sender);

			if (segue.Identifier == ShowHomeSegue) {
				if (sender == this)
					return;
				var indexPath = TableView.IndexPathForCell ((UITableViewCell)sender);
				HomeStore.Home = Homes [indexPath.Row];
			}
		}

		#region Table View Methods

		public override nint RowsInSection (UITableView tableView, nint section)
		{
			var rows = Homes.Count;
			TableView.SetBackgroundMessage(rows == 0 ? "No Homes": null);
			return rows;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell (HomeCell, indexPath);
			var home = Homes [(int)indexPath.Row];

			cell.TextLabel.Text = home.Name;

			var detailTextLabel = cell.DetailTextLabel;
			if (detailTextLabel != null)
				detailTextLabel.Text = SharedTextForHome (home);

			return cell;
		}

		#endregion

		#region Helper Methods

		int CompareHomes (HMHome x, HMHome y)
		{
			if (x.IsAdmin () == y.IsAdmin ())
				return x.Name.CompareTo (y.Name);
			return x.IsAdmin () ? 1 : 0;
		}

		// Regenerates the list of homes using list provided by the home manager.
		// The list is then sorted and the view is reloaded.
		void ResetHomeList ()
		{
			var h = HomeManager.Homes;
			Array.Sort (h, CompareHomes);

			Homes.Clear ();
			Homes.AddRange (h);

			TableView.ReloadData ();
		}

		protected void SortHomes ()
		{
			Homes.Sort (CompareHomes);
		}

		protected virtual void DidAddHome (HMHome home)
		{
			Homes.Add (home);

			SortHomes ();

			var newHomeIndex = Homes.IndexOf (home);
			var indexPathOfNewHome = NSIndexPath.FromRowSection (newHomeIndex, 0);
			TableView.InsertRows (new []{ indexPathOfNewHome }, UITableViewRowAnimation.Automatic);
		}

		protected virtual void DidRemoveHome (HMHome home)
		{
			var removedHomeIndex = Homes.IndexOf (home);
			if (removedHomeIndex < 0)
				return;

			Homes.RemoveAt (removedHomeIndex);
			var indexPath = NSIndexPath.FromRowSection (removedHomeIndex, 0);
			TableView.DeleteRows (new []{ indexPath }, UITableViewRowAnimation.Automatic);
		}

		protected static string SharedTextForHome (HMHome home)
		{
			return home.IsAdmin () ? "My Home" : "Shared with me";
		}

		#endregion

		#region HMHomeDelegate Methods

		[Export ("homeDidUpdateName:")]
		protected virtual void HomeDidUpdateName(HMHome home)
		{
			var homeIndex = Homes.IndexOf (home);
			if (homeIndex >= 0) {
				var indexPath = NSIndexPath.FromRowSection (homeIndex, 0);
				TableView.ReloadRows (new []{ indexPath }, UITableViewRowAnimation.Automatic);
			}
		}

		#endregion

		#region HMHomeManagerDelegate Methods

		[Export ("homeManagerDidUpdateHomes:")]
		public void DidUpdateHomes (HMHomeManager manager)
		{
			RegisterAsDelegate ();
			ResetHomeList ();

			var home = HomeStore.Home;
			if (home != null && Array.IndexOf (manager.Homes, home) < 0) {
				// Close all modal and detail views.
				DismissViewController(true, null);
				if (NavigationController != null)
					NavigationController.PopToRootViewController (true);
			}
		}

		[Export ("homeManager:didAddHome:")]
		public void DidAddHome (HMHomeManager manager, HMHome home)
		{
			home.Delegate = this;
			DidAddHome (home);
		}

		// Removes the home from the current list of homes and updates the view.
		// If the removed home was the current home, this view controller will dismiss
		// all modals views and pop all detail views.
		[Export ("homeManager:didRemoveHome:")]
		public void DidRemoveHome (HMHomeManager manager, HMHome home)
		{
			DidRemoveHome (home);
			var selectedHome = HomeStore.Home;
			if (selectedHome != null && selectedHome == home) {
				HomeStore.Home = null;

				// Close all modal and detail views.
				DismissViewController(true, null);
				if (NavigationController != null)
					NavigationController.PopToRootViewController (true);
			}
		}

		#endregion
	}
}
