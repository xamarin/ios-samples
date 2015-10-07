using System;
using System.Collections.Generic;
using System.Linq;

using HomeKit;
using Foundation;

namespace HomeKitCatalog
{
	public static class HMHomeExtensions
	{
		// All the services within all the accessories within the home.
		static HashSet<HMService> GetAllServices (this HMHome self)
		{
			return new HashSet<HMService> (self.Accessories.SelectMany (a => a.Services));
		}

		// All the characteristics within all of the services within the home.
		public static HashSet<HMCharacteristic> GetAllCharacteristics (this HMHome self)
		{
			return new HashSet<HMCharacteristic> (self.Accessories.SelectMany (a => a.Services).SelectMany (s => s.Characteristics));
		}

		// returns:  A dictionary mapping localized service types to an array of all services of that type.
		public static Dictionary<string, List<HMService>> GetServiceTable (this HMHome self)
		{
			var serviceDictionary = new Dictionary<string, List<HMService>> ();
			foreach (var service in self.GetAllServices()) {
				if (!service.IsControllType ())
					continue;

				var serviceType = service.LocalizedDescription;
				List<HMService> list;
				if (serviceDictionary.TryGetValue (serviceType, out list))
					list.Add (service);
				else
					serviceDictionary [serviceType] = new List<HMService> { service };
			}

			foreach (var list in serviceDictionary.Values)
				list.Sort ((x, y) => x.Name.CompareTo (y.Name));

			return serviceDictionary;
		}

		// returns:  All rooms in the home, including `roomForEntireHome`.
		public static HMRoom[] GetAllRooms (this HMHome self)
		{
			var rooms = self.Rooms;
			var allRooms = new HMRoom[rooms.Length + 1];
			Array.Copy (rooms, allRooms, rooms.Length);
			allRooms [rooms.Length] = self.GetRoomForEntireHome ();
			return allRooms;
		}

		public static bool IsAdmin (this HMHome home)
		{
			return home.GetHomeAccessControl (home.CurrentUser).Administrator;
		}

		// returns:  All accessories which are 'control accessories'.
		public static IEnumerable<HMAccessory> SortedControlAccessories (this HMHome self)
		{
			Func<HMAccessory, bool> predicate = accessory => accessory.Services.Any (s => s.IsControllType ());
			var filteredAccessories = self.Accessories.Where (predicate).ToArray ();
			filteredAccessories.SortByLocalizedName (a => a.Name);
			return filteredAccessories;
		}

		public static IEnumerable<HMAccessory> AccessoriesWithIdentifiers (this HMHome self, HashSet<NSUuid> identifiers)
		{
			return self.Accessories.Where (a => identifiers.Contains (a.UniqueIdentifier));
		}

		// Searches through the home's accessories to find the accessory that is bridging the provided accessory.
		public static HMAccessory BridgeForAccessory (this HMHome self, HMAccessory  accessory)
		{
			if (!accessory.Bridged)
				return null;

			foreach (var bridge in self.Accessories) {
				var bridgedIds = bridge.UniqueIdentifiersForBridgedAccessories;
				foreach (var id in bridgedIds) {
					if (accessory.UniqueIdentifier == id)
						return bridge;
				}
			}

			return null;
		}

		public static string GetNameForRoom (this HMHome home, HMRoom room)
		{
			var entireHome = room == home.GetRoomForEntireHome ();
			return entireHome ? string.Format ("{0} Default Room", room.Name) : room.Name;
		}

		// returns:  A list of rooms that exist in the home and have not yet been added to this zone.
		public static IEnumerable<HMRoom> RoomsNotAlreadyInZone (this HMHome home, HMZone zone, IEnumerable<HMRoom> includingRooms = null)
		{
			var rooms = home.Rooms.Where (room => !zone.Rooms.Contains (room));
			if (includingRooms != null)
				rooms = rooms.Concat (includingRooms);
			return rooms;
		}

		// returns:  A list of services that exist in the home and have not yet been added to this service group.
		public static IEnumerable<HMService> ServicesNotAlreadyInServiceGroup (this HMHome self, HMServiceGroup serviceGroup)
		{
			var servicesInGroup = new HashSet<HMService> (serviceGroup.Services);
			Func<HMService, bool> filter = s => !servicesInGroup.Contains (s) && s.ServiceType != HMServiceType.AccessoryInformation;
			return self.GetAllServices ().Where (filter);
		}
	}
}