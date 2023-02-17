using Foundation;
using System;
using UIKit;

namespace tvText {
	/// <summary>
	/// Contains the information about a <c>PictureInformation</c> object displayed in the 
	/// <c>SearchResultsViewController</c> as a Collection View.
	/// </summary>
	public partial class SearchResultViewCell : UICollectionViewCell {
		#region Private Variables
		/// <summary>
		/// The backing store for the displayed picture information.
		/// </summary>
		private PictureInformation _pictureInfo = null;
		#endregion

		#region Computed Properties
		/// <summary>
		/// Gets or sets the picture information being displayed by this cell.
		/// </summary>
		/// <value>The <c>PictureInformation</c> object.</value>
		public PictureInformation PictureInfo {
			get { return _pictureInfo; }
			set {
				_pictureInfo = value;
				UpdateUI ();
			}
		}

		/// <summary>
		/// Gets or sets the color of the label displayed under the picture.
		/// </summary>
		/// <value>The color of the text.</value>
		public UIColor TextColor {
			get { return Title.TextColor; }
			set { Title.TextColor = value; }
		}
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="T:tvText.SearchResultViewCell"/> class.
		/// </summary>
		/// <param name="handle">Handle.</param>
		public SearchResultViewCell (IntPtr handle) : base (handle)
		{
			// Initialize
			UpdateUI ();
		}
		#endregion

		#region Private Methods
		/// <summary>
		/// Updates the user interface and displays the current <c>PictureInformation</c> image
		/// and title.
		/// </summary>
		private void UpdateUI ()
		{
			// Anything to process?
			if (PictureInfo == null) return;

			try {
				Picture.Image = UIImage.FromBundle (PictureInfo.ImageName);
				Picture.AdjustsImageWhenAncestorFocused = true;
				Title.Text = PictureInfo.Title;
				TextColor = UIColor.LightGray;
			} catch {
				// Ignore errors if view isn't fully loaded
			}
		}
		#endregion
	}

}
