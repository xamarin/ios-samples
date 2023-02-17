using System;
using Foundation;
using UIKit;
using System.CodeDom.Compiler;
using HomeKit;

namespace HomeKitIntro {
	public partial class CharacteristicCellStepper : UITableViewCell {
		#region Static Properties
		public static readonly NSString Key = new NSString ("CharacteristicCellStepper");
		#endregion

		#region Private Variables
		private bool _wiredup = false;
		#endregion

		#region Computed Properties
		/// <summary>
		/// Gets or sets the characteristic.
		/// </summary>
		/// <value>The characteristic.</value>
		public HMCharacteristic Characteristic { get; set; }

		/// <summary>
		/// Gets or sets the controller.
		/// </summary>
		/// <value>The controller.</value>
		public CharacteristicTableViewController Controller { get; set; }
		#endregion

		#region Constructors
		public CharacteristicCellStepper (IntPtr handle) : base (handle)
		{
		}
		#endregion

		#region Public Methods
		public void DisplayInfo (string title, float value, float min, float max, bool enabled)
		{

			// Update UI
			Title.Text = title;
			SubTitle.Text = value.ToString ();

			// Setup slider
			Stepper.MinimumValue = min;
			Stepper.MaximumValue = max;
			Stepper.Value = value;
			Stepper.Enabled = enabled;

			// Wireup events
			WireupEvents ();
		}
		#endregion

		#region Private Methods
		/// <summary>
		/// Wireups the events.
		/// </summary>
		private void WireupEvents ()
		{

			// Already wired up?
			if (_wiredup)
				return;

			// Display the new value
			Stepper.ValueChanged += (sender, e) => {
				SubTitle.Text = Stepper.Value.ToString ();

				// Set updated value to the characteristic
				Characteristic.WriteValue (NSObject.FromObject (Stepper.Value), (err) => {
					// Was there an error?
					if (err != null) {
						// Yes, inform user
						AlertView.PresentOKAlert ("Update Error", err.LocalizedDescription, Controller);
					}
				});
			};

			// Mark as wired up
			_wiredup = true;

		}
		#endregion
	}
}
