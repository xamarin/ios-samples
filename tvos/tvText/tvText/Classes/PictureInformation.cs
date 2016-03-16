using System;
using Foundation;

namespace tvText
{
	/// <summary>
	/// Holds all information about a picture that will be displayed in the Search
	/// Results Collection View.
	/// </summary>
	public class PictureInformation : NSObject
	{
		#region Computed Properties
		/// <summary>
		/// Gets or sets the title.
		/// </summary>
		/// <value>The title.</value>
		public string Title { get; set;}

		/// <summary>
		/// Gets or sets the name of the image.
		/// </summary>
		/// <value>The name of the image.</value>
		/// <remarks>Images are stored in the <c>Assets.xcassets</c> collection.</remarks>
		public string ImageName { get; set;}

		/// <summary>
		/// Gets or sets the keywords.
		/// </summary>
		/// <value>The keywords.</value>
		public string Keywords { get; set;}
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="T:tvText.PictureInformation"/> class.
		/// </summary>
		/// <param name="title">The picture title.</param>
		/// <param name="imageName">The name of an image from the <c>Assets.xcassets</c> collection.</param>
		/// <param name="keywords">The Keywords used to identify this picture.</param>
		public PictureInformation (string title, string imageName, string keywords)
		{
			// Initialize
			this.Title = title;
			this.ImageName = imageName;
			this.Keywords = keywords;
		}
		#endregion
	}
}

