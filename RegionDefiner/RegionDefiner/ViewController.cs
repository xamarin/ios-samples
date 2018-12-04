using CoreGraphics;
using Foundation;
using MapKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;

namespace RegionDefiner
{
    public partial class ViewController : UIViewController, IMKMapViewDelegate, IUIGestureRecognizerDelegate
    {
        private readonly List<MyAnnotation> items = new List<MyAnnotation>();

        private MKPolygon polygon;

        protected ViewController(IntPtr handle) : base(handle) { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            mapView.Delegate = this;
        }

        partial void Log(UIBarButtonItem sender)
        {
            if (items.Count < 3)
            {
                Console.WriteLine("Minimum of 3 vertices to make polygon");
            }

            var builder = new StringBuilder("Coordinates:\n");
            foreach (var item in items)
            {
                builder.AppendLine($"{item.Coordinate.Longitude}, {item.Coordinate.Latitude},");
            }

            builder = builder.Remove(builder.Length - 2, 1);
            Console.WriteLine(builder);
        }

        partial void Reset(UIBarButtonItem sender)
        {
            if (mapView.Annotations != null && polygon != null)
            {
                mapView.RemoveAnnotations(mapView.Annotations);

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
                polygon.Dispose();
                polygon = null;
            }

            polygon = MKPolygon.FromCoordinates(points);
            mapView.AddOverlay(polygon);
        }

        #region IUIGestureRecognizerDelegate

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

        #endregion

        #region IMKMapViewDelegate

        private MKPolygonRenderer polygonRenderer;

        [Export("mapView:rendererForOverlay:")]
        public MKOverlayRenderer OverlayRenderer(MKMapView mapView, IMKOverlay overlay)
        {
            if (polygonRenderer != null && polygonRenderer.Polygon == polygon)
            {
                return polygonRenderer;
            }

            polygon = overlay as MKPolygon;
            polygonRenderer = new MKPolygonRenderer(polygon)
            {
                FillColor = new UIColor(0, 1, 0, .3f),
                StrokeColor = new UIColor(0, 1, 0, 0.9f),
                LineWidth = 1f
            };

            return polygonRenderer;
        }

        #endregion
    }
}