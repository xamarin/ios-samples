
namespace SimpleWatchConnectivity
{
    using CoreAnimation;
    using CoreFoundation;
    using CoreGraphics;
    using Foundation;
    using System;
    using UIKit;
    using WatchConnectivity;

    public partial class MainViewController : UIViewController
    {
        // We log the file transfer progress on the log view, so need to
        // oberve the file transfer progress.
        private readonly FileTransferObservers fileTransferObservers = new FileTransferObservers();

        public MainViewController(IntPtr handle) : base(handle) { }

        ~MainViewController()
        {
            NSNotificationCenter.DefaultCenter.RemoveObserver(this);
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            NSNotificationCenter.DefaultCenter.AddObserver(NotificationName.DataDidFlow, this.DataDidFlow);
            NSNotificationCenter.DefaultCenter.AddObserver(NotificationName.ActivationDidComplete, this.ActivationDidComplete);
            NSNotificationCenter.DefaultCenter.AddObserver(NotificationName.ReachabilityDidChange, this.ReachabilityDidChange);
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            var layer = new CALayer
            {
                ShadowOpacity = 1f,
                ShadowOffset = new CGSize(0f, 1f)
            };

            // Make sure the shadow is outside of the bottom of the screen.
            var rect = this.TableContainerView.Bounds;
            rect.Size = new CGSize(rect.Size.Width,
                                   rect.Size.Height + this.View.Window.SafeAreaInsets.Bottom);

            var path = UIBezierPath.FromRoundedRect(rect,
                                                    UIRectCorner.TopRight | UIRectCorner.TopLeft,
                                                    new CGSize(10f, 10f));
            var shapeLayer = new CAShapeLayer
            {
                Path = path.CGPath,
                FillColor = UIColor.White.CGColor
            };

            layer.AddSublayer(shapeLayer);

            this.TableContainerView.Layer.AddSublayer(layer);
            this.TablePlaceholderView.Layer.ZPosition = layer.ZPosition + 1f;
        }

        /// <summary>
        /// Append the message to the end of the text view and make sure it is visiable.
        /// </summary>
        private void Log(string message)
        {
            this.LogView.Text = $"{LogView.Text}\n\n{message}";
            this.LogView.ScrollRangeToVisible(new NSRange(LogView.Text.Length, 1));
        }

        private void UpdateReachabilityColor()
        {
            // WCSession.isReachable triggers a warning if the session is not activated.
            var isReachable = false;
            if (WCSession.DefaultSession.ActivationState == WCSessionActivationState.Activated)
            {
                isReachable = WCSession.DefaultSession.Reachable;
            }

            this.ReachableLabel.BackgroundColor = isReachable ? UIColor.Green : UIColor.Red;
        }

        private void ActivationDidComplete(NSNotification notification)
        {
            this.UpdateReachabilityColor();
        }

        private void ReachabilityDidChange(NSNotification notification)
        {
            this.UpdateReachabilityColor();
        }

        partial void Clear(UIButton sender)
        {
            this.LogView.Text = string.Empty;
        }

        /// <summary>
        /// Update the UI based on the userInfo dictionary of the notification.
        /// </summary>
        private void DataDidFlow(NSNotification notification)
        {
            if (notification.Object is CommandStatus commandStatus)
            {
                // If an error occurs, show the error message and returns.
                if (!string.IsNullOrEmpty(commandStatus.ErrorMessage))
                {
                    this.Log($"! {commandStatus.Command}...{commandStatus.ErrorMessage}");
                }
                else
                {
                    if (commandStatus.TimedColor != null)
                    {
                        this.Log($"#{commandStatus.Command}...\n{commandStatus.Phrase} at {commandStatus.TimedColor.TimeStamp}");

                        var fileURL = commandStatus.File?.FileUrl;
                        if (fileURL != null)
                        {
                            if (fileURL.PathExtension == "log")
                            {
                                var content = NSString.FromData(NSData.FromUrl(fileURL), NSStringEncoding.UTF8);
                                if (content.Length > 0)
                                {
                                    this.Log($"{fileURL.LastPathComponent}\n{content}");
                                }
                                else
                                {
                                    this.Log($"{fileURL.LastPathComponent}\n");
                                }
                            }
                        }

                        var fileTransfer = commandStatus.FileTransfer;
                        if (fileTransfer != null && commandStatus.Command == Command.TransferFile)
                        {
                            if (commandStatus.Phrase == Phrase.Finished)
                            {
                                this.fileTransferObservers.Unobserve(fileTransfer);
                            }
                            else if (commandStatus.Phrase == Phrase.Transferring)
                            {
                                this.fileTransferObservers.Observe(fileTransfer, (_) => this.LogProgress(fileTransfer));
                            }
                        }
                    }

                    this.NoteLabel.Hidden = !string.IsNullOrEmpty(this.LogView.Text);
                }
            }
        }

        /// <summary>
        /// Log the file transfer progress.
        /// </summary>
        private void LogProgress(WCSessionFileTransfer fileTransfer)
        {
            DispatchQueue.MainQueue.DispatchAsync(() =>
            {
                var dateFormatter = new NSDateFormatter { TimeStyle = NSDateFormatterStyle.Medium };
                var timeString = dateFormatter.StringFor(new NSDate());
                var fileName = fileTransfer.File.FileUrl.LastPathComponent;

                var progress = fileTransfer.Progress.LocalizedDescription ?? "No progress";
                this.Log($"- {fileName}: {progress} at {timeString}");
            });
        }
    }
}