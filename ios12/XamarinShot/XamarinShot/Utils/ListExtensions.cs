
namespace XamarinShot.Utils {
	using System;
	using System.Collections.Generic;

	public static class ListExtensions {
		public static void ForEach<T> (this IEnumerable<T> collection, Action<T> action)
		{
			foreach (var item in collection) {
				action (item);
			}
		}
	}
}
