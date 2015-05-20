using System;
using Foundation;
using UIKit;
using System.CodeDom.Compiler;
using HomeKit;

namespace HomeKitIntro
{
	public partial class AvailableCell : UITableViewCell
	{
		#region Static Properties
		public static readonly NSString Key = new NSString ("AvailableCell");
		#endregion

		#region Constructors
		public AvailableCell (IntPtr handle) : base (handle)
		{
		}
		#endregion

		#region Public Methods
		public void DisplayAccessory(HMAccessory accessory) {

			// Update the UI
			AvailableName.Text = accessory.Name;

			// Set icon based on the name
			if (accessory.Name.Contains ("Light")) {
				AvailableImage.Image = UIImage.FromFile ("61-brightness.png");
			} else if (accessory.Name.Contains ("Garage")) {
				AvailableImage.Image = UIImage.FromFile ("24-circle-north.png");
			} if (accessory.Name.Contains ("Thermostat")) {
				AvailableImage.Image = UIImage.FromFile ("81-dashboard.png");
			} else if (accessory.Name.Contains ("Switch")) {
				AvailableImage.Image = UIImage.FromFile ("51-power.png");
			} else if (accessory.Name.Contains ("Lock")) {
				AvailableImage.Image = UIImage.FromFile ("54-lock.png");
			}

		}
		#endregion
	}
}
