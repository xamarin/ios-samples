using System;
using System.Collections.Generic;

namespace Common
{
	public class ComparisionComparer<T> : Comparer<T>
	{
		readonly Comparison<T> comparision;

		public ComparisionComparer (Comparison<T> comparision)
		{
			if (comparision == null)
				throw new ArgumentNullException ();

			this.comparision = comparision;
		}

		public override int Compare (T x, T y)
		{
			return comparision (x, y);
		}
	}
}

