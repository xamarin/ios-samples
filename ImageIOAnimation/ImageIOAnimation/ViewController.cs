using Foundation;
using System;
using UIKit;

namespace ImageIOAnimation {
	class MyDelegate : UITableViewDelegate {

		ViewController viewController;

		public MyDelegate (ViewController view)
		{
			viewController = view;
		}

		public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
		{
			NSString filename = new NSString (viewController.gifNames [indexPath.Row]);
			AnimatedImage image = viewController.animatedImageForFilename (filename);
			nfloat aspectRatio = image.Size.Height / image.Size.Width;
			nfloat height = aspectRatio * viewController.View.Bounds.Size.Width;
			return height;
		}
	}

	public partial class ViewController : UIViewController, IUITableViewDataSource, IUITableViewDelegate {

		NSMutableDictionary<NSString, AnimatedImage> animatedImagesForFilenames;
		public string [] gifNames = { "earth", "elmo", "hack", "meme" };

		public ViewController (IntPtr handle) : base (handle) { }

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			// Perform any additional setup after loading the view, typically from a nib.
			UITableView tableView = new UITableView (this.View.Bounds);
			tableView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
			tableView.DataSource = this;
			tableView.Delegate = new MyDelegate (this);
			tableView.ContentInset = UIEdgeInsets.Zero;
			float Epsilon = 1.192092896e-07F;
			tableView.SeparatorInset = new UIEdgeInsets (0, Epsilon, 0, 0);
			tableView.LayoutMargins = UIEdgeInsets.Zero;
			tableView.PreservesSuperviewLayoutMargins = false;
			this.View.AddSubview (tableView);

			this.animatedImagesForFilenames = new NSMutableDictionary<NSString, AnimatedImage> ();
		}

		public AnimatedImage animatedImageForFilename (NSString filename)
		{
			AnimatedImage image = animatedImagesForFilenames [filename];
			if (image == null) {
				NSUrl url = NSBundle.MainBundle.GetUrlForResource (filename, "gif");
				//image = TJAnimatedImage.AnimatedImageWithURL (url);
				image = AnimatedImage.AnimatedImageWithData (NSData.FromUrl (url));
				CoreGraphics.CGSize size = image.Size;
				this.animatedImagesForFilenames [filename] = image;
			}
			return image;
		}

		public nint NumberOfSections (UITableView tableView)
		{
			return 1;
		}

		public nint RowsInSection (UITableView tableView, nint section)
		{
			return gifNames.Length;
		}

		public UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			const string kIdentifier = "cell";
			const int kImageViewTag = 555;
			UITableViewCell cell = tableView.DequeueReusableCell (kIdentifier);
			CustomUIImageView imageView;
			if (cell != null) {
				imageView = (CustomUIImageView)cell.ContentView.ViewWithTag (kImageViewTag);

			} else {
				cell = new UITableViewCell (UITableViewCellStyle.Default, kIdentifier);
				imageView = new CustomUIImageView (cell.ContentView.Bounds) {
					AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight,
					Tag = kImageViewTag
				};
				cell.ContentView.AddSubview (imageView);
			}

			imageView.AnimatedImage = this.animatedImageForFilename (new NSString (gifNames [indexPath.Row]));
			return cell;
		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}
	}
}