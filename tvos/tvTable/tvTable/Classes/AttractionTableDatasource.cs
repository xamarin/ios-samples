using System;
using System.Collections.Generic;
using UIKit;

namespace tvTable {
	/// <summary>
	/// Attraction table datasource defines all of the data for the Attraction Table
	/// and provides the individual cells as asked for by the table.
	/// </summary>
	public class AttractionTableDatasource : UITableViewDataSource {
		#region Constants
		/// <summary>
		/// The unique ID for the prototype cell as defined in the designer.
		/// </summary>
		const string CellID = "AttrCell";
		#endregion

		#region Computed Properties
		/// <summary>
		/// Gets or sets the a shortcut to the parent Table View Controller.
		/// </summary>
		/// <value>The <c>AttractionTableViewController</c>.</value>
		public AttractionTableViewController Controller { get; set; }

		/// <summary>
		/// Gets or sets the collection of cities.
		/// </summary>
		/// <value>A collection of <c>CityInformation</c> objects.</value>
		public List<CityInformation> Cities { get; set; }
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="T:tvTable.AttractionTableDatasource"/> class.
		/// </summary>
		/// <param name="controller">Controller.</param>
		public AttractionTableDatasource (AttractionTableViewController controller)
		{
			// Initialize
			this.Controller = controller;
			this.Cities = new List<CityInformation> ();
			PopulateCities ();
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Populates the list of cities.
		/// </summary>
		public void PopulateCities ()
		{
			// Clear existing
			Cities.Clear ();

			// Define cities and attractions
			var Paris = new CityInformation ("Paris");
			Paris.AddAttraction ("Eiffel Tower", "Is a wrought iron lattice tower on the Champ de Mars in Paris, France.", "EiffelTower");
			Paris.AddAttraction ("Musée du Louvre", "is one of the world's largest museums and a historic monument in Paris, France.", "Louvre");
			Paris.AddAttraction ("Moulin Rouge", "French for 'Red Mill', is a cabaret in Paris, France.", "MoulinRouge");
			Paris.AddAttraction ("La Seine", "Is a 777-kilometre long river and an important commercial waterway within the Paris Basin.", "RiverSeine");
			Cities.Add (Paris);

			var SanFran = new CityInformation ("San Francisco");
			SanFran.AddAttraction ("Alcatraz Island", "Is located in the San Francisco Bay, 1.25 miles (2.01 km) offshore from San Francisco.", "Alcatraz");
			SanFran.AddAttraction ("Golden Gate Bridge", "Is a suspension bridge spanning the Golden Gate strait between San Francisco Bay and the Pacific Ocean", "GoldenGateBridge");
			SanFran.AddAttraction ("San Francisco", "Is the cultural, commercial, and financial center of Northern California.", "SanFrancisco");
			SanFran.AddAttraction ("Telegraph Hill", "Is primarily a residential area, much quieter than adjoining North Beach.", "TelegraphHill");
			Cities.Add (SanFran);

			var Houston = new CityInformation ("Houston");
			Houston.AddAttraction ("City Hall", "It was constructed in 1938-1939, and is located in Downtown Houston.", "CityHall");
			Houston.AddAttraction ("Houston", "Is the most populous city in Texas and the fourth most populous city in the US.", "Houston");
			Houston.AddAttraction ("Texas Longhorn", "Is a breed of cattle known for its characteristic horns, which can extend to over 6 ft.", "LonghornCattle");
			Houston.AddAttraction ("Saturn V Rocket", "was an American human-rated expendable rocket used by NASA between 1966 and 1973.", "Rocket");
			Cities.Add (Houston);
		}
		#endregion

		#region Override Methods
		/// <summary>
		/// Gets the cell for the current Table View Row (Attraction).
		/// </summary>
		/// <returns>The <c>AttractionTableCell</c> representing the current row.</returns>
		/// <param name="tableView">Table view.</param>
		/// <param name="indexPath">Index path.</param>
		public override UITableViewCell GetCell (UITableView tableView, Foundation.NSIndexPath indexPath)
		{
			// Get cell
			var cell = tableView.DequeueReusableCell (CellID) as AttractionTableCell;

			// Populate cell
			cell.Attraction = Cities [indexPath.Section].Attractions [indexPath.Row];

			// Return new cell
			return cell;
		}

		/// <summary>
		/// Numbers the of sections (Cities).
		/// </summary>
		/// <returns>The of <c>CityInformation</c> object it the <c>Cities</c> collection.</returns>
		/// <param name="tableView">Table view.</param>
		public override nint NumberOfSections (UITableView tableView)
		{
			// Return number of cities
			return Cities.Count;
		}

		/// <summary>
		/// The number of rows (attractions) in the currently selected section (city).
		/// </summary>
		/// <returns>The number of <c>AttractionInformation</c> objects in the current <c>CityInformation</c> object.</returns>
		/// <param name="tableView">Table view.</param>
		/// <param name="section">The currently selected city.</param>
		public override nint RowsInSection (UITableView tableView, nint section)
		{
			// Return the number of attractions in the given city
			return Cities [(int) section].Attractions.Count;
		}

		/// <summary>
		/// Returns the title for the current section (city).
		/// </summary>
		/// <returns>The <c>Name</c> of the current <c>CityInformation</c> selected.</returns>
		/// <param name="tableView">Table view.</param>
		/// <param name="section">The currently selected vity.</param>
		public override string TitleForHeader (UITableView tableView, nint section)
		{
			// Get the name of the current city
			return Cities [(int) section].Name;
		}
		#endregion
	}
}

