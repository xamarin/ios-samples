﻿using System;
using Conference_Diffable.CompositionalLayout.CellsandSupplementaryViews;
using Conference_Diffable.CompositionalLayout.Controllers;
using Foundation;
using UIKit;

namespace Conference_Diffable.CompositionalLayout.AppSamplesViewControllers {
	public partial class ConferenceNewsFeedViewController : UIViewController {
		class Section : NSObject, IEquatable<Section> {
			public static Section Main { get; } = new Section (1);

			public int Value { get; private set; }

			Section (int value) => Value = value;

			public static bool operator == (Section left, Section right)
			{
				if (ReferenceEquals (left, right))
					return true;

				if (ReferenceEquals (left, null))
					return false;

				if (ReferenceEquals (right, null))
					return false;

				return left.Equals (right);
			}

			public static bool operator != (Section left, Section right) => !(left == right);
			public override bool Equals (object obj) => this == (Section)obj;
			public bool Equals (Section other) => Value == other.Value;
			public override int GetHashCode () => HashCode.Combine (base.GetHashCode (), Value);
		}

		UICollectionViewDiffableDataSource<Section, ConferenceNewsController.NewsFeedItem> dataSource;
		UICollectionView collectionView;

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			// Perform any additional setup after loading the view, typically from a nib.

			NavigationItem.Title = "Conference News Feed";
			ConfigureHierarchy ();
			ConfigureDataSource ();
		}

		UICollectionViewLayout CreateLayout ()
		{
			var estimateHeight = 100;
			var layoutSize = NSCollectionLayoutSize.Create (NSCollectionLayoutDimension.CreateFractionalWidth (1),
				NSCollectionLayoutDimension.CreateEstimated (estimateHeight));
			var item = NSCollectionLayoutItem.Create (layoutSize);
			var group = NSCollectionLayoutGroup.CreateHorizontal (layoutSize, item, 1);
			var section = NSCollectionLayoutSection.Create (group);
			section.ContentInsets = new NSDirectionalEdgeInsets (10, 10, 10, 10);
			section.InterGroupSpacing = 10;
			var layout = new UICollectionViewCompositionalLayout (section);
			return layout;
		}

		void ConfigureHierarchy ()
		{
			collectionView = new UICollectionView (View.Bounds, CreateLayout ()) {
				TranslatesAutoresizingMaskIntoConstraints = false,
				BackgroundColor = UIColor.SystemBackgroundColor,
			};
			View.AddSubview (collectionView);
			collectionView.RegisterClassForCell (typeof (ConferenceNewsFeedCell), ConferenceNewsFeedCell.Key);

			collectionView.LeadingAnchor.ConstraintEqualTo (View.LeadingAnchor).Active = true;
			collectionView.TrailingAnchor.ConstraintEqualTo (View.TrailingAnchor).Active = true;
			collectionView.TopAnchor.ConstraintEqualTo (View.TopAnchor).Active = true;
			collectionView.BottomAnchor.ConstraintEqualTo (View.BottomAnchor).Active = true;
		}

		void ConfigureDataSource ()
		{
			var newsController = new ConferenceNewsController ();
			dataSource = new UICollectionViewDiffableDataSource<Section, ConferenceNewsController.NewsFeedItem> (collectionView, CellProviderHandler);

			// load our data
			var newsItems = newsController.News;
			var snapshot = new NSDiffableDataSourceSnapshot<Section, ConferenceNewsController.NewsFeedItem> ();
			snapshot.AppendSections (new [] { Section.Main });
			snapshot.AppendItems (newsItems);

			dataSource.ApplySnapshot (snapshot, false);

			UICollectionViewCell CellProviderHandler (UICollectionView collectionView, NSIndexPath indexPath, NSObject obj)
			{
				var newsItem = obj as ConferenceNewsController.NewsFeedItem;

				// Get a cell of the desired kind.
				var cell = collectionView.DequeueReusableCell (ConferenceNewsFeedCell.Key, indexPath) as ConferenceNewsFeedCell;

				// Populate the cell with our item description.
				cell.TitleLabel.Text = newsItem.Title;
				cell.BodyLabel.Text = newsItem.Body;
				cell.DateLabel.Text = newsItem.Date.ToShortDateString ();
				cell.ShowsSeparator = indexPath.Item != newsController.News.Length - 1;

				// Return the cell.
				return cell;
			}
		}
	}
}

