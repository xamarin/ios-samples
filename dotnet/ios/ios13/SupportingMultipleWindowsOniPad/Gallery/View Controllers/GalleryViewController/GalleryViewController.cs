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

	Photo GetPhoto (NSIndexPath indexPath) => photoSections! [indexPath.Section]!.Photos! [indexPath.Row];

	#endregion

	#region UICollectionView Drag Delegate

	public UIDragItem [] GetItemsForBeginningDragSession (UICollectionView collectionView, IUIDragSession session, NSIndexPath indexPath)
	{
		var selectedPhoto = GetPhoto (indexPath);

		var userActivity = selectedPhoto!.OpenDetailUserActivity ();
		var itemProvider = new NSItemProvider (UIImage.FromFile (selectedPhoto!.Name!)!);
		itemProvider.RegisterObject (userActivity, NSItemProviderRepresentationVisibility.All);

		var dragItem = new UIDragItem (itemProvider) {
			LocalObject = selectedPhoto
		};

		return new [] { dragItem };
	}

	#endregion

	#region UICollectionView Delegate

	[Export ("collectionView:didSelectItemAtIndexPath:")]
	public void ItemSelected (UICollectionView collectionView, NSIndexPath indexPath)
	{
		var selectedPhoto = GetPhoto (indexPath);

		var detailViewController = PhotoDetailViewController.LoadFromStoryboard ();
		detailViewController!.Photo = selectedPhoto;
		NavigationController.PushViewController (detailViewController, true);
	}

	#endregion

	#region UICollectionView Data Source

	[Export ("numberOfSectionsInCollectionView:")]
	public nint NumberOfSections (UICollectionView collectionView) => photoSections!.Length;

	public nint GetItemsCount (UICollectionView collectionView, nint section) => photoSections! [section]!.Photos!.Length;

	public UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
	{
		var cell = collectionView.DequeueReusableCell (GalleryCollectionViewCell.Key, indexPath) as GalleryCollectionViewCell;

		var photo = GetPhoto (indexPath);
		cell!.Image = UIImage.FromFile (photo!.Name!)!;

		return cell;
	}

	#endregion

	#region MyRegion

	nfloat itemsPerRow = 5;
	nfloat spacing = 20;

	[Export ("collectionView:layout:sizeForItemAtIndexPath:")]
	public CGSize GetSizeForItem (UICollectionView collectionView, UICollectionViewLayout layout, NSIndexPath indexPath)
	{
		var totalSpacing = (2 * spacing) + ((itemsPerRow - 1) * spacing);
		var width = (collectionView.Bounds.Width - totalSpacing) / itemsPerRow;
		return new CGSize (width, width);
	}

	[Export ("collectionView:layout:insetForSectionAtIndex:")]
	public UIEdgeInsets GetInsetForSection (UICollectionView collectionView, UICollectionViewLayout layout, nint section)
		=> new UIEdgeInsets (spacing, spacing, spacing, spacing);

	[Export ("collectionView:layout:minimumLineSpacingForSectionAtIndex:")]
	public nfloat GetMinimumLineSpacingForSection (UICollectionView collectionView, UICollectionViewLayout layout, nint section)
		=> spacing;

	[Export ("collectionView:layout:minimumInteritemSpacingForSectionAtIndex:")]
	public nfloat GetMinimumInteritemSpacingForSection (UICollectionView collectionView, UICollectionViewLayout layout, nint section)
		=> spacing;

	#endregion
}
