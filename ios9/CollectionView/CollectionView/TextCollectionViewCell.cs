using System;

using UIKit;

namespace CollectionView {
	public partial class TextCollectionViewCell : UICollectionViewCell {
		#region Computed Properties
		public string Title {
			get {
				return TextLabel.Text;
			}
			set {
				TextLabel.Text = value;
			}
		}
		#endregion

		#region Constructors
		public TextCollectionViewCell (IntPtr handle) : base (handle)
		{
		}
		#endregion
	}
}
