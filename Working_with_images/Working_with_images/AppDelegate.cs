using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Working_with_images
{
    /// <summary>
    /// The UIApplicationDelegate for the application. This class is responsible for launching the 
    /// User Interface of the application, as well as listening (and optionally responding) to 
    /// application events from iOS.
    /// </summary>
    [Register ("AppDelegate")]
    public partial class AppDelegate : UIApplicationDelegate
    {
        // class-level declarations
        UIWindow window;
         
        /// <summary>
        /// This method is invoked when the application has loaded and is ready to run. In this 
        /// method you should instantiate the window, load the UI into it and then make the window
        /// visible.
        /// </summary>
        /// <remarks>
        /// You have 5 seconds to return from this method, or iOS will terminate your application.
        /// </remarks>
        public override bool FinishedLaunching (UIApplication app, NSDictionary options)
        {
            // create a new window instance based on the screen size
            window = new UIWindow (UIScreen.MainScreen.Bounds);
            
            // If you have defined a view, add it here:
            // window.AddSubview (navigationController.View);
                 
            // make the window visible
            window.MakeKeyAndVisible ();
                 
            return true;
        }
    }
}
