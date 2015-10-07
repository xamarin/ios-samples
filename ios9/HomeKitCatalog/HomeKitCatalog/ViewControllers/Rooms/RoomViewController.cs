using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using HomeKit;
using UIKit;

namespace HomeKitCatalog
{
	public partial class RoomViewController : HMCatalogViewController, IHMAccessoryDelegate
	{
		static readonly NSString AccessoryCell = (NSString)"AccessoryCell";
		static readonly NSString UnreachableAccessoryCell = (NSString)"UnreachableAccessoryCell";
		static readonly string ModifyAccessorySegue = "Modify Accessory";

		HMRoom room;
		public HMRoom Room {
			get {
				return room;
			}
			set {
				room = value;
				NavigationItem.Title = room.Name;
			}
		}

		List<HMAccessory> Accessories { get; set; }

		public RoomViewController (IntPtr handle)
			: base (handle)
		{
		}

		[Export ("initWithCoder:")]
		public RoomViewController (NSCoder coder)
			: base (coder)
		{
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			ReloadData ();
		}

		#region Table View Methods

		public override nint RowsInSection (UITableView tableView, nint section)
		{
			var rows = Accessories.Count;
			TableView.SetBackgroundMessage (rows == 0 ? "No Accessories" : null);
			return rows;
		}

		// returns:  `true` if the current room is not the home's roomForEntireHome; `false` otherwise.
		public override bool CanEditRow (UITableView tableView, NSIndexPath indexPath)
		{
			return Room != Home.GetRoomForEntireHome ();
		}

		public override string TitleForDeleteConfirmation (UITableView tableView, NSIndexPath indexPath)
		{
			return "Unassign";
		}

		public override void CommitEditingStyle (UITableView tableView, UITableViewCellEditingStyle editingStyle, NSIndexPath indexPath)
		{
			if (editingStyle == UITableViewCellEditingStyle.Delete)
				UnassignAccessory (Accessories [indexPath.Row]);
		}

		// returns:  A cell representing an accessory.
		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var accessory = Accessories [indexPath.Row];
			var reuseIdentifier = AccessoryCell;

			if (!accessory.Reachable)
				reuseIdentifier = UnreachableAccessoryCell;

			var cell = tableView.DequeueReusableCell (reuseIdentifier, indexPath);
			cell.TextLabel.Text = accessory.Name;

			return cell;
		}

		public override string TitleForHeader (UITableView tableView, nint section)
		{
			return Accessories.Any () ? "Accessories" : null;
		}

		#endregion

		#region Helpers

		// Updates the internal array of accessories and reloads the table view.
		void ReloadData ()
		{
			if (Accessories == null)
				Accessories = new List<HMAccessory> ();

			Accessories.Clear ();
			Accessories.AddRange (Room.Accessories);
			Accessories.SortByLocalizedName (a => a.Name);
		}

		// Sorts the internal list of accessories by localized name.
		void SortAccessories ()
		{
			Accessories.SortByLocalizedName (a => a.Name);
		}

		// Registers as the delegate for the current home and all accessories in our room.
		protected override void RegisterAsDelegate ()
		{
			base.RegisterAsDelegate ();
			foreach (var accessory in Room.Accessories)
				accessory.Delegate = this;
		}

		// Sets the accessory and home of the modifyAccessoryViewController that will be presented.
		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			base.PrepareForSegue (segue, sender);
			var indexPath = TableView.IndexPathForCell ((UITableViewCell)sender);
			if (segue.Identifier == ModifyAccessorySegue) {
				var modifyViewController = (ModifyAccessoryViewController)segue.IntendedDestinationViewController ();
				modifyViewController.Accessory = Room.Accessories [indexPath.Row];
			}
		}

		// Adds an accessory into the internal list of accessories and inserts the row into the table view.
		void DidAssignAccessory (HMAccessory accessory)
		{
			Accessories.Add (accessory);
			SortAccessories ();

			var newAccessoryIndex = Accessories.IndexOf (accessory);
			if (newAccessoryIndex >= 0) {
				var newAccessoryIndexPath = NSIndexPath.FromRowSection (newAccessoryIndex, 0);
				TableView.InsertRows (new []{ newAccessoryIndexPath }, UITableViewRowAnimation.Automatic);
			}
		}

		// Removes an accessory from the internal list of accessory (if it exists) and deletes the row from the table view.
		void DidUnassignAccessory (HMAccessory accessory)
		{
			var accessoryIndex = Accessories.IndexOf (accessory);
			if (accessoryIndex >= 0) {
				Accessories.RemoveAt (accessoryIndex);
				var accessoryIndexPath = NSIndexPath.FromRowSection (accessoryIndex, 0);
				TableView.DeleteRows (new [] { accessoryIndexPath }, UITableViewRowAnimation.Automatic);
			}
		}

		// Assigns an accessory to the current room.
		void AssignAccessory (HMAccessory accessory)
		{
			DidAssignAccessory (accessory);
			Home.AssignAccessory (accessory, room, error => {
				if (error != null) {
					DisplayError (error);
					DidUnassignAccessory (accessory);
				}
			});
		}

		// Assigns the current room back into `roomForEntireHome`.
		void UnassignAccessory (HMAccessory accessory)
		{
			DidUnassignAccessory (accessory);
			Home.AssignAccessory (accessory, Home.GetRoomForEntireHome (), error => {
				if (error != null) {
					DisplayError (error);
					DidAssignAccessory (accessory);
				}
			});
		}

		// Finds an accessory in the internal array of accessories and updates its row in the table view.
		void DidModifyAccessory (HMAccessory accessory)
		{
			var index = Accessories.IndexOf (accessory);
			if (index >= 0)
				TableView.ReloadRows (new []{ NSIndexPath.FromRowSection (index, 0) }, UITableViewRowAnimation.Automatic);
		}

		#endregion

		#region HMHomeDelegate Methods

		// If the accessory was added to this room, insert it.
		[Export ("home:didAddAccessory:")]
		public void DidAddAccessory (HMHome home, HMAccessory accessory)
		{
			if (accessory.Room == room) {
				accessory.Delegate = this;
				DidAssignAccessory (accessory);
			}
		}

		// Remove the accessory from our room, if required.
		[Export ("home:didRemoveAccessory:")]
		public void DidRemoveAccessory (HMHome home, HMAccessory accessory)
		{
			DidUnassignAccessory (accessory);
		}

		// Handles the update.
		//
		// We act based on one of three options:
		//
		// 1. A new accessory is being added to this room.
		// 2. An accessory is being assigned from this room to another room.
		// 3. We can ignore this message.
		[Export ("home:didUpdateRoom:forAccessory:")]
		public void DidUpdateRoom (HMHome home, HMRoom room, HMAccessory accessory)
		{
			if (room == Room)
				DidAssignAccessory (accessory);
			else if (Accessories.Contains (accessory))
				DidUnassignAccessory (accessory);
		}

		// If our room was removed, pop back.
		[Export ("home:didRemoveRoom:")]
		public void DidRemoveRoom (HMHome home, HMRoom room)
		{
			if (room == Room)
				NavigationController.PopViewController (true);
		}

		// If our room was renamed, reload our title.
		[Export ("home:didUpdateNameForRoom:")]
		public void DidUpdateNameForRoom (HMHome home, HMRoom room)
		{
			if (room == Room)
				NavigationItem.Title = room.Name;
		}

		#endregion

		#region HMAccessoryDelegate Methods

		// Accessory updates will reload the cell for the accessory.

		[Export ("accessoryDidUpdateReachability:")]
		public void DidUpdateReachability (HMAccessory accessory)
		{
			DidModifyAccessory (accessory);
		}

		[Export ("accessoryDidUpdateName:")]
		public void DidUpdateName (HMAccessory accessory)
		{
			DidModifyAccessory (accessory);
		}

		#endregion
	}
}
