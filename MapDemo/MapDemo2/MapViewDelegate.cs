using MapKit;
using UIKit;

namespace MapDemo
{
    public class MapViewDelegate : MKMapViewDelegate
    {
        private const string MonkeyId = "MonkeyAnnotation";
        private const string PinId = "PinAnnotation";

        public override MKAnnotationView GetViewForAnnotation(MKMapView mapView, IMKAnnotation annotation)
        {
            MKAnnotationView result = null;

            if (annotation is MKUserLocation)
                return result;

            if (annotation is MonkeyAnnotation)
            {
                // show monkey annotation
                result = mapView.DequeueReusableAnnotation(MonkeyId) ?? new MKAnnotationView(annotation, MonkeyId);
                result.Draggable = true;
                result.CanShowCallout = true;
                result.Image = UIImage.FromBundle("Monkey");
                result.RightCalloutAccessoryView = UIButton.FromType(UIButtonType.DetailDisclosure);
            }
            else
            {
                // show pin annotation
                var annotationView = mapView.DequeueReusableAnnotation(PinId) as MKPinAnnotationView ?? new MKPinAnnotationView(annotation, PinId);
                annotationView.PinTintColor = UIColor.Red;
                annotationView.CanShowCallout = true;

                result = annotationView;
            }

            return result;
        }

        public override void CalloutAccessoryControlTapped(MKMapView mapView, MKAnnotationView view, UIControl control)
        {
            if (view.Annotation is MonkeyAnnotation monkeyAnnotation)
            {
                var alert = UIAlertController.Create("Monkey Annotation", monkeyAnnotation.Title, UIAlertControllerStyle.Alert);
                alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                UIApplication.SharedApplication.Windows[0].RootViewController.PresentViewController(alert, true, null);
            }
        }

        public override MKOverlayRenderer OverlayRenderer(MKMapView mapView, IMKOverlay overlay)
        {
            return new MKCircleRenderer(overlay as MKCircle)
            {
                FillColor = UIColor.Red,
                Alpha = 0.4f
            };
        }
    }
}