using System;
using System.Collections.Generic;
using UIKit;
using MapKit;
using CoreGraphics;
using CoreLocation;
using Foundation;
using MobileCoreServices;
using CoreFoundation;

namespace DragSource {
	public partial class ViewController : UIViewController {
		#region Computed Properties
		public UIView ContentView { get; set; } = new UIView ();
		public UIScrollView ScrollView { get; set; } = new UIScrollView ();
		public List<UIView> ExampleViews { get; set; } = new List<UIView> ();

		public UIImage [] ImageStack1 {
			get { return UIImageExtensions.ImagesForNames (new string [] { "Corkboard", "Food1" }); }
		}

		public UIImage [] ImageStack2 {
			get { return UIImageExtensions.ImagesForNames (new string [] { "Fish", "Flowers1", "Flowers2" }); }
		}

		public UIImage [] ImageStack3 {
			get { return UIImageExtensions.ImagesForNames (new string [] { "Flowers4", "Flowers5" }); }
		}

		public UIImage [] ImageStack4 {
			get { return UIImageExtensions.ImagesForNames (new string [] { "Cooking", "Food4", "Food5" }); }
		}

		public UIImage [] ImageStack5 {
			get { return UIImageExtensions.ImagesForNames (new string [] { "Pond", "Rainbow", "Sand", "Sunset" }); }
		}

		public ImageContainerView [] SlowImageViews {
			get {
				var results = new List<ImageContainerView> ();

				foreach (UIImage image in ImageStack5) {
					results.Add (new ImageContainerView (new SlowDraggableImageView (image, 10f), 25));
				}

				return results.ToArray ();
			}
		}

		public DraggableStackedPhotosView [] StackedPhotoViews {
			get {
				var results = new List<DraggableStackedPhotosView> ();

				results.Add (new DraggableStackedPhotosView (ImageStack1));
				results.Add (new DraggableStackedPhotosView (ImageStack2));
				results.Add (new DraggableStackedPhotosView (ImageStack3));
				results.Add (new DraggableStackedPhotosView (ImageStack4));

				return results.ToArray ();
			}
		}

		public List<(UIView view, string description)> ExamplesAndDescriptions {
			get {
				var results = new List<(UIView, string)> ();

				results.Add ((new PhotoStackColumnView (StackedPhotoViews), "DraggableStackedPhotosView"));
				results.Add ((new ImageContainerView (new DraggableQRCodeImageView (UIImage.FromBundle ("QRCode")), 100), "DraggableQRCodeImageView"));
				results.Add ((new ImageContainerView (new DraggableLocationImageView (UIImage.FromBundle ("GGBridge"), new CLLocation (37.8199, -122.4783)), 100), "DraggableLocationImageView"));
				results.Add ((new PhotoStackColumnView (SlowImageViews), "SlowDraggableImageView"));

				return results;
			}
		}
		#endregion

		#region Constructors
		protected ViewController (IntPtr handle) : base (handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}
		#endregion

		#region Override Methods
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			ScrollView.AddSubview (ContentView);
			View.AddSubview (ScrollView);
			Console.WriteLine ("View Loaded");

			foreach ((UIView view, string description) in ExamplesAndDescriptions) {
				view.Layer.BorderColor = UIColor.LightGray.CGColor;
				view.Layer.BorderWidth = 1;
				Console.WriteLine ("Processing: {0}", description);

				var label = new UILabel () {
					Text = description,
					Font = UIFont.BoldSystemFontOfSize (28),
					//TextColor = UIColor.White,
					TranslatesAutoresizingMaskIntoConstraints = false,
					//ShadowColor = UIColor.Black,
					//ShadowOffset = new CGSize(0.5f, 0.5f)
				};
				view.AddSubview (label);
				NSLayoutConstraint.ActivateConstraints (new NSLayoutConstraint []{
					label.CenterXAnchor.ConstraintEqualTo(view.CenterXAnchor),
					label.CenterYAnchor.ConstraintEqualTo(view.BottomAnchor, constant:-20)
				});

				ContentView.AddSubview (view);
				ExampleViews.Add (view);
			}

			ScrollView.PagingEnabled = true;
		}

		public override void ViewWillLayoutSubviews ()
		{
			base.ViewWillLayoutSubviews ();

			//Background.Frame = new CGRect(0, 0, View.Frame.Width, View.Frame.Height);

			var size = View.Bounds.Size;
			for (int n = 0; n < ExampleViews.Count; ++n) {
				ExampleViews [n].Frame = new CGRect (new CGPoint (0, n * size.Height), size);
			}
			ContentView.Frame = new CGRect (0, 0, size.Width, ExampleViews.Count * size.Height);
			ScrollView.Frame = new CGRect (0, 0, size.Width, size.Height);
			ScrollView.ContentSize = ContentView.Bounds.Size;
		}
		#endregion
	}
}
