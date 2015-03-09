using System;

using UIKit;
using Foundation;
using CoreImage;
using CoreGraphics;
using System.Threading.Tasks;
using System.Collections.Generic;
using ObjCRuntime;
using System.Runtime.InteropServices;

namespace PhotoHandoff
{
	[Register("DetailViewController")]
	public class DetailViewController : UIViewController, IUIScrollViewDelegate, IUIPopoverPresentationControllerDelegate, IUIStateRestoring
	{
		const int BlurButtonTag = 1;
		const int SepiaButtonTag = 2;

		const string kImageIdentifierKey = "kImageIdentifierKey";
		const string kDataSourceKey = "kDataSourceKey";
		const string kImageFiltersKey = "kImageFiltersKey";
		const string kFilterButtonKey = "kFilterButtonKey";
		const string kActivityViewControllerKey = "kActivityViewControllerKey";

		[Outlet("imageView")]
		UIImageView ImageView { get; set; }
		UIImage image;

		bool filtering;
		bool needsFilter;

		Dictionary<string, ImageFilter> filters = new Dictionary<string, ImageFilter>();

		[Outlet("scrollView")]
		UIScrollView ScrollView { get; set; }

		[Outlet("constraintLeft")]
		NSLayoutConstraint ConstraintLeft { get; set; }

		[Outlet("constraintRight")]
		NSLayoutConstraint ConstraintRight { get; set; }

		[Outlet("constraintTop")]
		NSLayoutConstraint ConstraintTop { get; set; }

		[Outlet("constraintBottom")]
		NSLayoutConstraint ConstraintBottom { get; set; }

		[Outlet("blurButton")]
		UIBarButtonItem BlurButton { get; set; }

		[Outlet("sepiaButton")]
		UIBarButtonItem SepiaButton { get; set; }

		nfloat lastZoomScale;

		string currentlyPresentedFilterTitle;

		UIActivityViewController activityViewController;
		FilterViewController currentFilterViewController;

		public string ImageIdentifier { get; set; }

		public DataSource DataSource { get; set; }

		public DetailViewController (IntPtr handle)
			: base(handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			CreateImageFilter (BlurFilter.Key);
			CreateImageFilter (ModifyFilter.Key);
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			ImageView.Hidden = true;
			UpdateImage (false, false);
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
			ImageView.Hidden = false;

			UpdateConstraints ();
			UpdateZoom ();
		}

		protected override void Dispose (bool disposing)
		{
			CleanupFilters ();
			base.Dispose (disposing);
		}

		void CleanupFilters()
		{
			foreach (var item in filters)
				item.Value.DirtyChanged -= OnDirtyChanged;
		}

		void UpdateImage (bool coalesce, bool animate)
		{
			if (!TryInit (animate))
				return;

			if (filtering) {
				needsFilter = true;
				return;
			}

			if (image == null)
				return;

			var blurFilter = (BlurFilter)GetFilter (BlurFilter.Key);
			var modifyFilter = (ModifyFilter)GetFilter (ModifyFilter.Key);
			bool dirty = blurFilter != null ? blurFilter.Dirty : false;
			dirty |= modifyFilter != null ? modifyFilter.Dirty : false;
			filtering = true;

			TryStartIndicatorForFilter ();

			Action runFilters = () => {
				var filterInput = new CIImage (image.CGImage);
				CIImage filteredCIImage = Apply(blurFilter, filterInput, dirty);

				filterInput = filteredCIImage ?? new CIImage (image.CGImage);
				filteredCIImage = Apply(modifyFilter, filterInput, dirty) ?? filteredCIImage;

				CGImage cgFilteredImage = null;
				if (filteredCIImage != null) {
					CIContext context = CIContext.FromOptions (new CIContextOptions{ UseSoftwareRenderer = false });
					cgFilteredImage = context.CreateCGImage (filteredCIImage, filteredCIImage.Extent);
				}

				if (coalesce)
					InvokeOnMainThread (() => Apply(cgFilteredImage, image, dirty));
				else
					Apply(cgFilteredImage, image, dirty);
			};

			if (coalesce)
				Task.Delay (250).ContinueWith (_ => runFilters());
			else
				runFilters ();

			blurFilter.Dirty = modifyFilter.Dirty = false;
		}

		ImageFilter GetFilter(string key)
		{
			ImageFilter filter;
			filters.TryGetValue (key, out filter);
			return filter;
		}

		void Apply(CGImage filteredImage, UIImage defaultImage, bool dirty)
		{
			if (filteredImage != null) {
				ImageView.Image = new UIImage (filteredImage);
				filteredImage.Dispose ();
			} else if (dirty) {
				ImageView.Image = defaultImage;
			}

			filtering = false;

			TryStopIndicatorForFilter ();

			if (needsFilter) {
				needsFilter = false;
				UpdateImage (true, false);
			}
			UpdateActivity ();
			UpdateConstraints ();
			UpdateZoom ();
		}

		void TryStartIndicatorForFilter()
		{
			if (currentFilterViewController == null)
				return;

			currentFilterViewController.ActivityIndicator.StartAnimating ();
		}

		void TryStopIndicatorForFilter()
		{
			if (currentFilterViewController == null)
				return;

			currentFilterViewController.ActivityIndicator.StopAnimating ();
		}

		bool TryInit(bool animate)
		{
			if (image != null)
				return true; // already initialized

			// warning: called without an imageIdentifier set
			if (string.IsNullOrEmpty (ImageIdentifier))
				return false;

			string title = DataSource.GetTitleForIdentifier (ImageIdentifier);
			image = DataSource.ImageForIdentifier (ImageIdentifier);

			SetTitleImage (title, image, animate);
			return true;
		}

		void SetTitleImage(string title, UIImage img, bool animate)
		{
			if (animate) {
				ImageView.Alpha = 0;
				UIView.Animate (0.5, () => {
					Title = title;
					ImageView.Image = img;
					ImageView.Alpha = 1;
				});
			} else {
				Title = title;
				ImageView.Image = img;
			}
		}

		CIImage Apply(BlurFilter blurFilter, CIImage input, bool dirty)
		{
			if (blurFilter == null || !blurFilter.Active || !dirty)
				return null;

			CIFilter filter = new CIGaussianBlur {
				Image = input,
				Radius = blurFilter.BlurRadius * 50,
			};
			return filter.OutputImage;
		}

		CIImage Apply(ModifyFilter modifyFilter, CIImage input, bool dirty)
		{
			if (modifyFilter == null || !modifyFilter.Active || !dirty)
				return null;

			CIFilter filter = new CISepiaTone {
				Image = input,
				Intensity = modifyFilter.Intensity,
			};
			return filter.OutputImage;
		}

		// dismiss any filter view controller and invoke the caller's completion handler then done
		public void DismissFromUserActivity (Action completionHandler)
		{
			InvokeOnMainThread (() => {
				if (currentFilterViewController != null)
					currentFilterViewController.DismissViewController(false, completionHandler);
				else if(completionHandler != null)
					completionHandler();
			});
		}

		#region UIScrollViewDelegate

		[Export ("scrollViewDidZoom:")]
		public virtual void DidZoom (UIScrollView scrollView)
		{
			// zooming requires we update our subview constraints
			UpdateConstraints ();
		}

		[Export ("viewForZoomingInScrollView:")]
		public virtual UIView ViewForZoomingInScrollView (UIScrollView scrollView)
		{
			return ImageView;
		}

		#endregion

		#region AutoLayout

		void UpdateConstraints ()
		{
			var img = ImageView.Image;
			if (img == null)
				return;

			var imageWidth = img.Size.Width;
			var imageHeight = img.Size.Height;

			var viewWidth = ScrollView.Bounds.Size.Width;
			var viewHeight = ScrollView.Bounds.Size.Height;

			// center image if it is smaller than screen
			var hPadding = (viewWidth - ScrollView.ZoomScale * imageWidth) / 2;
			hPadding = NMath.Max (0, hPadding);

			var vPadding = (viewHeight - ScrollView.ZoomScale * imageHeight) / 2;
			vPadding = NMath.Max (0, vPadding);

			ConstraintLeft.Constant = hPadding;
			ConstraintRight.Constant = hPadding;

			ConstraintTop.Constant = vPadding;
			ConstraintBottom.Constant = vPadding;
		}

		void UpdateZoom ()
		{
			// zoom to show as much image as possible unless image is smaller than screen
			var scrollSize = ScrollView.Bounds.Size;

			var img = ImageView.Image;
			var imgSize = img != null ? img.Size : CGSize.Empty;
			nfloat minZoom = NMath.Min(scrollSize.Width / imgSize.Width, scrollSize.Height / imgSize.Height);

			minZoom = NMath.Min (1, minZoom);

			ScrollView.MinimumZoomScale = minZoom;

			// force scrollViewDidZoom fire if zoom did not change
			if (minZoom == lastZoomScale)
				minZoom += 0.000001f;

			lastZoomScale = ScrollView.ZoomScale = minZoom;
		}

		// Update zoom scale and constraints
		// It will also animate because WillAnimateRotation
		// is called from within an animation block
		public override void WillAnimateRotation (UIInterfaceOrientation toInterfaceOrientation, double duration)
		{
			base.WillAnimateRotation (toInterfaceOrientation, duration);
			UpdateZoom ();
		}

		#endregion

		#region Filtering

		void OnDirtyChanged(object sender, EventArgs e)
		{
			var filter = (ImageFilter)sender;

			if(filter.Dirty)
				UpdateImage (true, false);
		}

		ImageFilter CreateImageFilter(string key)
		{
			ImageFilter filter;
			if (!filters.TryGetValue (key, out filter)) {
				filter = CreateFilter (key, true);
				Subscribe (filter);
				filters[key] = filter;
			}

			return filter;
		}

		static ImageFilter CreateFilter(string key, bool setDefaults)
		{
			ImageFilter filter = null;
			if (key == BlurFilter.Key)
				filter = new BlurFilter (setDefaults);
			else if (key == ModifyFilter.Key)
				filter = new ModifyFilter (setDefaults);
			else
				throw new NotImplementedException ();

			UIApplication.RegisterObjectForStateRestoration (filter, key);
			filter.Dirty = false;
			filter.RestorationType = typeof(DetailViewController);
			return filter;
		}

		#endregion

		#region Filter View Controllers

		// as a delegate to UIPopoverPresentationController, we are notified when our
		// filterViewController is being dimissed (tapped outside our popover)
		[Export ("popoverPresentationControllerDidDismissPopover:")]
		void DidDismissPopover (UIPopoverPresentationController popoverPresentationController)
		{
			UIViewController testVC = popoverPresentationController.PresentedViewController;
			if (testVC == currentFilterViewController)
				currentFilterViewController = null;
		}

		// We are notified when our FilterViewController is
		// being dismissed on its own
		public void WasDismissed()
		{
			currentFilterViewController = null;
		}

		void CreateAndPresentFilterVC(UIBarButtonItem sender, ImageFilter filter, string identifier)
		{
			currentFilterViewController = (FilterViewController)Storyboard.InstantiateViewController (identifier);
			currentFilterViewController.Filter = filter;
			currentFilterViewController.ModalPresentationStyle = UIModalPresentationStyle.Popover;
			currentFilterViewController.PopoverPresentationController.BarButtonItem = sender;
			currentFilterViewController.UserActivity = UserActivity;

			// so "WasDismissed" can be called
			currentFilterViewController.MasterViewController = this;

			// so "popoverPresentationControllerDidDismissPopover" can be called
			currentFilterViewController.PopoverPresentationController.WeakDelegate = this;

			PresentViewController (currentFilterViewController, true, () => UpdateImage (false, false));
		}

		[Export("presentFilter:")]
		void PresentFilter(NSObject sender)
		{
			var button = (UIBarButtonItem)sender;
			string key = null, identifier = null;

			if (button.Tag == BlurButtonTag) {
				key = BlurFilter.Key;
				identifier = "blurController";
			} else if (button.Tag == SepiaButtonTag) {
				key = ModifyFilter.Key;
				identifier = "modsController";
			}

			if (key == null)
				return;

			currentlyPresentedFilterTitle = button.Title;
			ImageFilter filter = CreateImageFilter (key);

			// check for activity view controller is open, dismiss it
			TryDismiss (activityViewController, ()=> {
				activityViewController = null;
				TryDismiss(currentFilterViewController, () => CreateAndPresentFilterVC (button, filter, identifier));
			});
		}

		void TryDismiss(UIViewController controller, Action completionHandler)
		{
			if (controller != null)
				controller.DismissViewController (false, completionHandler);
			else if(completionHandler != null)
				completionHandler ();
		}

		// user tapped "blur" or "sepia" buttons (lower right)
		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			base.PrepareForSegue (segue, sender);

			string key = null;
			string segueIdentifier = segue.Identifier;

			if (segueIdentifier == "showBlurInfo")
				key = BlurFilter.Key;
			else if (segueIdentifier == "showModifyInfo")
				key = ModifyFilter.Key;

			if (key == null)
				return;

			ImageFilter filter = CreateImageFilter (key);
			var filterViewController = (FilterViewController)segue.DestinationViewController;
			filterViewController.Filter = filter;
		}

		void CleanupActivity()
		{
			activityViewController = null;
		}

		void SetupActivityCompletion()
		{
			if (activityViewController == null)
				return;

			activityViewController.SetCompletionHandler ((aType, completed, items, aError) => CleanupActivity ());
		}

		[Export("share:")]
		void Share(NSObject sender)
		{
			if (ImageView.Image == null)
				return;

			var items = new NSObject[] {
				ImageView.Image
			};
			activityViewController = new UIActivityViewController (items, null) {
				ModalPresentationStyle = UIModalPresentationStyle.Popover,
				RestorationIdentifier = "Activity"
			};
			if(activityViewController.PopoverPresentationController != null)
				activityViewController.PopoverPresentationController.BarButtonItem = (UIBarButtonItem)sender;
			SetupActivityCompletion ();

			TryDismiss (currentFilterViewController, () => {
				// the filter view controller was dismissed
				currentFilterViewController = null;

				// now show our activity view controller
				PresentViewController (activityViewController, true, null);
			});
		}

		#endregion

		#region UIStateRestoration

		[Export ("objectWithRestorationIdentifierPath:coder:")]
		static UIStateRestoring Restore (string[] identifierComponents, NSCoder coder)
		{
			string key = identifierComponents [identifierComponents.Length - 1];
			return CreateFilter (key, false);
		}

		public override void EncodeRestorableState (NSCoder coder)
		{
			base.EncodeRestorableState (coder);
			// TODO: https://trello.com/c/TydBAJP0
			coder.Encode ((NSString)ImageIdentifier, kImageIdentifierKey);
			coder.Encode (DataSource, kDataSourceKey);
			coder.Encode (filters.Convert (), kImageFiltersKey);

			if (!string.IsNullOrEmpty (currentlyPresentedFilterTitle))
				coder.Encode ((NSString)currentlyPresentedFilterTitle, kFilterButtonKey);

			coder.Encode (activityViewController, kActivityViewControllerKey);
		}

		public override void DecodeRestorableState (NSCoder coder)
		{
			base.DecodeRestorableState (coder);

			// TODO: https://trello.com/c/TydBAJP0
			ImageIdentifier = (NSString)coder.DecodeObject (kImageIdentifierKey);
			DataSource = (DataSource)coder.DecodeObject (kDataSourceKey);

			NSObject nDict;
			if (coder.TryDecodeObject (kImageFiltersKey, out nDict))
				filters = ((NSDictionary)nDict).Convert<ImageFilter> ();

			// TODO: https://trello.com/c/TydBAJP0
			currentlyPresentedFilterTitle = (NSString)coder.DecodeObject (kFilterButtonKey);

			NSObject avc;
			if (coder.TryDecodeObject (kActivityViewControllerKey, out avc))
				activityViewController = (UIActivityViewController)avc;

			SetupActivityCompletion ();
		}

		public override void ApplicationFinishedRestoringState ()
		{
			base.ApplicationFinishedRestoringState ();

			CenterImageView ();
			SubscribeToFilters ();
			TryPresentFilterButton ();
		}

		void CenterImageView()
		{
			CGSize size = View.Bounds.Size;
			CGPoint imageCenter = ImageView.Center;
			var center = new CGPoint(size.Width / 2, size.Height / 2);
			ImageView.Center = center;
			ImageView.Bounds = View.Bounds;
			UpdateImage (false, false);
		}

		void SubscribeToFilters()
		{
			foreach (var kvp in filters)
				Subscribe (kvp.Value);
		}

		void Subscribe(ImageFilter subject)
		{
			subject.DirtyChanged += OnDirtyChanged;
		}

		void TryPresentFilterButton()
		{
			if (UIDevice.CurrentDevice.UserInterfaceIdiom != UIUserInterfaceIdiom.Pad
			    || string.IsNullOrEmpty (currentlyPresentedFilterTitle))
				return;

			UIBarButtonItem button = GetCurrentFilterButton ();
			if (button != null)
				InvokeOnMainThread (() => PresentFilter (button));
		}

		UIBarButtonItem GetCurrentFilterButton()
		{
			if (currentlyPresentedFilterTitle == "blur")
				return BlurButton;
			else if (currentlyPresentedFilterTitle == "sepia")
				return SepiaButton;

			return null;
		}

		#endregion

		#region NSUserActivity

		void UpdateActivity ()
		{
			if (UserActivity == null)
				return;

			if (ImageIdentifier != null)
				UserActivity.NeedsSave = true;
			else
				Console.WriteLine ("warning - asked to save activity without an ImageIdentifier");
		}

		public void PrepareForActivity ()
		{
			// handle any kind of work in preparation of the new activity being handed to us
		}

		public override void UpdateUserActivityState (NSUserActivity activity)
		{
			base.UpdateUserActivityState (activity);

			UserInfo info = GetFilterValues ();
			activity.AddUserInfoEntries (info.Dictionary);
		}

		UserInfo GetFilterValues()
		{
			var info = new UserInfo ();

			// obtain the filter values and save them
			ImageFilter filter;
			if(filters.TryGetValue (BlurFilter.Key, out filter))
				info.BlurRadius = ((BlurFilter)filter).BlurRadius;

			if (filters.TryGetValue (ModifyFilter.Key, out filter))
				info.Intensity = ((ModifyFilter)filter).Intensity;

			return info;
		}

		public void RestoreActivityForImageIdentifier (UserInfo userInfo)
		{
			if (userInfo == null)
				throw new ArgumentNullException ("userInfo");

			if (activityViewController != null) {
				SetActivityViewControllerCompletionHandler (userInfo);
				return;
			}

			ImageIdentifier = userInfo.ImageId;

			image = null;   // clear the old image
			UpdateImage (false, true);  // apply the new image (based on ImageIdentifier) and apply the 2 filter values

			// setup our filters (if not already allocated) and assign their values
			float blurFilterValue = userInfo.BlurRadius;
			var blurFilter = (BlurFilter)CreateImageFilter (BlurFilter.Key);
			blurFilter.BlurRadius = blurFilterValue;

			// the blur has changed from the activity on the other device
			blurFilter.Dirty = blurFilterValue > 0 ? true : blurFilter.Dirty;

			var sepiaFilterValue = userInfo.Intensity;
			var sepiaFilter = (ModifyFilter)CreateImageFilter (ModifyFilter.Key);
			sepiaFilter.Intensity = sepiaFilterValue;
			// the sepia has changed from the activity on the other device
			sepiaFilter.Dirty = sepiaFilterValue > 0 ? true : sepiaFilter.Dirty;

			// providing a different image requires us to adjust our view constraints and zoom
			UpdateConstraints ();
			UpdateZoom ();

			// a different image means updating our current user activity
			UpdateActivity ();

			// dismiss either filter view controller if necessary
			if (currentFilterViewController != null)
				currentFilterViewController.DismissViewController (false, null);
			else if (activityViewController != null)
				activityViewController.DismissViewController (false, null);
		}

		void SetActivityViewControllerCompletionHandler(UserInfo info)
		{
			activityViewController.SetCompletionHandler ((activityType, completed, returnedItems, error) => {
				CleanupActivity();
				RestoreActivityForImageIdentifier(info);
			});
		}

		#endregion
	}
}

