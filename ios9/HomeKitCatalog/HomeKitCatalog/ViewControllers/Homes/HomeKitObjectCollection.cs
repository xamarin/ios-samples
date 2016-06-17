using System;
using System.Collections.Generic;

using Foundation;
using HomeKit;

namespace HomeKitCatalog
{
	// Represents the all different types of HomeKit objects.
	public enum HomeKitObjectSection
	{
		Accessory,
		Room,
		Zone,
		User,
		ActionSet,
		Trigger,
		ServiceGroup
	}

	/// <summary>
	/// The `HomeKitObjectCollection` is a model object for the `HomeViewController`. It manages arrays of HomeKit objects.
	/// Manages internal lists of HomeKit objects to allow for save insertion into a table view.
	/// </summary>
	public class HomeKitObjectCollection
	{
		readonly List<HMAccessory> accessories;
		public List<HMAccessory> Accessories {
			get {
				return accessories;
			}
		}

		readonly List<HMRoom> rooms;
		public List<HMRoom> Rooms {
			get {
				return rooms;
			}
		}

		readonly List<HMZone> zones;
		public List<HMZone> Zones {
			get {
				return zones;
			}
		}

		readonly List<HMActionSet> actionSets;
		public List<HMActionSet> ActionSets {
			get {
				return actionSets;
			}
		}

		readonly List<HMTrigger> triggers;
		public List<HMTrigger> Triggers {
			get {
				return triggers;
			}
		}

		readonly List<HMServiceGroup> serviceGroups;
		public List<HMServiceGroup> ServiceGroups {
			get {
				return serviceGroups;
			}
		}

		public HomeKitObjectCollection ()
		{
			accessories = new List<HMAccessory> ();
			rooms = new List<HMRoom> ();
			zones = new List<HMZone> ();
			actionSets = new List<HMActionSet> ();
			triggers = new List<HMTrigger> ();
			serviceGroups = new List<HMServiceGroup> ();
		}

		#region Add

		public NSIndexPath Add (HMAccessory accessory)
		{
			Accessories.Add (accessory);
			Accessories.SortByLocalizedName (a => a.Name);
			return IndexPathOfObject (accessory);
		}

		public NSIndexPath Add (HMRoom room)
		{
			Rooms.Add (room);
			Rooms.SortByLocalizedName (r => r.Name);
			return IndexPathOfObject (room);
		}

		public NSIndexPath Add (HMZone zone)
		{
			Zones.Add (zone);
			Zones.SortByLocalizedName (z => z.Name);
			return IndexPathOfObject (zone);
		}

		public NSIndexPath Add (HMActionSet actionSet)
		{
			ActionSets.Add (actionSet);
			ActionSets.SortByTypeAndLocalizedName ();
			return IndexPathOfObject (actionSet);
		}

		public NSIndexPath Add (HMTrigger trigger)
		{
			Triggers.Add (trigger);
			Triggers.SortByLocalizedName (t => t.Name);
			return IndexPathOfObject (trigger);
		}

		public NSIndexPath Add (HMServiceGroup serviceGroup)
		{
			ServiceGroups.Add (serviceGroup);
			ServiceGroups.SortByLocalizedName (sg => sg.Name);
			return IndexPathOfObject (serviceGroup);
		}

		#endregion

		#region Remove

		public NSIndexPath Remove (HMAccessory accessory)
		{
			var indexPath = IndexPathOfObject (accessory);
			Accessories.RemoveAt (indexPath.Row);
			return indexPath;
		}

		public NSIndexPath Remove (HMRoom room)
		{
			var indexPath = IndexPathOfObject (room);
			Rooms.RemoveAt (indexPath.Row);
			return indexPath;
		}

		public NSIndexPath Remove (HMZone zone)
		{
			var indexPath = IndexPathOfObject (zone);
			Zones.RemoveAt (indexPath.Row);
			return indexPath;
		}

		public NSIndexPath Remove (HMActionSet actionSet)
		{
			var indexPath = IndexPathOfObject (actionSet);
			ActionSets.RemoveAt (indexPath.Row);
			return indexPath;
		}

		public NSIndexPath Remove (HMTrigger trigger)
		{
			var indexPath = IndexPathOfObject (trigger);
			Triggers.RemoveAt (indexPath.Row);
			return indexPath;
		}

		public NSIndexPath Remove (HMServiceGroup serviceGroup)
		{
			var indexPath = IndexPathOfObject (serviceGroup);
			ServiceGroups.RemoveAt (indexPath.Row);
			return indexPath;
		}

		#endregion

		#region IndexPathOfObject

		public NSIndexPath IndexPathOfObject (HMAccessory accessory)
		{
			var index = Accessories.IndexOf (accessory);
			return index < 0 ? null : BuildIndexPath (index, HomeKitObjectSection.Accessory);
		}

		public NSIndexPath IndexPathOfObject (HMRoom room)
		{
			var index = Rooms.IndexOf (room);
			return index < 0 ? null : BuildIndexPath (index, HomeKitObjectSection.Room);
		}

		public NSIndexPath IndexPathOfObject (HMZone zone)
		{
			var index = Zones.IndexOf (zone);
			return index < 0 ? null : BuildIndexPath (index, HomeKitObjectSection.Zone);
		}

		public NSIndexPath IndexPathOfObject (HMActionSet actionSet)
		{
			var index = ActionSets.IndexOf (actionSet);
			return index < 0 ? null : BuildIndexPath (index, HomeKitObjectSection.ActionSet);
		}

		public NSIndexPath IndexPathOfObject (HMTrigger trigger)
		{
			var index = Triggers.IndexOf (trigger);
			return index < 0 ? null : BuildIndexPath (index, HomeKitObjectSection.Trigger);
		}

		public NSIndexPath IndexPathOfObject (HMServiceGroup serviceGroup)
		{
			var index = ServiceGroups.IndexOf (serviceGroup);
			return index < 0 ? null : BuildIndexPath (index, HomeKitObjectSection.ServiceGroup);
		}

		static NSIndexPath BuildIndexPath (int row, HomeKitObjectSection section)
		{
			return NSIndexPath.FromRowSection (row, (int)section);
		}

		#endregion

		public void ResetWithHome (HMHome home)
		{
			Accessories.Clear ();
			Accessories.AddRange (home.Accessories);
			Accessories.SortByLocalizedName (a => a.Name);

			Rooms.Clear ();
			Rooms.AddRange (home.GetAllRooms());
			Rooms.SortByLocalizedName (r => r.Name);

			Zones.Clear ();
			Zones.AddRange (home.Zones);
			Zones.SortByLocalizedName(z => z.Name);

			ActionSets.Clear ();
			ActionSets.AddRange (home.ActionSets);
			ActionSets.SortByTypeAndLocalizedName ();

			Triggers.Clear ();
			Triggers.AddRange (home.Triggers);
			Triggers.SortByLocalizedName (t => t.Name);

			ServiceGroups.Clear ();
			ServiceGroups.AddRange (home.ServiceGroups);
			ServiceGroups.SortByLocalizedName (sg => sg.Name);
		}

		public int ObjectsCountForSection (HomeKitObjectSection section)
		{
			switch (section) {
			case HomeKitObjectSection.Accessory:
				return Accessories.Count;
			case HomeKitObjectSection.Room:
				return Rooms.Count;
			case HomeKitObjectSection.Zone:
				return Zones.Count;
			case HomeKitObjectSection.User:
				return 0;
			case HomeKitObjectSection.ActionSet:
				return ActionSets.Count;
			case HomeKitObjectSection.Trigger:
				return Triggers.Count;
			case HomeKitObjectSection.ServiceGroup:
				return ServiceGroups.Count;
			default:
				throw new InvalidOperationException ("Unexpected `HomeKitObjectSection` value.");
			}
		}
	}
}