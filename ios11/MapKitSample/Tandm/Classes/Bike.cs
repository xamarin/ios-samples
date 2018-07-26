using System;
using System.Collections.Generic;
using Foundation;
using UIKit;
using MapKit;
using CoreLocation;

namespace Tandm
{
	public class Bike : MKPointAnnotation
	{
		#region Static Methods
		public static Bike[] FromDictionaryArray(NSArray dictionaryArray) {
			var bikes = new List<Bike>();

			for (nuint n = 0; n < dictionaryArray.Count; ++n){
				var dictionary = dictionaryArray.GetItem<NSDictionary<NSString, NSNumber>>(n);
				if (dictionary !=null) {
					var lat = dictionary[new NSString("lat")];
					var lgn = dictionary[new NSString("long")];
					var type = dictionary[new NSString("type")];
					bikes.Add(new Bike(lat, lgn, type));
				}
			}

			return bikes.ToArray();
		}
		#endregion

		#region Computed Properties
		public BikeType Type { get; set; } = BikeType.Tricycle;
		#endregion

		#region Constructors
		public Bike()
		{
		}

		public Bike(NSNumber lat, NSNumber lgn, NSNumber type)
		{
			// Initialize
			Coordinate = new CLLocationCoordinate2D(lat.NFloatValue, lgn.NFloatValue);

			switch(type.NUIntValue) {
				case 0:
					Type = BikeType.Unicycle;
					break;
				case 1:
					Type = BikeType.Tricycle;
					break;
			}
		}
		#endregion
	}
}
