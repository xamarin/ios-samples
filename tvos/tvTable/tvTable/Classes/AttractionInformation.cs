using System;
using Foundation;

namespace tvTable
{
	/// <summary>
	/// This class stores information about a give attraction for a <c>CityInformation</c>
	/// instance.
	/// </summary>
	public class AttractionInformation : NSObject
	{
		#region Computed Properties
		/// <summary>
		/// Gets or sets the city that this Attraction belongs to.
		/// </summary>
		/// <value>The <c>CityInformation</c> object.</value>
		public CityInformation City { get; set;}

		/// <summary>
		/// Gets or sets the name of the Attraction.
		/// </summary>
		/// <value>The name.</value>
		public string Name { get; set;}

		/// <summary>
		/// Gets or sets a short description for the Attraction.
		/// </summary>
		/// <value>The description.</value>
		public string Description { get; set;}

		/// <summary>
		/// Gets or sets the Image Asset Name of the image that will be displayed for this Attraction.
		/// </summary>
		/// <value>The name of the image as stored in the <c>Assets.xcassets</c>.</value>
		public string ImageName { get; set;}

		/// <summary>
		/// Gets or sets a flag marking this Attraction as a favorite.
		/// </summary>
		/// <value><c>true</c> if the Attraction is a favorite, else <c>false</c>.</value>
		public bool IsFavorite { get; set;}

		/// <summary>
		/// Gets or sets a flag denoting if the user wants directions to this Attraction.
		/// </summary>
		/// <value>The add directions.</value>
		public bool AddDirections { get; set;}
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="T:tvTable.AttractionInformation"/> class.
		/// </summary>
		/// <param name="name">The name of the attraction.</param>
		/// <param name="description">A short description of the attraction.</param>
		/// <param name="imageName">The <c>Assets.xcassets</c> name of the image that will be displayed for the attraction.</param>
		public AttractionInformation (string name, string description, string imageName)
		{
			// Initialize
			this.Name = name;
			this.Description = description;
			this.ImageName = imageName;
		}
		#endregion
	}
}

