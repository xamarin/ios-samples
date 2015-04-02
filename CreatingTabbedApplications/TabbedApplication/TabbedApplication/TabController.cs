using System;
using UIKit;

namespace TabbedApplication
{
	public class TabController : UITabBarController
	{
        
		UIViewController tab1, tab2, tab3;
        
		public TabController ()
		{
			tab1 = new UIViewController ();
//			tab1.Title = "Green";
			tab1.TabBarItem = new UITabBarItem (UITabBarSystemItem.Favorites, 0);
			tab1.View.BackgroundColor = UIColor.Green;
			     
			tab2 = new UIViewController ();
//			tab2.Title = "Orange";
			tab2.TabBarItem = new UITabBarItem ();
			tab2.TabBarItem.Image = UIImage.FromFile ("second.png");
			tab2.TabBarItem.Title = "Second";
			tab2.View.BackgroundColor = UIColor.Orange;
       
			tab3 = new UIViewController ();
            tab3.Title = "Red";
			tab3.View.BackgroundColor = UIColor.Red;
            tab3.TabBarItem.BadgeValue = "Hi";
            
			var tabs = new UIViewController[] {
                tab1, tab2, tab3
            };
            
			ViewControllers = tabs;
		}  
	}
}


