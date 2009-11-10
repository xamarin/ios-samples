using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Drawing;

public partial class MainViewController {
        [Connect]                                                                                                                            
        public UIWindow window {
                get { return (UIWindow) GetNativeField ("window"); }
                set { SetNativeField ("window", value); }
        }

        [Connect]                                                                                                                            
        public UINavigationController navigationController {
                get { return (UINavigationController) GetNativeField ("navigationController"); }
                set { SetNativeField ("navigationController", value); }
        }
}

public partial class AppDelegate {
        [Connect]                                                                                                                            
        public UIWindow window {
                get { return (UIWindow) GetNativeField ("window"); }
                set { SetNativeField ("window", value); }
        }

        [Connect]                                                                                                                            
        public UINavigationController navigationController {
                get { return (UINavigationController) GetNativeField ("navigationController"); }
                set { SetNativeField ("navigationController", value); }
        }
}