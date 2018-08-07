using System;
using Foundation;

#if __IOS__
using CoreSpotlight;
using UIKit;
using System.Linq.Expressions;
using System.Xml;
#endif

namespace SoupKit.Support
{
    public static class NSUserActivityHelper
    {
        public static class ActivityKeys
        {
            public const string MenuItems = "menuItems";
            public const string SegueId = "segueID";
        }

        static string SearchableItemContentType = "Soup Menu";

        public static string ViewMenuActivityType = "com.xamarin.SoupChef.viewMenu";

        public static NSUserActivity ViewMenuActivity {
            get
            {
                var userActivity = new NSUserActivity(ViewMenuActivityType)
                {
                    Title = NSBundleHelper.SoupKitBundle.GetLocalizedString("ORDER_LUNCH_TITLE", "View menu activity title"),
                    EligibleForSearch = true,
                    EligibleForPrediction = true
                };

#if __IOS__
                var attributes = new CSSearchableItemAttributeSet(NSUserActivityHelper.SearchableItemContentType)
                {
                    ThumbnailData = UIImage.FromBundle("tomato").AsPNG(),
                    Keywords = ViewMenuSearchableKeywords,
                    DisplayName = NSBundleHelper.SoupKitBundle.GetLocalizedString("ORDER_LUNCH_TITLE", "View menu activity title"),
                    ContentDescription = NSBundleHelper.SoupKitBundle.GetLocalizedString("VIEW_MENU_CONTENT_DESCRIPTION", "View menu content description")
                };
                userActivity.ContentAttributeSet = attributes;
#endif

                var phrase = NSBundleHelper.SoupKitBundle.GetLocalizedString("ORDER_LUNCH_SUGGESTED_PHRASE", "Voice shortcut suggested phrase");
                userActivity.SuggestedInvocationPhrase = phrase;
                return userActivity;
            }
        }

        static string[] ViewMenuSearchableKeywords = new string[] {
            NSBundleHelper.SoupKitBundle.GetLocalizedString("ORDER",  "Searchable Keyword"),
            NSBundleHelper.SoupKitBundle.GetLocalizedString("SOUP", "Searchable Keyword"),
            NSBundleHelper.SoupKitBundle.GetLocalizedString("MENU", "Searchable Keyword")
        };
    }
}
