using System;

namespace DigitDetection
{
	public static class Atomics
	{
		public static void Increment ()
		{
			throw new NotImplementedException ();
			//atomic_fetch_add (&cnt, 1);
		}

		public static void Reset ()
		{
			throw new NotImplementedException ();
			//cnt = ATOMIC_VAR_INIT (0);
		}

		public static int GetCount ()
		{
			throw new NotImplementedException ();
			//return atomic_load (&cnt);
		}
	}
}
