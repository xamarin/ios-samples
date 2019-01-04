using CoreGraphics;
using CoreLocation;
using Foundation;
using MapCallouts.Annotations;
using MapKit;
using System;
using System.Collections.Generic;
using System.Linq;
using UIKit;

namespace MapCallouts
{
    /// <summary>
    /// The primary view controller containing the `MKMapView`, as well as adding and removing `MKMarkerAnnotationView` through its toolbar.
    /// </summary>
    public partial class MapViewController : UIViewController, IMKMapViewDelegate
    {
        private List<MKAnnotation> allAnnotations;

        private List<MKAnnotation> displayedAnnotations;

        public MapViewController(IntPtr handle) : base(handle) { }

        protected List<MKAnnotation> DisplayedAnnotations
        {
            get
            {
                return this.displayedAnnotations;
            }

            set
            {
                if (this.displayedAnnotations != null)
                {
                    this.mapView.RemoveAnnotations(this.displayedAnnotations.ToArray());
                }

                this.displayedAnnotations = value;

                if (this.displayedAnnotations != null)
                {
                    this.mapView.AddAnnotations(this.displayedAnnotations.ToArray());
                }

                this.CenterMapOnSanFrancisco();
            }
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            this.RegisterMapAnnotationViews();


            var flowerAnnotation = new CustomAnnotation(new CLLocationCoordinate2D(37.772_623f, -122.460_217f),
                                                        NSBundle.MainBundle.GetLocalizedString("FLOWERS_TITLE"));
            flowerAnnotation.ImageName = "conservatory_of_flowers";

            // Create the array of annotations and the specific annotations for the points of interest.
            allAnnotations = new List<MKAnnotation> { new SanFranciscoAnnotation(), new BridgeAnnotation(), new FerryBuildingAnnotation(), flowerAnnotation };

            // Dispaly all annotations on the map.
            this.ShowAllAnnotations(this);
        }

        /// <summary>
        /// Register the annotation views with the `mapView` so the system can create and efficently reuse the annotation views.
        /// </summary>
        private void RegisterMapAnnotationViews()
        {
            this.mapView.Register(typeof(MKMarkerAnnotationView), nameof(BridgeAnnotation));
            this.mapView.Register(typeof(CustomAnnotationView), nameof(CustomAnnotation));
            this.mapView.Register(typeof(MKAnnotationView), nameof(SanFranciscoAnnotation));
            this.mapView.Register(typeof(MKMarkerAnnotationView), nameof(FerryBuildingAnnotation));
        }

        private void CenterMapOnSanFrancisco()
        {
            var span = new MKCoordinateSpan(0.2f, 0.2f);
            var center = new CLLocationCoordinate2D(37.786_996f, -122.440_100f);
            this.mapView.SetRegion(new MKCoordinateRegion(center, span), true);
        }

        partial void ShowOnlySanFranciscoAnnotation(NSObject sender)
        {
            // User tapped "City" button in the bottom toolbar
            this.DisplayOne(typeof(SanFranciscoAnnotation));
        }

        partial void ShowOnlyBridgeAnnotation(NSObject sender)
        {
            // User tapped "Bridge" button in the bottom toolbar
            this.DisplayOne(typeof(BridgeAnnotation));
        }

        partial void ShowOnlyFlowerAnnotation(NSObject sender)
        {
            // User tapped "Flower" button in the bottom toolbar
            this.DisplayOne(typeof(CustomAnnotation));
        }

        partial void ShowOnlyFerryBuildingAnnotation(NSObject sender)
        {
            // User tapped "Ferry" button in the bottom toolbar
            this.DisplayOne(typeof(FerryBuildingAnnotation));
        }

        partial void ShowAllAnnotations(NSObject sender)
        {
            // User tapped "All" button in the bottom toolbar
            this.DisplayedAnnotations = this.allAnnotations;
        }

        private void DisplayOne(Type annotationType)
        {
            var annotation = this.allAnnotations.FirstOrDefault(item => item.GetType() == annotationType);
            if (annotation != null)
            {
                this.DisplayedAnnotations = new List<MKAnnotation> { annotation };
            }
            else
            {
                this.DisplayedAnnotations = new List<MKAnnotation>();
            }
        }

        #region IMKMapViewDelegate

        /// <summary>
        /// Called whent he user taps the disclosure button in the bridge callout.
        /// </summary>
        [Export("mapView:annotationView:calloutAccessoryControlTapped:")]
        public void CalloutAccessoryControlTapped(MKMapView mapView, MKAnnotationView view, UIControl control)
        {
            // This illustrates how to detect which annotation type was tapped on for its callout.
            if (view.Annotation is BridgeAnnotation annotation)
            {
                Console.WriteLine("Tapped Golden Gate Bridge annotation accessory view");

                var detailNavController = base.Storyboard?.InstantiateViewController("DetailNavController");
                if (detailNavController != null)
                {
                    detailNavController.ModalPresentationStyle = UIModalPresentationStyle.Popover;
                    var presentationController = detailNavController.PopoverPresentationController;
                    if (presentationController != null)
                    {
                        presentationController.PermittedArrowDirections = UIPopoverArrowDirection.Any;

                        // Anchor the popover to the button that triggered the popover.
                        presentationController.SourceRect = control.Frame;
                        presentationController.SourceView = control;
                    }

                    base.PresentViewController(detailNavController, true, null);
                }
            }
        }

        /// <summary>
        /// The map view asks `mapView(_:viewFor:)` for an appropiate annotation view for a specific annotation.
        /// </summary>
        [Export("mapView:viewForAnnotation:")]
        public MKAnnotationView GetViewForAnnotation(MKMapView mapView, IMKAnnotation annotation)
        {
            MKAnnotationView result = null;
            if (!(annotation is MKUserLocation))
            {
                if (annotation is BridgeAnnotation bridgeAnnotation)
                {
                    result = this.SetupBridgeAnnotationView(bridgeAnnotation, this.mapView);
                }
                else if (annotation is CustomAnnotation customAnnotation)
                {
                    result = this.SetupCustomAnnotationView(customAnnotation, this.mapView);
                }
                else if (annotation is SanFranciscoAnnotation sanFranciscoAnnotation)
                {
                    result = this.SetupSanFranciscoAnnotationView(sanFranciscoAnnotation, this.mapView);
                }
                else if (annotation is FerryBuildingAnnotation ferryBuildingAnnotation)
                {
                    result = this.SetupFerryBuildingAnnotationView(ferryBuildingAnnotation, this.mapView);
                }
            }

            return result;
        }

        /// <summary>
        /// The map view asks `mapView(_:viewFor:)` for an appropiate annotation view for a specific annotation. The annotation
        /// should be configured as needed before returning it to the system for display.
        /// </summary>
        private MKAnnotationView SetupSanFranciscoAnnotationView(SanFranciscoAnnotation annotation, MKMapView mapView)
        {
            var reuseIdentifier = nameof(SanFranciscoAnnotation);
            var flagAnnotationView = mapView.DequeueReusableAnnotation(reuseIdentifier, annotation);

            flagAnnotationView.CanShowCallout = true;

            // Provide the annotation view's image.
            var image = UIImage.FromBundle("flag");
            flagAnnotationView.Image = image;

            // Provide the left image icon for the annotation.
            flagAnnotationView.LeftCalloutAccessoryView = new UIImageView(UIImage.FromBundle("sf_icon"));

            // Offset the flag annotation so that the flag pole rests on the map coordinate.
            var offset = new CGPoint(image.Size.Width / 2f, -(image.Size.Height / 2f));
            flagAnnotationView.CenterOffset = offset;

            return flagAnnotationView;
        }

        /// <summary>
        /// Create an annotation view for the Golden Gate Bridge, customize the color, and add a button to the callout.
        /// </summary>
        private MKAnnotationView SetupBridgeAnnotationView(BridgeAnnotation annotation, MKMapView mapView)
        {
            var identifier = nameof(BridgeAnnotation);
            var view = mapView.DequeueReusableAnnotation(identifier, annotation);
            if (view is MKMarkerAnnotationView markerAnnotationView)
            {
                markerAnnotationView.AnimatesWhenAdded = true;
                markerAnnotationView.CanShowCallout = true;
                markerAnnotationView.MarkerTintColor = UIColor.FromName("internationalOrange");

                /*
                 Add a detail disclosure button to the callout, which will open a new view controller or a popover.
                 When the detail disclosure button is tapped, use mapView(_:annotationView:calloutAccessoryControlTapped:)
                 to determine which annotation was tapped.
                 If you need to handle additional UIControl events, such as `.touchUpOutside`, you can call
                 `addTarget(_:action:for:)` on the button to add those events.
                 */
                var rightButton = new UIButton(UIButtonType.DetailDisclosure);
                markerAnnotationView.RightCalloutAccessoryView = rightButton;
            }

            return view;
        }

        private MKAnnotationView SetupCustomAnnotationView(CustomAnnotation annotation, MKMapView mapView)
        {
            return mapView.DequeueReusableAnnotation(nameof(CustomAnnotation), annotation);
        }

        /// <summary>
        /// Create an annotation view for the Ferry Building, and add an image to the callout.
        /// </summary>
        private MKAnnotationView SetupFerryBuildingAnnotationView(FerryBuildingAnnotation annotation, MKMapView mapView)
        {
            var identifier = nameof(FerryBuildingAnnotation);
            var view = mapView.DequeueReusableAnnotation(identifier, annotation);
            if (view is MKMarkerAnnotationView markerAnnotationView)
            {
                markerAnnotationView.AnimatesWhenAdded = true;
                markerAnnotationView.CanShowCallout = true;
                markerAnnotationView.MarkerTintColor = UIColor.Purple;

                // Provide an image view to use as the accessory view's detail view.
                markerAnnotationView.DetailCalloutAccessoryView = new UIImageView(UIImage.FromBundle("ferry_building"));
            }

            return view;
        }

        #endregion
    }
}