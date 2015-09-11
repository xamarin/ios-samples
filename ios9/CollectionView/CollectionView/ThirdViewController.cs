using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace CollectionView {
	/// <summary>
	/// Third view controller.
	/// </summary>
	/// <remarks>
	/// Origionally created by Wojciech Łukaszuk on 7/16/15.
	/// Copyright (c) 2015 Wojciech Łukaszuk. Under The MIT License (MIT).
	/// Ported from http://nshint.io/blog/2015/07/16/uicollectionviews-now-have-easy-reordering/ to
	/// Xamarin.iOS by Kevin Mullins.
	/// </remarks>
	public partial class ThirdViewController : UICollectionViewController {
		#region Constructors
		public ThirdViewController (IntPtr handle) : base (handle)
		{
		}
		#endregion

		#region Override Methods
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Create a custom gesture recognizer
			var longPressGesture = new UILongPressGestureRecognizer (gesture => {

				// Take action based on state
				switch (gesture.State) {
				case UIGestureRecognizerState.Began:
					var selectedIndexPath = CollectionView.IndexPathForItemAtPoint (gesture.LocationInView (View));
					if (selectedIndexPath != null) {
						CollectionView.BeginInteractiveMovementForItem (selectedIndexPath);
					}
					break;
				case UIGestureRecognizerState.Changed:
					CollectionView.UpdateInteractiveMovement (gesture.LocationInView (View));
					break;
				case UIGestureRecognizerState.Ended:
					CollectionView.EndInteractiveMovement ();
					break;
				default:
					CollectionView.CancelInteractiveMovement ();
					break;
				}
			});

			// Add the custom recognizer to the collection view
			CollectionView.AddGestureRecognizer(longPressGesture);
		}

		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();

			var waterfallLayout = new WaterfallCollectionLayout ();

			// Wireup events
			waterfallLayout.SizeForItem += (collectionView, layout, indexPath) => {
				var collection = collectionView as WaterfallCollectionView;
				return new CGSize ((View.Bounds.Width - 40) / 3, collection.Source.Heights [(int)indexPath.Item]);
			};

			// Attach the custom layout to the collection
			CollectionView.SetCollectionViewLayout (waterfallLayout, false);
		}
		#endregion
	}
}
