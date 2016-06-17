using System;

namespace UIKitEnhancements
{
	/// <summary>
	/// Holds information about a give item on a table-based menu
	/// </summary>
	public class MenuItem
	{
		#region Computed Properties
		/// <summary>
		/// Gets or sets the title.
		/// </summary>
		/// <value>The title.</value>
		public string Title { get; set; }

		/// <summary>
		/// Gets or sets the subtitle.
		/// </summary>
		/// <value>The subtitle.</value>
		public string Subtitle{ get; set; }

		/// <summary>
		/// Gets or sets the segue.
		/// </summary>
		/// <value>The segue.</value>
		public string Segue{ get; set; }

		/// <summary>
		/// Gets or sets the URL.
		/// </summary>
		/// <value>The UR.</value>
		public string URL{ get; set; }
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="UIKitEnhancements.MenuItem"/> class.
		/// </summary>
		public MenuItem ()
		{
			// Initialize
			Title = "";
			Subtitle = "";
			Segue = "";
			URL = "";
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="UIKitEnhancements.MenuItem"/> class.
		/// </summary>
		/// <param name="title">Title.</param>
		/// <param name="subtitle">Subtitle.</param>
		public MenuItem(string title, string subtitle, string segue) {

			// Initialize
			Title = title;
			Subtitle = subtitle;
			Segue = segue;
			URL = "";
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="UIKitEnhancements.MenuItem"/> class.
		/// </summary>
		/// <param name="title">Title.</param>
		/// <param name="subtitle">Subtitle.</param>
		/// <param name="segue">Segue.</param>
		/// <param name="url">URL.</param>
		public MenuItem(string title, string subtitle, string segue, string url) {

			// Initialize
			Title = title;
			Subtitle = subtitle;
			Segue = segue;
			URL = url;
		}
		#endregion
	}
}

