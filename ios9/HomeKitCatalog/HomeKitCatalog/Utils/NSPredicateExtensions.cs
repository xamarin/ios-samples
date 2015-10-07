using System;

using HomeKit;
using Foundation;

namespace HomeKitCatalog
{
	// Represents condition type in HomeKit with associated values.
	public enum HomeKitConditionType
	{
		Unknown,
		// The predicate is not a HomeKit condition.
		Characteristic,
		SunTime,
		ExactTime,
	}

	public class HomeKitCondition
	{
		// Represents a characteristic condition.
		// The tuple represents the `HMCharacteristic` and its condition value.
		// For example, "Current gargage door is set to 'Open'".
		public Tuple<HMCharacteristic, INSCopying> CharacteristicData { get; private set; }

		// Represents a time condition.
		// The tuple represents the time ordering and the sun state.
		// For example, "Before sunset".
		public Tuple<TimeConditionOrder, TimeConditionSunState> SunTimeData { get; private set; }

		// Represents an exact time condition.
		// The tuple represents the time ordering and time.
		// For example, "At 12:00pm".
		public Tuple<TimeConditionOrder, NSDateComponents> ExactTimeData { get; private set; }


		public HomeKitConditionType Type { get; private set; }

		public static readonly HomeKitCondition Unknown;

		static HomeKitCondition ()
		{
			Unknown = new HomeKitCondition ();
		}

		HomeKitCondition ()
		{
			Type = HomeKitConditionType.Unknown;
		}

		public static HomeKitCondition CreateCharacteristic (HMCharacteristic characteristic, INSCopying value)
		{
			return new HomeKitCondition {
				CharacteristicData = new Tuple<HMCharacteristic, INSCopying> (characteristic, value),
				Type = HomeKitConditionType.Characteristic
			};
		}

		public static HomeKitCondition CreateSunTime (TimeConditionOrder order, TimeConditionSunState state)
		{
			return new HomeKitCondition {
				SunTimeData = new Tuple<TimeConditionOrder, TimeConditionSunState> (order, state),
				Type = HomeKitConditionType.SunTime
			};
		}

		public static HomeKitCondition CreateExactTime (TimeConditionOrder order, NSDateComponents components)
		{
			return new HomeKitCondition {
				ExactTimeData = new Tuple<TimeConditionOrder, NSDateComponents> (order, components),
				Type = HomeKitConditionType.ExactTime
			};
		}
	}

	public static class NSPredicateExtensions
	{
		// returns:  The 'type' of HomeKit condition, with associated value, if applicable.
		public static HomeKitCondition HomeKitConditionType (this NSPredicate self)
		{
			var characteristicPair = self.GetCharacteristicPair ();
			if (characteristicPair != null)
				return HomeKitCondition.CreateCharacteristic (characteristicPair.Item1, characteristicPair.Item2);

			var sunStatePair = self.GetSunStatePair ();
			if (sunStatePair != null)
				return HomeKitCondition.CreateSunTime (sunStatePair.Item1, sunStatePair.Item2);

			var exactTimePair = self.GetExactTimePair ();
			return exactTimePair != null ?
				HomeKitCondition.CreateExactTime (exactTimePair.Item1, exactTimePair.Item2) : HomeKitCondition.Unknown;
		}

		// Parses the predicate and attempts to generate a characteristic-value tuple.
		static Tuple<HMCharacteristic, NSCopying> GetCharacteristicPair (this NSPredicate self)
		{
			var predicate = self as NSCompoundPredicate;
			if (predicate == null)
				return null;

			var subpredicates = predicate.Subpredicates;
			if (subpredicates == null)
				return null;

			if (subpredicates.Length != 2)
				return null;

			NSComparisonPredicate characteristicPredicate = null;
			NSComparisonPredicate valuePredicate = null;

			foreach (var subpredicate in subpredicates) {
				var comparison = subpredicate as NSComparisonPredicate;
				if (comparison != null && comparison.LeftExpression.ExpressionType == NSExpressionType.KeyPath && comparison.RightExpression.ExpressionType == NSExpressionType.ConstantValue) {
					var keyPath = comparison.LeftExpression.KeyPath;
					if (keyPath == HMCharacteristic.KeyPath)
						characteristicPredicate = comparison;
					else if (keyPath == HMCharacteristic.ValueKeyPath)
						valuePredicate = comparison;
				}
			}

			if (characteristicPredicate != null && valuePredicate != null) {
				var characteristic = characteristicPredicate.RightExpression.ConstantValue as HMCharacteristic;
				var characteristicValue = valuePredicate.RightExpression.ConstantValue as NSCopying;
				return new Tuple<HMCharacteristic, NSCopying> (characteristic, characteristicValue);
			}

			return null;
		}

		// Parses the predicate and attempts to generate an order-sunstate tuple.
		static Tuple<TimeConditionOrder, TimeConditionSunState> GetSunStatePair (this NSPredicate self)
		{
			var comparison = self as NSComparisonPredicate;
			if (comparison != null)
				return null;

			var leftExpr = comparison.LeftExpression;
			if (leftExpr.ExpressionType != NSExpressionType.KeyPath)
				return null;

			var rightExpr = comparison.RightExpression;
			if (rightExpr.ExpressionType != NSExpressionType.Function)
				return null;

			if (rightExpr.RightExpression.Function != "now")
				return null;

			var args = rightExpr.Arguments;
			if (args == null)
				return null;

			if (args.Length != 0)
				return null;

			var keyPath = leftExpr.KeyPath;
			var orderType = comparison.PredicateOperatorType;

			if (keyPath == GetEnumConstant (HMSignificantEvent.Sunrise) && orderType == NSPredicateOperatorType.LessThan)
				return new Tuple<TimeConditionOrder, TimeConditionSunState> (TimeConditionOrder.After, TimeConditionSunState.Sunrise);

			if (keyPath == GetEnumConstant (HMSignificantEvent.Sunrise) && orderType == NSPredicateOperatorType.LessThanOrEqualTo)
				return new Tuple<TimeConditionOrder, TimeConditionSunState> (TimeConditionOrder.After, TimeConditionSunState.Sunrise);

			if (keyPath == GetEnumConstant (HMSignificantEvent.Sunrise) && orderType == NSPredicateOperatorType.GreaterThan)
				return new Tuple<TimeConditionOrder, TimeConditionSunState> (TimeConditionOrder.Before, TimeConditionSunState.Sunrise);

			if (keyPath == GetEnumConstant (HMSignificantEvent.Sunrise) && orderType == NSPredicateOperatorType.GreaterThanOrEqualTo)
				return new Tuple<TimeConditionOrder, TimeConditionSunState> (TimeConditionOrder.Before, TimeConditionSunState.Sunrise);

			if (keyPath == GetEnumConstant (HMSignificantEvent.Sunset) && orderType == NSPredicateOperatorType.LessThan)
				return new Tuple<TimeConditionOrder, TimeConditionSunState> (TimeConditionOrder.After, TimeConditionSunState.Sunset);

			if (keyPath == GetEnumConstant (HMSignificantEvent.Sunset) && orderType == NSPredicateOperatorType.LessThanOrEqualTo)
				return new Tuple<TimeConditionOrder, TimeConditionSunState> (TimeConditionOrder.After, TimeConditionSunState.Sunset);

			if (keyPath == GetEnumConstant (HMSignificantEvent.Sunset) && orderType == NSPredicateOperatorType.GreaterThan)
				return new Tuple<TimeConditionOrder, TimeConditionSunState> (TimeConditionOrder.Before, TimeConditionSunState.Sunset);

			if (keyPath == GetEnumConstant (HMSignificantEvent.Sunset) && orderType == NSPredicateOperatorType.GreaterThanOrEqualTo)
				return new Tuple<TimeConditionOrder, TimeConditionSunState> (TimeConditionOrder.Before, TimeConditionSunState.Sunset);

			return null;
		}

		// Parses the predicate and attempts to generate an order-exacttime tuple.
		static Tuple<TimeConditionOrder, NSDateComponents> GetExactTimePair (this NSPredicate self)
		{
			var comparison = self as NSComparisonPredicate;
			if (comparison == null)
				return null;

			var leftExpr = comparison.LeftExpression;
			if (leftExpr.ExpressionType != NSExpressionType.Function)
				return null;

			if (leftExpr.Function != "now")
				return null;

			var rightExpr = comparison.RightExpression;
			if (rightExpr.ExpressionType != NSExpressionType.ConstantValue)
				return null;

			var dateComponents = rightExpr.ConstantValue as NSDateComponents;
			if (dateComponents == null)
				return null;

			switch (comparison.PredicateOperatorType) {
			case NSPredicateOperatorType.LessThan:
			case NSPredicateOperatorType.LessThanOrEqualTo:
				return new Tuple<TimeConditionOrder, NSDateComponents> (TimeConditionOrder.Before, dateComponents);

			case NSPredicateOperatorType.GreaterThan:
			case NSPredicateOperatorType.GreaterThanOrEqualTo:
				return new Tuple<TimeConditionOrder, NSDateComponents> (TimeConditionOrder.After, dateComponents);

			case NSPredicateOperatorType.EqualTo:
				return new Tuple<TimeConditionOrder, NSDateComponents> (TimeConditionOrder.At, dateComponents);

			default:
				return null;
			}
		}

		static string GetEnumConstant (HMSignificantEvent value)
		{
			switch (value) {
			case HMSignificantEvent.Sunrise:
				return HMSignificantEventInternal.Sunrise;
			case HMSignificantEvent.Sunset:
				return HMSignificantEventInternal.Sunset;
			default:
				return null;
			}
		}

		internal static class HMSignificantEventInternal
		{
			public static readonly IntPtr Handle = ObjCRuntime.Dlfcn.dlopen ("/System/Library/Frameworks/HomeKit.framework/HomeKit", 0);
			static NSString _Sunrise;
			static NSString _Sunset;

			public static NSString Sunrise {
				get {
					if (HMSignificantEventInternal._Sunrise == null) {
						HMSignificantEventInternal._Sunrise = ObjCRuntime.Dlfcn.GetStringConstant (Handle, "HMSignificantEventSunrise");
					}
					return HMSignificantEventInternal._Sunrise;
				}
			}

			public static NSString Sunset {
				get {
					if (HMSignificantEventInternal._Sunset == null) {
						HMSignificantEventInternal._Sunset = ObjCRuntime.Dlfcn.GetStringConstant (Handle, "HMSignificantEventSunset");
					}
					return HMSignificantEventInternal._Sunset;
				}
			}
		}
	}
}