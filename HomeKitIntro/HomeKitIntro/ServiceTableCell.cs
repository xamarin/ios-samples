using System;
using Foundation;
using UIKit;
using System.CodeDom.Compiler;
using HomeKit;

namespace HomeKitIntro
{
	public partial class ServiceTableCell : UITableViewCell
	{
		#region Static Properties
		public static readonly NSString Key = new NSString ("ServiceTableCell");
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="HomeKitIntro.ServiceTableCell"/> class.
		/// </summary>
		/// <param name="handle">Handle.</param>
		public ServiceTableCell (IntPtr handle) : base (handle)
		{
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Displaies the information.
		/// </summary>
		/// <param name="title">Title.</param>
		/// <param name="subtitle">Subtitle.</param>
		public void DisplayInformation(string title, string subtitle) {

			// Update GUI
			Title.Text = title;
			SubTitle.Text = subtitle;

		}

		/// <summary>
		/// Displaies the service.
		/// </summary>
		/// <param name="name">Name.</param>
		/// <param name="type">Type.</param>
		public void DisplayService(string name, HMServiceType type) {

			// Update GUI
			Title.Text = name;

			// Take action based on type
			switch (type) {
			case HMServiceType.GarageDoorOpener:
				SubTitle.Text = "Garage Door Opener";
				break;
			case HMServiceType.LightBulb:
				SubTitle.Text = "Light Bulb";
				break;
			case HMServiceType.LockManagement:
			case HMServiceType.LockMechanism:
				SubTitle.Text = "Lock";
				break;
			case HMServiceType.None:
				SubTitle.Text = "None";
				break;
			case HMServiceType.Switch:
				SubTitle.Text = "Switch";
				break;
			case HMServiceType.Thermostat:
				SubTitle.Text = "Thermostat";
				break;
			}
		}
		#endregion
	}
}
