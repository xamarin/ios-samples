
namespace AVCamBarcode.Extensions {
	using CoreText;
	using UIKit;

	public static class FontExtensions {
		public static CTFont ToCTFont (this UIFont font)
		{
			return new CTFont (font.Name, font.PointSize);
		}
	}
}
