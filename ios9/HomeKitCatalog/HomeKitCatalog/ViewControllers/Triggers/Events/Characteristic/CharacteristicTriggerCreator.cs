using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using HomeKit;

namespace HomeKitCatalog
{
	// Represents modes for a `CharacteristicTriggerCreator`.
	public enum CharacteristicTriggerCreatorMode
	{
		Event,
		Condition
	}

	// An `EventTriggerCreator` subclass which allows for the creation of characteristic triggers.
	public class CharacteristicTriggerCreator : EventTriggerCreator
	{
		// This object will be a characteristic cell delegate and will therefore
		// be receiving updates when UI elements change value. However, this object
		// can construct both characteristic events and characteristic triggers.
		// Setting the `mode` determines how this trigger creator will handle
		// cell delegate callbacks.
		public CharacteristicTriggerCreatorMode Mode { get; set; }

		readonly Dictionary<HMCharacteristic, INSCopying> targetValueMap = new Dictionary<HMCharacteristic, INSCopying> ();
			
		// `HMCharacteristicEvent`s that should be removed if `SaveTriggerWithName()` is called.
		readonly List<HMCharacteristicEvent> removalCharacteristicEvents = new List<HMCharacteristicEvent> ();

		public CharacteristicTriggerCreator (HMTrigger trigger, HMHome home)
			: base (trigger, home)
		{
			Mode = CharacteristicTriggerCreatorMode.Event;
		}

		protected override void UpdateTrigger ()
		{
			var eventTrigger = EventTrigger;
			if (eventTrigger == null)
				return;

			MatchEventsFromTriggerIfNecessary ();
			RemovePendingEventsFromTrigger ();

			foreach (var kvp in targetValueMap) {
				var characteristic = kvp.Key;
				var triggerValue = kvp.Value;

				var newEvent = new HMCharacteristicEvent (characteristic, triggerValue);

				SaveTriggerGroup.Enter ();
				eventTrigger.AddEvent (newEvent, error => {
					if (error != null)
						Errors.Add (error);
					SaveTriggerGroup.Leave ();
				});
			}
			SavePredicate ();
		}

		// returns:  A new `HMEventTrigger` with the pending characteristic events and constructed predicate.
		protected override HMTrigger NewTrigger ()
		{
			return new HMEventTrigger (Name, PendingCharacteristicEvents(), NewPredicate ());
		}

		// Remove all objects from the map so they don't show up in the `events` computed array.
		protected override void CleanUp ()
		{
			targetValueMap.Clear ();
		}

		// Removes an event from the map table if it's a new event and queues it for removal if it already existed in the event trigger.
		public void RemoveEvent (HMCharacteristicEvent e)
		{
			// Remove the characteristic from the target value map.
			targetValueMap.Remove (e.Characteristic);

			// If the given event is in the event array, queue it for removal.
			var eventTrigger = EventTrigger;
			if (eventTrigger != null && eventTrigger.CharacteristicEvents ().Contains (e))
				removalCharacteristicEvents.Add (e);
		}

		#region Helper Methods

		HMCharacteristicEvent[] PendingCharacteristicEvents ()
		{
			return targetValueMap.Select (kvp => new HMCharacteristicEvent (kvp.Key, kvp.Value)).ToArray ();
		}

		// Loops through the characteristic events in the trigger.
		// If any characteristics in our map table are also in the event,
		// replace the value with the one we have stored and remove that entry from our map table.
		void MatchEventsFromTriggerIfNecessary ()
		{
			var eventTrigger = EventTrigger;
			if (eventTrigger == null)
				return;

			// Find events who's characteristic is in our map table.
			foreach (var e in eventTrigger.CharacteristicEvents()) {
				INSCopying triggerValue;
				if (targetValueMap.TryGetValue (e.Characteristic, out triggerValue)) {
					SaveTriggerGroup.Enter ();
					e.UpdateTriggerValue (triggerValue, error => {
						if (error != null)
							Errors.Add (error);
						SaveTriggerGroup.Leave ();
					});
				}
			}
		}

		// Removes all `HMCharacteristicEvent`s from the `removalCharacteristicEvents`
		// array and stores any errors that accumulate.
		void RemovePendingEventsFromTrigger ()
		{
			var eventTrigger = EventTrigger;
			if (eventTrigger == null)
				return;

			foreach (var e in removalCharacteristicEvents) {
				SaveTriggerGroup.Enter ();
				eventTrigger.RemoveEvent (e, error => {
					if (error != null)
						Errors.Add (error);
					SaveTriggerGroup.Leave ();
				});
			}

			removalCharacteristicEvents.Clear ();
		}

		// All `HMCharacteristic`s in the `targetValueMap`.
		HMCharacteristic[] AllCharacteristics ()
		{
			var characteristics = new HashSet<HMCharacteristic> (targetValueMap.Keys);
			return characteristics.ToArray ();
		}

		// Saves a characteristic and value into the pending map of characteristic events.
		void UpdateEventValue (INSCopying value, HMCharacteristic characteristic)
		{
			for (int index = 0; index < removalCharacteristicEvents.Count; index++) {
				var e = removalCharacteristicEvents [index];

				// We have this event pending for deletion, but we are going to want to update it.
				// remove it from the removal array.
				if (e.Characteristic == characteristic) {
					removalCharacteristicEvents.RemoveAt (index);
					break;
				}
			}
			targetValueMap [characteristic] = value;
		}

		// The current, sorted collection of `HMCharacteristicEvent`s accumulated by
		// filtering out the events pending removal from the original trigger events and
		// then adding new pending events.
		public HMCharacteristicEvent[] Events ()
		{
			var eventTrigger = EventTrigger;
			HMCharacteristicEvent[] characteristicEvents = eventTrigger != null ? eventTrigger.CharacteristicEvents () : new HMCharacteristicEvent[0];

			var originalEvents = characteristicEvents.Where (c => !removalCharacteristicEvents.Contains (c));
			var allEvents = originalEvents.Union (PendingCharacteristicEvents ()).ToArray ();

			Array.Sort (allEvents, (event1, event2) => {
				var type1 = event1.Characteristic.LocalizedCharacteristicType ();
				var type2 = event2.Characteristic.LocalizedCharacteristicType ();
				return type1.CompareTo (type2);
			});
			return allEvents;
		}

		#endregion

		#region CharacteristicCellDelegate Methods

		// If the mode is event, update the event value. Otherwise, default to super implementation
		public override void CharacteristicCellDidUpdateValueForCharacteristic (CharacteristicCell cell, NSObject value, HMCharacteristic characteristic, bool immediate)
		{
			switch (Mode) {
			case CharacteristicTriggerCreatorMode.Event:
				UpdateEventValue ((INSCopying)value, characteristic);
				break;

			default:
				base.CharacteristicCellDidUpdateValueForCharacteristic (cell, value, characteristic, immediate);
				break;
			}
		}

		public override void CharacteristicCellReadInitialValueForCharacteristic (CharacteristicCell cell, HMCharacteristic characteristic, Action<NSObject, NSError> completion)
		{
			// This is a condition, fall back to the `EventTriggerCreator` read.
			if (Mode == CharacteristicTriggerCreatorMode.Condition) {
				base.CharacteristicCellReadInitialValueForCharacteristic (cell, characteristic, completion);
				return;
			}

			INSCopying value;
			if (targetValueMap.TryGetValue (characteristic, out value)) {
				completion ((NSObject)value, null);
				return;
			}

			// The user may have updated the cell value while the read was happening. We check the map one more time.
			characteristic.ReadValue (error => {
				INSCopying v;
				if (targetValueMap.TryGetValue (characteristic, out v))
					completion ((NSObject)v, null);
				else
					completion (characteristic.Value, error);
			});
		}

		#endregion
	}
}