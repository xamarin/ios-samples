using System;
using System.Collections.Generic;
using System.Linq;

using CoreFoundation;
using Foundation;
using HomeKit;

namespace HomeKitCatalog
{
	// A base class for all trigger creators.
	// These classes manage the temporary state of the trigger and unify some of the saving processes.
	public class TriggerCreator
	{
		protected HMHome Home { get; set; }

		protected HMTrigger Trigger { get; set; }

		protected string Name { get; private set; }

		readonly DispatchGroup saveTriggerGroup = DispatchGroup.Create ();
		protected DispatchGroup SaveTriggerGroup {
			get {
				return saveTriggerGroup;
			}
		}

		readonly List<NSError> errors = new List<NSError> ();
		protected List<NSError> Errors {
			get {
				return errors;
			}
		} 

		// Initializes a trigger creator from an existing trigger (if it exists), and the current home.
		public TriggerCreator (HMTrigger trigger, HMHome home)
		{
			Home = home;
			Trigger = trigger;
		}

		// Completes one of two actions based on the current status of the `trigger` object:
		// 1. Updates the existing trigger.
		// 2. Creates a new trigger.
		public void SaveTriggerWithName (string name, IEnumerable<HMActionSet> actionSets, Action<HMTrigger, IEnumerable<NSError>> completion)
		{
			Name = name;
			if (Trigger != null) {
				// Let the subclass update the trigger.
				UpdateTrigger ();
				UpdateNameIfNecessary ();
				ConfigureWithActionSets (actionSets);
			} else {
				Trigger = NewTrigger ();
				SaveTriggerGroup.Enter ();
				Home.AddTrigger (Trigger, error => {
					if (error != null)
						errors.Add (error);
					else
						ConfigureWithActionSets (actionSets);
					SaveTriggerGroup.Leave ();
				});
			}

			// Call the completion block with our event trigger and any accumulated errors from the saving process.
			SaveTriggerGroup.Notify (DispatchQueue.MainQueue, () => {
				CleanUp ();
				completion (Trigger, errors);
			});
		}

		// Updates the trigger's internals.
		// Action sets and the trigger name need not be configured.
		protected virtual void UpdateTrigger ()
		{
		}

		// Creates a new trigger to be added to the home.
		// Action sets and the trigger name need not be configured.
		protected virtual HMTrigger NewTrigger ()
		{
			return null;
		}

		// Cleans up an internal structures after the trigger has been saved.
		protected virtual void CleanUp ()
		{
		}

		#region Helper Methods

		// Syncs the trigger's action sets with the specified array of action sets.
		void ConfigureWithActionSets (IEnumerable<HMActionSet> actionSets)
		{
			var trigger = Trigger;
			if (trigger == null)
				return;

			// Save a standard completion handler to use when we either add or remove an action set.
			Action<NSError> defaultCompletion = error => {
				// Leave the dispatch group, to notify that we've finished this task.
				if (error != null)
					errors.Add (error);
				SaveTriggerGroup.Leave ();
			};

			// First pass, remove the action sets that have been deselected.
			foreach (var actionSet in trigger.ActionSets) {
				if (actionSets.Contains (actionSet))
					continue;
				SaveTriggerGroup.Enter ();
				trigger.RemoveActionSet (actionSet, defaultCompletion);
			}

			// Second pass, add the new action sets that were just selected.
			foreach (var actionSet in actionSets) {
				if (trigger.ActionSets.Contains (actionSet))
					continue;
				SaveTriggerGroup.Enter ();
				trigger.AddActionSet (actionSet, defaultCompletion);
			}
		}

		// Updates the trigger's name from the stored name, entering and leaving the dispatch group if necessary.
		void UpdateNameIfNecessary ()
		{
			var trigger = Trigger;
			if (trigger == null && trigger.Name == Name)
				return;

			SaveTriggerGroup.Enter ();

			trigger.UpdateName (Name, error => {
				if (error != null)
					errors.Add (error);
				SaveTriggerGroup.Leave ();
			});
		}

		#endregion
	}
}