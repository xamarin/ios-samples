using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using HomeKit;

namespace HomeKitCatalog
{
	// Handles interactions with `NSUserDefault`s to save the user's favorite accessories.
	public class FavoritesManager
	{
		const string accessoryToCharacteristicIdentifierMappingKey = "FavoritesManager.accessoryToCharacteristicIdentifierMappingKey";
		const string accessoryIdentifiersKey = "FavoritesManager.accessoryIdentifiersKey";

		// A shared, singleton manager.
		static readonly FavoritesManager sharedManager = new FavoritesManager ();

		public static FavoritesManager SharedManager {
			get {
				return sharedManager;
			}
		}

		// An internal mapping of accessory unique identifiers to an array of their
		// favorite characteristic's unique identifiers.
		readonly Dictionary<NSUuid, List<NSUuid>> accessoryToCharacteristicIdentifiers = new Dictionary<NSUuid, List<NSUuid>> ();

		static HMHome Home {
			get {
				return HomeStore.SharedStore.Home;
			}
		}

		static HMHomeManager HomeManager {
			get {
				return HomeStore.SharedStore.HomeManager;
			}
		}

		static HMHome[] Homes {
			get {
				return HomeManager.Homes;
			}
		}

		FavoritesManager ()
		{
			var userDefaults = NSUserDefaults.StandardUserDefaults;

			var mapData = userDefaults.DataForKey (accessoryToCharacteristicIdentifierMappingKey);
			if (mapData != null) {
				var rawDictionary = NSKeyedUnarchiver.UnarchiveObject (mapData) as NSDictionary;
				if (rawDictionary != null) {
					foreach (var kvp in rawDictionary)
						accessoryToCharacteristicIdentifiers [(NSUuid)kvp.Key] = new List<NSUuid> (NSArray.FromArray<NSUuid> ((NSArray)kvp.Value));
				}
			}
		}

		// returns:  An array of all favorite characteristics. The array is sorted by localized type.
		public HMCharacteristic[] FavoriteCharacteristics ()
		{
			// Find all of the favorite characteristics.
			var favoriteCharacteristics = Homes.SelectMany (home => {
				return home.GetAllCharacteristics ().Where (c => c.IsFavorite ());
			}).ToArray ();

			Array.Sort (favoriteCharacteristics, CharacteristicOrderedBefore);
			return favoriteCharacteristics;
		}

		// returns:  An array of all favorite accessories. The array is sorted by localized name.
		public HMAccessory[] FavoriteAccessories ()
		{
			var ids = new HashSet<NSUuid> (accessoryToCharacteristicIdentifiers.Keys);

			HMAccessory[] allAccessories = Homes.SelectMany (h => h.Accessories)
				.Where (a => ids.Contains (a.UniqueIdentifier))
				.ToArray ();

			allAccessories.SortByLocalizedName (a => a.Name);
			return allAccessories;
		}

		// returns:  An array of tuples representing accessories and all favorite characteristics they contain.
		// The array is sorted by localized type.
		public Tuple<HMAccessory, HMCharacteristic[]>[] FavoriteGroups ()
		{
			return FavoriteAccessories ().Select (a => {
				var characteristics = FavoriteCharacteristicsForAccessory (a);
				return new Tuple<HMAccessory, HMCharacteristic[]> (a, characteristics);
			}).ToArray ();
		}

		// Evaluates whether or not an `HMCharacteristic` is a favorite.
		public bool IsFavorite (HMCharacteristic characteristic)
		{
			var service = characteristic.Service;
			if (service == null)
				return false;

			var accessory = service.Accessory;
			if (accessory == null)
				return false;

			List<NSUuid> characteristicIdentifiers;
			var accessoryIdentifier = accessory.UniqueIdentifier;
			if (!accessoryToCharacteristicIdentifiers.TryGetValue (accessoryIdentifier, out characteristicIdentifiers))
				return false;

			return characteristicIdentifiers.Contains (characteristic.UniqueIdentifier);
		}

		// Favorites a characteristic.
		public void FavoriteCharacteristic (HMCharacteristic characteristic)
		{
			if (IsFavorite (characteristic))
				return;

			var service = characteristic.Service;
			if (service == null)
				return;

			var accessory = service.Accessory;
			if (accessory == null)
				return;

			List<NSUuid> characteristicIdentifiers;
			var aId = accessory.UniqueIdentifier;
			var cId = characteristic.UniqueIdentifier;
			bool alreadyFavorite = accessoryToCharacteristicIdentifiers.TryGetValue (aId, out characteristicIdentifiers);
			if (alreadyFavorite)
				characteristicIdentifiers.Add (cId);
			else
				accessoryToCharacteristicIdentifiers [aId] = new List<NSUuid> { cId };

			Save ();
		}

		// Provides an array of favorite `HMCharacteristic`s within a given accessory.
		public HMCharacteristic[] FavoriteCharacteristicsForAccessory (HMAccessory accessory)
		{
			var result = accessory.Services
				.SelectMany (s => s.Characteristics)
				.Where (c => c.IsFavorite ())
				.ToArray ();

			Array.Sort (result, CharacteristicOrderedBefore);
			return result;
		}

		// Unfavorites a characteristic.
		public void UnfavoriteCharacteristic (HMCharacteristic characteristic)
		{
			var service = characteristic.Service;
			if (service == null)
				return;

			var accessory = service.Accessory;
			if (accessory == null)
				return;

			List<NSUuid> characteristicIdentifiers;
			var accessoryIdentifier = accessory.UniqueIdentifier;
			if (!accessoryToCharacteristicIdentifiers.TryGetValue (accessoryIdentifier, out characteristicIdentifiers))
				return;

			var indexOfCharacteristic = characteristicIdentifiers.IndexOf (characteristic.UniqueIdentifier);
			if (indexOfCharacteristic < 0)
				return;

			// Remove the characteristic from the mapped collection.
			characteristicIdentifiers.RemoveAt (indexOfCharacteristic);

			// If that was the last characteristic for that accessory, remove the accessory from the internal array.
			if (characteristicIdentifiers.Count == 0)
				accessoryToCharacteristicIdentifiers.Remove (accessoryIdentifier);

			Save ();
		}

		#region Helper Methods

		// First, cleans out the internal identifier structures, then saves
		// the `accessoryToCharacteristicIdentifiers` map and `accessoryIdentifiers` array into `NSUserDefaults`.
		// This method should be called whenever a change is made to the internal structures.
		void Save ()
		{
			RemoveUnusedIdentifiers ();

			var userDefaults = NSUserDefaults.StandardUserDefaults;

			NSDictionary nativeMap = Convert (accessoryToCharacteristicIdentifiers);
			var mapData = NSKeyedArchiver.ArchivedDataWithRootObject (nativeMap);

			userDefaults [accessoryToCharacteristicIdentifierMappingKey] = mapData;
		}

		// Filters out any accessories or characteristic which are not longer valid in HomeKit.
		void RemoveUnusedIdentifiers ()
		{
			IEnumerable<NSUuid> accessories = Homes.SelectMany (h => h.Accessories).Select (a => a.UniqueIdentifier);
			var validAccessories = new HashSet<NSUuid> (accessories);

			var toRemove = accessoryToCharacteristicIdentifiers.Keys.Where (k => !validAccessories.Contains (k));
			foreach (var k in toRemove)
				accessoryToCharacteristicIdentifiers.Remove (k);
		}

		static NSDictionary Convert (Dictionary<NSUuid, List<NSUuid>> dictionary)
		{
			var result = new NSMutableDictionary ();
			foreach (var kvp in dictionary) {
				NSUuid[] array = kvp.Value.ToArray ();
				var nativeArray = NSArray.FromNSObjects (array);
				result.Add (kvp.Key, nativeArray);
			}

			return result;
		}

		#endregion

		static int CharacteristicOrderedBefore (HMCharacteristic characteristic1, HMCharacteristic characteristic2)
		{
			var type1 = characteristic1.LocalizedCharacteristicType ();
			var type2 = characteristic2.LocalizedCharacteristicType ();

			return type1.CompareTo (type2);
		}
	}
}