using System;
using Foundation;
using UIKit;

namespace tvTable
{
	/// <summary>
	/// Controls the <c>AttractionView</c> used to present the detailed information about a <c>AttractionInformation</c>
	/// object attached to the highlighted Row in the <c>AttractionTableView</c>.
	/// </summary>
	public partial class AttractionViewController : UIViewController
	{
		#region Private Variables
		/// <summary>
		/// The backing store for the <c>AttractionInformation</c> object that the details are being displayed for.
		/// </summary>
		private AttractionInformation _attraction = null;
		#endregion

		#region Computed Properties
		/// <summary>
		/// Gets or sets the attraction that detailed information is being didplayed for.
		/// </summary>
		/// <value>The <c>AttractionInformation</c> object.</value>
		public AttractionInformation Attraction {
			get { return _attraction; }
			set {
				_attraction = value;
				UpdateUI ();
			}
		}

		/// <summary>
		/// Gets or sets the split view that the <c>AttractionView</c> is being displayed in.
		/// </summary>
		/// <value>The <c>MasertSplitView</c> object.</value>
		public MasterSplitView SplitView { get; set;}
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="T:tvTable.AttractionViewController"/> class.
		/// </summary>
		/// <param name="handle">Handle.</param>
		public AttractionViewController (IntPtr handle) : base (handle)
		{
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Updates the user interface by populating the fields of the <c>AttractionView</c>.
		/// </summary>
		public void UpdateUI ()
		{
			// Trap all errors
			try {
				City.Text = Attraction.City.Name;
				Title.Text = Attraction.Name;
				SubTitle.Text = Attraction.Description;

				IsFlighBooked.Hidden = (!Attraction.City.FlightBooked);
				IsFavorite.Hidden = (!Attraction.IsFavorite);
				IsDirections.Hidden = (!Attraction.AddDirections);
				BackgroundImage.Image = UIImage.FromBundle (Attraction.ImageName);
				AttractionImage.Image = BackgroundImage.Image;
			} catch {
				// Since the UI might not be fully loaded, ignore
				// all errors at this point
			}
		}
		#endregion

		#region Override Methods
		/// <summary>
		/// Called before the View is presented to the user to allow it to be configured.
		/// </summary>
		/// <param name="animated"><c>true</c> if animated, else <c>false</c>.</param>
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			// Ensure the UI Updates
			UpdateUI ();
		}
		#endregion

		#region Actions
		/// <summary>
		/// Handles the user clicking the Flight button.
		/// </summary>
		/// <param name="sender">Sender.</param>
		partial void BookFlight (NSObject sender)
		{
			// Ask user to book flight
			AlertViewController.PresentOKCancelAlert ("Book Flight",
			                                          string.Format ("Would you like to book a flight to {0}?", Attraction.City.Name),
			                                          this,
			                                          (ok) => {
				Attraction.City.FlightBooked = ok;
				IsFlighBooked.Hidden = (!Attraction.City.FlightBooked);
			});
		}

		/// <summary>
		/// Handles the user clicking the Directions button.
		/// </summary>
		/// <param name="sender">Sender.</param>
		partial void GetDirections (NSObject sender)
		{
			// Ask user to add directions
			AlertViewController.PresentOKCancelAlert ("Add Directions",
													 string.Format ("Would you like to add directions to {0} to you itinerary?", Attraction.Name),
													 this,
													 (ok) => {
														 Attraction.AddDirections = ok;
														 IsDirections.Hidden = (!Attraction.AddDirections);
													 });
		}

		/// <summary>
		/// Handles the user clicking the Favorites button.
		/// </summary>
		/// <param name="sender">Sender.</param>
		partial void MarkFavorite (NSObject sender)
		{
			// Flip favorite state
			Attraction.IsFavorite = (!Attraction.IsFavorite);
			IsFavorite.Hidden = (!Attraction.IsFavorite);

			// Reload table
			SplitView.Master.TableController.TableView.ReloadData ();
		}
		#endregion
	}
}
