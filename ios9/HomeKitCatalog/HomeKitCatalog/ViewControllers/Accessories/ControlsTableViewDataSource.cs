using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using HomeKit;
using UIKit;

namespace HomeKitCatalog
{
	// A `UITableViewDataSource` that populates the table in `ControlsViewController`.
	public class ControlsTableViewDataSource: UITableViewDataSource
	{
		static readonly NSString serviceCell = (NSString)"ServiceCell";
		static readonly NSString unreachableServiceCell = (NSString)"UnreachableServiceCell";

		Dictionary<string, List<HMService>> serviceTable;
		string[] sortedKeys;

		UITableView tableView;

		static HMHome Home {
			get {
				return HomeStore.SharedStore.Home;
			}
		}

		public ControlsTableViewDataSource (UITableView tableView)
		{
			this.tableView = tableView;
			tableView.DataSource = this;
		}

		// Reloads the table, sets the table's dataSource to self, regenerated the service table, creates a sorted list of keys,
		// sets the home's delegate, and reloads the table.
		public void ReloadTable ()
		{
			var home = Home;
			if (home != null) {
				serviceTable = home.GetServiceTable ();
				sortedKeys = serviceTable.Keys.ToArray ();
				Array.Sort (sortedKeys);
			} else {
				serviceTable = null;
				sortedKeys = null;
			}

			tableView.ReloadData ();
		}

		public override string TitleForHeader (UITableView tableView, nint section)
		{
			return sortedKeys != null ? sortedKeys [(int)section] : null;
		}

		public override nint NumberOfSections (UITableView tableView)
		{
			return sortedKeys != null ? sortedKeys.Length : 0;
		}

		// returns:  A message that corresponds to the current most important reason that there are no services in the table.
		// Either "No Accessories" or "No Services".
		public string EmptyMessage ()
		{
			int count = 0;

			var home = Home;
			if (home != null)
				count = home.Accessories.Length;

			return count == 0 ? "No Accessories" : "No Services";
		}

		public override nint RowsInSection (UITableView tableView, nint section)
		{
			return serviceTable [sortedKeys [(int)section]].Count;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var service = ServiceForIndexPath (indexPath);
			var reuseIdentifier = service.Accessory.Reachable ? serviceCell : unreachableServiceCell;

			var cell = (ServiceCell)tableView.DequeueReusableCell (reuseIdentifier, indexPath);
			cell.Service = service;

			return cell;
		}

		// returns:  The service represented at the index path in the table.
		public HMService ServiceForIndexPath (NSIndexPath indexPath)
		{
			List<HMService> services;
			if (sortedKeys != null
				&& serviceTable != null
				&& serviceTable.TryGetValue (sortedKeys [indexPath.Section], out services))
				return services [indexPath.Row];

			return null;
		}
	}
}