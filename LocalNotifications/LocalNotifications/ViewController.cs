using Foundation;
using System;
using UIKit;
using UserNotifications;

namespace LocalNotifications
{
    public partial class ViewController : UIViewController
    {
        protected ViewController(IntPtr handle) : base(handle) { }

        partial void SendNotification(UIButton sender)
        {
            var content = new UNMutableNotificationContent();
            content.Title = "View Alert";
            content.Body = "Your 10 second alert has fired!";
            content.Sound = UNNotificationSound.Default;
            content.Badge = 1;

            var trigger = UNTimeIntervalNotificationTrigger.CreateTrigger(10, false);

            var requestID = "sampleRequest";
            var request = UNNotificationRequest.FromIdentifier(requestID, content, trigger);

            // schedule it
            UNUserNotificationCenter.Current.AddNotificationRequest(request, (error) =>
            {
                if (error != null)
                {
                    Console.WriteLine($"Error: {error.LocalizedDescription ?? ""}");
                }
                else
                {
                    Console.WriteLine("Scheduled...");
                }
            });
        }
    }
}