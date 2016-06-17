using System;
using Foundation;
using UIKit;

namespace tvTable
{
	/// <summary>
	/// Controls the Split View that acts as the main interface for the tvOS app. A <c>AttractionTableView</c> is presented
	/// in the left hand side of the Split View containing a collection of <c>CityInformation</c> and <c>AttractionInformation</c>
	/// objects. If a Row is highlighted in the <c>AttractionTableView</c>, the details of the <c>AttractionInformation</c>
	/// for that row will be presented in the <c>AttractionView</c> on the right hand side of the Split View.
	/// </summary>
	public partial class MasterSplitView : UISplitViewController
	{
		#region Computed Properties
		/// <summary>
		/// A shortcut to the <c>AttractionViewController</c> used to present the Details of the highlighted
		/// <c>AttractionInformation</c> in the <c>AttractionTableView</c>
		/// </summary>
		/// <value>The <c>AttractionViewController</c>.</value>
		public AttractionViewController Details {
			get { return ViewControllers [1] as AttractionViewController; }
		}

		/// <summary>
		/// A shortcut to the <c>MasterNavigationController</c> that houses the <c>AttractionTableView</c>. This is 
		/// presented as the Master view in the Split View.
		/// </summary>
		/// <value>The <c>MasterNavigationController</c>.</value>
		public MasterNavigationController Master {
			get { return ViewControllers [0] as MasterNavigationController; }
		}
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="T:tvTable.MasterSplitView"/> class.
		/// </summary>
		/// <param name="handle">Handle.</param>
		public MasterSplitView (IntPtr handle) : base (handle)
		{
		}
		#endregion

		#region Override Methods
		/// <summary>
		/// Called when the View is loaded so that it can be initialized.
		/// </summary>
		/// <returns>The did load.</returns>
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Initialize
			Master.SplitView = this;
			Details.SplitView = this;

			// Wire-up events
			Master.TableController.TableDelegate.AttractionHighlighted += (attraction) => {
				// Display new attraction
				Details.Attraction = attraction;
			};
		}
		#endregion
	}
}
