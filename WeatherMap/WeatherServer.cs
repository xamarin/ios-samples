// 
// WeatherServer.cs
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
using System.Collections.Generic;
using Foundation;
using MapKit;
using SQLite;

namespace WeatherMap
{
	public class WeatherServer : IDisposable
	{
		SQLiteConnection store;
		
		public WeatherServer ()
		{
			// Create our SQL connection context
			store = new SQLiteConnection ("WeatherMap.sqlite");
			
			// Create a table mapping for WeatherItem (if it doesn't already exist)
			store.CreateTable<WeatherForecast> ();
			
			// Check to see if we've already got some items in the database
			if (store.Table<WeatherForecast> ().Count () == 0) {
				// Populate our database with a list of WeatherItems
				store.Insert (new WeatherForecast () {
					Place = "S.F.",
					Latitude = 37.779941,
					Longitude = -122.417908,
					High = 80,
					Low = 50,
					Condition = WeatherConditions.Sunny
				});
				
				store.Insert (new WeatherForecast () {
					Place = "Denver",
					Latitude = 39.752601,
					Longitude = -104.982605,
					High = 40,
					Low = 30,
					Condition = WeatherConditions.Snow
				});
				
				store.Insert (new WeatherForecast () {
					Place = "Chicago",
					Latitude = 41.863425,
					Longitude = -87.652359,
					High = 45,
					Low = 29,
					Condition = WeatherConditions.Cloudy
				});
				
				store.Insert (new WeatherForecast () {
					Place = "Seattle",
					Latitude = 47.615884,
					Longitude = -122.332764,
					High = 75,
					Low = 45,
					Condition = WeatherConditions.Showers
				});
				
				store.Insert (new WeatherForecast () {
					Place = "Boston",
					Latitude = 42.350425,
					Longitude = -71.070557,
					High = 75,
					Low = 45,
					Condition = WeatherConditions.PartlyCloudy
				});
				
				store.Insert (new WeatherForecast () {
					Place = "Miami",
					Latitude = 25.780107,
					Longitude = -80.244141,
					High = 90,
					Low = 75,
					Condition = WeatherConditions.Thunderstorms
				});
				
				store.Commit ();
			}
		}
		
		public WeatherForecastAnnotation[] GetForecastAnnotations (MKCoordinateRegion region, int maxCount)
		{
			double longMin = region.Center.Longitude - region.Span.LongitudeDelta / 2.0;
			double longMax = region.Center.Longitude + region.Span.LongitudeDelta / 2.0;
			double latMin = region.Center.Latitude - region.Span.LatitudeDelta / 2.0;
			double latMax = region.Center.Latitude + region.Span.LatitudeDelta / 2.0;
			
			// Query for WeatherForecasts within our specified region
			var results = from item in store.Table<WeatherForecast> ()
				where (item.Latitude > latMin && item.Latitude < latMax && item.Longitude > longMin && item.Longitude < longMax)
					orderby item.Latitude
					orderby item.Longitude
					select item;
			
			// Iterate over the results and add them to a list
			var list = new List<WeatherForecastAnnotation> ();
			foreach (var forecast in results)
				list.Add (new WeatherForecastAnnotation (forecast));
			
			if (list.Count <= maxCount) {
				// We got fewer results than the max, so just return what we found
				return list.ToArray ();
			}
			
			// Calculate a stride so we can get an evenly distributed sampling of the results
			double index = 0.0, stride = (double) (list.Count - 1) / (double) maxCount;
			var annotations = new WeatherForecastAnnotation [maxCount];
			
			for (int i = 0; i < maxCount && (int) index < list.Count; i++, index += stride)
				annotations[i] = list[(int) index];
			
			return annotations;
		}
		
		public void Dispose ()
		{
			if (store != null) {
				store.Commit ();
				store.Dispose ();
				store = null;
			}
		}
	}
}

