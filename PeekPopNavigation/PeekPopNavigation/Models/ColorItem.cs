using Foundation;
using UIKit;

namespace PeekPopNavigation.Models
{
    /// <summary>
    /// A model class used to represent starred and unstarred colors.
    /// </summary>
    public class ColorItem : NSObject
    {
        public static NSString ColorItemUpdated { get; } = new NSString("com.xamarin.peek-pop-navigation.ColorItemUpdated");
        public static NSString ColorItemDeleted { get; } = new NSString("com.xamarin.peek-pop-navigation.ColorItemDeleted");

        private string name;
        private UIColor color;
        private bool starred;

        public ColorItem(string name, UIColor color, bool starred)
        {
            this.name = name;
            this.color = color;
            this.starred = starred;
        }

        public string Name
        {
            get
            {
                return this.name;
            }

            set
            {
                this.name = value;
                NSNotificationCenter.DefaultCenter.PostNotificationName(ColorItemUpdated, this);
            }
        }

        public UIColor Color
        {
            get
            {
                return this.color;
            }

            set
            {
                this.color = value;
                NSNotificationCenter.DefaultCenter.PostNotificationName(ColorItemUpdated, this);
            }
        }

        public bool Starred
        {
            get
            {
                return this.starred;
            }

            set
            {
                this.starred = value;
                NSNotificationCenter.DefaultCenter.PostNotificationName(ColorItemUpdated, this);
            }
        }
    }
}