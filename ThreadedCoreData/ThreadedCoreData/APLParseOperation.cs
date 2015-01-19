using System;
using System.Json;
using Foundation;
using CoreData;
using System.Collections.Generic;
using ObjCRuntime;
using UIKit;
using System.Linq;

namespace ThreadedCoreData
{
	public class APLParseOperation : NSOperation
	{
		public const string EarthquakesErrorNotificationName = "EarthquakeErrorNotif";
		public const string EarthquakesMessageErrorKey = "EarthquakesMsgErrorKey";
		const int MaximumNumberOfEarthquakesToParse = 50;
		const int SizeOfEarthquakesBatch = 10;
		NSPersistentStoreCoordinator sharedPSC;
		NSManagedObjectContext managedObjectContext;
		NSData earthquakeData;
		NSDateFormatter dateFormatter;

		public APLParseOperation (NSData data, NSPersistentStoreCoordinator persistentStoreCoordinator)
		{
			dateFormatter = new NSDateFormatter () {
				DateFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'",
				TimeZone = NSTimeZone.LocalTimeZone,
				Locale = NSLocale.FromLocaleIdentifier ("en_US_POSIX")
			};

			earthquakeData = data;
			sharedPSC = persistentStoreCoordinator;
		}

		public override void Main ()
		{
			base.Main ();

			managedObjectContext = new NSManagedObjectContext () {
				PersistentStoreCoordinator = sharedPSC
			};

			Parse (earthquakeData);
		}

		void Parse (NSData data)
		{
			try {
				var earthquakes = new List<Earthquake> ();

				var dump = JsonValue.Parse (NSString.FromData (data, NSStringEncoding.UTF8)) as JsonObject;
				JsonValue featureCollection = dump ["features"];

				for (int i = 0; i < MaximumNumberOfEarthquakesToParse; i++) {

					var currentEarthquake = new Earthquake ();

					var earthquake = featureCollection [i] as JsonObject;
					JsonValue earthquakeProperties = earthquake ["properties"];
					currentEarthquake.Magnitude = NSNumber.FromFloat ((float)earthquakeProperties ["mag"]);
					currentEarthquake.USGSWebLink = new NSString ((string)earthquakeProperties ["url"]);
					currentEarthquake.Location = new NSString ((string)earthquakeProperties ["place"]);

					//date and time in milliseconds since epoch
					var seconds = (Int64)earthquakeProperties ["time"];
					var date = new DateTime (1970, 1, 1, 0, 0, 0).AddMilliseconds (seconds);
					string str = date.ToString ("yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'");
					currentEarthquake.Date = dateFormatter.Parse (str);

					JsonValue earthquakeGeometry = earthquake ["geometry"];
					var coordinates = earthquakeGeometry ["coordinates"] as JsonArray;
					currentEarthquake.Longitude = NSNumber.FromFloat (coordinates [0]);
					currentEarthquake.Latitude = NSNumber.FromFloat (coordinates [1]);

					if (earthquakes.Count > SizeOfEarthquakesBatch) {
						AddEarthquakesToList (earthquakes);
						earthquakes.Clear ();
					} else {
						earthquakes.Add (currentEarthquake);
					}
				}

				if (earthquakes.Count > 0)
					AddEarthquakesToList (earthquakes);

			} catch (Exception e) {
				Console.WriteLine (e.StackTrace + e.Message);
				var userInfo = new NSDictionary (NSError.LocalizedDescriptionKey, "Error while parsing GeoJSON");

				var parsingError = new NSError (new NSString (), 0, userInfo);
				InvokeOnMainThread (new Selector ("HandleEarthquakesError:"), parsingError);
			}
		}

		[Export("HandleEarthquakesError:")]
		public void HandleEarthquakesError (NSError parserError)
		{
			var userInfo = new NSDictionary (EarthquakesMessageErrorKey, parserError);
			NSNotificationCenter.DefaultCenter.PostNotificationName (EarthquakesErrorNotificationName, this, userInfo);
		}

		void AddEarthquakesToList (List<Earthquake> earthquakes)
		{
			var entity = NSEntityDescription.EntityForName ("Earthquake", managedObjectContext);
			var fetchRequest = new NSFetchRequest ();
			fetchRequest.Entity = entity;

			var date = (NSPropertyDescription)entity.PropertiesByName.ValueForKey (new NSString ("date"));
			var location = (NSPropertyDescription)entity.PropertiesByName.ValueForKey (new NSString ("location"));

			fetchRequest.PropertiesToFetch = new NSPropertyDescription[] { date, location };
			fetchRequest.ResultType = NSFetchRequestResultType.DictionaryResultType;

			NSError error;

			foreach (var earthquake in earthquakes) {
				var arguments = new NSObject[] { earthquake.Location, earthquake.Date };
				fetchRequest.Predicate = NSPredicate.FromFormat ("location = %@ AND date = %@", arguments);
				var fetchedItems = NSArray.FromNSObjects (managedObjectContext.ExecuteFetchRequest (fetchRequest, out error));

				if (fetchedItems.Count == 0) {

					if (string.IsNullOrEmpty (entity.Description))
						continue;

					var managedEarthquake = new ManagedEarthquake (entity, managedObjectContext) {
						Magnitude = earthquake.Magnitude,
						Location = earthquake.Location,
						Date = earthquake.Date,
						USGSWebLink = earthquake.USGSWebLink,
						Latitude = earthquake.Latitude,
						Longitude = earthquake.Longitude
					};

					managedObjectContext.InsertObject (managedEarthquake);
				}

				var gregorian = new NSCalendar (NSCalendarType.Gregorian);
				var offsetComponents = new NSDateComponents ();
				offsetComponents.Day = -14;// 14 days back from today
				NSDate twoWeeksAgo = gregorian.DateByAddingComponents (offsetComponents, NSDate.Now, NSCalendarOptions.None);

				// use the same fetchrequest instance but switch back to NSManagedObjectResultType
				fetchRequest.ResultType = NSFetchRequestResultType.ManagedObject;
				fetchRequest.Predicate = NSPredicate.FromFormat ("date < %@", new NSObject[] { twoWeeksAgo });

				var olderEarthquakes = NSArray.FromObjects (managedObjectContext.ExecuteFetchRequest (fetchRequest, out error));

				for (nuint i = 0; i < olderEarthquakes.Count; i++)
					managedObjectContext.DeleteObject (olderEarthquakes.GetItem<ManagedEarthquake> (i));

				if (managedObjectContext.HasChanges) {
					if (!managedObjectContext.Save (out error))
						Console.WriteLine (string.Format ("Unresolved error {0}", error.LocalizedDescription));
				}
			}
		}
	}
}

