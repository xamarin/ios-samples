using System;

using HomeKit;

namespace HomeKitCatalog
{
	public static class HMActionSetExtensions
	{
		static readonly HMActionSetType[] BuiltInActionSetTypes = { 
			HMActionSetType.WakeUp,
			HMActionSetType.HomeDeparture,
			HMActionSetType.HomeArrival,
			HMActionSetType.Sleep,
		};

		// returns: `true` if the action set is built-in; `false` otherwise.
		public static bool IsBuiltIn (this HMActionSet actionSet)
		{
			return Array.IndexOf (BuiltInActionSetTypes, actionSet.ActionSetType) >= 0;
		}

		public static int CompareWitBuiltIn (this HMActionSet actionSet, HMActionSet anotherBuiltIn)
		{
			var index = Array.IndexOf (BuiltInActionSetTypes, actionSet.ActionSetType);
			var anotherIndex = Array.IndexOf (BuiltInActionSetTypes, anotherBuiltIn.ActionSetType);

			if (index < 0 || anotherIndex < 0)
				throw new InvalidOperationException ();

			return anotherIndex - index;
		}
	}
}