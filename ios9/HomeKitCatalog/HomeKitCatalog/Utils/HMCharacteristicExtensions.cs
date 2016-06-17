using System;
using System.Linq;

using Foundation;
using HomeKit;

namespace HomeKitCatalog
{
	public static class HMCharacteristicExtensions
	{
		static readonly NSNumberFormatter formatter = new NSNumberFormatter();

		static readonly HMCharacteristicMetadataFormat[] numericFormats = {
			HMCharacteristicMetadataFormat.Int,
			HMCharacteristicMetadataFormat.Float,
			HMCharacteristicMetadataFormat.UInt8,
			HMCharacteristicMetadataFormat.UInt16,
			HMCharacteristicMetadataFormat.UInt32,
			HMCharacteristicMetadataFormat.UInt64
		};

		// Returns the description for a provided value, taking the characteristic's metadata and possible values into account.
		public static string DescriptionForValue(this HMCharacteristic self, NSObject value)
		{
			if (self.IsWriteOnly ())
				return "Write-Only Characteristic";

			var number = value as NSNumber;
			return number != null ? self.DescriptionForValue (number.Int32Value) : value.ToString ();
		}

		public static string DescriptionForValue (this HMCharacteristic self, int value)
		{
			if (self.IsBoolean ())
				return Convert.ToBoolean(value) ? "On" : "Off";

			var predeterminedValueString = self.PredeterminedValueDescriptionForNumber (value);
			if (predeterminedValueString != null)
				return predeterminedValueString;

			var metadata = self.Metadata;
			if (metadata != null) {
				var stepValue = metadata.StepValue;
				if (stepValue != null) {
					formatter.MaximumFractionDigits = (int)Math.Log10 (1f / stepValue.DoubleValue);
					var str = formatter.StringFromNumber (value);
					if (!string.IsNullOrEmpty (str))
						return str + self.LocalizedUnitDescription ();
				}
			}

			return value.ToString ();
		}

		public static string PredeterminedValueDescriptionForNumber(this HMCharacteristic self, int number)
		{
			switch (self.CharacteristicType) {
			case HMCharacteristicType.PowerState:
			case HMCharacteristicType.InputEvent:
			case HMCharacteristicType.OutputState:
				return Convert.ToBoolean (number) ? "On" : "Off";

			case HMCharacteristicType.OutletInUse:
			case HMCharacteristicType.MotionDetected:
			case HMCharacteristicType.AdminOnlyAccess:
			case HMCharacteristicType.AudioFeedback:
			case HMCharacteristicType.ObstructionDetected:
				return Convert.ToBoolean (number) ? "Yes" : "No";

			case HMCharacteristicType.TargetDoorState:
			case HMCharacteristicType.CurrentDoorState:
				var doorState = (HMCharacteristicValueDoorState)number;
				switch (doorState) {
				case HMCharacteristicValueDoorState.Open:
					return "Open";
				case HMCharacteristicValueDoorState.Opening:
					return "Opening";
				case HMCharacteristicValueDoorState.Closed:
					return "Closed";
				case HMCharacteristicValueDoorState.Closing:
					return "Closing";
				case HMCharacteristicValueDoorState.Stopped:
					return "Stopped";
				}
				break;

			case HMCharacteristicType.TargetHeatingCooling:
				var targetMode = (HMCharacteristicValueHeatingCooling)number;
				switch (targetMode) {
				case HMCharacteristicValueHeatingCooling.Off:
					return "Off";
				case HMCharacteristicValueHeatingCooling.Heat:
					return "Heat";
				case HMCharacteristicValueHeatingCooling.Cool:
					return "Cool";
				case HMCharacteristicValueHeatingCooling.Auto:
					return "Auto";
				}
				break;

			case HMCharacteristicType.CurrentHeatingCooling:
				var currentMode = (HMCharacteristicValueHeatingCooling)number;
				switch (currentMode) {
				case HMCharacteristicValueHeatingCooling.Off:
					return "Off";
				case HMCharacteristicValueHeatingCooling.Heat:
					return "Heating";
				case HMCharacteristicValueHeatingCooling.Cool:
					return "Cooling";
				case HMCharacteristicValueHeatingCooling.Auto:
					return "Auto";
					}
				break;

			case HMCharacteristicType.TargetLockMechanismState:
			case HMCharacteristicType.CurrentLockMechanismState:
				var lockState = (HMCharacteristicValueLockMechanismState)number;
				switch (lockState) {
				case HMCharacteristicValueLockMechanismState.Unsecured:
					return "Unsecured";
				case HMCharacteristicValueLockMechanismState.Secured:
					return "Secured";
				case HMCharacteristicValueLockMechanismState.Unknown:
					return "Unknown";
				case HMCharacteristicValueLockMechanismState.Jammed:
					return "Jammed";
				}
				break;

			case HMCharacteristicType.TemperatureUnits:
				var unit = (HMCharacteristicValueTemperatureUnit)number;
				switch (unit) {
				case HMCharacteristicValueTemperatureUnit.Celsius:
					return "Celsius";
				case HMCharacteristicValueTemperatureUnit.Fahrenheit:
					return "Fahrenheit";
				}
				break;

			case HMCharacteristicType.LockMechanismLastKnownAction:
				var lastKnownAction = (HMCharacteristicValueLockMechanism)number;
				switch (lastKnownAction) {
				case HMCharacteristicValueLockMechanism.LastKnownActionSecuredUsingPhysicalMovementInterior:
					return "Interior Secured";
				case HMCharacteristicValueLockMechanism.LastKnownActionUnsecuredUsingPhysicalMovementInterior:
					return "Exterior Unsecured";
				case HMCharacteristicValueLockMechanism.LastKnownActionSecuredUsingPhysicalMovementExterior:
					return "Exterior Secured";
				case HMCharacteristicValueLockMechanism.LastKnownActionUnsecuredUsingPhysicalMovementExterior:
					return "Exterior Unsecured";
				case HMCharacteristicValueLockMechanism.LastKnownActionSecuredWithKeypad:
					return "Keypad Secured";
				case HMCharacteristicValueLockMechanism.LastKnownActionUnsecuredWithKeypad:
					return "Keypad Unsecured";
				case HMCharacteristicValueLockMechanism.LastKnownActionSecuredRemotely:
					return "Secured Remotely";
				case HMCharacteristicValueLockMechanism.LastKnownActionUnsecuredRemotely:
					return "Unsecured Remotely";
				case HMCharacteristicValueLockMechanism.LastKnownActionSecuredWithAutomaticSecureTimeout:
					return "Secured Automatically";
				case HMCharacteristicValueLockMechanism.LastKnownActionSecuredUsingPhysicalMovement:
					return "Secured Using Physical Movement";
				case HMCharacteristicValueLockMechanism.LastKnownActionUnsecuredUsingPhysicalMovement:
					return "Unsecured Using Physical Movement";
				}
				break;

			case HMCharacteristicType.RotationDirection:
				var rotationDirection = (HMCharacteristicValueRotationDirection)number;
				switch (rotationDirection) {
				case HMCharacteristicValueRotationDirection.Clockwise:
					return "Clockwise";
				case HMCharacteristicValueRotationDirection.CounterClockwise:
					return "Counter Clockwise";
				}
				break;

			case HMCharacteristicType.AirParticulateSize:
				var size = (HMCharacteristicValueAirParticulate)number;
				switch (size) {
				case HMCharacteristicValueAirParticulate.Size10:
					return "Size 10";

				case HMCharacteristicValueAirParticulate.Size2_5:
					return "Size 2.5";
				}
				break;

			case HMCharacteristicType.PositionState:
				var state = (HMCharacteristicValuePositionState)number;
				switch (state) {
				case HMCharacteristicValuePositionState.Opening:
					return "Opening";
				case HMCharacteristicValuePositionState.Closing:
					return "Closing";
				case HMCharacteristicValuePositionState.Stopped:
					return "Stopped";
				}
				break;

			case HMCharacteristicType.CurrentSecuritySystemState:
				var positionState = (HMCharacteristicValueCurrentSecuritySystemState)number;
				switch (positionState) {
				case HMCharacteristicValueCurrentSecuritySystemState.AwayArm:
					return "Away";
				case HMCharacteristicValueCurrentSecuritySystemState.StayArm:
					return "Home";
				case HMCharacteristicValueCurrentSecuritySystemState.NightArm:
					return "Night";
				case HMCharacteristicValueCurrentSecuritySystemState.Disarmed:
					return "Disarm";
				case HMCharacteristicValueCurrentSecuritySystemState.Triggered:
					return "Triggered";
					}
				break;

			case HMCharacteristicType.TargetSecuritySystemState:
				var securityState = (HMCharacteristicValueTargetSecuritySystemState)number;
				switch (securityState) {
				case HMCharacteristicValueTargetSecuritySystemState.AwayArm:
					return "Away";
				case HMCharacteristicValueTargetSecuritySystemState.StayArm:
					return "Home";
				case HMCharacteristicValueTargetSecuritySystemState.NightArm:
					return "Night";
				case HMCharacteristicValueTargetSecuritySystemState.Disarm:
					return "Disarm";
				}
				break;
			}

			return null;
		}

		static string LocalizedUnitDescription (this HMCharacteristic self)
		{
			var metadata = self.Metadata;
			if (metadata != null) {
				HMCharacteristicMetadataUnits units = metadata.Units;
				switch (units) {
				case HMCharacteristicMetadataUnits.Celsius:
					return "℃";
				case HMCharacteristicMetadataUnits.ArcDegree:
					return "º";
				case HMCharacteristicMetadataUnits.Fahrenheit:
					return "℉";
				case HMCharacteristicMetadataUnits.Percentage:
					return "%";
				}
			}
			return string.Empty;
		}

		public static string LocalizedCharacteristicType(this HMCharacteristic self)
		{
			var type = self.LocalizedDescription;

			string description = null;
			if (self.IsReadOnly ())
				description = "Read Only";
			else if (self.IsWriteOnly ())
				description = "Write Only";

			if (description != null)
				type = string.Format ("{0} {1}", type, description);

			return type;
		}

		public static bool IsInteger (this HMCharacteristic self)
		{
			return self.IsNumeric () && !self.IsFloatingPoint ();
		}

		public static bool IsNumeric(this HMCharacteristic self)
		{
			var metadata = self.Metadata;
			return metadata != null && numericFormats.Contains (metadata.Format);
		}

		public static bool IsBoolean(this HMCharacteristic self)
		{
			return CharacteristicFormatEqualsTo (self, HMCharacteristicMetadataFormat.Bool);
		}

		public static bool IsTextWritable(this HMCharacteristic self)
		{
			return CharacteristicFormatEqualsTo (self, HMCharacteristicMetadataFormat.String) && self.Writable;
		}

		public static bool IsFloatingPoint(this HMCharacteristic self)
		{
			return CharacteristicFormatEqualsTo (self, HMCharacteristicMetadataFormat.Float);
		}

		public static bool IsReadOnly(this HMCharacteristic self)
		{
			return self.Readable && !self.Writable;
		}

		public static bool IsWriteOnly(this HMCharacteristic self)
		{
			return !self.Readable && self.Writable;
		}

		public static bool IsIdentify(this HMCharacteristic self)
		{
			return self.CharacteristicType == HMCharacteristicType.Identify;
		}

		public static int[] AllPossibleValues (this HMCharacteristic self)
		{
			if (!self.IsInteger ())
				return null;

			var metadata = self.Metadata;
			if (metadata == null)
				return null;

			var stepValue = metadata.StepValue;
			if (stepValue == null)
				return null;

			var step = stepValue.DoubleValue;
			return Enumerable.Range (0, self.NumberOfChoices ())
				.Select (i => (int)(i * step))
				.ToArray ();
		}

		static int NumberOfChoices(this HMCharacteristic self)
		{
			var metadata = self.Metadata;
			if (metadata == null)
				return 0;

			var minimumValue = metadata.MinimumValue;
			if (minimumValue == null)
				return 0;

			var maximumValue = metadata.MaximumValue;
			if (maximumValue == null)
				return 0;

			int minimum = minimumValue.Int32Value;
			int maximum = maximumValue.Int32Value;
			int range = maximum - minimum;

			var stepValue = metadata.StepValue;
			if (stepValue != null)
				range = (int)(range / stepValue.DoubleValue);

			return range + 1;
		}

		public static bool HasPredeterminedValueDescriptions(this HMCharacteristic self)
		{
			var number = self.Value as NSNumber;
			if (number == null)
				return false;

			var num = number.Int32Value;
			return self.PredeterminedValueDescriptionForNumber (num) != null;
		}

		static bool CharacteristicFormatEqualsTo(HMCharacteristic characteristic, HMCharacteristicMetadataFormat format)
		{
			var metadata = characteristic.Metadata;
			return metadata != null && metadata.Format == format;
		}

		public static bool IsFavorite(this HMCharacteristic self)
		{
			return FavoritesManager.SharedManager.IsFavorite (self);
		}

		public static void IsFavorite(this HMCharacteristic self, bool newValue)
		{
			if (newValue)
				FavoritesManager.SharedManager.FavoriteCharacteristic (self);
			else
				FavoritesManager.SharedManager.UnfavoriteCharacteristic (self);
		}
	}
}