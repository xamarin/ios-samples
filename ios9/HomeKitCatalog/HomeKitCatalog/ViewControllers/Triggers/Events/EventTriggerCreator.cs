using System;
using System.Collections.Generic;

using HomeKit;
using Foundation;

namespace HomeKitCatalog
{
	// A base class for event trigger creators.
	// These classes manage the state for characteristic trigger conditions.
	public class EventTriggerCreator : TriggerCreator, ICharacteristicCellDelegate
	{
		/// A mapping of `HMCharacteristic`s to their values.
		readonly Dictionary<HMCharacteristic, NSObject> conditionValueMap = new Dictionary<HMCharacteristic, NSObject> ();

		protected HMEventTrigger EventTrigger {
			get {
				return Trigger as HMEventTrigger;
			}
		}

		// An array of top-level `NSPredicate` objects.
		// Currently, HMCatalog only supports top-level `NSPredicate`s
		// which have type `AndPredicateType`.
		NSPredicate[] OriginalConditions {
			get {
				var eventTrigger = EventTrigger;
				if (eventTrigger != null) {
					var compoundPredicate = eventTrigger.Predicate as NSCompoundPredicate;
					if (compoundPredicate != null) {
						var subpredicates = compoundPredicate.Subpredicates;
						if (subpredicates != null)
							return subpredicates;
					}
				}

				return new NSPredicate[0];
			}
		}

		// An array of new conditions which will be written when the trigger is saved.
		List<NSPredicate> conditions;

		public List<NSPredicate> Conditions {
			get {
				if (conditions == null)
					conditions = new List<NSPredicate> (OriginalConditions);

				return conditions;
			}
		}

		public EventTriggerCreator (HMTrigger trigger, HMHome home)
			: base (trigger, home)
		{
		}

		// Adds a predicate to the pending conditions.
		public void AddCondition (NSPredicate predicate)
		{
			Conditions.Add (predicate);
		}

		// Removes a predicate from the pending conditions.
		public void RemoveCondition (NSPredicate predicate)
		{
			var index = Conditions.IndexOf (predicate);
			if (index >= 0)
				Conditions.RemoveAt (index);
		}

		// returns:  The new `NSCompoundPredicate`, generated from the pending conditions.
		protected NSPredicate NewPredicate ()
		{
			return NSCompoundPredicate.CreateAndPredicate (Conditions.ToArray ());
		}

		// Handles the value update and stores the value in the condition map.
		public virtual void CharacteristicCellDidUpdateValueForCharacteristic (CharacteristicCell cell, NSObject value, HMCharacteristic characteristic, bool immediate)
		{
			conditionValueMap [characteristic] = value;
		}

		// Tries to use the value from the condition-value map, but falls back
		// to reading the characteristic's value from HomeKit.
		public virtual void CharacteristicCellReadInitialValueForCharacteristic (CharacteristicCell cell, HMCharacteristic characteristic, Action<NSObject, NSError> completion)
		{
			if (TryReadValue (characteristic, completion))
				return;

			characteristic.ReadValue (error => {
				// The user may have updated the cell value while the
				// read was happening. We check the map one more time.
				if (!TryReadValue (characteristic, completion))
					completion (characteristic.Value, error);
			});
		}

		bool TryReadValue (HMCharacteristic  characteristic, Action<NSObject, NSError> completion)
		{
			NSObject value;
			if (conditionValueMap.TryGetValue (characteristic, out value)) {
				completion (value, null);
				return true;
			}

			return false;
		}

		#region Helper Methods

		// Updates the predicates and saves the new, generated predicate to the event trigger.
		protected void SavePredicate ()
		{
			UpdatePredicates ();
			SaveTriggerGroup.Enter ();

			var eventTrigger = EventTrigger;
			if (eventTrigger != null) {
				eventTrigger.UpdatePredicate (NewPredicate (), error => {
					if (error != null)
						Errors.Add (error);
					SaveTriggerGroup.Leave ();
				});
			}
		}

		// Generates predicates from the characteristic-value map and adds them to the pending conditions.
		public void UpdatePredicates ()
		{
			foreach (var kvp  in conditionValueMap) {
				HMCharacteristic characteristic = kvp.Key;
				NSObject value = kvp.Value;
				NSPredicate predicate = HMEventTrigger.CreatePredicateForEvaluatingTrigger (characteristic, NSPredicateOperatorType.EqualTo, value);
				AddCondition (predicate);
			}

			conditionValueMap.Clear ();
		}

		#endregion
	}
}