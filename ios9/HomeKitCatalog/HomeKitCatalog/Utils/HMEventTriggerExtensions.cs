using System;
using System.Linq;

using HomeKit;

namespace HomeKitCatalog
{
	public static class HMEventTriggerExtensions
	{
		// returns:  `true` if the trigger contains a location event, `false` otherwise.
		public static bool IsLocationEvent (this HMEventTrigger trigger)
		{
			return trigger.Events.Any (e => e is HMLocationEvent);
		}

		public static HMLocationEvent LocationEvent (this HMEventTrigger self)
		{
			foreach (var e in self.Events) {
				var locationEvent = e as HMLocationEvent;
				if (locationEvent != null)
					return locationEvent;
			}

			return null;
		}

		public static HMCharacteristicEvent[] CharacteristicEvents(this HMEventTrigger self)
		{
			return self.Events.OfType<HMCharacteristicEvent> ().ToArray ();
		}
	}
}