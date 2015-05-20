using System;
using Foundation;
using UIKit;
using System.CodeDom.Compiler;

namespace HomeKitIntro
{
	public partial class AccessoryTableViewController : UITableViewController
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
		public AccessoryTableSource DataSource {
			get { return (AccessoryTableSource)TableView.Source; }
		}

		/// <summary>
		/// Gets or sets the index of the accessory being edited.
		/// </summary>
		/// <value>The index of the accessory.</value>
		public int AccessoryIndex { get; set; }
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="HomeKitIntro.AccessoryTableViewController"/> class.
		/// </summary>
		/// <param name="handle">Handle.</param>
		public AccessoryTableViewController (IntPtr handle) : base (handle)
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

		#region Private Methods
		/// <summary>
		/// Sets the name of the home.
		/// </summary>
		private void SetHomeName() {
			// Was a primary home found?
			if (ThisApp.HomeManager.PrimaryHome == null) {
				// No,
				Title = "No Home";
			} else {
				// Yes, display its name
				Title = ThisApp.HomeManager.PrimaryHome.Name;

				// Wireup home view
				ThisApp.HomeManager.PrimaryHome.DidAddAccessory += (sender, e) => {
					// Update list
					ReloadData();
				};

				ThisApp.HomeManager.PrimaryHome.DidRemoveAccessory += (sender, e) => {
					// Update list
					ReloadData();
				};
			}
		}

		#endregion

		#region Override Methods
		/// <summary>
		/// Views the did load.
		/// </summary>
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Register the tableview's datasource
			TableView.Source = new AccessoryTableSource (this);

			// Wireup events
			ThisApp.HomeManager.DidUpdateHomes += (sender, e) => {
				// Update title and reload data
				SetHomeName();
				ReloadData();
			};

			ThisApp.HomeManager.DidUpdatePrimaryHome += (sender, e) => {
				// Update title and reload data
				SetHomeName();
				ReloadData();
			};

			ThisApp.HomeManager.DidRemoveHome += (sender, e) => {
				// Update title and reload data
				SetHomeName();
				ReloadData();
			};

			ThisApp.UpdateGUI += () => {
				// Update list of items
				ReloadData();
			};
		}

		/// <summary>
		/// Prepares for segue.
		/// </summary>
		/// <param name="segue">Segue.</param>
		/// <param name="sender">Sender.</param>
		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			base.PrepareForSegue (segue, sender);

			// Get the services view controller
			var view = segue.DestinationViewController as ServiceTableViewController;

			// Send over the accessory to display
			view.Accessory = ThisApp.HomeManager.PrimaryHome.Accessories [AccessoryIndex];
		}
		#endregion
	}
}
