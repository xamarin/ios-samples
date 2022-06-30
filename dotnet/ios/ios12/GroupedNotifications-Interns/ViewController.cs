using UserNotifications;

namespace GroupedNotifications
{
    public partial class ViewController : UIViewController
    {
        string[] appointments = new string[] { "Dentist", "Doctor", "Salon", "Gym", "Plumber", "Roofer", "Mechanic" };
        string[] friends = new string[] { "Alice", "Bob", "Candace", "David", "Emily", "Frank", "Georgia", "Harry", "Isabelle" };
        string[] messages = new string[]
        {
            "Hey {0}!",
            "Oh hi, what's up?",
            "Let's go to the movies today at 2:00.",
            "I'd rather go hiking.",
            "But it's raining!",
            "How about a board game?",
            "Not in the mood. Bowling?",
            "I already went bowling today. Get some ice cream?",
            "No thanks. Let's clean my basement!",
            "What? That's a terrible idea."
        };
        int unthreadedMessagesSent = 0;
        int threadNumber = 0;
        int threadMessagesSent = 0;
        string friend = String.Empty;
        string threadId = String.Empty;

        protected ViewController(IntPtr handle) : base(handle) { }

        partial void UpdateThreadId(UIButton sender)
        {
            StartNewThread();
        }

        async partial void ScheduleUnthreadedNotification(UIButton sender)
        {
            var center = UNUserNotificationCenter.Current;

            UNNotificationSettings settings = await center.GetNotificationSettingsAsync();
            if (settings.AuthorizationStatus != UNAuthorizationStatus.Authorized)
            {
                return;
            }

            string appointment = appointments[unthreadedMessagesSent % appointments.Length];

            var content = new UNMutableNotificationContent()
            {
                Title = appointment,
                Body = "See you for your appointment today at 2:00pm!",
                SummaryArgument = appointment
            };

            var request = UNNotificationRequest.FromIdentifier(
                Guid.NewGuid().ToString(),
                content,
                UNTimeIntervalNotificationTrigger.CreateTrigger(1, false)
            );

            center.AddNotificationRequest(request, null);

            unthreadedMessagesSent += 1;
        }

        async partial void ScheduleThreadedNotification(UIButton sender)
        {
            var center = UNUserNotificationCenter.Current;

            UNNotificationSettings settings = await center.GetNotificationSettingsAsync();
            if (settings.AuthorizationStatus != UNAuthorizationStatus.Authorized)
            {
                return;
            }

            string? author = friend;
            if (threadMessagesSent % 2 == 0)
            {
                author = "Me";
            }

            string message = "Fine!";
            if (threadMessagesSent < messages.Length)
            {
                message = messages[threadMessagesSent % messages.Length];
                if (threadMessagesSent == 0)
                {
                    message = String.Format(message, friend);
                }
            }

            var content = new UNMutableNotificationContent()
            {
                ThreadIdentifier = threadId,
                Title = author,
                Body = message,
                SummaryArgument = author
            };

            var request = UNNotificationRequest.FromIdentifier(
                Guid.NewGuid().ToString(),
                content,
                UNTimeIntervalNotificationTrigger.CreateTrigger(1, false)
            );

            center.AddNotificationRequest(request, null);

            threadMessagesSent += 1;
        }

        void StartNewThread()
        {
            threadId = $"message-{friend}";
            threadMessagesSent = 0;
            friend = friends[threadNumber % friends.Length];
            MessageWithFriendButton.SetTitle($"Message with {friend}", UIControlState.Normal);
            threadNumber += 1;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            StartNewThread();
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
        }
    }
}
