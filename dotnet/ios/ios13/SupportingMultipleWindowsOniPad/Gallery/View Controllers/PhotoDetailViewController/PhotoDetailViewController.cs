/*
See LICENSE folder for this sample’s licensing information.

Abstract:
A view controller that contains an image view to show a particular photo.
*/

namespace Gallery;
public partial class PhotoDetailViewController : UIViewController {
	public Photo? Photo { get; set; }

	protected PhotoDetailViewController (IntPtr handle) : base (handle)
	{
	}

	public override void ViewDidLoad ()
	{
		base.ViewDidLoad ();
		// Perform any additional setup after loading the view, typically from a nib.
	}

	public override void ViewWillAppear (bool animated)
	{
		base.ViewWillAppear (animated);

		if (Photo is not null && Photo.Name is not null)
			imageView.Image = UIImage.FromFile (Photo.Name);
	}

	public override void ViewDidAppear (bool animated)
	{
		base.ViewDidAppear (animated);

		if (View is not null && View.Window.WindowScene is not null)
			View.Window.WindowScene.UserActivity = Photo?.OpenDetailUserActivity ();
	}

	public override void ViewWillDisappear (bool animated)
	{
		base.ViewWillDisappear (animated);

		if (View is not null && View.Window.WindowScene is not null)
			View.Window.WindowScene.UserActivity = null;
	}

	public static PhotoDetailViewController? LoadFromStoryboard ()
	{
		var storyboard = UIStoryboard.FromName ("Main", NSBundle.MainBundle);
		return storyboard.InstantiateViewController (nameof (PhotoDetailViewController)) as PhotoDetailViewController;
	}
}
