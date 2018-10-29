using System;
using Foundation;

//#if __IOS__
using CoreSpotlight;
using UIKit;
using System.Linq.Expressions;
using System.Xml;
//#endif

namespace SoupChef.Support
{
    public static class NSUserActivityHelper
    {
        public static class ActivityKeys
        {
            public const string MenuItems = "menuItems";
            public const string SegueId = "segueID";
            public const string OrderId = "orderID";
        }

        private static string SearchableItemContentType = "Soup Menu";

        public static string ViewMenuActivityType = "com.xamarin.SoupChef.viewMenu";
        public static string OrderCompleteActivityType = "com.xamarin.SoupChef.orderComplete";

        public static NSUserActivity ViewMenuActivity
        {
            get
            {
                var userActivity = new NSUserActivity(ViewMenuActivityType)
                {
                    // User activites should be as rich as possible, with icons and localized strings for appropiate content attributes.
                    Title = NSBundle.MainBundle.GetLocalizedString("ORDER_LUNCH_TITLE", "View menu activity title"),
                    EligibleForPrediction = true
                };

                // #if canImport(CoreSpotlight)
                var attributes = new CSSearchableItemAttributeSet(NSUserActivityHelper.SearchableItemContentType /*kUTTypeContent*/)
                {
                    ThumbnailData = UIImage.FromBundle("tomato").AsPNG(),
                    Keywords = new string[] { "Order", "Soup", "Menu" },
                    DisplayName = NSBundle.MainBundle.GetLocalizedString("ORDER_LUNCH_TITLE", "View menu activity title"),
                    ContentDescription = NSBundle.MainBundle.GetLocalizedString("VIEW_MENU_CONTENT_DESCRIPTION", "View menu content description")
                };

                userActivity.ContentAttributeSet = attributes;
                //#endif

                var phrase = NSBundle.MainBundle.GetLocalizedString("ORDER_LUNCH_SUGGESTED_PHRASE", "Voice shortcut suggested phrase");
                userActivity.SuggestedInvocationPhrase = phrase;
                return userActivity;
            }
        }
    }
}