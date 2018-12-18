using System;

using Foundation;

namespace UICatalog
{
	public static class LocalizationExtension
	{
		public static string Localize(this string source)
		{
			return NSBundle.MainBundle.LocalizedString(source, "");
		}
	}
}