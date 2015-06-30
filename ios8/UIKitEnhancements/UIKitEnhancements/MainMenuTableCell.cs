using System;
using Foundation;
using UIKit;
using System.CodeDom.Compiler;

namespace UIKitEnhancements
{
	public partial class MainMenuTableCell : UITableViewCell
	{
		#region Static Properties
		public static readonly NSString Key = new NSString ("MainMenuTableCell");
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="UIKitEnhancements.MainMenuTableCell"/> class.
		/// </summary>
		/// <param name="handle">Handle.</param>
		public MainMenuTableCell (IntPtr handle) : base (handle)
		{
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Displays the menu item.
		/// </summary>
		/// <param name="item">Item.</param>
		public void DisplayMenuItem(MenuItem item) {

			// Update GUI
			Title.Text = item.Title;
			Subtitle.Text = item.Subtitle;
		}
		#endregion
	}
}
