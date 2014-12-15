//
// WorldCitiesViewController.cs
//
// Author:
//       Mike Krüger <mkrueger@xamarin.com>
//
// Copyright (c) 2011 Xamarin Inc. (http://xamarin.com)
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
using UIKit;
using CoreGraphics;
using System;
using Foundation;
using System.Collections.Generic;
using MapKit;
using CoreLocation;
using ObjCRuntime;

namespace WorldCities
{
	public partial class WorldCitiesViewController : UIViewController
	{
		public WorldCity SelectedCity { get; set; }

		public WorldCitiesViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			if (SelectedCity != null)
				ChooseWorldCity ();
		}

		partial void SelectorChanged (Foundation.NSObject sender)
		{
			var segmented = sender as UISegmentedControl;
			if (segmented == null)
				return;
			switch (segmented.SelectedSegment) {
			case 1:
				MapView.MapType = MapKit.MKMapType.Satellite;
				break;
			case 2:
				MapView.MapType = MapKit.MKMapType.Hybrid;
				break;
			default:
				MapView.MapType = MapKit.MKMapType.Standard;
				break;
			}
		}

		[Export("AnimateToWorld")]
		void AnimateToWorld ()
		{
			var worldCity = SelectedCity;
			var current = MapView.Region;
			var zoomOut = new MKCoordinateRegion (new CLLocationCoordinate2D ((current.Center.Latitude + worldCity.Latitude) / 2.0, (current.Center.Longitude + worldCity.Longitude) / 2.0),
				new MKCoordinateSpan (90, 90));
			MapView.SetRegion (zoomOut, true);
		}

		[Export("AnimateToPlace")]
		void AnimateToPlace ()
		{
			var worldCity = SelectedCity;
			var region = new MKCoordinateRegion (new CLLocationCoordinate2D (worldCity.Latitude, worldCity.Longitude),
				new MKCoordinateSpan (0.4, 0.4));
			MapView.SetRegion (region, true);
		}

		void ChooseWorldCity ()
		{
			var aPlace = SelectedCity;
			Title = aPlace.Name;
			var current = MapView.Region;
			if (current.Span.LatitudeDelta < 10) {
				PerformSelector (new Selector ("AnimateToWorld"), null, 0.3f);
				PerformSelector (new Selector ("AnimateToPlace"), null, 1.7f);
			} else {
				PerformSelector (new Selector ("AnimateToPlace"), null, 0.3f);
			}
		}

		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			// Return true for supported orientations
			return (toInterfaceOrientation != UIInterfaceOrientation.PortraitUpsideDown);
		}
	}
}
