using System;
using Foundation;
using UIKit;
using System.CodeDom.Compiler;
using HomeKit;

namespace HomeKitIntro
{
	public partial class CharacteristicTableViewController : UITableViewController
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
		public CharacteristicTableSource DataSource {
			get { return (CharacteristicTableSource)TableView.Source; }
		}

		/// <summary>
		/// Gets or sets the service.
		/// </summary>
		/// <value>The service.</value>
		public HMService Service { get; set; }
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="HomeKitIntro.CharacteristicTableViewController"/> class.
		/// </summary>
		/// <param name="handle">Handle.</param>
		public CharacteristicTableViewController (IntPtr handle) : base (handle)
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
		/// Views the did load.
		/// </summary>
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Register the tableview's datasource
			TableView.Source = new CharacteristicTableSource (this);

			// Display service name
			Title = Service.Name;

		}
		#endregion
	}
}
