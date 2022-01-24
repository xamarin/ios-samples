using CoreFoundation;

namespace DragBoard;

/// <summary>
/// A view controller that supports pasting and installs drag and drop interactions on its view.
/// It also provides helper functions used by paste and drop operations to load and display images in the pin board.
/// </summary>
public partial class DragBoardViewController : UIViewController
{
	List<UIImage> Images { get; set; } = new List<UIImage> ();
	List<UIView> Views { get; set; } = new List<UIView> ();
	// A property that keeps track of the location where the drop operation was performed.
	public CGPoint DropPoint { get; set; } = new CGPoint (0, 0);
	public CGPoint MovePoint { get; set; } = new CGPoint (0, 0);

	public DragBoardViewController (IntPtr handle) : base (handle)
	{
	}

	public override void ViewDidLoad ()
	{
		base.ViewDidLoad ();

		SetupPasteMenu ();

		// Set a paste configuration
		PasteConfiguration = new UIPasteConfiguration (typeof (UIImage));

		if (View is null)
			return;

		// Add drag interaction
		View.AddInteraction (new UIDragInteraction (this));

		// Add drag interaction
		View.AddInteraction (new UIDropInteraction (this));
	}

	public void PasteItemProviders (NSItemProvider[] itemProviders)
	{
		if (itemProviders is not null) {
			foreach (var item in itemProviders) {
				LoadImage (item, DropPoint);
			}
		}
	}

	// Asynchronously loads an image from the given item provider and
	// displays it it in the image view.
	//
	// - Parameters:
	//   - itemProvider: an item provider that can load an image.
	//   - imageView: the image view that will display the loaded image.
	public void LoadImage (NSItemProvider itemProvider, CGPoint center)
	{
		var progress = itemProvider.LoadObject<UIImage> ( (droppedImage, err) =>
		{
			if (droppedImage is UIImage image) {
				DispatchQueue.MainQueue.DispatchAsync ( ()=>{
					var imageView = CreateNewImageView (image);
					imageView.Center = center;
					Images.Add (image);
				});
			}
		});
	}

	// Creates a new image view with the given image and
	// scales it down if it exceeds a maximum size.
	//
	// - Parameter image: the image to be displayed in the image view.
	// - Returns: A newly created image view with the given image, resized if necessary.
	public UIImageView CreateNewImageView (UIImage image) {

		var imageView = new UIImageView () {
			Image = image,
			ContentMode = UIViewContentMode.ScaleAspectFit,
			UserInteractionEnabled = true
		};
		var size = image.Size;
		var longestSide = (float)Math.Max (size.Width, size.Height);
		var maximumLength = 200f;
		var scaleFactor = 1f;

		// If the given image exceeds `maximumLength`,
		// we resize the image view to match that length
		// while preserving the original aspect ratio.
		if (longestSide > maximumLength ) {
			scaleFactor = maximumLength / longestSide;
		}
		size = new CGSize ( (float)Math.Round (size.Width * scaleFactor), (float)Math.Round (size.Height * scaleFactor));
		imageView.Frame = new CGRect (imageView.Frame.Location, size);

		if (View is not null) {
			Views.Add (imageView);
			View.AddSubview (imageView);
		}
		return imageView;
	}

	// Changes the alpha value of drag items already
	// inserted in the pin board.
	//
	// - Parameters:
	//   - items: the list of drag items.
	//   - alpha: the alpha value applied to each drag item.
	public void FadeItems (UIDragItem[] items, float alpha) {
		foreach (UIDragItem item in items) {
			if (item.LocalObject is NSNumber index) {
				Views[index.Int32Value].Alpha = alpha;
			}
		}
	}
}
