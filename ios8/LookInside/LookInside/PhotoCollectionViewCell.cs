using System;

using UIKit;

namespace LookInside
{
	public class PhotoCollectionViewCell : UICollectionViewCell
	{
		UIImageView imageView;
		public UIImage Image {
			get {
				var oldValue = UIApplication.CheckForIllegalCrossThreadCalls;
				UIApplication.CheckForIllegalCrossThreadCalls = false;
				var img = imageView.Image;
				UIApplication.CheckForIllegalCrossThreadCalls = oldValue;
				return img;
			}
			set {
				imageView.Image = value;
			}
		}

		public PhotoCollectionViewCell (IntPtr handle)
			: base(handle)
		{
			imageView = new UIImageView ();
			imageView.ContentMode = UIViewContentMode.ScaleAspectFill;

			ContentView.AddSubview (imageView);
			ContentView.ClipsToBounds = true;
		}

		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();
			imageView.Frame = ContentView.Bounds;
		}

		public override void PrepareForReuse ()
		{
			base.PrepareForReuse ();

			if (imageView.Image != null) {
				imageView.Image.Dispose ();
				imageView.Image = null;
			}
		}
	}
}

