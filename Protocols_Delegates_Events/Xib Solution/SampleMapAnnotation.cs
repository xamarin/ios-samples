using System;
using CoreLocation;
using MapKit;

namespace Protocols_Delegates_Events
{
	/// <summary>
	/// Annotation class that subclasses MKAnnotation abstract class
	/// MKAnnotation is bound by MonoTouch to the MKAnnotation protocol
	/// </summary>
	public class SampleMapAnnotation : MKAnnotation
	{
		string _title;
		CLLocationCoordinate2D _coordinate;

		public SampleMapAnnotation (CLLocationCoordinate2D coordinate)
		{
			SetCoordinate (coordinate);
			_title = "Sample";
		}




		public override CLLocationCoordinate2D Coordinate {
			get {
				return _coordinate;
			}
		}

		public override void SetCoordinate (CLLocationCoordinate2D value)
		{
			_coordinate = value;
		}

		public override string Title {
			get {
				return _title;
			}
		}
	}
}

