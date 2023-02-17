
namespace SimpleWatchConnectivity.WatchAppExtension {
	using Foundation;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using UIKit;
	using WatchConnectivity;
	using WatchKit;

	public partial class MainInterfaceController : WKInterfaceController {
		// Retain the controllers so that we don't have to reload root controllers for every switch.
		private static readonly List<MainInterfaceController> Instances = new List<MainInterfaceController> ();

		private readonly FileTransferObservers fileTransferObservers = new FileTransferObservers ();

		private Command? command;

		public MainInterfaceController (IntPtr handle) : base (handle) { }

		public override void Awake (NSObject context)
		{
			base.Awake (context);

			if (context is CommandStatus commandStatus) {
				this.command = commandStatus.Command;
				this.UpdateUI (commandStatus);
				MainInterfaceController.Instances.Add (this);
			} else {
				this.StatusLabel.SetText ("Activating...");
				this.ReloadRootController ();
			}

			// Install notification observer.
			NSNotificationCenter.DefaultCenter.AddObserver (NotificationName.DataDidFlow, this.DataDidFlow);
			NSNotificationCenter.DefaultCenter.AddObserver (NotificationName.ActivationDidComplete, this.ActivationDidComplete);
			NSNotificationCenter.DefaultCenter.AddObserver (NotificationName.ReachabilityDidChange, this.ReachabilityDidChange);
		}

		~MainInterfaceController ()
		{
			NSNotificationCenter.DefaultCenter.RemoveObserver (this);
		}

		public override void WillActivate ()
		{
			base.WillActivate ();

			if (this.command.HasValue) {
				// For .updateAppContext, retrieve the receieved app context if any and update the UI.
				// For .transferFile and .transferUserInfo, log the outstanding transfers if any.
				if (this.command == Command.UpdateAppContext) {
					var timedColor = WCSession.DefaultSession.ReceivedApplicationContext;
					if (timedColor.Count > 0) {
						var commandStatus = new CommandStatus (this.command.Value, Phrase.Received) { TimedColor = new TimedColor (timedColor) };
						this.UpdateUI (commandStatus);
					}
				} else if (this.command == Command.TransferFile) {
					var transferCount = WCSession.DefaultSession.OutstandingFileTransfers.Length;
					if (transferCount > 0) {
						var commandStatus = new CommandStatus (Command.TransferFile, Phrase.Finished);
						this.LogOutstandingTransfers (commandStatus, transferCount);
					}
				} else if (this.command == Command.TransferUserInfo) {
					var transferCount = WCSession.DefaultSession.OutstandingUserInfoTransfers.Length;
					if (transferCount > 0) {
						var commandStatus = new CommandStatus (Command.TransferUserInfo, Phrase.Finished);
						this.LogOutstandingTransfers (commandStatus, transferCount);
					}
				}

				// Update the status group background color.
				if (this.command != Command.TransferFile && this.command != Command.TransferUserInfo) {
					this.StatusGroup.SetBackgroundColor (UIColor.Black);
				}
			}
		}

		/// <summary>
		/// Load paged-based UI. If a current context is specified, use the timed color it provided.
		/// </summary>
		private void ReloadRootController (CommandStatus currentContext = null)
		{
			var commands = Enum.GetValues (typeof (Command)).Cast<Command> ();
			var contexts = new List<CommandStatus> ();
			foreach (var aCommand in commands) {
				var commandStatus = new CommandStatus (aCommand, Phrase.Finished);

				if (currentContext != null && aCommand == currentContext.Command) {
					commandStatus.Phrase = currentContext.Phrase;
					commandStatus.TimedColor = currentContext.TimedColor;
				}

				contexts.Add (commandStatus);
			}

			var names = new string [contexts.Count];
			for (int i = 0; i < contexts.Count; i++) {
				names [i] = nameof (MainInterfaceController);
			}

			WKInterfaceController.ReloadRootPageControllers (names, contexts.ToArray (), WKPageOrientation.Horizontal, 0);
		}

		/// <summary>
		/// Update the UI based on the command status.
		/// </summary>
		private void DataDidFlow (NSNotification notification)
		{
			if (notification.Object is CommandStatus commandStatus) {
				// If the data is from current channel, simple update color and time stamp, then return.
				if (commandStatus.Command == command) {
					this.UpdateUI (commandStatus, commandStatus.ErrorMessage);
				} else {
					// Move the screen to the page matching the data channel, then update the color and time stamp.
					var index = MainInterfaceController.Instances.FindIndex (instance => instance.command == commandStatus.Command);
					if (index != -1) {
						var controller = MainInterfaceController.Instances [index];
						controller.BecomeCurrentPage ();
						controller.UpdateUI (commandStatus, commandStatus.ErrorMessage);
					}
				}
			}
		}

		private void ActivationDidComplete (NSNotification notification)
		{
			Console.WriteLine ($"'ActivationDidComplete': activationState: {WCSession.DefaultSession.ActivationState}");
		}

		private void ReachabilityDidChange (NSNotification notification)
		{
			Console.WriteLine ($"'ReachabilityDidChange': isReachable: {WCSession.DefaultSession.Reachable}");
		}

		/// <summary>
		/// Do the command associated with the current page.
		/// </summary>
		partial void CommandAction ()
		{
			if (this.command.HasValue) {
				switch (command.Value) {
				case Command.UpdateAppContext:
					SessionCommands.UpdateAppContext (TestDataProvider.AppContext);
					break;
				case Command.SendMessage:
					SessionCommands.SendMessage (TestDataProvider.Message);
					break;
				case Command.SendMessageData:
					SessionCommands.SendMessageData (TestDataProvider.MessageData);
					break;
				case Command.TransferUserInfo:
					SessionCommands.TransferUserInfo (TestDataProvider.UserInfo);
					break;
				case Command.TransferFile:
					SessionCommands.TransferFile (TestDataProvider.File, TestDataProvider.FileMetaData);
					break;
				case Command.TransferCurrentComplicationUserInfo:
					SessionCommands.TransferCurrentComplicationUserInfo (TestDataProvider.CurrentComplicationInfo);
					break;
				}
			}
		}

		/// <summary>
		/// Show outstanding transfer UI for .transferFile and .transferUserInfo.
		/// </summary>
		partial void StatusAction ()
		{
			if (this.command == Command.TransferFile) {
				this.PresentController (nameof (FileTransfersController), this.command.Value.ToString ());
			} else if (this.command == Command.TransferUserInfo) {
				this.PresentController (nameof (UserInfoTransfersController), this.command.Value.ToString ());
			}
		}

		#region Update status view.

		// Update the user interface with the command status.
		// Note that there isn't a timed color when the interface controller is initially loaded.
		private void UpdateUI (CommandStatus commandStatus, string errorMessage = null)
		{
			var timedColor = commandStatus.TimedColor;
			if (timedColor == null) {
				this.StatusLabel.SetText (string.Empty);
				this.CommandButton.SetTitle (commandStatus.Command.ToString ());
			} else {
				var title = new NSAttributedString (commandStatus.Command.ToString (), new UIStringAttributes { ForegroundColor = timedColor.Color });
				this.CommandButton.SetTitle (title);
				this.StatusLabel.SetTextColor (timedColor.Color);

				// If there is an error, show the message and return.
				if (!string.IsNullOrEmpty (errorMessage)) {
					this.StatusLabel.SetText ($"{errorMessage}");
				} else {
					// Observe the file transfer if it's phrase is "transferring".
					// Unobserve a file transfer if it's phrase is "finished".
					if (commandStatus.FileTransfer != null && commandStatus.Command == Command.TransferFile) {
						if (commandStatus.Phrase == Phrase.Finished) {
							this.fileTransferObservers.Unobserve (commandStatus.FileTransfer);
						} else if (commandStatus.Phrase == Phrase.Transferring) {
							this.fileTransferObservers.Observe (commandStatus.FileTransfer, (_) => this.LogProgress (commandStatus));
						}
					}

					// Log the outstanding file transfers if any.
					if (commandStatus.Command == Command.TransferFile) {
						var transferCount = WCSession.DefaultSession.OutstandingFileTransfers.Length;
						if (transferCount > 0) {
							this.LogOutstandingTransfers (commandStatus, transferCount);
							return;
						}
					}
					// Log the outstanding UserInfo transfers if any.
					else if (commandStatus.Command == Command.TransferUserInfo) {
						var transferCount = WCSession.DefaultSession.OutstandingUserInfoTransfers.Length;
						if (transferCount > 0) {
							this.LogOutstandingTransfers (commandStatus, transferCount);
							return;
						}
					}

					this.StatusLabel.SetText ($"{commandStatus.Phrase.ToString ()}  at\n{timedColor.TimeStamp}");
				}
			}
		}

		/// <summary>
		/// Log the outstanding transfer information if any.
		/// </summary>
		private void LogOutstandingTransfers (CommandStatus commandStatus, int outstandingCount)
		{
			if (commandStatus.Phrase == Phrase.Transferring) {
				var text = $"{commandStatus.Phrase.ToString ()} at\n{commandStatus.TimedColor.TimeStamp}\nOutstanding: {outstandingCount}\n Tap to view";
				this.StatusLabel.SetText (text);
			} else if (commandStatus.Phrase == Phrase.Finished) {
				this.StatusLabel.SetText ($"Outstanding: {outstandingCount}\n Tap to view");
			}
		}

		/// <summary>
		/// Log the file transfer progress. The command status is captured at the momment when the file transfer is observed.
		/// </summary>
		/// <param name="commandStatus">Command status.</param>
		private void LogProgress (CommandStatus commandStatus)
		{
			if (commandStatus.FileTransfer != null) {
				var fileName = commandStatus.FileTransfer.File.FileUrl.LastPathComponent;
				var progress = commandStatus.FileTransfer.Progress.LocalizedDescription ?? "No progress";
				this.StatusLabel.SetText ($"{commandStatus.Phrase.ToString ()}\n{fileName}\n{progress}");
			}
		}

		#endregion
	}
}
