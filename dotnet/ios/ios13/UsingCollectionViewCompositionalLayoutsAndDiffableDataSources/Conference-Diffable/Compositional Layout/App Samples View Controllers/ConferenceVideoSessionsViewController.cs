/*
See LICENSE folder for this sample’s licensing information.

Abstract:
Sample showing how we might build the videos sessions UI
*/

using Conference_Diffable.CompositionalLayout.Controllers;

namespace Conference_Diffable.CompositionalLayout.AppSamplesViewControllers;

public partial class ConferenceVideoSessionsViewController : UIViewController {
	static readonly string titleElementKind = nameof (titleElementKind);

	ConferenceVideoController? videosController;
	UICollectionView? collectionView;
	UICollectionViewDiffableDataSource<ConferenceVideoController.VideoCollection, ConferenceVideoController.Video>? dataSource;
	NSDiffableDataSourceSnapshot<ConferenceVideoController.VideoCollection, ConferenceVideoController.Video>? currentSnapshot;

	public override void ViewDidLoad ()
	{
		base.ViewDidLoad ();
		// Perform any additional setup after loading the view, typically from a nib.

		NavigationItem.Title = "Conference Videos";
		videosController = new ConferenceVideoController ();
		ConfigureHierarchy ();
		ConfigureDataSource ();
	}

	UICollectionViewLayout CreateLayout ()
	{
		var config = new UICollectionViewCompositionalLayoutConfiguration { InterSectionSpacing = 20 };
		return new UICollectionViewCompositionalLayout (SectionProviderHandler, config);

		NSCollectionLayoutSection SectionProviderHandler (nint sectionIndex, INSCollectionLayoutEnvironment layoutEnvironment)
		{
			var itemSize = NSCollectionLayoutSize.Create (NSCollectionLayoutDimension.CreateFractionalWidth (1),
				NSCollectionLayoutDimension.CreateFractionalHeight (1));
			var item = NSCollectionLayoutItem.Create (itemSize);

			// if we have the space, adapt and go 2-up + peeking 3rd item
			var groupFractionalWidth = layoutEnvironment.Container.EffectiveContentSize.Width > 500 ? 0.425f : 0.85f;
			var groupSize = NSCollectionLayoutSize.Create (NSCollectionLayoutDimension.CreateFractionalWidth (groupFractionalWidth),
				NSCollectionLayoutDimension.CreateAbsolute (250));
			var group = NSCollectionLayoutGroup.CreateHorizontal (groupSize, item);

			var section = NSCollectionLayoutSection.Create (group);
			section.OrthogonalScrollingBehavior = UICollectionLayoutSectionOrthogonalScrollingBehavior.Continuous;
			section.InterGroupSpacing = 20;
			section.ContentInsets = new NSDirectionalEdgeInsets (0, 20, 0, 20);

			var titleSize = NSCollectionLayoutSize.Create (NSCollectionLayoutDimension.CreateFractionalWidth (1),
				NSCollectionLayoutDimension.CreateEstimated (44));
			var titleSupplementary = NSCollectionLayoutBoundarySupplementaryItem.Create (titleSize, titleElementKind, NSRectAlignment.Top);
			section.BoundarySupplementaryItems = new [] { titleSupplementary };

			return section;
		}
	}

	void ConfigureHierarchy ()
	{
		collectionView = new UICollectionView (View!.Bounds, CreateLayout ()) {
			TranslatesAutoresizingMaskIntoConstraints = false,
			BackgroundColor = UIColor.SystemBackgroundColor,
		};
		View.AddSubview (collectionView);
		collectionView.RegisterClassForCell (typeof (ConferenceVideoCell), ConferenceVideoCell.Key);
		collectionView.RegisterClassForSupplementaryView (typeof (TitleSupplementaryView), new NSString (titleElementKind), TitleSupplementaryView.Key);

		collectionView.LeadingAnchor.ConstraintEqualTo (View.LeadingAnchor).Active = true;
		collectionView.TrailingAnchor.ConstraintEqualTo (View.TrailingAnchor).Active = true;
		collectionView.TopAnchor.ConstraintEqualTo (View.TopAnchor).Active = true;
		collectionView.BottomAnchor.ConstraintEqualTo (View.BottomAnchor).Active = true;
	}

	void ConfigureDataSource ()
	{
		dataSource = new UICollectionViewDiffableDataSource<ConferenceVideoController.VideoCollection, ConferenceVideoController.Video> (collectionView!, CellProviderHandler) {
			SupplementaryViewProvider = SupplementaryViewProviderHandler
		};
		currentSnapshot = new NSDiffableDataSourceSnapshot<ConferenceVideoController.VideoCollection, ConferenceVideoController.Video> ();

		foreach (var videoCollection in videosController!.Collections!) {
			currentSnapshot.AppendSections (new [] { videoCollection });
			currentSnapshot.AppendItems (videoCollection.Videos!);
		}

		dataSource.ApplySnapshot (currentSnapshot, false);

		UICollectionViewCell CellProviderHandler (UICollectionView collectionView, NSIndexPath indexPath, NSObject obj)
		{
			var video = obj as ConferenceVideoController.Video;

			// Get a cell of the desired kind.
			var cell = collectionView.DequeueReusableCell (ConferenceVideoCell.Key, indexPath) as ConferenceVideoCell;

			// Populate the cell with our item description.
			cell!.TitleLabel!.Text = video?.Title;
			cell.CategoryLabel!.Text = video?.Category;

			// Return the cell.
			return cell;
		}

		UICollectionReusableView SupplementaryViewProviderHandler (UICollectionView collectionView, string kind, NSIndexPath indexPath)
		{
			// Get a supplementary view of the desired kind.
			var titleSupplementary = collectionView.DequeueReusableSupplementaryView (new NSString (kind),
				TitleSupplementaryView.Key, indexPath) as TitleSupplementaryView;

			// Populate the view with our section's description.
			var videoCategory = currentSnapshot?.SectionIdentifiers [indexPath.Section];
			titleSupplementary!.Label!.Text = videoCategory?.Title;

			// Return the view.
			return titleSupplementary;
		}
	}
}
