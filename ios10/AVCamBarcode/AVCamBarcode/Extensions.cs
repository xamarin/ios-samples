using System.Linq;
using System.Collections.Generic;

using AVFoundation;
using UIKit;
using CoreText;

namespace AVCamBarcode
{
	public static class AVMetadataObjectTypeExtensions
	{
		public static IEnumerable<AVMetadataObjectType> GetFlags (this AVMetadataObjectType value)
		{
			var shifts = 0;
			var val = (ulong)value;
			while (val != 0) {
				if ((val & 1) == 1)
					yield return (AVMetadataObjectType)(1 << shifts);
				shifts++;
				val >>= 1;
			}
		}

		public static AVMetadataObjectType Combine (IEnumerable<AVMetadataObjectType> flags)
		{
			return flags.Aggregate ((agr, f) => agr | f);
		}
	}

	public static class IEnumerableExtensions
	{
		public static IEnumerable<T> Concat<T> (this IEnumerable<T> seq, T last)
		{
			foreach (var item in seq)
				yield return item;

			yield return last;
		}
	}

	public static class FontExtensions
	{
		public static CTFont ToCTFont (this UIFont font)
		{
			return new CTFont (font.Name, font.PointSize);
		}
	}
}
