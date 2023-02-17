
namespace SimpleWatchConnectivity.WatchAppExtension {
	using Foundation;
	using System;

	public partial class UserInfoTransferRowController : NSObject {
		public UserInfoTransferRowController (IntPtr handle) : base (handle) { }

		[Outlet]
		protected WatchKit.WKInterfaceLabel TitleLabel { get; set; }

		/// <summary>
		/// Update the table cell with the transfer's timed color.
		/// </summary>
		public virtual void Update (SessionTransfer transfer)
		{
			this.TitleLabel.SetText (transfer.TimedColor.TimeStamp);
			this.TitleLabel.SetTextColor (transfer.TimedColor.Color);
		}
	}
}
