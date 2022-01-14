
//using VisionObjectTrack;

//// This is the main entry point of the application.
//// if you want to use a different Application Delegate class from "AppDelegate"
//// you can specify it here.
//UIApplication.Main (args, null, typeof (AppDelegate));

namespace VisionObjectTrack
{
        using UIKit;

        public class Application
        {
                // This is the main entry point of the application.
                static void Main (string [] args)
                {
                        // if you want to use a different Application Delegate class from "AppDelegate"
                        // you can specify it here.
                        UIApplication.Main (args, null, typeof (AppDelegate));
                }
        }
}