using System;
using System.Collections.Generic;
using System.Linq;

using CoreFoundation;
using ExternalAccessory;
using Foundation;
using HomeKit;
using UIKit;

namespace HomeKitCatalog
{
	public enum AccessoryType
	{
		// A HomeKit object
		HomeKit,
		// An external, `EAWiFiUnconfiguredAccessory` object
		External
	}

	// Represents an accessory type and encapsulated accessory.
	public class Accessory : IEquatable<Accessory>
	{
		Func<string> nameGetter;

		public string Name {
			get {
				return nameGetter ();
			}
		}

		public AccessoryType Type { get; set; }

		public NSObject AccessoryObject { get; private set; }

		public HMAccessory HomeKitAccessoryObject { get; private set; }

		public EAWiFiUnconfiguredAccessory ExternalAccessoryObject { get; private set; }

		public static bool operator == (Accessory lhs, Accessory rhs)
		{
			return lhs.Name == rhs.Name;
		}

		public static bool operator != (Accessory lhs, Accessory rhs)
		{
			return lhs.Name != rhs.Name;
		}

		public bool Equals (Accessory other)
		{
			return Name == other.Name;
		}

		public override bool Equals (object obj)
		{
			if (obj == null || obj.GetType () != typeof(Accessory))
				return false;

			var other = (Accessory)obj;
			return Name == other.Name;
		}

		public override int GetHashCode ()
		{
			return Name.GetHashCode ();
		}

		public static Accessory CreateHomeKitObject (HMAccessory accessory)
		{
			return new Accessory {
				Type = AccessoryType.HomeKit,
				AccessoryObject = accessory,
				HomeKitAccessoryObject = accessory,
				nameGetter = () => accessory.Name
			};
		}

		public static Accessory CreateExternalObject (EAWiFiUnconfiguredAccessory accessory)
		{
			return new Accessory {
				Type = AccessoryType.External,
				AccessoryObject = accessory,
				ExternalAccessoryObject = accessory,
				nameGetter = () => accessory.Name
			};
		}
	}

	public partial class AccessoryBrowserViewController : HMCatalogViewController, IModifyAccessoryDelegate, IEAWiFiUnconfiguredAccessoryBrowserDelegate, IHMAccessoryBrowserDelegate
	{
		static readonly NSString AccessoryCell = (NSString)"AccessoryCell";
		static readonly NSString AddedAccessoryCell = (NSString)"AddedAccessoryCell";
		const string AddAccessorySegue = "Add Accessory";

		readonly List<HMAccessory> addedAccessories = new List<HMAccessory> ();
		readonly List<Accessory> displayedAccessories = new List<Accessory> ();
		readonly HMAccessoryBrowser accessoryBrowser = new HMAccessoryBrowser ();
		EAWiFiUnconfiguredAccessoryBrowser externalAccessoryBrowser;

		#region ctors

		public AccessoryBrowserViewController (IntPtr handle)
			: base (handle)
		{
		}

		[Export ("initWithCoder:")]
		public AccessoryBrowserViewController (NSCoder coder)
			: base (coder)
		{
		}

		#endregion

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			TableView.EstimatedRowHeight = 44;
			TableView.RowHeight = UITableView.AutomaticDimension;
			accessoryBrowser.Delegate = this;

			// We can't use the ExternalAccessory framework on the iPhone simulator.
			if (!UIDevice.CurrentDevice.Model.Contains ("Simulator"))
				externalAccessoryBrowser = new EAWiFiUnconfiguredAccessoryBrowser (this, DispatchQueue.MainQueue);

			StartBrowsing ();
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			ReloadTable ();
		}

		// Stops browsing and dismisses the view controller.
		[Export ("dismiss:")]
		void Dismiss (NSString sender)
		{
			StopBrowsing ();
			DismissViewController (true, null);
		}

		// Sets the accessory, home, and delegate of a ModifyAccessoryViewController.
		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			base.PrepareForSegue (segue, sender);

			var accessorySender = sender as HMAccessory;
			if (accessorySender != null && segue.Identifier == AddAccessorySegue) {
				var modifyViewController = (ModifyAccessoryViewController)segue.IntendedDestinationViewController ();
				modifyViewController.Accessory = accessorySender;
				modifyViewController.Delegate = this;
			}
		}

		#region Table View Methods

		// Generates the number of rows based on the number of displayed accessories.
		// This method will also display a table view background message, if required.
		// returns:  The number of rows based on the number of displayed accessories.
		public override nint RowsInSection (UITableView tableView, nint section)
		{
			var rows = displayedAccessories.Count;
			var message = rows == 0 ? "No Discovered Accessories" : null;

			TableView.SetBackgroundMessage (message);

			return rows;
		}

		// returns:  Creates a cell that lists an accessory, and if it hasn't been added to the home,
		// shows a disclosure indicator instead of a checkmark.
		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			Accessory accessory = displayedAccessories [indexPath.Row];
			var reuseIdentifier = AccessoryCell;

			if (accessory.Type == AccessoryType.HomeKit && addedAccessories.Contains (accessory.HomeKitAccessoryObject))
				reuseIdentifier = AddedAccessoryCell;

			var cell = tableView.DequeueReusableCell (reuseIdentifier, indexPath);
			cell.TextLabel.Text = accessory.Name;

			var hkAccessory = accessory.HomeKitAccessoryObject;

			var detailTextLabel = cell.DetailTextLabel;
			if (detailTextLabel != null) {
				detailTextLabel.Text = (hkAccessory != null)
				? hkAccessory.Category.LocalizedDescription
				: "External Accessory";
			}

			return cell;
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			var accessory = displayedAccessories [indexPath.Row];
			switch (accessory.Type) {
			case AccessoryType.HomeKit:
				ConfigureAccessory (accessory.HomeKitAccessoryObject);
				break;
			case AccessoryType.External:
				if (externalAccessoryBrowser != null)
					externalAccessoryBrowser.ConfigureAccessory (accessory.ExternalAccessoryObject, this);
				break;
			}
		}

		#endregion

		#region Helper Methods

		// Starts browsing on both HomeKit and External accessory browsers.
		void StartBrowsing ()
		{
			accessoryBrowser.StartSearchingForNewAccessories ();
			if (externalAccessoryBrowser != null)
				externalAccessoryBrowser.StartSearchingForUnconfiguredAccessories (null);
		}

		// Stops browsing on both HomeKit and External accessory browsers.
		void StopBrowsing ()
		{
			accessoryBrowser.StopSearchingForNewAccessories ();
			if (externalAccessoryBrowser != null)
				externalAccessoryBrowser.StopSearchingForUnconfiguredAccessories ();
		}

		// Concatenates and sorts the discovered and added accessories.
		// TODO: can we use IEnumerable<Accessory> ???
		List<Accessory> AllAccessories ()
		{
			var hashSet = new HashSet<Accessory> ();

			var discovered = accessoryBrowser.DiscoveredAccessories.Select (Accessory.CreateHomeKitObject);
			hashSet.UnionWith (discovered);

			var added = addedAccessories.Select (Accessory.CreateHomeKitObject);
			hashSet.UnionWith (added);

			if (externalAccessoryBrowser != null) {
				NSSet external = externalAccessoryBrowser.UnconfiguredAccessories;
				if (external != null) {
					var unconfiguredAccessories = external.Cast<EAWiFiUnconfiguredAccessory> ().Select (Accessory.CreateExternalObject);
					hashSet.UnionWith (unconfiguredAccessories);
				}
			}

			var result = hashSet.ToList ();
			result.SortByLocalizedName (a => a.Name);

			return result;
		}

		/// Updates the displayed accesories array and reloads the table view.
		void ReloadTable ()
		{
			displayedAccessories.Clear ();
			displayedAccessories.AddRange (AllAccessories ());
			TableView.ReloadData ();
		}

		/// Sends the accessory to the next view.
		void ConfigureAccessory (HMAccessory accessory)
		{
			if (displayedAccessories.Contains (Accessory.CreateHomeKitObject (accessory)))
				PerformSegue (AddAccessorySegue, accessory);
		}

		// Finds an unconfigured accessory with a specified name.
		HMAccessory UnconfiguredHomeKitAccessory (string name)
		{
			return displayedAccessories.Where (a => a.Type == AccessoryType.HomeKit)
				.Select (a => a.HomeKitAccessoryObject)
				.FirstOrDefault (a => a.Name == name);
		}

		#endregion

		#region IModifyAccessoryDelegate Methods

		public void AccessoryViewControllerDidSaveAccessory (ModifyAccessoryViewController accessoryViewController, HMAccessory accessory)
		{
			addedAccessories.Add (accessory);
			ReloadTable ();
		}

		#endregion

		#region EAWiFiUnconfiguredAccessoryBrowserDelegate Methods

		// Any updates to the external accessory browser causes a reload in the table view.

		public void DidUpdateState (EAWiFiUnconfiguredAccessoryBrowser browser, EAWiFiUnconfiguredAccessoryBrowserState state)
		{
			ReloadTable ();
		}

		public void DidFindUnconfiguredAccessories (EAWiFiUnconfiguredAccessoryBrowser browser, NSSet accessories)
		{
			ReloadTable ();
		}

		public void DidRemoveUnconfiguredAccessories (EAWiFiUnconfiguredAccessoryBrowser browser, NSSet accessories)
		{
			ReloadTable ();
		}

		// If the configuration was successful, presents the 'Add Accessory' view.
		public void DidFinishConfiguringAccessory (EAWiFiUnconfiguredAccessoryBrowser browser, EAWiFiUnconfiguredAccessory accessory, EAWiFiUnconfiguredAccessoryConfigurationStatus status)
		{
			if (status != EAWiFiUnconfiguredAccessoryConfigurationStatus.Success)
				return;

			var foundAccessory = UnconfiguredHomeKitAccessory (accessory.Name);
			if (foundAccessory != null)
				ConfigureAccessory (foundAccessory);
		}

		#endregion

		#region HMAccessoryBrowserDelegate Methods

		// Inserts the accessory into the internal array and inserts the row into the table view.
		[Export ("accessoryBrowser:didFindNewAccessory:")]
		public void DidFindNewAccessory (HMAccessoryBrowser browser, HMAccessory accessory)
		{
			var newAccessory = Accessory.CreateHomeKitObject (accessory);
			if (displayedAccessories.Contains (newAccessory))
				return;
			displayedAccessories.Add (newAccessory);
			displayedAccessories.SortByLocalizedName (a => a.Name);

			var newIndex = displayedAccessories.IndexOf (newAccessory);
			if (newIndex >= 0) {
				var newIndexPath = NSIndexPath.FromRowSection (newIndex, 0);
				TableView.InsertRows (new []{ newIndexPath }, UITableViewRowAnimation.Automatic);
			}
		}

		// Removes the accessory from the internal array and deletes the row from the table view.
		[Export ("accessoryBrowser:didRemoveNewAccessory:")]
		public void DidRemoveNewAccessory (HMAccessoryBrowser browser, HMAccessory accessory)
		{
			var removedAccessory = Accessory.CreateHomeKitObject (accessory);
			var removedIndex = displayedAccessories.IndexOf (removedAccessory);
			if (removedIndex < 0)
				return;

			var removedIndexPath = NSIndexPath.FromRowSection (removedIndex, 0);
			displayedAccessories.RemoveAt (removedIndex);
			TableView.DeleteRows (new []{ removedIndexPath }, UITableViewRowAnimation.Automatic);
		}

		#endregion
	}
}
