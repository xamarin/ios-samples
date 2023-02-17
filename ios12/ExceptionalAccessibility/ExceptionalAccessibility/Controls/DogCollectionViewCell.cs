
namespace ExceptionalAccessibility {
	using System;
	using UIKit;

	/// <summary>
	/// Subclass of `UICollectionViewCell`; represents a particular dog in the collection view.
	/// </summary>
	public partial class DogCollectionViewCell : UICollectionViewCell {
		public DogCollectionViewCell (IntPtr handle) : base (handle) { }
	}
}
