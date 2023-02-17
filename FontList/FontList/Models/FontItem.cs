using UIKit;

namespace FontList.Models {
	public class FontItem {
		/// <summary>
		/// The name of the nav item, shows up as the label
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// The font used to display the item.
		/// </summary>
		public UIFont Font { get; set; }
	}
}
