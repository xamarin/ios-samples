using System;
using System.Collections.Generic;

using HomeKit;

namespace HomeKitCatalog
{
	public static class ArraySotringExtensions
	{
		public static void SortByLocalizedName<T> (this T[] array, Func<T, string> nameGetter)
		{
			Array.Sort(array, (x, y) => {
				var xName = nameGetter(x);
				var yName = nameGetter (y);
				return xName.CompareTo(yName);
			});
		}


		public static void SortByLocalizedName<T> (this List<T> list, Func<T, string> nameGetter)
		{
			list.Sort ((x, y) => {
				var xName = nameGetter(x);
				var yName = nameGetter (y);
				return xName.CompareTo(yName);
			});
		}

		public static void SortByTypeAndLocalizedName (this List<HMActionSet> list)
		{
			list.Sort (ActionSetComparision);
		}

		static int ActionSetComparision (HMActionSet x, HMActionSet y)
		{
			bool xBuiltIn = x.IsBuiltIn ();
			bool yBuiltIn = y.IsBuiltIn ();

			// If comparing a built-in and a user-defined, the built-in is ranked first.
			if (xBuiltIn != yBuiltIn)
				return xBuiltIn.CompareTo (yBuiltIn);

			// If comparing two built-ins, we follow a standard ranking
			if (xBuiltIn && yBuiltIn)
				return x.CompareWitBuiltIn (y);

			// If comparing two user-defines, sort by localized name.
			return x.Name.CompareTo (y.Name);
		}
	}
}