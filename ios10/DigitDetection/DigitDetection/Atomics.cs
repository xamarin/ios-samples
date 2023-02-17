using System.Threading;

namespace DigitDetection {
	public static class Atomics {
		static int cnt;

		public static void Increment ()
		{
			Interlocked.Increment (ref cnt);
		}

		public static void Reset ()
		{
			Interlocked.Exchange (ref cnt, 0);
		}

		public static int GetCount ()
		{
			return cnt;
		}
	}
}
