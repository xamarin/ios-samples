using System;
using Foundation;
using HomeKit;

namespace HomeKitCatalog
{
	public interface ICharacteristicCellDelegate
	{
		// Called whenever the control within the cell updates its value.
		//
		// parameter cell:           The cell which has updated its value.
		// parameter newValue:       The new value represented by the cell's control.
		// parameter characteristic: The characteristic the cell represents.
		// parameter immediate:      Whether or not to update external values immediately.
		//
		// For example, Slider cells should not update immediately upon value change,
		// so their values are cached and updates are coalesced. Subclasses can decide
		// whether or not their values are meant to be updated immediately.
		void CharacteristicCellDidUpdateValueForCharacteristic (CharacteristicCell cell, NSObject value, HMCharacteristic characteristic, bool immediate);

		// Called when the characteristic cell needs to reload its value from an external source.
		// Consider using this call to look up values in memory or query them from an accessory.
		//
		// parameter cell:           The cell requesting a value update.
		// parameter characteristic: The characteristic for whose value the cell is asking.
		// parameter completion:     The closure that the cell provides to be called when values have been read successfully.
		void CharacteristicCellReadInitialValueForCharacteristic (CharacteristicCell cell, HMCharacteristic characteristic, Action<NSObject, NSError> completion);
	}
}