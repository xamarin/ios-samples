using System;
using CoreData;
using Foundation;
using ObjCRuntime;

namespace ThreadedCoreData
{
	public class ManagedEarthquake : NSManagedObject
	{
		public NSNumber Magnitude { 
			get {
				return (NSNumber)Runtime.GetNSObject (ValueForKey (new NSString ("magnitude")));
			}
			set {
				SetValueForKey (value, new NSString ("magnitude"));
			} 
		}

		public NSString Location { 
			get {
				return (NSString)Runtime.GetNSObject (ValueForKey (new NSString ("location")));
			} 
			set {
				SetValueForKey (value, new NSString ("location"));
			}
		}

		public NSDate Date {
			get {
				return (NSDate)Runtime.GetNSObject (ValueForKey (new NSString ("date")));
			} 
			set {
				SetValueForKey (value, new NSString ("date"));
			}
		}

		public NSString USGSWebLink {
			get {
				return (NSString)Runtime.GetNSObject (ValueForKey (new NSString ("USGSWebLink")));
			} 
			set {
				SetValueForKey (value, new NSString ("USGSWebLink"));
			}
		}

		public NSNumber Latitude {
			get {
				return (NSNumber)Runtime.GetNSObject (ValueForKey (new NSString ("latitude")));
			} 
			set {
				SetValueForKey (value, new NSString ("latitude"));
			}
		}

		public NSNumber Longitude {
			get {
				return (NSNumber)Runtime.GetNSObject (ValueForKey (new NSString ("longitude")));
			} 
			set {
				SetValueForKey (value, new NSString ("longitude"));
			}
		}

		public ManagedEarthquake (NSEntityDescription description, NSManagedObjectContext context) : base(description, context)
		{
			this.IsDirectBinding = true;
		}

		public ManagedEarthquake (IntPtr handle): base(handle)
		{
		}
	}

	public class Earthquake
	{
		public NSNumber Magnitude { get; set; }

		public NSString Location { get; set; }

		public NSDate Date { get; set; }

		public NSString USGSWebLink { get; set; }

		public NSNumber Latitude { get; set; }

		public NSNumber Longitude { get; set; }

		public Earthquake ()
		{
		}
	}
}