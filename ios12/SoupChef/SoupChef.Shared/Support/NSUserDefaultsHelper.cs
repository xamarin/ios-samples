
namespace SoupKit.Support
{
    using System;
    using Foundation;
    using System.Resources;

    public static class NSUserDefaultsHelper
    {
        private const string AppGroup = "group.com.xamarin.SoupChef";

        public static class StorageKeys
        {
            public const string SoupMenu = "soupMenu";
            public const string OrderHistory = "orderHistory";
            public const string VoiceShortcutHistory = "voiceShortcutHistory";
        }

        public static NSUserDefaults DataSuite
        {
            get
            {
                var dataSuite = new NSUserDefaults(AppGroup, NSUserDefaultsType.SuiteName);
                if (dataSuite == null)
                {
                    throw new Exception($"Could not load UserDefaults for app group {AppGroup}");
                }

                return dataSuite;
            }
        }
    }
}