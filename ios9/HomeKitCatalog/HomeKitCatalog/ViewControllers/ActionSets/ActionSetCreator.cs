using System;
using System.Collections.Generic;
using System.Linq;

using CoreFoundation;
using Foundation;
using HomeKit;

namespace HomeKitCatalog
{
	// A `CharacteristicCellDelegate` that builds an `HMActionSet` when it receives delegate callbacks.
	public class ActionSetCreator : ICharacteristicCellDelegate
	{
		HMActionSet ActionSet { get; set; }

		HMHome Home { get; set; }

		NSError SaveError { get; set; }

		// The structure we're going to use to hold the target values.
		readonly Dictionary<HMCharacteristic, NSObject> targetValueMap = new Dictionary<HMCharacteristic, NSObject> ();

		// A dispatch group to wait for all of the individual components of the saving process.
		DispatchGroup saveActionSetGroup = DispatchGroup.Create ();

		public ActionSetCreator (HMActionSet actionSet, HMHome home)
		{
			ActionSet = actionSet;
			Home = home;
		}

		// If there is an action set, saves the action set and then updates its name.
		// Otherwise creates a new action set and adds all actions to it.
		public void SaveActionSetWithName (string name, Action<NSError> completionHandler)
		{
			var actionSet = ActionSet;
			if (actionSet != null) {
				SaveActionSet (actionSet);
				UpdateNameIfNecessary (name);
			} else {
				CreateActionSetWithName (name);
			}

			saveActionSetGroup.Notify (DispatchQueue.MainQueue, () => {
				completionHandler (SaveError);
				SaveError = null;
			});
		}

		// Adds all of the actions that have been requested to the Action Set, then runs a completion block.
		void SaveActionSet (HMActionSet actionSet)
		{
			var actions = ActionsFromMapTable ();
			foreach (var action in actions) {
				saveActionSetGroup.Enter ();
				AddAction (action, actionSet, error => {
					if (error != null) {
						Console.WriteLine ("HomeKit: Error adding action: {0}", error.LocalizedDescription);
						SaveError = error;
					}
					saveActionSetGroup.Leave ();
				});
			}
		}

		// Sets the name of an existing action set.
		void UpdateNameIfNecessary (string name)
		{
			if (ActionSet != null && ActionSet.Name == name)
				return;
			saveActionSetGroup.Enter ();
			ActionSet.UpdateName (name, error => {
				if (error != null) {
					Console.WriteLine ("HomeKit: Error updating name: {0}", error.LocalizedDescription);
					SaveError = error;
				}
				saveActionSetGroup.Leave ();
			});
		}

		// Creates and saves an action set with the provided name.
		void CreateActionSetWithName (string name)
		{
			saveActionSetGroup.Enter ();
			Home.AddActionSet (name, (actionSet, error) => {
				if (error != null) {
					Console.WriteLine ("HomeKit: Error creating action set: {0}", error.LocalizedDescription);
					SaveError = error;
				} else {
					// There is no error, so the action set has a value.
					SaveActionSet (actionSet);
				}
				saveActionSetGroup.Leave ();
			});
		}

		// Checks to see if an action already exists to modify the same characteristic
		// as the action passed in. If such an action exists, the method tells the
		// existing action to update its target value. Otherwise, the new action is
		// simply added to the action set.
		void AddAction (HMCharacteristicWriteAction action, HMActionSet actionSet, Action<NSError>completion)
		{
			var existingAction = ExistingActionInActionSetMatchingAction (action);
			if (existingAction != null)
				existingAction.UpdateTargetValue (action.TargetValue, completion);
			else
				actionSet.AddAction (action, completion);
		}

		// Checks to see if there is already an HMCharacteristicWriteAction in
		// the action set that matches the provided action.
		HMCharacteristicWriteAction ExistingActionInActionSetMatchingAction (HMCharacteristicWriteAction action)
		{
			var actionSet = ActionSet;
			if (actionSet != null) {
				foreach (var existingAction in actionSet.Actions.Cast<HMCharacteristicWriteAction>()) {
					if (action.Characteristic == existingAction.Characteristic)
						return existingAction;
				}
			}
			return null;
		}

		// Iterates over a map table of HMCharacteristic -> object and creates
		// an array of HMCharacteristicWriteActions based on those targets.
		HMCharacteristicWriteAction[] ActionsFromMapTable ()
		{
			return targetValueMap.Keys.Select (characteristic => {
				NSObject targetValue = targetValueMap [characteristic];
				return new HMCharacteristicWriteAction (characteristic, targetValue);
			}).ToArray ();
		}

		// returns:  `true` if the characteristic count is greater than zero; `false` otherwise.
		public bool ContainsActions {
			get {
				return AllCharacteristics ().Any ();
			}
		}

		// All existing characteristics within `HMCharacteristiWriteActions` and target values in the target value map.
		public HMCharacteristic[] AllCharacteristics ()
		{
			var characteristics = new HashSet<HMCharacteristic> ();

			var actionSet = ActionSet;
			if (actionSet != null) {
				var actions = actionSet.Actions.OfType<HMCharacteristicWriteAction> ();
				characteristics.UnionWith (actions.Select (a => a.Characteristic));
			}

			characteristics.UnionWith (targetValueMap.Keys);
			return characteristics.ToArray ();
		}

		// Searches through the target value map and existing `HMCharacteristicWriteActions`
		// to find the target value for the characteristic in question.
		public NSObject TargetValueForCharacteristic (HMCharacteristic characteristic)
		{
			NSObject value;
			if (targetValueMap.TryGetValue (characteristic, out value))
				return value;

			var actionSet = ActionSet;
			if (actionSet != null) {
				foreach (var action in actionSet.Actions) {
					var writeAction = action as HMCharacteristicWriteAction;
					if (writeAction != null && writeAction.Characteristic == characteristic)
						return writeAction.TargetValue;
				}
			}

			return null;
		}

		// First removes the characteristic from the `targetValueMap`.
		// Then removes any `HMCharacteristicWriteAction`s from the action set
		// which set the specified characteristic.
		public void RemoveTargetValueForCharacteristic (HMCharacteristic characteristic, Action completion)
		{
			/*
				We need to create a dispatch group here, because in many cases
				there will be one characteristic saved in the Action Set, and one
				in the target value map. We want to run the completion closure only one time,
				to ensure we've removed both.
			*/
			var group = DispatchGroup.Create ();
			if (targetValueMap.ContainsKey (characteristic)) {
				// Remove the characteristic from the target value map.
				group.DispatchAsync (DispatchQueue.MainQueue, () => targetValueMap.Remove (characteristic));
			}

			var actionSet = ActionSet;
			if (actionSet != null) {
				var actions = actionSet.Actions.OfType<HMCharacteristicWriteAction> ();
				foreach (var action in actions) {
					if (action.Characteristic == characteristic) {
						// Also remove the action, and only relinquish the dispatch group once the action set has finished.
						group.Enter ();
						actionSet.RemoveAction (action, error => {
							if (error != null)
								Console.WriteLine (error.LocalizedDescription);
							group.Leave ();
						});
					}

				}
			}
			// Once we're positive both have finished, run the completion closure on the main queue.
			group.Notify (DispatchQueue.MainQueue, completion);
		}

		#region ICharacteristicCellDelegate implementation

		// Receives a callback from a `CharacteristicCell` with a value change. Adds this value change into the targetValueMap, overwriting other value changes.
		public void CharacteristicCellDidUpdateValueForCharacteristic (CharacteristicCell cell, NSObject value, HMCharacteristic characteristic, bool immediate)
		{
			targetValueMap [characteristic] = value;
		}

		// Receives a callback from a `CharacteristicCell`, requesting an initial value for a given characteristic.
		// It checks to see if we have an action in this Action Set that matches the characteristic.
		// If so, calls the completion closure with the target value.
		public void CharacteristicCellReadInitialValueForCharacteristic (CharacteristicCell cell, HMCharacteristic characteristic, Action<NSObject, NSError> completion)
		{
			var value = TargetValueForCharacteristic (characteristic);
			if (value != null) {
				completion (value, null);
				return;
			}

			characteristic.ReadValue (error => {
				// The user may have updated the cell value while the
				// read was happening. We check the map one more time.
				var v = TargetValueForCharacteristic (characteristic);
				if (v != null)
					completion (v, null);
				else
					completion (characteristic.Value, error);
			});
		}

		#endregion
	}
}

