using System;
using Foundation;
using UIKit;

namespace tvTable
{
	/// <summary>
	/// This is the prototype for every Cell (row) that will be added to the <c>AttractionTableView</c>.
	/// </summary>
	public partial class AttractionTableCell : UITableViewCell
	{
		#region Private Variables
		/// <summary>
		/// The backing store for the <c>AttractionInfomarion</c> object being displayed in this cell.
		/// </summary>
		private AttractionInformation _attraction = null;
		#endregion

		#region Computed Properties
		/// <summary>
		/// Gets or sets the attraction being displayed in this Cell.
		/// </summary>
		/// <value>The <c>AttractionInformation</c> object.</value>
		public AttractionInformation Attraction {
			get { return _attraction; }
			set {
				_attraction = value;
				UpdateUI ();
			}
		}
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="T:tvTable.AttractionTableCell"/> class.
		/// </summary>
		/// <param name="handle">Handle.</param>
		public AttractionTableCell (IntPtr handle) : base (handle)
		{
		}
		#endregion

		#region Private Methods
		/// <summary>
		/// Updates the user interface by populating the fields of this Cell with information from the 
		/// attached attraction.
		/// </summary>
		private void UpdateUI ()
		{
			// Trap all errors
			try {
				Title.Text = Attraction.Name;
				Favorite.Hidden = (!Attraction.IsFavorite);
			} catch {
				// Since the UI might not be fully loaded, ignore
				// all errors at this point
			}
		}
		#endregion
	}
}
