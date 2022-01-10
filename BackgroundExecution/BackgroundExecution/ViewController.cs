using System;
using UIKit;

namespace BackgroundExecution
{
    public partial class ViewController : UIViewController
    {
        protected ViewController(IntPtr handle) : base(handle) { }

        partial void StartLongRunningTask(UIButton sender)
        {
            nint taskId = 0;

            // register our background task
            taskId = UIApplication.SharedApplication.BeginBackgroundTask(() =>
            {
                Console.WriteLine("Running out of time to complete you background task!");
                UIApplication.SharedApplication.EndBackgroundTask(taskId);
            });

            Console.WriteLine($"Starting background task {taskId}");

            // sleep for five seconds
            System.Threading.Thread.Sleep(5000);

            Console.WriteLine($"Background task {taskId} completed.");

            // mark our background task as complete
            UIApplication.SharedApplication.EndBackgroundTask(taskId);
        }
    }
}