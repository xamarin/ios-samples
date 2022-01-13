/*
See LICENSE folder for this sample’s licensing information.

Abstract:
A view controller that displays a collection of photos.
 It also shows how to create a new scene session via Drag and Drop.
*/

using ObjCRuntime;

namespace Gallery;
public partial class GalleryViewController : UIViewController, IUICollectionViewDataSource, IUICollectionViewDelegate, IUICollectionViewDragDelegate, IUICollectionViewDelegateFlowLayout {
	#region Data

	PhotoSection [] PhotoSections = Array.Empty<PhotoSection> ();

	#endregion

	#region Constructors

	protected GalleryViewController (IntPtr handle) : base (handle)
	{
	}

	#endregion

	#region View controller lifecycle

	public override void ViewDidLoad ()
	{
		base.ViewDidLoad ();
		// Perform any additional setup after loading the view, typically from a nib.

		PhotoSections = PhotoManager.SharedInstance.Sections;
		Title = "Gallery";

		galleryCollectionView.DataSource = this;
		galleryCollectionView.Delegate = this;
		galleryCollectionView.DragDelegate = this;
		galleryCollectionView.RegisterNibForCell (UINib.FromName (GalleryCollectionViewCell.Key, null), GalleryCollectionViewCell.Key);
	}

	#endregion

	#region Internal Functionality

	bool TryGetPhoto (NSIndexPath indexPath, out Photo photo)
	{
		if (PhotoSections[indexPath.Section]?.Photos?[indexPath.Row] is null)
		{
			photo = new Photo ();
			return false;
		}

		photo = PhotoSections[indexPath.Section]!.Photos![indexPath.Row];
		return true;
	}

	#endregion

	#region UICollectionView Drag Delegate

	public UIDragItem [] GetItemsForBeginningDragSession (UICollectionView collectionView, IUIDragSession session, NSIndexPath indexPath)
	{
		if (!TryGetPhoto (indexPath, out var selectedPhoto))
			throw new InvalidOperationException (nameof (selectedPhoto));

		var userActivity = selectedPhoto.OpenDetailUserActivity ();

		if (selectedPhoto.Name is null)
			throw new InvalidOperationException (nameof (selectedPhoto.Name));

		var selectedPhotoImage = UIImage.FromFile (selectedPhoto.Name);

		if (selectedPhotoImage is null)
			throw new InvalidOperationException (nameof (selectedPhotoImage));

		var itemProvider = new NSItemProvider (selectedPhotoImage);

		itemProvider.RegisterObject (userActivity, NSItemProviderRepresentationVisibility.All);

		var dragItem = new UIDragItem (itemProvider)
		{
			LocalObject = selectedPhoto
		};

		return new[] { dragItem };
	}

	#endregion

	#region UICollectionView Delegate

	[Export ("collectionView:didSelectItemAtIndexPath:")]
	public void ItemSelected (UICollectionView collectionView, NSIndexPath indexPath)
	{
		if (!TryGetPhoto (indexPath, out var selectedPhoto))
			return;

		var detailViewController = PhotoDetailViewController.LoadFromStoryboard ();

		if (detailViewController is null)
			throw new InvalidOperationException (nameof (detailViewController));

		detailViewController.Photo = selectedPhoto;
		NavigationController.PushViewController (detailViewController, true);
	}

	#endregion

	#region UICollectionView Data Source

	[Export ("numberOfSectionsInCollectionView:")]
	public nint NumberOfSections (UICollectionView collectionView)
	{
		return PhotoSections.Length;
	}

	public nint GetItemsCount (UICollectionView collectionView, nint section)
	{
		return PhotoSections[section]?.Photos?.Length ?? 0;
	}

	public UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
	{
		var cell = collectionView.DequeueReusableCell (GalleryCollectionViewCell.Key, indexPath) as GalleryCollectionViewCell;

		if (cell is null)
			throw new InvalidOperationException (nameof (cell));

		if (!TryGetPhoto (indexPath, out var photo))
			throw new InvalidOperationException (nameof (photo));

		if (photo.Name is null)
			throw new InvalidOperationException (nameof (photo.Name));

		cell.Image = UIImage.FromFile (photo.Name) ?? throw new InvalidOperationException (nameof (cell.Image));

		return cell;
	}

	#endregion

	#region MyRegion

	nfloat ItemsPerRow = 5;
	nfloat Spacing = 20;

	[Export ("collectionView:layout:sizeForItemAtIndexPath:")]
	public CGSize GetSizeForItem (UICollectionView collectionView, UICollectionViewLayout layout, NSIndexPath indexPath)
	{
		var totalSpacing = (2 * Spacing) + ( (ItemsPerRow - 1) * Spacing);
		var width = (collectionView.Bounds.Width - totalSpacing) / ItemsPerRow;
		return new CGSize (width, width);
	}

	[Export ("collectionView:layout:insetForSectionAtIndex:")]
	public UIEdgeInsets GetInsetForSection (UICollectionView collectionView, UICollectionViewLayout layout, nint section)
		=> new UIEdgeInsets (Spacing, Spacing, Spacing, Spacing);

	[Export ("collectionView:layout:minimumLineSpacingForSectionAtIndex:")]
	public nfloat GetMinimumLineSpacingForSection (UICollectionView collectionView, UICollectionViewLayout layout, nint section)
		=> Spacing;

	[Export ("collectionView:layout:minimumInteritemSpacingForSectionAtIndex:")]
	public nfloat GetMinimumInteritemSpacingForSection (UICollectionView collectionView, UICollectionViewLayout layout, nint section)
		=> Spacing;

	#endregion
}
