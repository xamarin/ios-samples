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

	PhotoSection []? photoSections;

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

		photoSections = PhotoManager.SharedInstance.Sections;
		Title = "Gallery";

		galleryCollectionView.DataSource = this;
		galleryCollectionView.Delegate = this;
		galleryCollectionView.DragDelegate = this;
		galleryCollectionView.RegisterNibForCell (UINib.FromName (GalleryCollectionViewCell.Key, null), GalleryCollectionViewCell.Key);
	}

	#endregion

	#region Internal Functionality

	Photo? GetPhoto (NSIndexPath indexPath)
	{
		return photoSections?[indexPath.Section]?.Photos?[indexPath.Row];
	}

	#endregion

	#region UICollectionView Drag Delegate

	public UIDragItem [] GetItemsForBeginningDragSession (UICollectionView collectionView, IUIDragSession session, NSIndexPath indexPath)
	{
		var selectedPhoto = GetPhoto (indexPath);

		if (selectedPhoto is null)
			throw new NullReferenceException ("selectedPhoto was null");

		var userActivity = selectedPhoto.OpenDetailUserActivity ();

		if (selectedPhoto.Name is null)
			throw new NullReferenceException ("selectedPhoto.Name was null");

		var selectedPhotoImage = UIImage.FromFile (selectedPhoto.Name);

		if (selectedPhotoImage is null)
			throw new NullReferenceException ("selectedPhotoImage was null");

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
		var selectedPhoto = GetPhoto (indexPath);

		if (selectedPhoto is null)
			throw new NullReferenceException ("selectedPhoto was null");

		var detailViewController = PhotoDetailViewController.LoadFromStoryboard ();

		if (detailViewController is null)
			throw new NullReferenceException ("detailViewController was null");

		detailViewController.Photo = selectedPhoto;
		NavigationController.PushViewController (detailViewController, true);
	}

	#endregion

	#region UICollectionView Data Source

	[Export ("numberOfSectionsInCollectionView:")]
	public nint NumberOfSections (UICollectionView collectionView)
	{
		if (photoSections is null)
			throw new NullReferenceException ("photoSections was null");

		return photoSections.Length;
	}

	public nint GetItemsCount (UICollectionView collectionView, nint section)
	{
		if (photoSections is null || photoSections[section] is null || photoSections[section].Photos is null)
			return 0;

		return photoSections![section]!.Photos!.Length;
	}


	public UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
	{
		var cell = collectionView.DequeueReusableCell (GalleryCollectionViewCell.Key, indexPath) as GalleryCollectionViewCell;

		if (cell is null)
			throw new NullReferenceException ("cell was null");

		var photo = GetPhoto (indexPath);

		if (photo is null)
			throw new NullReferenceException ("photo was null");

		if (photo.Name is null)
			throw new NullReferenceException ("photo.Name was null");

		var photoImage = UIImage.FromFile (photo.Name);

		if (photoImage is null)
			throw new NullReferenceException ("photoImage was null");

		cell.Image = photoImage;

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
