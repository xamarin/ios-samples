using System;
using Foundation;
using UIKit;

namespace tvTable {
	/// <summary>
	/// Controls the <c>AttractionTableView</c> used to display a collection of <c>CityInformation</c> and
	/// <c>AttractionInformation</c> objects.
	/// </summary>
	public partial class AttractionTableViewController : UITableViewController {
		#region Computed Properties
		/// <summary>
		/// Gets a shortcut to the <c>AttractionTableDatasource</c> that is providing the data for the 
		/// <c>AttractionTableView</c>.
		/// </summary>
		/// <value>The datasource.</value>
		public AttractionTableDatasource Datasource {
			get { return TableView.DataSource as AttractionTableDatasource; }
		}

		/// <summary>
		/// Gets a shortcut to the <c>AttractionTableDelegate</c> used to respond to user interaction with the
		/// <c>AttractionTableView</c>.
		/// </summary>
		/// <value>The table delegate.</value>
		public AttractionTableDelegate TableDelegate {
			get { return TableView.Delegate as AttractionTableDelegate; }
		}
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="T:tvTable.AttractionTableViewController"/> class.
		/// </summary>
		/// <param name="handle">Handle.</param>
		public AttractionTableViewController (IntPtr handle) : base (handle)
		{
		}
		#endregion

		#region Override Methods
		/// <summary>
		/// This method is called after the <c>AttractionTableView</c> has been loaded from the Storyboard
		/// to initialize it.
		/// </summary>
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Setup table
			TableView.DataSource = new AttractionTableDatasource (this);
			TableView.Delegate = new AttractionTableDelegate (this);
			TableView.ReloadData ();
		}
		#endregion
	}
}
