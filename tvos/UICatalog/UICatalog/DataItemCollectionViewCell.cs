using Foundation;
using UIKit;

namespace UICatalog {
	public partial class DataItemCollectionViewCell : UICollectionViewCell {

		[Export ("reuseIdentifier")]
		public static new string ReuseIdentifier => "DataItemCell";

		public DataItem RepresentedDataItem { get; set; }

		[Export ("initWithCoder:")]
		public DataItemCollectionViewCell (NSCoder coder): base (coder)
		{
		}

		public override void AwakeFromNib ()
		{
			ImageView.AdjustsImageWhenAncestorFocused = true;
			ImageView.ClipsToBounds = false;
			Label.Alpha = 0f;
		}

		public override void PrepareForReuse ()
		{
			base.PrepareForReuse ();
			Label.Alpha = 0f;
		}

		public override void DidUpdateFocus (UIFocusUpdateContext context, UIFocusAnimationCoordinator coordinator)
		{
			coordinator.AddCoordinatedAnimations (() => {
				Label.Alpha = Focused ? 0f : 1f;
			}, null);
		}
	}
}

