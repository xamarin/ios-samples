using System;
using Foundation;
using UIKit;
using System.CodeDom.Compiler;
using HomeKit;

namespace HomeKitIntro
{
	public partial class AccessoryCell : UITableViewCell
	{
		#region Static Properties
		public static readonly NSString Key = new NSString ("AccessoryCell");
		#endregion

		#region Constructors
		public AccessoryCell (IntPtr handle) : base (handle)
		{
		}
		#endregion

		#region Public Methods
		public void DisplayAccessory(HMAccessory accessory) {

			// Update the UI
			AccessoryName.Text = accessory.Name;

			// Set icon based on the name
			if (accessory.Name.Contains ("Light")) {
				AccessoryImage.Image = UIImage.FromFile ("61-brightness.png");
			} else if (accessory.Name.Contains ("Garage")) {
				AccessoryImage.Image = UIImage.FromFile ("24-circle-north.png");
			} if (accessory.Name.Contains ("Thermostat")) {
				AccessoryImage.Image = UIImage.FromFile ("81-dashboard.png");
			} else if (accessory.Name.Contains ("Switch")) {
				AccessoryImage.Image = UIImage.FromFile ("51-power.png");
			} else if (accessory.Name.Contains ("Lock")) {
				AccessoryImage.Image = UIImage.FromFile ("54-lock.png");
			}

		}
		#endregion
	}
}
