using System;
using System.Collections.Generic;

namespace TableViewDragAndDrop {
	/// <summary>
	/// The data model used to populate the table view on app launch
	/// </summary>
	public partial class Model {
		public List<String> PlaceNames;
		public Model ()
		{
			PlaceNames = new List<string> {
			"Exploratorium",
			"SF MOMA",
			"California Academy of Sciences",
			"San Francisco Zoo",
			"Golden Gate Park",
			"De Young Museum",
			"Pier 39",
			"Aquarium of the Bay",
			"AT&T Park"
			};
		}

		/// <summary>
		/// The traditional method for rearranging rows in a table view
		/// </summary>
		public void MoveItem (int sourceIndex, int destinationIndex)
		{
			var place = PlaceNames [sourceIndex];
			PlaceNames.RemoveAt (sourceIndex);
			PlaceNames.Insert (destinationIndex, place);
		}

		/// <summary>
		/// The method for adding a new item to the table view's data model
		/// </summary>
		public void AddItem (string name, int index)
		{
			PlaceNames.Insert (index, name);
		}
	}
}
