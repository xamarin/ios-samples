
namespace SimpleWatchConnectivity.WatchAppExtension {
	using System;

	public partial class FileTransferRowController : UserInfoTransferRowController {
		private string progress;

		public FileTransferRowController (IntPtr handle) : base (handle) { }

		public string Progress {
			get {
				return this.progress;
			}
			set {
				this.progress = value;
				this.ProgressLabel.SetText (this.progress);
			}
		}

		/// <summary>
		/// Update the table cell with the transfer's timed color.
		/// </summary>
		public override void Update (SessionTransfer transfer)
		{
			this.TitleLabel.SetText (transfer.TimedColor.TimeStamp);
			this.TitleLabel.SetTextColor (transfer.TimedColor.Color);
			this.ProgressLabel.SetText ("%0 completed");
			this.ProgressLabel.SetTextColor (transfer.TimedColor.Color);
		}
	}
}
