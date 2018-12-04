using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreGraphics;
using Foundation;
using MapKit;
using UIKit;

namespace RegionDefiner
{
    public partial class ViewController : UIViewController, IMKMapViewDelegate, IUIGestureRecognizerDelegate
    {
        private readonly List<MyAnnotation> items = new List<MyAnnotation>();

        protected ViewController(IntPtr handle) : base(handle) { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            mapView.Delegate = this;
        }

        partial void Log(UIBarButtonItem sender)
        {
            Console.WriteLine(coordinates());
        }

        public string coordinates()
        {
            if (items.Count < 3)
                return "Minimum of 3 vertices to make polygon";

            var masterString = new StringBuilder("\n" + "{ " + "\"type\"" + ":" + "  \"MultiPolygon\"" + ",\n" + " \"coordinates\"" + ":" + " [ " + " \n" + "[ [");
            foreach (MyAnnotation pin in items)
                masterString = masterString.AppendFormat(" [{0}, {1}] , \n", pin.Coordinate.Longitude, pin.Coordinate.Latitude);

            masterString = masterString.Append("]]" + "\n" + "]" + "\n" + "}");
            masterString = masterString.Replace("]" + " , \n" + "]]", "] ] ]");
            return masterString.ToString();

        }

        partial void Reset(UIBarButtonItem sender)
        {
            if (mapView.Annotations != null && polygon != null)
            {
                mapView.RemoveAnnotations(mapView.Annotations);
                mapView.RemoveOverlay(polygon);

                items.Clear();
                UpdatePolygon();
            }
        }

        private void UpdatePolygon()
        {
            var points = items.Select(item => item.Coordinate).ToArray();
            if (polygon != null)
            {
                mapView.RemoveOverlay(polygon);
            }

            polygon = MKPolygon.FromCoordinates(points);
            mapView.AddOverlay(polygon);
        }

        partial void HandleLongPress(UILongPressGestureRecognizer recognizer)
        {
            if (recognizer.State == UIGestureRecognizerState.Began)
            {
                var longPressPoint = recognizer.LocationInView(mapView);
                DropPinAtPoint(longPressPoint);
            }
        }

        private void DropPinAtPoint(CGPoint pointToConvert)
        {
            var convertedPoint = mapView.ConvertPoint(pointToConvert, mapView);
            var pinTitle = $"Pin Number {items.Count}";
            var subCoordinates = $"{convertedPoint.Latitude},{convertedPoint.Longitude}";
            var droppedPin = new MyAnnotation(convertedPoint, pinTitle, subCoordinates);
            mapView.AddAnnotation(droppedPin);
            items.Add(droppedPin);
            UpdatePolygon();
        }


        #region IMKMapViewDelegate

        private MKPolygon polygon;

        private MKPolygonView polygonView;

        [Export("mapView:viewForOverlay:")]
        public MKOverlayView GetViewForOverlay(MKMapView mapView, IMKOverlay overlay)
        {
            if (polygonView != null && polygonView.Polygon == polygon)
                return polygonView;

            polygon = overlay as MKPolygon;
            polygonView = new MKPolygonView(polygon)
            {
                FillColor = new UIColor(0, 1, 0, .3f),
                StrokeColor = new UIColor(0, 1, 0, 0.9f),
                LineWidth = 1f
            };

            return polygonView;
        }

        #endregion
    }
}
