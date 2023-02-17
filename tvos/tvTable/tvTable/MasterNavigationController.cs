using System;
using Foundation;
using UIKit;

namespace tvTable {
	/// <summary>
	/// Controls the <c>NavigationBar</c> for the Master side of the Split View that houses
	/// the <c>AttractionTableView</c>.
	/// </summary>
	public partial class MasterNavigationController : UINavigationController {
		#region Computed Properties
		/// <summary>
		/// Gets or sets the split view that this <c>UINavigationController</c> is hosted in.
		/// </summary>
		/// <value>The <c>MasterSplitView</c>.</value>
		public MasterSplitView SplitView { get; set; }

		/// <summary>
		/// A shortcut to the <c>AttractionTableViewController</c> that is presenting the Master
		/// view for the Split View.
		/// </summary>
		/// <value>The <c>AttractionTableViewController</c>.</value>
		public AttractionTableViewController TableController {
			get { return TopViewController as AttractionTableViewController; }
		}
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="T:tvTable.MasterNavigationController"/> class.
		/// </summary>
		/// <param name="handle">Handle.</param>
		public MasterNavigationController (IntPtr handle) : base (handle)
		{
		}
		#endregion
	}
}
