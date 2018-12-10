using System;
using System.Threading.Tasks;
using Foundation;
using UIKit;

namespace BackgroundExecution
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
    [Register("AppDelegate")]
    public class AppDelegate : UIApplicationDelegate
    {
        public override UIWindow Window { get; set; }

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            // Override point for customization after application launch.
            // If not required for your application you can safely delete this method

            // how to check if multi-tasking is supported
            if (UIDevice.CurrentDevice.IsMultitaskingSupported)
            {
                // code here to change your app's behavior
            }

            return true;
        }

        // Runs when the activation transitions from running in the background to being the foreground application.
        public override void OnActivated(UIApplication application)
        {
            Console.WriteLine("App is becoming active");
        }

        public override void OnResignActivation(UIApplication application)
        {
            Console.WriteLine("App moving to inactive state.");
        }

        public override void WillTerminate(UIApplication application)
        {
            Console.WriteLine("App is terminating.");
        }

        public override void DidEnterBackground(UIApplication application)
        {
            Console.WriteLine("App entering background state.");

            nint taskID = 0;
            // if you're creating a VOIP application, this is how you set the keep alive
            //UIApplication.SharedApplication.SetKeepAliveTimout(600, () => { /* keep alive handler code*/ });

            // register a long running task, and then start it on a new thread so that this method can return
            taskID = UIApplication.SharedApplication.BeginBackgroundTask(() =>
            {
                Console.WriteLine("Running out of time to complete you background task!");
                UIApplication.SharedApplication.EndBackgroundTask(taskID);
            });

            Task.Factory.StartNew(() => FinishLongRunningTask(taskID));
        }

        private void FinishLongRunningTask(nint taskID)
        {
            Console.WriteLine($"Starting task {taskID}");
            Console.WriteLine($"Background time remaining: {UIApplication.SharedApplication.BackgroundTimeRemaining}");

            // sleep for 15 seconds to simulate a long running task
            System.Threading.Thread.Sleep(15000);

            Console.WriteLine($"Task {taskID} finished");
            Console.WriteLine($"Background time remaining: {UIApplication.SharedApplication.BackgroundTimeRemaining}");

            // call our end task
            UIApplication.SharedApplication.EndBackgroundTask(taskID);
        }
    }
}