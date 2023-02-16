using System;
using System.Collections.Generic;
using Foundation;

namespace tvTable {
	/// <summary>
	/// Holds information about a given city that will be idplayed as the Sections of a Table View. Each City
	/// also contains a collection of <c>AttractionInformation</c> objects that will be the Rows in each Table
	/// View section.
	/// </summary>
	public class CityInformation : NSObject {
		#region Computed Properties
		/// <summary>
		/// Gets or sets the name of the city.
		/// </summary>
		/// <value>The name.</value>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the list of attractions.
		/// </summary>
		/// <value>A collection of <c>AttractionInformation</c> objects.</value>
		public List<AttractionInformation> Attractions { get; set; }

		/// <summary>
		/// Gets or sets a flag denoting that the user wants to book a flight to this city.
		/// </summary>
		/// <value><c>true</c> is a flight has been booked, else <c>false</c>.</value>
		public bool FlightBooked { get; set; }
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="T:tvTable.CityInformation"/> class.
		/// </summary>
		/// <param name="name">The name of the city.</param>
		public CityInformation (string name)
		{
			// Initialize
			this.Name = name;
			this.Attractions = new List<AttractionInformation> ();
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Adds the attraction to the city.
		/// </summary>
		/// <param name="attraction">The <c>AttractionInformation</c> object to add.</param>
		public void AddAttraction (AttractionInformation attraction)
		{
			// Mark as belonging to this city
			attraction.City = this;

			// Add to collection
			Attractions.Add (attraction);
		}

		/// <summary>
		/// Adds the attraction to the city.
		/// </summary>
		/// <param name="name">The name of the attraction.</param>
		/// <param name="description">A short description of the attraction.</param>
		/// <param name="imageName">The <c>Assets.xcassets</c> name of the image to display for the attraction.</param>
		public void AddAttraction (string name, string description, string imageName)
		{
			// Create attraction
			var attraction = new AttractionInformation (name, description, imageName);

			// Mark as belonging to this city
			attraction.City = this;

			// Add to collection
			Attractions.Add (attraction);
		}
		#endregion
	}
}

