using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Foundation;
using UIKit;
using CoreGraphics;
using CoreImage;
using ObjCRuntime;
using System.Linq;

namespace StateRestoration
{
	public partial class DetailViewController : UIViewController, IUIObjectRestoration
	{
		const string blurFilterKey = "kBlurFilterKey";
		const string modifyFilterKey = "kModifyFilterKey";
		const string imageIdentifierKey = "kImageIdentifierKey";
		const string dataSourceKey = "kDataSourceKey";
		const string barsHiddenKey = "kBarsHiddenKey";
		const string imageFiltersKey = "kImageFiltersKey";

		const float defaultScale = 1;

		UIImage image;
		nfloat lastScale;
		bool statusBarHidden;
		Dictionary<string,ImageFilter> filters;

		UITapGestureRecognizer tapGestureRecognizer;
		UITapGestureRecognizer doubleTapGestureRecognizer;
		UIPinchGestureRecognizer pinchGestureRecognizer;

		public string ImageIdentifier { get; set; }

		public DataSource DataSource { get; set; }

		public DetailViewController (IntPtr handle)
			: base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			lastScale = defaultScale;
			imageView.UserInteractionEnabled = true;
		}

		public override bool PrefersStatusBarHidden ()
		{
			return statusBarHidden;
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			UpdateImage ();

			TryAddDoubleTapGesture ();
			TryAddTapGetsture ();
			TryAddPinchGesture ();

			SetImageViewConstraints (UIApplication.SharedApplication.StatusBarOrientation);
		}

		void TryAddDoubleTapGesture ()
		{
			if (doubleTapGestureRecognizer != null)
				return;

			doubleTapGestureRecognizer = new UITapGestureRecognizer (OnDoubleTapGesture) {
				NumberOfTapsRequired = 2
			};
			imageView.AddGestureRecognizer (doubleTapGestureRecognizer);
		}

		void TryAddTapGetsture ()
		{
			if (tapGestureRecognizer != null)
				return;

			tapGestureRecognizer = new UITapGestureRecognizer (g => UpdateBars (0.25f)) {
				NumberOfTapsRequired = 1
			};

			tapGestureRecognizer.RequireGestureRecognizerToFail (doubleTapGestureRecognizer);
			imageView.AddGestureRecognizer (tapGestureRecognizer);
		}

		void TryAddPinchGesture ()
		{
			if (pinchGestureRecognizer != null)
				return;

			pinchGestureRecognizer = new UIPinchGestureRecognizer (OnPinch);
			imageView.AddGestureRecognizer (pinchGestureRecognizer);
		}

		void OnPinch (UIPinchGestureRecognizer recognizer)
		{
			if (recognizer.State == UIGestureRecognizerState.Ended) {
				lastScale = defaultScale;
				return;
			}

			nfloat scale = recognizer.Scale / lastScale;
			var newTransform = imageView.Transform;
			newTransform.Scale (scale, scale);
			imageView.Transform = newTransform;
			lastScale = recognizer.Scale;
		}

		public override void WillRotate (UIInterfaceOrientation toInterfaceOrientation, double duration)
		{
			if (imageView.Transform.IsIdentity)
				SetImageViewConstraints (toInterfaceOrientation);

			if (toInterfaceOrientation.IsLandscape () && !statusBarHidden)
				UpdateBars ();
			else if (statusBarHidden)
				UpdateBars ();
		}

		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			base.PrepareForSegue (segue, sender);

			string filterKey = MapToFilterKey (segue.Identifier);
			if (filterKey == null)
				return;

			var filterViewController = (FilterViewController)segue.DestinationViewController;
			filterViewController.filter = GetImageFilter (filterKey);
		}

		string MapToFilterKey (string segueIdentifier)
		{
			switch (segueIdentifier) {
				case "showBlurInfo":
					return blurFilterKey;
				case "showModifyInfo":
					return modifyFilterKey;
				default:
					return null;
			}
		}

		ImageFilter GetImageFilter (string key)
		{
			filters = filters ?? new Dictionary<string, ImageFilter> ();

			ImageFilter filter;
			if (!filters.TryGetValue (key, out filter)) {
				filter = CreateFilter (key, true);
				filters [key] = filter;
			}

			return filter;
		}

		static ImageFilter CreateFilter (string key, bool useDefault)
		{
			ImageFilter filter = CreateFilterFrom (key, useDefault);
			Register (filter, key);
			return filter;
		}

		static ImageFilter CreateFilterFrom (string key, bool useDefault)
		{
			switch (key) {
				case blurFilterKey:
					return new BlurFilter (useDefault);
				case modifyFilterKey:
					return new ModifyFilter (useDefault);
				default:
					throw new NotImplementedException ();
			}
		}

		static void Register (ImageFilter filter, string key)
		{
			UIApplication.RegisterObjectForStateRestoration (filter, key);
			filter.RestorationType = typeof(DetailViewController);
		}

		void OnDoubleTapGesture (UITapGestureRecognizer tapGesture)
		{
			UIView.Animate (1.0f, () => {
				if (imageView.Transform.IsIdentity) {
					imageView.Transform = CreateScaleTransform (imageView.Transform);
				} else {
					imageView.Transform = CGAffineTransform.MakeIdentity ();
					lastScale = defaultScale;
					SetImageViewConstraints (UIApplication.SharedApplication.StatusBarOrientation);
				}
			});
		}

		CGAffineTransform CreateScaleTransform (CGAffineTransform currentTransform)
		{
			var flip = UIApplication.SharedApplication.StatusBarOrientation.IsLandscape ();
			var scale = flip ? 2.0f : 2.5f;
			currentTransform.Scale (scale, scale);

			return currentTransform;
		}

		void UpdateImage ()
		{
			try {
				TryInitializeImage ();
			} catch (InvalidProgramException ex) {
				Console.WriteLine (ex);
				return;
			}

			if (filters == null)
				return;

			ApplyFilters ();
		}

		void TryInitializeImage ()
		{
			// already initialized
			if (image != null)
				return;

			// we cant init img
			if (string.IsNullOrEmpty (ImageIdentifier))
				throw new InvalidProgramException ("Warning: Called without an ImageIdentifier set");

			// initialization
			Title = DataSource.GetTitle (ImageIdentifier);
			image = DataSource.GetFullImage (ImageIdentifier);
			imageView.Image = image;
		}

		void ApplyFilters ()
		{
			var blurFilter = GetFilter<BlurFilter> (blurFilterKey);
			var modifyFilter = GetFilter<ModifyFilter> (modifyFilterKey);

			bool dirty = IsAnyDirty (blurFilter, modifyFilter);
			bool needBlur = blurFilter != null && blurFilter.Active && dirty;
			bool needModify = modifyFilter != null && modifyFilter.Active && dirty;

			CIImage filteredCIImage = null;
			var original = new CIImage (image.CGImage);
			if (needBlur)
				filteredCIImage = Apply (original, blurFilter);
			if (needModify)
				filteredCIImage = Apply (filteredCIImage ?? original, modifyFilter);

			if (filteredCIImage != null)
				imageView.Image = CreateImage (filteredCIImage);
			else if (dirty)
				imageView.Image = image;

			MarkAsNotDirty (blurFilter);
			MarkAsNotDirty (modifyFilter);
		}

		T GetFilter<T> (string key) where T : ImageFilter
		{
			ImageFilter f;
			filters.TryGetValue (key, out f);
			return (T)f;
		}

		bool IsAnyDirty (params ImageFilter[] filters)
		{
			return filters.Where (f => f != null).Any (f => f.Dirty);
		}

		CIImage Apply (CIImage input, BlurFilter filter)
		{
			var f = new CIGaussianBlur () {
				Image = input,
				Radius = filter.BlurRadius * 50,
			};
			return f.OutputImage;
		}

		CIImage Apply (CIImage input, ModifyFilter filter)
		{
			var f = new CISepiaTone () {
				Image = input,
				Intensity = filter.Intensity
			};
			return f.OutputImage;
		}

		UIImage CreateImage (CIImage img)
		{
			CIContext context = CreateContext ();
			CGImage cgImage = context.CreateCGImage (img, img.Extent);
			return new UIImage (cgImage);
		}

		CIContext CreateContext ()
		{
			var options = new CIContextOptions {
				UseSoftwareRenderer = false
			};
			return CIContext.FromOptions (options);
		}

		void MarkAsNotDirty (ImageFilter filter)
		{
			if (filter != null)
				filter.Dirty = false;
		}

		void UpdateBars (float animationDuration = 0)
		{
			bool visible = !toolBar.Hidden;
			if (visible && NavigationController.TopViewController != this) {
				Console.WriteLine ("Asked to hide bar, but not the top view controller, skipping update of bars");
				return;
			}

			bool newHiddenValue = !toolBar.Hidden;
			AnimateBars (animationDuration, newHiddenValue);
		}

		void AnimateBars (float animationDuration, bool hidden)
		{
			UIView.Animate (animationDuration, () => {
				float alpha = hidden ? 0.0f : 1.0f;
				UIApplication.SharedApplication.SetStatusBarHidden (hidden, false);
				statusBarHidden = hidden;

				SetNeedsStatusBarAppearanceUpdate ();

				if (!hidden) {
					toolBar.Hidden = false;
					NavigationController.SetNavigationBarHidden (false, false);
				}
				toolBar.Alpha = alpha;
				NavigationController.NavigationBar.Alpha = alpha;
			}, () => {
				if (hidden) {
					toolBar.Hidden = true;
					NavigationController.SetNavigationBarHidden (true, false);
				}
			});
		}

		void SetImageViewConstraints (UIInterfaceOrientation orientation)
		{
			bool flip = orientation.IsLandscape ();
			var bounds = UIScreen.MainScreen.Bounds;
			NSLayoutConstraint[] constraints = imageView.Constraints;

			foreach (var constraint in constraints) {
				if (constraint.FirstAttribute == NSLayoutAttribute.Height)
					constraint.Constant = flip ? bounds.Size.Width : bounds.Size.Height;
				else if (constraint.FirstAttribute == NSLayoutAttribute.Width)
					constraint.Constant = flip ? bounds.Size.Height : bounds.Size.Width;
			}
			imageView.SetNeedsUpdateConstraints ();
		}

		partial void Share (NSObject sender)
		{
			if (imageView.Image == null)
				return;

			var avc = new UIActivityViewController (new [] { imageView.Image }, null);
			avc.RestorationIdentifier = "Activity";
			PresentViewController (avc, true, null);
		}

		// State Restoration
		[Export ("objectWithRestorationIdentifierPath:coder:")]
		public static UIStateRestoring GetStateRestorationObjectFromPath (string[] identifierComponents, NSCoder coder)
		{
			var key = identifierComponents [identifierComponents.Length - 1];
			return CreateFilter (key, false);
		}

		public override void EncodeRestorableState (NSCoder coder)
		{
			base.EncodeRestorableState (coder);

			coder.Encode ((NSString)ImageIdentifier, imageIdentifierKey);
			coder.Encode (DataSource, dataSourceKey);
			TryEncodeFilters (coder);
			TryEncodeBarsVisibility (coder);
		}

		void TryEncodeFilters (NSCoder coder)
		{
			if (filters == null)
				return;

			var nativeDict = new NSMutableDictionary ();
			foreach (var key in filters.Keys)
				nativeDict [key] = filters [key];

			coder.Encode (nativeDict, imageFiltersKey);
		}

		void TryEncodeBarsVisibility (NSCoder coder)
		{
			if (UIApplication.SharedApplication.StatusBarOrientation == UIInterfaceOrientation.Portrait)
				coder.Encode (toolBar.Hidden, barsHiddenKey);
		}

		public override void DecodeRestorableState (NSCoder coder)
		{
			base.DecodeRestorableState (coder);

			ImageIdentifier = (NSString)coder.DecodeObject (imageIdentifierKey);
			DataSource = (DataSource)coder.DecodeObject (dataSourceKey);
			TryDecodeFilters (coder);
			TryDecodeBarsVisibility (coder);
		}

		void TryDecodeFilters (NSCoder coder)
		{
			if (!coder.ContainsKey (imageFiltersKey))
				return;

			var filtersNS = (NSMutableDictionary)coder.DecodeObject (imageFiltersKey);
			if (filtersNS == null)
				return;

			filters = new Dictionary<string, ImageFilter> ();

			var blurFilter = (ImageFilter)filtersNS [blurFilterKey];
			if (blurFilter != null)
				filters [blurFilterKey] = blurFilter;

			var modifyFilter = (ImageFilter)filtersNS [modifyFilterKey];
			if (modifyFilter != null)
				filters [modifyFilterKey] = modifyFilter;
		}

		void TryDecodeBarsVisibility (NSCoder coder)
		{
			bool hidden = coder.DecodeBool (barsHiddenKey);
			if (hidden)
				UpdateBars ();
		}

		public override void ApplicationFinishedRestoringState ()
		{
			base.ApplicationFinishedRestoringState ();
			UpdateImage ();
		}
	}
}