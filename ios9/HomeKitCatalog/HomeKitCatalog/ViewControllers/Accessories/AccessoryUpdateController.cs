using System;
using System.Collections.Generic;

using CoreFoundation;
using Foundation;
using HomeKit;

namespace HomeKitCatalog
{
	// An object that responds to `CharacteristicCell` updates and notifies HomeKit of changes.
	// TODO: do we need NSObject here?
	public class AccessoryUpdateController : NSObject, ICharacteristicCellDelegate
	{
		readonly DispatchQueue updateQueue = new DispatchQueue ("CharacteristicUpdateQueue");
		readonly Dictionary<HMCharacteristic, NSObject> pendingWrites = new Dictionary<HMCharacteristic, NSObject> ();
		readonly Dictionary<HMCharacteristic, NSObject> sentWrites = new Dictionary<HMCharacteristic, NSObject> ();

		NSTimer updateValueTimer;

		public AccessoryUpdateController ()
		{
			StartListeningForCellUpdates ();
		}

		#region ICharacteristicCellDelegate implementation

		// Responds to a cell change, and if the update was marked immediate, updates the characteristics.
		public void CharacteristicCellDidUpdateValueForCharacteristic (CharacteristicCell cell, NSObject value, HMCharacteristic characteristic, bool immediate)
		{
			pendingWrites [characteristic] = value;
			if (immediate)
				UpdateCharacteristics ();
		}

		// Reads the characteristic's value and calls the completion with the characteristic's value.
		// If there is a pending write request on the same characteristic, the read is ignored to prevent "UI glitching".
		public void CharacteristicCellReadInitialValueForCharacteristic (CharacteristicCell cell, HMCharacteristic characteristic, Action<NSObject, NSError> completion)
		{
			characteristic.ReadValue (error => updateQueue.DispatchSync (() => {
				NSObject sentValue;
				if (sentWrites.TryGetValue (characteristic, out sentValue)) {
					completion (sentValue, null);
					return;
				}
				DispatchQueue.MainQueue.DispatchAsync (() => completion (characteristic.Value, error));
			}));
		}

		#endregion

		// Creates and starts the update value timer.
		void StartListeningForCellUpdates ()
		{
			updateValueTimer = NSTimer.CreateRepeatingScheduledTimer (0.1, UpdateCharacteristics);
		}

		// Invalidates the update timer.
		void StopListeningForCellUpdates ()
		{
			updateValueTimer.Invalidate ();
		}

		void UpdateCharacteristics (NSTimer timer)
		{
			UpdateCharacteristics ();
		}

		// Sends all pending requests in the array.
		void UpdateCharacteristics ()
		{
			updateQueue.DispatchSync (() => {
				foreach (var kvp in pendingWrites) {
					var characteristic = kvp.Key;
					var value = kvp.Value;

					sentWrites [characteristic] = value;

					characteristic.WriteValue (value, error => {
						if (error != null)
							Console.WriteLine ("HomeKit: Could not change value: {0}.", error.LocalizedDescription);
						DidCompleteWrite (characteristic, value);
					});
				}

				pendingWrites.Clear ();
			});
		}

		// Synchronously adds the characteristic-value pair into the `sentWrites` map.
		void DidSendWrite (HMCharacteristic characteristic, NSObject  value)
		{
			updateQueue.DispatchSync (() => {
				sentWrites [characteristic] = value;
			});
		}

		// Synchronously removes the characteristic-value pair from the `sentWrites` map.
		void DidCompleteWrite (HMCharacteristic characteristic, NSObject value)
		{
			updateQueue.DispatchSync (() => sentWrites.Remove (characteristic));
		}

	}
}