using System;
using Foundation;
using UIKit;
using System.CodeDom.Compiler;
using HomeKit;

namespace HomeKitIntro
{
	public partial class AvailableTableViewController : UITableViewController
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
		public AvailableTableSource DataSource {
			get { return (AvailableTableSource)TableView.Source; }
		}

		/// <summary>
		/// Gets or sets the index of the accessory being edited.
		/// </summary>
		/// <value>The index of the accessory.</value>
		public int AccessoryIndex { get; set; }

		/// <summary>
		/// Gets or sets the accessory browser.
		/// </summary>
		/// <value>The accessory browser.</value>
		public HMAccessoryBrowser AccessoryBrowser { get; set; }
		#endregion

		#region Constructors
		public AvailableTableViewController (IntPtr handle) : base (handle)
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
		/// <Docs>Called after the object has been loaded from the nib file. Overriders must call base.AwakeFromNib().</Docs>
		/// <summary>
		/// Awakes from nib.
		/// </summary>
		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();

			// Create a new accessory browser
			AccessoryBrowser = new HMAccessoryBrowser ();

			// Wireup changes
			AccessoryBrowser.DidFindNewAccessory += (sender, e) => {
				// Update display
				ReloadData();
			};

			AccessoryBrowser.DidRemoveNewAccessory += (sender, e) => {
				// Update display
				ReloadData();

				// Inform the rest of the UI that it needs to refresh
				ThisApp.RaiseUpdateGUI();
			};
		}

		/// <summary>
		/// Views the did load.
		/// </summary>
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Register the tableview's datasource
			TableView.Source = new AvailableTableSource (this);

		}

		/// <summary>
		/// Views the will appear.
		/// </summary>
		/// <param name="animated">If set to <c>true</c> animated.</param>
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			// Ask the browser to start searching for new items
			AccessoryBrowser.StartSearchingForNewAccessories ();
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);

			// Ask the accessory browser to stop seraching for new accessories
			AccessoryBrowser.StopSearchingForNewAccessories ();
		}
		#endregion
	}
}
