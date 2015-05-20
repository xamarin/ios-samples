using System;
using Foundation;
using UIKit;
using System.CodeDom.Compiler;
using HomeKit;

namespace HomeKitIntro
{
	public partial class ServiceTableViewController : UITableViewController
	{
		#region Computed Properties
		/// <summary>
		/// Returns the delegate of the current running application
		/// </summary>
		/// <value>The this app.</value>
		public AppDelegate ThisApp {
			get { return (AppDelegate)UIApplication.SharedApplication.Delegate; }
		}

		/// <summary>
		/// Gets the data source.
		/// </summary>
		/// <value>The data source.</value>
		public ServiceTableSource DataSource {
			get { return (ServiceTableSource)TableView.Source; }
		}

		/// <summary>
		/// Gets or sets the accessory being displayed
		/// </summary>
		/// <value>The accessory.</value>
		public HMAccessory Accessory { get; set; }

		/// <summary>
		/// Gets or sets the index of the service.
		/// </summary>
		/// <value>The index of the service.</value>
		public int ServiceIndex { get; set; }
		#endregion

		#region Constructors
		public ServiceTableViewController (IntPtr handle) : base (handle)
		{
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Reloads the data.
		/// </summary>
		public void ReloadData() {

			// Ask the table to redisplay its information
			TableView.ReloadData ();
		}
		#endregion

		#region Override Methods
		/// <summary>
		/// Views the will appear.
		/// </summary>
		/// <param name="animated">If set to <c>true</c> animated.</param>
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			// Adjust title
			Title = Accessory.Name;

			// Register the tableview's datasource
			TableView.Source = new ServiceTableSource (this);

		}

		/// <summary>
		/// Prepares for segue.
		/// </summary>
		/// <param name="segue">Segue.</param>
		/// <param name="sender">Sender.</param>
		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			base.PrepareForSegue (segue, sender);

			// Take action based on the segue name
			switch (segue.Identifier) {
			case "CharacteristicSegue":
				// Grab controller
				var characteristicView = segue.DestinationViewController as CharacteristicTableViewController;

				// Pass in service
				characteristicView.Service = Accessory.Services [ServiceIndex];
				break;
			}
		}
		#endregion
	}
}
