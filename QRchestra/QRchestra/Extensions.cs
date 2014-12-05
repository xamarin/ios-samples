using System;
using Foundation;
using System.Collections.Generic;

namespace QRchestra
{
	public static class Extensions
	{
		public static NSNumber ToNSNumber (this int num)
		{
			return NSNumber.FromInt32 (num);
		}
	}
}

