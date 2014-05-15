using System;
using System.Collections.Generic;
using CoreGraphics;
using System.Linq;
using Foundation;
using UIKit;
using MapKit;


namespace MapCallouts
{
	public partial class MainViewController : UIViewController
	{
		enum AnnotationIndex
		{
			City,
			Bridge,
		}
		
		MKAnnotation[] mapAnnotations;
		DetailViewController detailViewController;
		
		const float AnnotationPadding = 10;
		const float CalloutHeight = 40;
		
		//loads the MainViewController.xib file and connects it to this object
		public MainViewController () : base ("MainViewController", null)
		{
			detailViewController = new DetailViewController ();
		}
		
		void GotoLocation ()
		{
			MKCoordinateRegion newRegion;
			newRegion.Center.Latitude = 37.786996;
			newRegion.Center.Longitude = -122.440100;
			newRegion.Span.LatitudeDelta = 0.112872;
			newRegion.Span.LongitudeDelta = 0.109863;
		
			mapView.SetRegion (newRegion, true);
		}
		
		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
		}

		public override void ViewDidLoad ()
		{
			mapView.MapType = MKMapType.Standard;
			mapView.GetViewForAnnotation = GetViewForAnnotation;
			
			UIBarButtonItem temporaryBarButtonItem = new UIBarButtonItem ();
			temporaryBarButtonItem.Title = "Back";
			NavigationItem.BackBarButtonItem = temporaryBarButtonItem;
			
			mapAnnotations = new MKAnnotation [2];
			mapAnnotations [(int) AnnotationIndex.City] = new SFAnnotation ();
			mapAnnotations [(int) AnnotationIndex.Bridge] = new BridgeAnnotation ();
			
			GotoLocation ();
			
			base.ViewDidLoad ();
		}
		
		public override void ViewDidUnload ()
		{
			mapView = null;
			mapAnnotations = null;
			detailViewController = null;
			
			base.ViewDidUnload ();
		}
		
		void RemoveAllAnnotations ()
		{
			foreach (var obj in mapView.Annotations) {
				mapView.RemoveAnnotation ((MKAnnotation) obj);
			}
		}
		
		partial void allAction (Foundation.NSObject sender)
		{
			GotoLocation ();
			RemoveAllAnnotations ();
			mapView.AddAnnotation (mapAnnotations);
		}
		
		partial void bridgeAction (Foundation.NSObject sender)
		{
			GotoLocation ();
			RemoveAllAnnotations ();
			mapView.AddAnnotation (mapAnnotations [(int) AnnotationIndex.Bridge]);
		}
		
		partial void cityAction (Foundation.NSObject sender)
		{
			GotoLocation ();
			RemoveAllAnnotations ();
			mapView.AddAnnotation (mapAnnotations [(int) AnnotationIndex.City]);
		}
		
		void showDetails ()
		{
			NavigationController.PushViewController (detailViewController, true);
		}
		
		// We need to store the pinviews we create somewhere
		// the managed garbage collector can see them, otherwise
		// the GC will collect them too early.
		List<object> pinViews = new List<object> ();
		MKAnnotationView GetViewForAnnotation (MKMapView mapView, NSObject annotation)
		{
			// if it's the user location, just return nil.
			if (annotation is MKUserLocation)
				return null;
			
			// handle our two custom annotations
			//
			if (annotation is BridgeAnnotation) { // for Golden Gate Bridge
				const string BridgeAnnotationIdentifier = "bridgeAnnotationIdentifier";
				MKPinAnnotationView pinView = (MKPinAnnotationView) mapView.DequeueReusableAnnotation (BridgeAnnotationIdentifier);
				if (pinView == null) {
					MKPinAnnotationView customPinView = new MKPinAnnotationView (annotation, BridgeAnnotationIdentifier);
					customPinView.PinColor = MKPinAnnotationColor.Purple;
					customPinView.AnimatesDrop = true;
					customPinView.CanShowCallout = true;
					
					UIButton rightButton = UIButton.FromType (UIButtonType.DetailDisclosure);
					rightButton.AddTarget ((object sender, EventArgs ea) => showDetails (), UIControlEvent.TouchUpInside);
					customPinView.RightCalloutAccessoryView = rightButton;
					pinViews.Add (customPinView);
					return customPinView;
				} else {
					pinView.Annotation = annotation;
				}
				return pinView;
			} else if (annotation is SFAnnotation) { // for City of San Francisco
				const string SFAnnotationIdentifier = "SFAnnotationIdentifier";
				MKAnnotationView pinView = (MKAnnotationView) mapView.DequeueReusableAnnotation (SFAnnotationIdentifier);
				if (pinView == null) {
					MKAnnotationView annotationView = new MKAnnotationView (annotation, SFAnnotationIdentifier);
					annotationView.CanShowCallout = true;
					
					UIImage flagImage = UIImage.FromFile ("flag.png");
					
					CGRect resizeRect = CGRect.Empty;
					
					resizeRect.Size = flagImage.Size;
					CGSize maxSize = View.Bounds.Inset (AnnotationPadding, AnnotationPadding).Size;
					maxSize.Height -= NavigationController.NavigationBar.Frame.Size.Height - CalloutHeight;
					if (resizeRect.Size.Width > maxSize.Width)
						resizeRect.Size = new CGSize (maxSize.Width, resizeRect.Size.Height / resizeRect.Size.Width * maxSize.Width);
					if (resizeRect.Size.Height > maxSize.Height)
						resizeRect.Size = new CGSize (resizeRect.Size.Width / resizeRect.Size.Height * maxSize.Height, maxSize.Height);
				
					resizeRect.Location = CGPoint.Empty;
					UIGraphics.BeginImageContext (resizeRect.Size);
					flagImage.Draw (resizeRect);
					
					UIImage resizedImage = UIGraphics.GetImageFromCurrentImageContext ();
					UIGraphics.EndImageContext ();
					
					annotationView.Image = resizedImage;
					annotationView.Opaque = false;
					
					UIImageView sfIconView = new UIImageView (UIImage.FromFile ("SFIcon.png"));
					annotationView.LeftCalloutAccessoryView = sfIconView;
					pinViews.Add (annotationView);
					return annotationView;
				} else {
					pinView.Annotation = annotation;
				}
				return pinView;
			}
			return null;
		}
	}
}
