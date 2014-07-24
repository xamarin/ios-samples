using System;
using CoreGraphics;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using MapKit;
using Foundation;
using UIKit;

using CoreLocation;

namespace RegionDefiner
{
	public  partial class RegionDefinerViewController : UIViewController
	{
		MKMapView mainMapView;
		UILongPressGestureRecognizer longPress;
		MyAnnotation droppedPin;
		static List<MyAnnotation> itemsArray;
		
		public static MKPolygon Polygon { get ; set; }
		
		public static MKPolygonView PolygonView { get; set; }
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			CGSize containerSize = View.Bounds.Size;
			
			mainMapView = new MKMapView () {
				UserInteractionEnabled = true,
				Delegate = new MapDelegate (),
				ScrollEnabled = true,
			};
			
			var toolbar = new UIToolbar () {
				AutoresizingMask = UIViewAutoresizing.FlexibleTopMargin | UIViewAutoresizing.FlexibleWidth,
			};
			toolbar.SizeToFit ();
			CGSize toolbarSize = toolbar.Bounds.Size;
			
			toolbar.Frame = new CGRect (0, containerSize.Height - toolbarSize.Height, containerSize.Width, toolbarSize.Height);
			mainMapView.Frame = new CGRect (0, 0, containerSize.Width, containerSize.Height - toolbarSize.Height);
			
			var resetButton = new UIBarButtonItem ("Reset", UIBarButtonItemStyle.Bordered, removePins);
			var flexibleSpace = new UIBarButtonItem (UIBarButtonSystemItem.FlexibleSpace, null, null);
			var share = new UIBarButtonItem ("Log", UIBarButtonItemStyle.Bordered, tappedShare);
			
			View.AddSubview (mainMapView);
			View.AddSubview (toolbar);
			
			toolbar.SetItems (new UIBarButtonItem [] {
				resetButton,
				flexibleSpace,
				share
			}, true);
			
			setUpGesture ();
			itemsArray = new List<MyAnnotation> ();
			
		}
		
		public void setUpGesture ()
		{
			longPress = new UILongPressGestureRecognizer ();
			longPress.AddTarget (this, new ObjCRuntime.Selector ("HandleLongPress:"));
			longPress.Delegate = new GestureDelegate ();
			View.AddGestureRecognizer (longPress);
			
		}
		
		[Foundation.Export("HandleLongPress:")]
		public void handleLongPress (UILongPressGestureRecognizer recognizer)
		{
			if (recognizer.State == UIGestureRecognizerState.Began) {
				CGPoint longPressPoint = recognizer.LocationInView (mainMapView);
				dropPinAtPoint (longPressPoint);
			}
		}
		
		public void updatePolygon ()
		{
			var points = itemsArray.Select (l => l.Coordinate).ToArray ();
			if (Polygon != null)
				mainMapView.RemoveOverlay (Polygon);
			Polygon = MKPolygon.FromCoordinates (points);
			mainMapView.AddOverlay (Polygon);			
		}
		
		public void dropPinAtPoint (CGPoint pointToConvert)
		{
			CLLocationCoordinate2D convertedPoint = mainMapView.ConvertPoint (pointToConvert, mainMapView);
			String pinTitle = String.Format ("Pin Number {0}", itemsArray.Count);
			String subCoordinates = String.Format ("{0},{1}", convertedPoint.Latitude.ToString (), convertedPoint.Longitude.ToString ());
			droppedPin = new MyAnnotation (convertedPoint, pinTitle, subCoordinates);
			mainMapView.AddAnnotation (droppedPin);
			itemsArray.Add (droppedPin);
			updatePolygon ();
		}
		
		public void removePins (object sender, EventArgs args)
		{
			if (mainMapView.Annotations != null && Polygon != null) {
				mainMapView.RemoveAnnotations (mainMapView.Annotations);
				itemsArray.RemoveRange (0, itemsArray.Count);
				mainMapView.RemoveOverlay (Polygon);
				updatePolygon ();
			}
		}
		
		public void tappedShare (object sender, EventArgs args)
		{
			Console.WriteLine (coordinates ());
		}
		
		public string coordinates ()
		{
			if (itemsArray.Count < 3) 
				return "Minimum of 3 vertices to make polygon";
			
			var masterString = new StringBuilder ("\n" + "{ " + "\"type\"" + ":" + "  \"MultiPolygon\"" + ",\n" + " \"coordinates\"" + ":" + " [ " + " \n" + "[ [");
			foreach (MyAnnotation pin in itemsArray)
				masterString = masterString.AppendFormat (" [{0}, {1}] , \n", pin.Coordinate.Longitude, pin.Coordinate.Latitude);
			
			masterString = masterString.Append ("]]" + "\n" + "]" + "\n" + "}");
			masterString = masterString.Replace ("]" + " , \n" + "]]", "] ] ]"); 
			return masterString.ToString ();
			
		}
	}
}


