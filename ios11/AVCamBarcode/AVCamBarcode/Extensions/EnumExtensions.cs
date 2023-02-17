
namespace AVCamBarcode.Extensions {
	using AVFoundation;
	using System.Collections.Generic;

	public static class EnumExtensions {
		public static IEnumerable<AVMetadataObjectType> GetFlags (this AVMetadataObjectType metadataObjectType)
		{
			var shifts = 0;

			var value = (ulong) metadataObjectType;
			while (value != 0) {
				if ((value & 1) == 1) {
					yield return (AVMetadataObjectType) (1 << shifts);
				}

				shifts++;
				value >>= 1;
			}
		}
	}
}
