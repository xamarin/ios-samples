﻿/*
See LICENSE folder for this sample’s licensing information.

Abstract:
Mimics the Settings.app for displaying a dynamic list of available wi-fi access points
*/

using System;
using System.Collections.Generic;
using System.Linq;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Conference_Diffable.Diffable {
	public class WiFiSettingsViewController : UIViewController {
		static readonly NSString key = new NSString (nameof (WiFiSettingsViewController));

		class Section : NSObject, IEquatable<Section> {
			public static Section Config { get; } = new Section (0);
			public static Section Networks { get; } = new Section (1);

			public int Value { get; private set; }

			Section (int value) => Value = value;

			public static Section [] AllSections { get; } = {
				Config, Networks
			};

			public static bool operator == (Section left, Section right)
			{
				if (ReferenceEquals (left, right))
					return true;

				if (ReferenceEquals (left, null))
					return false;

				if (ReferenceEquals (right, null))
					return false;

				return left.Equals (right);
			}

			public static bool operator != (Section left, Section right) => !(left == right);
			public override bool Equals (object obj) => this == (Section)obj;
			public bool Equals (Section other) => Value == other.Value;
			public override int GetHashCode () => HashCode.Combine (base.GetHashCode (), Value);
		}

		enum ItemType {
			WifiEnabled,
			CurrentNetwork,
			AvailableNetwork
		}

		class Item : NSObject, IEquatable<Item> {
			public string Id { get; private set; }
			public string Title { get; private set; }
			public ItemType Type { get; private set; }
			public WIFIController.Network Network { get; private set; }

			public Item (string title, ItemType type)
			{
				Title = title;
				Type = type;
				Id = new NSUuid ().ToString ();
			}

			public Item (WIFIController.Network network)
			{
				Title = network.Name;
				Type = ItemType.AvailableNetwork; 
				Network = network;
				Id = network.Id;
			}

			public bool IsConfig {
				get {
					var configItems = new List<ItemType> (new [] { ItemType.CurrentNetwork, ItemType.WifiEnabled });
					return configItems.Contains (Type);
				}
			}

			public bool IsNetwork { get => Type == ItemType.AvailableNetwork; }

			public static bool operator == (Item left, Item right)
			{
				if (ReferenceEquals (left, right))
					return true;

				if (ReferenceEquals (left, null))
					return false;

				if (ReferenceEquals (right, null))
					return false;

				return left.Equals (right);
			}

			public static bool operator != (Item left, Item right) => !(left == right);
			public override bool Equals (object obj) => this == (Item)obj;
			public bool Equals (Item other) => Id == other.Id;
			public override int GetHashCode () => HashCode.Combine (base.GetHashCode (), Id);
		}

		UITableView tableView;
		UITableViewDiffableDataSource<Section, Item> dataSource;
		NSDiffableDataSourceSnapshot<Section, Item> currentSnapshot;
		WIFIController wifiController;
		Item [] configurationItems;

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			// Perform any additional setup after loading the view, typically from a nib.

			NavigationItem.Title = "Wi-Fi";
			ConfigureHierarchy ();
			ConfigureDataSource ();
			UpdateUI (false);
		}

		void ConfigureHierarchy ()
		{
			configurationItems = new [] {
				new Item ("Wi-Fi", ItemType.WifiEnabled),
				new Item ("breeno-net", ItemType.CurrentNetwork)
			};

			tableView = new UITableView (CGRect.Empty, UITableViewStyle.InsetGrouped) {
				TranslatesAutoresizingMaskIntoConstraints = false
			};
			View.AddSubview (tableView);
			tableView.RegisterClassForCellReuse (typeof (UITableViewCell), key);

			tableView.LeadingAnchor.ConstraintEqualTo (View.LeadingAnchor).Active = true;
			tableView.TrailingAnchor.ConstraintEqualTo (View.TrailingAnchor).Active = true;
			tableView.TopAnchor.ConstraintEqualTo (View.TopAnchor).Active = true;
			tableView.BottomAnchor.ConstraintEqualTo (View.BottomAnchor).Active = true;
		}

		void ConfigureDataSource ()
		{
			wifiController = new WIFIController (wifiController => UpdateUI ());
			dataSource = new UITableViewDiffableDataSource<Section, Item> (tableView, CellProviderHandler);
			dataSource.DefaultRowAnimation = UITableViewRowAnimation.Fade;
			wifiController.ScanForNetworks = true;

			UITableViewCell CellProviderHandler (UITableView tableView, NSIndexPath indexPath, NSObject obj)
			{
				var item = obj as Item;

				// Get a cell of the desired kind.
				var cell = tableView.DequeueReusableCell (key, indexPath);

				// network cell
				if (item.IsNetwork) {
					cell.TextLabel.Text = item.Title;
					cell.Accessory = UITableViewCellAccessory.DetailDisclosureButton;
					cell.AccessoryView = null;
				} else if (item.IsConfig) {
					// configuration cells
					cell.TextLabel.Text = item.Title;

					if (item.Type == ItemType.WifiEnabled) {
						var enableWifiSwitch = new UISwitch { On = wifiController.WifiEnabled };
						enableWifiSwitch.ValueChanged += EnableWifiSwitch_ValueChanged;
						cell.AccessoryView = enableWifiSwitch;
					} else {
						cell.AccessoryView = null;
						cell.Accessory = UITableViewCellAccessory.DetailDisclosureButton;
					}
				}

				// Return the cell.
				return cell;
			}
		}

		void UpdateUI (bool animated = true)
		{
			var configItems = configurationItems.Where (c => !(c.Type == ItemType.CurrentNetwork && !wifiController.WifiEnabled)).ToArray ();

			currentSnapshot = new NSDiffableDataSourceSnapshot<Section, Item> ();
			currentSnapshot.AppendSections (new [] { Section.Config });
			currentSnapshot.AppendItems (configItems, Section.Config);

			if (wifiController.WifiEnabled) {
				var sortedNetworks = wifiController.AvailableNetworks.OrderBy (w => w.Name).ToArray ();
				var networkItems = sortedNetworks.Select (n => new Item (n)).ToArray ();
				currentSnapshot.AppendSections (new [] { Section.Networks });
				currentSnapshot.AppendItems (networkItems);
			}

			dataSource.ApplySnapshot (currentSnapshot, animated);
		}

		private void EnableWifiSwitch_ValueChanged (object sender, EventArgs e)
		{
			wifiController.WifiEnabled = (sender as UISwitch).On;
			UpdateUI ();
		}
	}
}
