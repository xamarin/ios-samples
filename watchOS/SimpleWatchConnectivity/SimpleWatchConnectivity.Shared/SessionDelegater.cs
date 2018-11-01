
namespace SimpleWatchConnectivity
{
    using CoreFoundation;
    using Foundation;
    using System;
    using System.Linq;
    using WatchConnectivity;

#if __WATCHOS__
    using ClockKit;
#endif

    /// <summary>
    /// Custom notifications.
    /// Posted when Watch Connectivity activation or reachibility status is changed, or when data is received or sent. Clients observe these notifications to update the UI.
    /// </summary>
    public static class NotificationName
    {
        public static NSString DataDidFlow = new NSString("DataDidFlow");
        public static NSString ActivationDidComplete = new NSString("ActivationDidComplete");
        public static NSString ReachabilityDidChange = new NSString("ReachabilityDidChange");
    }

    /// <summary>
    /// Implement WCSessionDelegate methods to receive Watch Connectivity data and notify clients.
    /// WCsession status changes are also handled here.
    /// </summary>
    public class SessionDelegater : WCSessionDelegate
    {
        /// <summary>
        /// Called when WCSession activation state is changed.
        /// </summary>
        public override void ActivationDidComplete(WCSession session, WCSessionActivationState activationState, NSError error)
        {
            this.PostNotificationOnMainQueueAsync(NotificationName.ActivationDidComplete);
        }

        /// <summary>
        /// Called when WCSession reachability is changed.
        /// </summary>
        public override void SessionReachabilityDidChange(WCSession session)
        {
            this.PostNotificationOnMainQueueAsync(NotificationName.ReachabilityDidChange);
        }

        /// <summary>
        /// Called when an app context is received.
        /// </summary>
        public override void DidReceiveApplicationContext(WCSession session, NSDictionary<NSString, NSObject> applicationContext)
        {
            var commandStatus = new CommandStatus(Command.UpdateAppContext, Phrase.Received) { TimedColor = new TimedColor(applicationContext) };
            this.PostNotificationOnMainQueueAsync(NotificationName.DataDidFlow, commandStatus);
        }

        /// <summary>
        /// Called when a message is received and the peer doesn't need a response.
        /// </summary>
        public override void DidReceiveMessage(WCSession session, NSDictionary<NSString, NSObject> message)
        {
            var commandStatus = new CommandStatus(Command.SendMessage, Phrase.Received) { TimedColor = new TimedColor(message) };
            this.PostNotificationOnMainQueueAsync(NotificationName.DataDidFlow, commandStatus);
        }

        /// <summary>
        /// Called when a message is received and the peer needs a response.
        /// </summary>
        public override void DidReceiveMessage(WCSession session, NSDictionary<NSString, NSObject> message, WCSessionReplyHandler replyHandler)
        {
            this.DidReceiveMessage(session, message);
            replyHandler(message); // Echo back the time stamp.
        }

        /// <summary>
        /// Called when a piece of message data is received and the peer doesn't need a respons
        /// </summary>
        public override void DidReceiveMessageData(WCSession session, NSData messageData)
        {
            var commandStatus = new CommandStatus(Command.SendMessageData, Phrase.Received) { TimedColor = TimedColor.Create(messageData) };
            this.PostNotificationOnMainQueueAsync(NotificationName.DataDidFlow, commandStatus);
        }

        /// <summary>
        /// Called when a piece of message data is received and the peer needs a response.
        /// </summary>
        public override void DidReceiveMessageData(WCSession session, NSData messageData, WCSessionReplyDataHandler replyHandler)
        {
            this.DidReceiveMessageData(session, messageData);
            replyHandler(messageData); // Echo back the time stamp.
        }

        /// <summary>
        /// Called when a userInfo is received.
        /// </summary>
        public override void DidReceiveUserInfo(WCSession session, NSDictionary<NSString, NSObject> userInfo)
        {
            var commandStatus = new CommandStatus(Command.TransferUserInfo, Phrase.Received) { TimedColor = new TimedColor(userInfo) };

            if (userInfo.TryGetValue(PayloadKey.IsCurrentComplicationInfo, out NSObject isComplicationInfoObject) &&
                isComplicationInfoObject is NSNumber isComplicationInfo && isComplicationInfo.BoolValue)
            {
                commandStatus.Command = Command.TransferCurrentComplicationUserInfo;

#if __WATCHOS__
                var server = CLKComplicationServer.SharedInstance;
                var complications = server.ActiveComplications;
                if (complications != null && complications.Any())
                {
                    foreach (var complication in complications)
                    {
                        // Call this method sparingly. If your existing complication data is still valid,
                        // consider calling the extendTimeline(for:) method instead.
                        server.ReloadTimeline(complication);
                    }
                }
#endif
            }

            this.PostNotificationOnMainQueueAsync(NotificationName.DataDidFlow, commandStatus);
        }

        /// <summary>
        /// Called when sending a userInfo is done.
        /// </summary>
        public override void DidFinishUserInfoTransfer(WCSession session, WCSessionUserInfoTransfer userInfoTransfer, NSError error)
        {
            var commandStatus = new CommandStatus(Command.TransferUserInfo, Phrase.Finished) { TimedColor = new TimedColor(userInfoTransfer.UserInfo) };

#if __IOS__
            if (userInfoTransfer.CurrentComplicationInfo)
            {
                commandStatus.Command = Command.TransferCurrentComplicationUserInfo;
            }
#endif

            if (error != null)
            {
                commandStatus.ErrorMessage = error.LocalizedDescription;
            }

            this.PostNotificationOnMainQueueAsync(NotificationName.DataDidFlow, commandStatus);
        }

        /// <summary>
        /// Called when a file is received.
        /// </summary>
        public override void DidReceiveFile(WCSession session, WCSessionFile file)
        {
            var commandStatus = new CommandStatus(Command.TransferFile, Phrase.Received)
            {
                File = file,
                TimedColor = new TimedColor(file.Metadata)
            };

            // Note that WCSessionFile.fileURL will be removed once this method returns,
            // so instead of calling postNotificationOnMainQueue(name: .dataDidFlow, userInfo: userInfo),
            // we dispatch to main queue SYNCHRONOUSLY.

            DispatchQueue.MainQueue.DispatchSync(() =>
            {
                NSNotificationCenter.DefaultCenter.PostNotificationName(NotificationName.DataDidFlow, commandStatus);
            });
        }

        /// <summary>
        /// Called when a file transfer is done.
        /// </summary>
        public override void DidFinishFileTransfer(WCSession session, WatchConnectivity.WCSessionFileTransfer fileTransfer, NSError error)
        {
            var commandStatus = new CommandStatus(Command.TransferFile, Phrase.Finished);

            if (error != null)
            {
                commandStatus.ErrorMessage = error.LocalizedDescription;
                this.PostNotificationOnMainQueueAsync(NotificationName.DataDidFlow, commandStatus);
            }
            else
            {
                commandStatus.FileTransfer = fileTransfer;
                commandStatus.TimedColor = new TimedColor(fileTransfer.File.Metadata);

#if __WATCHOS__
                if (!string.IsNullOrEmpty(WatchSettings.SharedContainerId))
                {
                    var defaults = new NSUserDefaults(WatchSettings.SharedContainerId, NSUserDefaultsType.SuiteName);
                    if (defaults.BoolForKey(WatchSettings.ClearLogsAfterTransferred))
                    {
                        Logger.Shared.ClearLogs();
                    }
                }
#endif
                this.PostNotificationOnMainQueueAsync(NotificationName.DataDidFlow, commandStatus);
            }
        }

        #region WCSessionDelegate methods for iOS only.
#if __IOS__

        public override void DidBecomeInactive(WCSession session)
        {
            Console.WriteLine($"DidBecomeInactive: activationState = {session.ActivationState}");
        }

        public override void DidDeactivate(WCSession session)
        {
            // Activate the new session after having switched to a new watch.
            session.ActivateSession();
        }

        public override void SessionWatchStateDidChange(WCSession session)
        {
            Console.WriteLine($"SessionWatchStateDidChange: activationState = {session.ActivationState}");
        }

#endif
        #endregion

        /// <summary>
        /// Post a notification on the main thread asynchronously.
        /// </summary>
        private void PostNotificationOnMainQueueAsync(NSString name, CommandStatus @object = null)
        {
            DispatchQueue.MainQueue.DispatchAsync(() =>
            {
                NSNotificationCenter.DefaultCenter.PostNotificationName(name, @object);
            });
        }
    }
}
