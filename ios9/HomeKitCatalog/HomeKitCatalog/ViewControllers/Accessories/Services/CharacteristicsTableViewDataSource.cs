using System;
using System.Collections.Generic;

using Foundation;
using HomeKit;
using UIKit;

namespace HomeKitCatalog
{
	/// Represents the sections in the `CharacteristicsViewController`.
	public enum CharacteristicTableViewSection
	{
		Characteristics,
		AssociatedServiceType
	}

	/// A `UITableViewDataSource` that populates a `CharacteristicsViewController`.
	public class CharacteristicsTableViewDataSource : NSObject, IUITableViewDelegate, IUITableViewDataSource
	{
		static readonly Dictionary<HMServiceType, string> serviceMap = new Dictionary<HMServiceType, string> {
			{ HMServiceType.LightBulb, "Lightbulb" },
			{ HMServiceType.Fan,  "Fan" }
		};

		static readonly NSString characteristicCell = (NSString)"CharacteristicCell";
		static readonly NSString sliderCharacteristicCell = (NSString)"SliderCharacteristicCell";
		static readonly NSString switchCharacteristicCell = (NSString)"SwitchCharacteristicCell";
		static readonly NSString segmentedControlCharacteristicCell = (NSString)"SegmentedControlCharacteristicCell";
		static readonly NSString textCharacteristicCell = (NSString)"TextCharacteristicCell";
		static readonly NSString serviceTypeCell = (NSString)"ServiceTypeCell";

		ICharacteristicCellDelegate Delegate;
		bool showsFavorites;
		bool allowsAllWrites;

		// returns:  The valid associated service types for this service,
		// e.g. `HMServiceTypeFan` or `HMServiceTypeLightbulb`
		public static readonly HMServiceType[] ValidAssociatedServiceTypes = {
			HMServiceType.Fan,
			HMServiceType.LightBulb
		};

		public HMService Service { get; set; }

		public CharacteristicsTableViewDataSource (HMService service, UITableView tableView, ICharacteristicCellDelegate @delegate, bool showsFavorites = false, bool allowsAllWrites = false)
		{
			Service = service;
			Delegate = @delegate;
			this.showsFavorites = showsFavorites;
			this.allowsAllWrites = allowsAllWrites;

			tableView.DataSource = this;
			tableView.RowHeight = UITableView.AutomaticDimension;
			tableView.EstimatedRowHeight = 50;
			RegisterReuseIdentifiers (tableView);
		}

		// Registers all of the characteristic cell reuse identifiers with this table.
		static void RegisterReuseIdentifiers (UITableView tableView)
		{
			var characteristicNib = UINib.FromName (characteristicCell, null);
			tableView.RegisterNibForCellReuse (characteristicNib, characteristicCell);

			var sliderNib = UINib.FromName (sliderCharacteristicCell, null);
			tableView.RegisterNibForCellReuse (sliderNib, sliderCharacteristicCell);

			var switchNib = UINib.FromName (switchCharacteristicCell, null);
			tableView.RegisterNibForCellReuse (switchNib, switchCharacteristicCell);

			var segmentedNib = UINib.FromName (segmentedControlCharacteristicCell, null);
			tableView.RegisterNibForCellReuse (segmentedNib, segmentedControlCharacteristicCell);

			var textNib = UINib.FromName (textCharacteristicCell, null);
			tableView.RegisterNibForCellReuse (textNib, textCharacteristicCell);

			tableView.RegisterClassForCellReuse (typeof(UITableViewCell), serviceTypeCell);
		}

		[Foundation.Export ("numberOfSectionsInTableView:")]
		public nint NumberOfSections (UITableView tableView)
		{
			return Service.SupportsAssociatedServiceType () ? 2 : 1;
		}

		// The characteristics section uses the services count to generate the number of rows.
		// The associated service type uses the valid associated service types.
		public nint RowsInSection (UITableView tableView, nint section)
		{
			switch ((CharacteristicTableViewSection)(int)section) {
			case CharacteristicTableViewSection.Characteristics:
				return Service.Characteristics.Length;
			case CharacteristicTableViewSection.AssociatedServiceType:
				// For 'None'.
				return ValidAssociatedServiceTypes.Length + 1;
			default:
				throw new InvalidOperationException ("Unexpected `CharacteristicTableViewSection` value.");
			}
		}

		// Looks up the appropriate service type for the row in the list and returns a localized version,
		// or 'None' if the row doesn't correspond to any valid service type.
		string DisplayedServiceTypeForRow (int row)
		{
			var serviceTypes = ValidAssociatedServiceTypes;
			return (row < serviceTypes.Length)
				? DescriptionForServiceType (ValidAssociatedServiceTypes [row])
				: "None";
		}

		// Evaluates whether or not a service type is selected for a given row.
		bool ServiceTypeIsSelectedForRow (int row)
		{
			var serviceTypes = ValidAssociatedServiceTypes;
			if (row >= serviceTypes.Length)
				return Service.AssociatedServiceType == null;

			var serviceType = Service.ServiceType;
			if (serviceType != HMServiceType.None)
				return serviceTypes [row] == serviceType;

			return false;
		}

		// Generates a cell for an associated service.
		UITableViewCell GetCellForService (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell (serviceTypeCell, indexPath);

			cell.TextLabel.Text = DisplayedServiceTypeForRow (indexPath.Row);
			cell.Accessory = ServiceTypeIsSelectedForRow (indexPath.Row) ? UITableViewCellAccessory.Checkmark : UITableViewCellAccessory.None;

			return cell;
		}

		// Generates a characteristic cell based on the type of characteristic located at the specified index path.
		UITableViewCell GetCellForCharacteristicCell (UITableView tableView, NSIndexPath indexPath)
		{
			HMCharacteristic characteristic = Service.Characteristics [indexPath.Row];

			var reuseIdentifier = characteristicCell;

			if ((characteristic.IsReadOnly () || characteristic.IsWriteOnly ()) && !allowsAllWrites)
				reuseIdentifier = characteristicCell;
			else if (characteristic.IsBoolean ())
				reuseIdentifier = switchCharacteristicCell;
			else if (characteristic.HasPredeterminedValueDescriptions ())
				reuseIdentifier = segmentedControlCharacteristicCell;
			else if (characteristic.IsNumeric ())
				reuseIdentifier = sliderCharacteristicCell;
			else if (characteristic.IsTextWritable ())
				reuseIdentifier = textCharacteristicCell;

		var cell = (CharacteristicCell)tableView.DequeueReusableCell(reuseIdentifier, indexPath);

			cell.ShowsFavorites = showsFavorites;
			cell.Delegate = Delegate;
			cell.Characteristic = characteristic;

			return cell;
		}

		// Uses convenience methods to generate a cell based on the index path's section.
		public UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			switch ((CharacteristicTableViewSection)indexPath.Section) {
			case CharacteristicTableViewSection.Characteristics:
				return GetCellForCharacteristicCell (tableView, indexPath);
			case CharacteristicTableViewSection.AssociatedServiceType:
				return GetCellForService (tableView, indexPath);
			default:
				throw new InvalidOperationException ("Unexpected `CharacteristicTableViewSection` value.");
			}
		}

		[Export ("tableView:titleForHeaderInSection:")]
		public string TitleForHeader (UITableView tableView, nint section)
		{
			switch ((CharacteristicTableViewSection)(int)section) {
			case CharacteristicTableViewSection.Characteristics:
				return "Characteristics";
			case CharacteristicTableViewSection.AssociatedServiceType:
				return "Associated Service Type";
			default:
				throw new InvalidOperationException ("Unexpected `CharacteristicTableViewSection` value.");
			}
		}

		static string DescriptionForServiceType (HMServiceType type)
		{
			string description;
			return serviceMap.TryGetValue (type, out description) ? description : type.ToString ();
		}
	}
}