using Foundation;

namespace Emporium
{
	/// <summary>
	/// The value of the EmporiumBundlePrefix setting is
	/// written to the Info.plist file of every project of the
	/// Emporium solution. Specifically, the value of EmporiumBundlePrefix is
	/// used as the string value for a key of EmporiumBundlePrefix. This value
	/// is loaded from the target's bundle to static property
	/// "Prefix" from the nested "Bundle" class.
	/// This avoids the need for developers to edit both EmporiumBundlePrefix
	/// and the code below. The value of "Bundle.Prefix" is then used as part of 
	/// an interpolated string to insert the user-defined value of EmporiumBundlePrefix
	/// into several static string constants below.
	/// </summary>
	public static class AppConfiguration
	{
		public static class Bundle
		{
			public static string Prefix {
				get {
					return (NSString)NSBundle.MainBundle.ObjectForInfoDictionary ("EmporiumBundlePrefix");
				}
			}
		}

		public static class UserActivity
		{
			public static string Payment {
				get {
					return string.Format ("{0}.payment", Bundle.Prefix);
				}
			}
		}

		public static class Merchant
		{
			public static string Identififer {
				get {
					return string.Format ("merchant.{0}", Bundle.Prefix);
				}
			}
		}
	}
}