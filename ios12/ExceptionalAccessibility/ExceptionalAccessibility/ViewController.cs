
namespace ExceptionalAccessibility {
	using CoreGraphics;
	using Foundation;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using UIKit;

	/// <summary>
	/// This view controller manages the displayed views, links up the data by syncing dog objects
	/// across the relevant views, handles displaying and removing the modal view as well as responding
	/// to button presses in the carousel.It also serves as the data source for the collection view.
	/// </summary>
	public partial class ViewController : UIViewController, IUICollectionViewDataSource, IUICollectionViewDelegateFlowLayout {
		private const string CellIdentifier = "dog collection view cell";

		private readonly List<Dog> dogs = Dog.All;

		private Dog currentlyFocusedDog;

		protected ViewController (IntPtr handle) : base (handle) { }

		protected Dog CurrentlyFocusedDog {
			get => this.currentlyFocusedDog;

			// Every time we update our Dog object, we need to relay that change to all the views that care.
			set {
				if (this.currentlyFocusedDog != value) {
					this.currentlyFocusedDog = value;
					if (this.dogStatsView != null) {
						this.dogStatsView.Dog = this.currentlyFocusedDog;
					}

					this.carouselContainerView.CurrentDog = this.currentlyFocusedDog;
					this.shelterNameLabel.Text = this.currentlyFocusedDog?.ShelterName;
					this.shelterInfoView.AccessibilityLabel = this.currentlyFocusedDog?.ShelterName;
				}
			}
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			if (this.dogs.Any ()) {
				this.CurrentlyFocusedDog = this.dogs.First ();
				this.carouselContainerView.Dogs = this.dogs;
			}

			this.galleryButton.AccessibilityLabel = "Show Gallery";

			this.shelterInfoView.IsAccessibilityElement = true;
			this.shelterInfoView.AccessibilityCustomActions = new UIAccessibilityCustomAction []
			{
				new UIAccessibilityCustomAction("Call", probe: ActivateCallButton),
				new UIAccessibilityCustomAction("Open address in Maps", probe: ActivateLocationButton)
			};
		}

		/// <summary>
		/// Called as a result of activating the "Call" custom action.
		/// </summary>
		private bool ActivateCallButton (UIAccessibilityCustomAction arg)
		{
			return true;
		}

		/// <summary>
		/// Called as a result of activating the "Open address in Maps" custom action.
		/// </summary>
		private bool ActivateLocationButton (UIAccessibilityCustomAction arg)
		{
			return true;
		}

		partial void galleryButtonPressed (UIButton sender)
		{
			var dogModalViewController = base.Storyboard?.InstantiateViewController ("DogModalViewController");
			if (dogModalViewController == null) {
				throw new Exception ("Could not create a \"DogModalViewController\" from the storyboard.");
			}

			if (dogModalViewController.View is DogModalView dogModalView) {
				// The gallery button shouldn't do anything if the currently focused dog doesn't have 2 or more images.
				if (this.currentlyFocusedDog != null && this.currentlyFocusedDog.Images.Count >= 2) {
					// Make the images of the modal view accessible and add accessibility labels to these images and the close button.
					dogModalView.closeButton.AccessibilityLabel = "Close";
					dogModalView.firstImageView.IsAccessibilityElement = true;
					dogModalView.firstImageView.AccessibilityLabel = "Image 1";
					dogModalView.firstImageView.Image = currentlyFocusedDog.Images [0];

					dogModalView.secondImageView.IsAccessibilityElement = true;
					dogModalView.secondImageView.AccessibilityLabel = "Image 2";
					dogModalView.secondImageView.Image = currentlyFocusedDog.Images [1];

					dogModalView.Alpha = 0f;
					View.AddSubview (dogModalView);

					UIView.AnimateNotify (0.25d, () => {
						dogModalView.Alpha = 1f;
					}, (finished) => {
						if (finished) {
							/*
                             Once the modal gallery view has been animated in, we need to post a notification
                             to VoiceOver that the screen has changed so that it knows to update its focus
                             to the new content now displayed on top of the older content.
                             */
							UIAccessibility.PostNotification (UIAccessibilityPostNotification.ScreenChanged, null);
						}
					});
				} else {
					return;
				}
			} else {
				throw new Exception ("\"DogModalViewController\" not configured with a \"DogModalView\".");
			}
		}

		#region IUICollectionViewDataSource

		public UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
		{
			if (collectionView.DequeueReusableCell (CellIdentifier, indexPath) is DogCollectionViewCell cell) {
				var dog = this.dogs [(int) indexPath.Item];
				cell.dogImageView.Image = dog.FeaturedImage;
				cell.IsAccessibilityElement = true;
				cell.AccessibilityLabel = dog.Name;

				return cell;
			} else {
				throw new Exception ($"Expected a `{nameof (DogCollectionViewCell)}` but did not receive one.");
			}
		}

		public nint GetItemsCount (UICollectionView collectionView, nint section)
		{
			return this.dogs.Count;
		}

		#endregion

		#region IUIScrollViewDelegate

		/// <summary>
		/// This keeps the cells of the collection view centered.
		/// </summary>
		[Export ("scrollViewWillEndDragging:withVelocity:targetContentOffset:")]
		public void WillEndDragging (UIScrollView scrollView, CoreGraphics.CGPoint velocity, ref CoreGraphics.CGPoint targetContentOffset)
		{
			if (this.dogCollectionView?.CollectionViewLayout is UICollectionViewFlowLayout layout) {
				var cellWidthIncludingSpacing = layout.ItemSize.Width + layout.MinimumLineSpacing;

				var offset = targetContentOffset;
				var index = Math.Round ((offset.X + scrollView.ContentInset.Left) / cellWidthIncludingSpacing);
				offset = new CGPoint (index * cellWidthIncludingSpacing - scrollView.ContentInset.Left,
									 -scrollView.ContentInset.Top);

				targetContentOffset = offset;
			}
		}

		/// <summary>
		/// In `scrollViewDidScroll`, we calculate our new centered cell's index, then find the corresponding `Dog`
		/// in our array and update our current `Dog`. We also animate in or out the gallery button based on
		/// whether or not we want to show it for the new dog.
		/// </summary>
		[Export ("scrollViewDidScroll:")]
		public void Scrolled (UIScrollView scrollView)
		{
			if (this.dogCollectionView.CollectionViewLayout is UICollectionViewFlowLayout flowLayout) {
				var itemWidth = flowLayout.ItemSize.Width;
				var offset = this.dogCollectionView.ContentOffset.X / itemWidth;
				var index = (int) Math.Round (offset);

				if (index >= 0 && index < this.dogs.Count) {
					var focusedDog = this.dogs [index];
					this.CurrentlyFocusedDog = focusedDog;

					if (focusedDog.Images.Count > 1) {
						if (this.galleryButton.Alpha == 0f) {
							UIView.Animate (0.25d, () => this.galleryButton.Alpha = 1f);
						}
					} else if (this.galleryButton.Alpha == 1f) {
						UIView.Animate (0.25d, () => this.galleryButton.Alpha = 0f);
					}

					/*
                        The information for the dog displayed below the collection view updates as you scroll,
                        but VoiceOver isn't aware that the views have changed their values. So we need to post
                        a layout changed notification to let VoiceOver know it needs to update its current
                        understanding of what's on screen.
                    */
					UIAccessibility.PostNotification (UIAccessibilityPostNotification.LayoutChanged, null);
				}
			}
		}

		#endregion
	}
}
