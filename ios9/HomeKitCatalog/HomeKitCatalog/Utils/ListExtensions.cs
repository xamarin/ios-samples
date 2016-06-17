using System.Collections.Generic;

namespace HomeKitCatalog
{
	public static class ListExtensions
	{
		public static T RemoveAtIndex<T> (this List<T> list, int index)
		{
			var item = list [index];
			list.RemoveAt (index);
			return item;
		}
	}
}

