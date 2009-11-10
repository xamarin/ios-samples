using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Drawing;

public partial class TwitterSharpAppDelegate : UIApplicationDelegate {
        [Connect]                                                                                                                            
        public UIWindow Window {
                get { return (UIWindow) GetNativeField ("Window"); }
                set { SetNativeField ("Window", value); }
        }

        [Connect]                                                                                                                            
        public UITabBarController TabBarController {
                get { return (UITabBarController) GetNativeField ("TabBarController"); }
                set { SetNativeField ("TabBarController", value); }
        }
}
