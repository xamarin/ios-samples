using System;
using Foundation;
using UIKit;
using System.CodeDom.Compiler;
using HomeKit;
namespace HomeKitIntro
{
	public class CharacteristicTableSource : UITableViewSource
	{
		#region Private Variables
		private CharacteristicTableViewController _controller;
		#endregion 

		#region Computed Properties
		/// <summary>
		/// Returns the delegate of the current running application
		/// </summary>
		/// <value>The this app.</value>
		public AppDelegate ThisApp {
			get { return (AppDelegate)UIApplication.SharedApplication.Delegate; }
		}
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="HomeKitIntro.CharacteristicTableSource"/> class.
		/// </summary>
		/// <param name="controller">Controller.</param>
		public CharacteristicTableSource (CharacteristicTableViewController controller)
		{
			// Initialize
			_controller = controller;
		}
		#endregion

		#region Override Methods
		/// <Docs>Table view displaying the sections.</Docs>
		/// <returns>Number of sections required to display the data. The default is 1 (a table must have at least one section).</returns>
		/// <para>Declared in [UITableViewDataSource]</para>
		/// <summary>
		/// Numbers the of sections.
		/// </summary>
		/// <param name="tableView">Table view.</param>
		public override nint NumberOfSections (UITableView tableView)
		{
			// Always one section
			return 1;
		}

		/// <Docs>Table view displaying the rows.</Docs>
		/// <summary>
		/// Rowses the in section.
		/// </summary>
		/// <returns>The in section.</returns>
		/// <param name="tableview">Tableview.</param>
		/// <param name="section">Section.</param>
		public override nint RowsInSection (UITableView tableview, nint section)
		{
			// Return the number of characteristics
			return _controller.Service.Characteristics.Length;
		}

		/// <Docs>Table view.</Docs>
		/// <summary>
		/// Gets the height for row.
		/// </summary>
		/// <returns>The height for row.</returns>
		/// <param name="tableView">Table view.</param>
		/// <param name="indexPath">Index path.</param>
		public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
		{
			// Force height to 60
			return 60;
		}

		/// <summary>
		/// Shoulds the highlight row.
		/// </summary>
		/// <returns><c>true</c>, if highlight row was shoulded, <c>false</c> otherwise.</returns>
		/// <param name="tableView">Table view.</param>
		/// <param name="rowIndexPath">Row index path.</param>
		public override bool ShouldHighlightRow (UITableView tableView, NSIndexPath rowIndexPath)
		{
			// Set highlighting
			return false;
		}

		/// <Docs>Table view containing the section.</Docs>
		/// <summary>
		/// Called to populate the header for the specified section.
		/// </summary>
		/// <see langword="null"></see>
		/// <returns>The for header.</returns>
		/// <param name="tableView">Table view.</param>
		/// <param name="section">Section.</param>
		public override string TitleForHeader (UITableView tableView, nint section)
		{
			// Return section title
			return "Characteristics";
		}

		/// <Docs>Table view containing the section.</Docs>
		/// <summary>
		/// Called to populate the footer for the specified section.
		/// </summary>
		/// <see langword="null"></see>
		/// <returns>The for footer.</returns>
		/// <param name="tableView">Table view.</param>
		/// <param name="section">Section.</param>
		public override string TitleForFooter (UITableView tableView, nint section)
		{
			// None
			return "";
		}

		/// <Docs>Table view requesting the cell.</Docs>
		/// <summary>
		/// Gets the cell.
		/// </summary>
		/// <returns>The cell.</returns>
		/// <param name="tableView">Table view.</param>
		/// <param name="indexPath">Index path.</param>
		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			string fieldName = "";
			CharacteristicDisplayType display;
			bool editable = true;

			// Get characteristic
			var characteristic = _controller.Service.Characteristics [indexPath.Row];

			// Take action based on type
			switch (characteristic.CharacteristicType) {
			case HMCharacteristicType.Brightness:
				fieldName = "Brightness";
				display = CharacteristicDisplayType.Slider;
				editable = true;
				break;
			case HMCharacteristicType.CurrentDoorState:
				fieldName = "Current Door State";
				display = CharacteristicDisplayType.Info;
				editable = false;
				break;
			case HMCharacteristicType.CurrentRelativeHumidity:
				fieldName = "Current Relative Humidity";
				display = CharacteristicDisplayType.Slider;
				editable = false;
				break;
			case HMCharacteristicType.CurrentTemperature:
				fieldName = "Current Temperature";
				display = CharacteristicDisplayType.Slider;
				editable = false;
				break;
			case HMCharacteristicType.HeatingCoolingStatus:
				fieldName = "Heating Cooling Status";
				display = CharacteristicDisplayType.Info;
				editable = false;
				break;
			case HMCharacteristicType.Hue:
				fieldName = "Hue";
				display = CharacteristicDisplayType.Slider;
				editable = true;
				break;
			case HMCharacteristicType.CurrentLockMechanismState:
				fieldName = "Current Lock State";
				display = CharacteristicDisplayType.Info;
				editable = true;
				break;
			case HMCharacteristicType.LockManagementAutoSecureTimeout:
				fieldName = "Auto Security Timeout";
				display = CharacteristicDisplayType.Slider;
				editable = true;
				break;
			case HMCharacteristicType.LockManagementControlPoint:
				fieldName = "Control Point";
				display = CharacteristicDisplayType.Info;
				editable = true;
				break;
			case HMCharacteristicType.LockMechanismLastKnownAction:
				fieldName = "Last Known Action";
				display = CharacteristicDisplayType.Info;
				editable = true;
				break;
			case HMCharacteristicType.TargetLockMechanismState:
				fieldName = "Target Lock State";
				display = CharacteristicDisplayType.Info;
				editable = true;
				break;
			case HMCharacteristicType.None:
				fieldName = "Descriptor";
				display = CharacteristicDisplayType.Info;
				editable = false;
				break;
			case HMCharacteristicType.ObstructionDetected:
				fieldName = "Obstruction Detected";
				display = CharacteristicDisplayType.Switch;
				editable = false;
				break;
			case HMCharacteristicType.PowerState:
				fieldName = "Power State";
				display = CharacteristicDisplayType.Switch;
				editable = true;
				break;
			case HMCharacteristicType.Saturation:
				fieldName = "Saturation";
				display = CharacteristicDisplayType.Slider;
				editable = true;
				break;
			case HMCharacteristicType.TargetDoorState:
				fieldName = "Target Door State";
				display = CharacteristicDisplayType.Info;
				editable = true;
				break;
			case HMCharacteristicType.TargetTemperature:
				fieldName = "Target Temperature";
				display = CharacteristicDisplayType.Slider;
				editable = true;
				break;
			default:
				// Just display as info it the type is unknown
				fieldName = characteristic.CharacteristicType.ToString();
				display = CharacteristicDisplayType.Info;
				editable = false;
				break;
			}

			// Update the current value of the field
			characteristic.ReadValue ((err) => {
				// Report errors to console
				if (err!=null) {
					Console.WriteLine("Error Updating {0}: {1}",fieldName,err.LocalizedDescription);
				}
			});

			// Display field
			switch (display) {
			case CharacteristicDisplayType.Info:
				// Get new cell
				var InfoCell = tableView.DequeueReusableCell (CharacteristicCellInfo.Key) as CharacteristicCellInfo;

				// Populate the cell
				InfoCell.DisplayInfo (fieldName, characteristic.Value.ToString());

				// Return cell
				return InfoCell;
			case CharacteristicDisplayType.Slider:
				// Get new cell
				var SliderCell = tableView.DequeueReusableCell (CharacteristicCellSlider.Key) as CharacteristicCellSlider;

				// Populate the cell
				SliderCell.DisplayInfo (fieldName, NSObjectConverter.ToFloat (characteristic.Value), (float)characteristic.Metadata.MinimumValue, (float)characteristic.Metadata.MaximumValue, editable);
				SliderCell.Characteristic = characteristic;
				SliderCell.Controller = _controller;

				// Return cell
				return SliderCell;
			case CharacteristicDisplayType.Stepper:
				// Get new cell
				var StepperCell = tableView.DequeueReusableCell (CharacteristicCellStepper.Key) as CharacteristicCellStepper;

				// Populate the cell
				StepperCell.DisplayInfo (fieldName, NSObjectConverter.ToFloat (characteristic.Value), (float)characteristic.Metadata.MinimumValue, (float)characteristic.Metadata.MaximumValue, editable);
				StepperCell.Characteristic = characteristic;
				StepperCell.Controller = _controller;

				// Return cell
				return StepperCell;
				break;
			case CharacteristicDisplayType.Switch:
				// Get new cell
				var SwitchCell = tableView.DequeueReusableCell (CharacteristicCellSwitch.Key) as CharacteristicCellSwitch;

				// Populate the cell
				SwitchCell.DisplayInfo (fieldName, NSObjectConverter.ToBool(characteristic.Value), editable);
				SwitchCell.Characteristic = characteristic;
				SwitchCell.Controller = _controller;

				// Return cell
				return SwitchCell;
			case CharacteristicDisplayType.Text:
				// Get new cell
				var TextCell = tableView.DequeueReusableCell (CharacteristicCellText.Key) as CharacteristicCellText;

				// Populate the cell
				TextCell.DisplayInfo (fieldName, NSObjectConverter.ToString(characteristic.Value), editable);
				TextCell.Characteristic = characteristic;
				TextCell.Controller = _controller;

				// Return cell
				return TextCell;
			}

			// Error!
			return null;
		}
		#endregion
	}
}

