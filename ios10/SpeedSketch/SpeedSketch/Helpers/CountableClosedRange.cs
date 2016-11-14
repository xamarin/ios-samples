using System.Collections;
using System.Collections.Generic;

namespace SpeedSketch
{
	public class CountableClosedRange : IEnumerable<int>
	{
		public int LowerBound { get; }
		public int UpperBound { get; }

		public CountableClosedRange (int lower, int upper)
		{
			LowerBound = lower;
			UpperBound = upper;
		}

		public IEnumerator<int> GetEnumerator ()
		{
			for (int i = LowerBound; i <= UpperBound; i++)
				yield return i;
		}

		IEnumerator IEnumerable.GetEnumerator ()
		{
			return GetEnumerator ();
		}
	}
}
