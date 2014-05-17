// 
// WeatherMapViewController.cs
//  
// Author: Jeffrey Stedfast <jeff@xamarin.com>
// 
// Copyright (c) 2011 Xamarin Inc.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//

using System;
using CoreGraphics;
using UIKit;
using MapKit;
using Foundation;
using CoreLocation;

namespace WeatherMap
{
	public partial class WeatherMapViewController : UIViewController
	{
		WeatherForecastAnnotation[] annotations;
		WeatherServer weatherServer;
		
		public WeatherMapViewController (string nibName, NSBundle bundle) : base (nibName, bundle)
		{
			weatherServer = new WeatherServer ();
			Title = "Weather Map";
		}
		
		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}
		
		MKAnnotationView GetWeatherAnnotationView (MKMapView map, NSObject annotation)
		{
			MKAnnotationView annotationView = mapView.DequeueReusableAnnotation ("annotationViewID");
			
			if (annotationView == null)
				annotationView = new WeatherAnnotationView (annotation, "annotationViewID");
			
			annotationView.Annotation = (MKAnnotation) annotation;
			
			return annotationView;
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			// Get the region for North America
			MKCoordinateRegion region = new MKCoordinateRegion (
				new CLLocationCoordinate2D (37.37, -96.24),
				new MKCoordinateSpan (28.49, 31.025)
			);
			
			// Connect to the RegionChanged event so we can update the displayed forecasts
			// depending on what area of the map is shown.
			mapView.RegionChanged += (sender, e) => {
				// Remove the current annotations
				if (annotations != null)
					mapView.RemoveAnnotations (annotations);
				
				// Get and set our new list of WeatherForecastAnnotations
				annotations = weatherServer.GetForecastAnnotations (mapView.Region, 4);
				mapView.AddAnnotation (annotations);
			};
			
			mapView.GetViewForAnnotation += GetWeatherAnnotationView;

			mapView.SetRegion (region, false);
		}
		
		public override void ViewDidUnload ()
		{
			base.ViewDidUnload ();
			
			// Release any retained subviews of the main view.
			// e.g. myOutlet = null;
			if (annotations != null) {
				for (int i = 0; i < annotations.Length; i++) {
					annotations[i].Dispose ();
					annotations[i] = null;
				}
				
				annotations = null;
			}
			
			mapView.Dispose ();
			mapView = null;
		}
		
		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			return false;
		}
		
		protected override void Dispose (bool disposing)
		{
			if (weatherServer != null) {
				weatherServer.Dispose ();
				weatherServer = null;
			}
			
			base.Dispose (disposing);
		}
	}
}
