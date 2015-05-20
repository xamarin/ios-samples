using System;
using Foundation;
using UIKit;
using System.CodeDom.Compiler;
using HomeKit;

namespace HomeKitIntro
{
	public partial class CharacteristicCellText : UITableViewCell
	{
		#region Static Properties
		public static readonly NSString Key = new NSString ("CharacteristicCellText");
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
		public CharacteristicCellText (IntPtr handle) : base (handle)
		{
		}
		#endregion

		#region Public Methods
		public void DisplayInfo(string title, string value, bool enabled) {

			// Update UI
			TItle.Text = title;
			SubTitle.Text = "Enter new value";

			// Setup switch
			Text.Text = value;
			Text.Enabled = enabled;

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

			// Wire text field
			Text.ShouldReturn += (UITextField field) => {
				field.ResignFirstResponder();

				// Set updated value to the characteristic
				Characteristic.WriteValue(NSObject.FromObject(Text.Text),(err) =>{
					// Was there an error?
					if (err!=null) {
						// Yes, inform user
						AlertView.PresentOKAlert("Update Error",err.LocalizedDescription,Controller);
					}
				});

				return true;
			};

			Text.ShouldEndEditing += (UITextField field) => {
				field.ResignFirstResponder();

				// Set updated value to the characteristic
				Characteristic.WriteValue(NSObject.FromObject(Text.Text),(err) =>{
					// Was there an error?
					if (err!=null) {
						// Yes, inform user
						AlertView.PresentOKAlert("Update Error",err.LocalizedDescription,Controller);
					}
				});

				return true;
			};

			// Mark as wired up
			_wiredup = true;

		}
		#endregion
	}
}
