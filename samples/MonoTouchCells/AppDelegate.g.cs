using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

public partial class AppDelegate : UIApplicationDelegate {

        [Connect]                                                                                                                            
        public UIWindow window {
                get { return (UIWindow) GetNativeField ("window"); }
                set { SetNativeField ("window", value); }
        }

        [Connect]                                                                                                                            
        public UINavigationController navController {
                get { return (UINavigationController) GetNativeField ("navController"); }
                set { SetNativeField ("navController", value); }
        }
}
