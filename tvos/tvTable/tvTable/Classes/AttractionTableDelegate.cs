using System;
using System.Collections.Generic;
using UIKit;

namespace tvTable
{
	/// <summary>
	/// The <c>AttractionTableDelegate</c> handles the user interactions on the <c>AttractionTable</c> such
	/// as highlighting or selecting a row.
	/// </summary>
	public class AttractionTableDelegate : UITableViewDelegate
	{
		#region Computed Properties
		/// <summary>
		/// Gets or sets a shortcut to the Table View Controller.
		/// </summary>
		/// <value>The <c>ActionTableViewController</c>.</value>
		public AttractionTableViewController Controller { get; set;}
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="T:tvTable.AttractionTableDelegate"/> class.
		/// </summary>
		/// <param name="controller">The <c>ActionTableViewController</c>.</param>
		public AttractionTableDelegate (AttractionTableViewController controller)
		{
			// Initializw
			this.Controller = controller;
		}
		#endregion

		#region Override Methods
		/// <summary>
		/// The method handles a Row being selected in the Table View.
		/// </summary>
		/// <param name="tableView">The parent Table view.</param>
		/// <param name="indexPath">An Index path representing the Section and the Row selected.</param>
		/// <remarks>A row is selected when the user clicks the touch area of the Apple Remote.</remarks>
		public override void RowSelected (UITableView tableView, Foundation.NSIndexPath indexPath)
		{
			var attraction = Controller.Datasource.Cities [indexPath.Section].Attractions [indexPath.Row];
			attraction.IsFavorite = (!attraction.IsFavorite);

			// Update UI
			Controller.TableView.ReloadData ();
		}

		/// <summary>
		/// The method determins if a given Row in the Table View can gain Focus.
		/// </summary>
		/// <returns><c>true</c> if the Row can gain Focus, else <c>false</c>.</returns>
		/// <param name="tableView">The parent Table view.</param>
		/// <param name="indexPath">An Index path representing the Section and the Row being tested for focus.</param>
		/// <remarks>This routine is being used to update the Detail view of the Split View when the user moves through
		/// the list of items in the Table View. Since all rows of the table are valid, we always return <c>true</c>
		/// so the user can Focus on each row.</remarks>
		public override bool CanFocusRow (UITableView tableView, Foundation.NSIndexPath indexPath)
		{
			// Inform caller of highlight change
			RaiseAttractionHighlighted (Controller.Datasource.Cities [indexPath.Section].Attractions [indexPath.Row]);
			return true;
		}
		#endregion

		#region Events
		/// <summary>
		/// Attraction highlighted delegate.
		/// </summary>
		public delegate void AttractionHighlightedDelegate (AttractionInformation attraction);

		/// <summary>
		/// Occurs when attraction highlighted.
		/// </summary>
		public event AttractionHighlightedDelegate AttractionHighlighted;

		/// <summary>
		/// Raises the attraction highlighted event.
		/// </summary>
		/// <param name="attraction">The <c>AttractionInformation</c> object for the highlighted row.</param>
		internal void RaiseAttractionHighlighted (AttractionInformation attraction)
		{
			// Inform caller
			if (this.AttractionHighlighted != null) this.AttractionHighlighted (attraction);
		}
		#endregion
	}
}

