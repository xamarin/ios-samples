using System;
using Foundation;
using UIKit;
using System.CodeDom.Compiler;
using HomeKit;

namespace HomeKitIntro
{
	public partial class CharacteristicCellSlider : UITableViewCell
	{
		#region Static Properties
		public static readonly NSString Key = new NSString ("CharacteristicCellSlider");
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
		public CharacteristicCellSlider (IntPtr handle) : base (handle)
		{
		}
		#endregion

		#region Public Methods
		public void DisplayInfo(string title, float value, float min, float max, bool enabled) {

			// Update UI
			Title.Text = title;
			SubTItle.Text = value.ToString ();

			// Setup slider
			Slider.MinValue = min;
			Slider.MaxValue = max;
			Slider.Value = value;
			Slider.Enabled = enabled;

			// Wireup events
			WireupEvents ();
		}
		#endregion

		#region Private Methods
		/// <summary>
		/// Wireups the events.
		/// </summary>
		private void WireupEvents () {

			// Already wired up?
			if (_wiredup)
				return;

			// Display the new value
			Slider.ValueChanged += (sender, e) => {
				var value = Math.Round(Slider.Value,0);
				SubTItle.Text = value.ToString();
			};

			// Send change of value on end of change
			Slider.TouchUpInside += (sender, e) => {
				// Is a characteristic attached?
				if (Characteristic==null) return;

				// Set updated value to the characteristic
				var value = Math.Round(Slider.Value,0);
				Characteristic.WriteValue(NSObject.FromObject(value),(err) =>{
					// Was there an error?
					if (err!=null) {
						// Yes, inform user
						AlertView.PresentOKAlert("Update Error",err.LocalizedDescription,Controller);
					}
				});
			};

			// Mark as wired up
			_wiredup = true;

		}
		#endregion
	}
}
