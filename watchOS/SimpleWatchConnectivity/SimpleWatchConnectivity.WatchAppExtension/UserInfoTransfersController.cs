
namespace SimpleWatchConnectivity.WatchAppExtension
{
    using Foundation;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using WatchConnectivity;
    using WatchKit;

    public partial class UserInfoTransfersController : WKInterfaceController
    {
        private Command command;

        public UserInfoTransfersController(IntPtr handle) : base(handle) { }

        protected virtual string RowType { get; } = nameof(UserInfoTransfersController);

        // Outstnding transfers can change in the background so make a copy (cache) to
        // make sure the data doesn't change during the table loading cycle.
        // Subclasses can override the computed property to provide the right transfers.
        protected List<SessionTransfer> TransfersStore { get; set; }

        [Outlet]
        protected WKInterfaceTable Table { get; set; }

        protected virtual List<SessionTransfer> Transfers
        {
            get
            {
                if (this.TransfersStore == null)
                {
                    this.TransfersStore = WCSession.DefaultSession.OutstandingUserInfoTransfers
                                                                  .Select(transfer => new SessionTransfer { SessionUserInfoTransfer = transfer })
                                                                  .ToList();
                }

                return this.TransfersStore;
            }
        }

        /// <summary>
        /// Load the table.Show the "Done" row if there isn't any outstanding transfers.
        /// </summary>
        protected void LoadTable()
        {
            if (this.Transfers.Any())
            {
                this.Table.SetNumberOfRows(this.Transfers.Count, this.RowType);

                for (var index = 0; index < this.Transfers.Count; index++)
                {
                    var transfer = this.Transfers[index];
                    if (this.Table.GetRowController(index) is UserInfoTransferRowController row)
                    {
                        row.Update(transfer);
                    }
                }
            }
            else
            {
                this.Table.SetNumberOfRows(1, nameof(DoneRowController));
            }
        }

        public override void Awake(NSObject context)
        {
            base.Awake(context);

            if (context != null && Enum.TryParse(context.ToString(), out Command acommand))
            {
                this.command = acommand;
                this.LoadTable();
            }
            else
            {
                throw new Exception("Invalid context for presenting this controller!");
            }
        }

        public override void WillActivate()
        {
            base.WillActivate();
            NSNotificationCenter.DefaultCenter.AddObserver(new NSString(NotificationName.DataDidFlow), this.DataDidFlow);
        }

        public override void DidDeactivate()
        {
            base.DidDeactivate();
            NSNotificationCenter.DefaultCenter.RemoveObserver(this);
        }

        /// <summary>
        /// Cancel the transfer when the table row is selected.
        /// </summary>
        public override void DidSelectRow(WKInterfaceTable table, nint rowIndex)
        {
            if (rowIndex >= this.Transfers.Count)
            {
                Console.WriteLine($"Selected row has been removed! Current transfers: {Transfers}");
            }
            else
            {
                var transfer = this.Transfers[(int)rowIndex];
                transfer.Cancel(this.command);
            }
        }

        /// <summary>
        /// Update the UI the notification object.
        /// </summary>
        private void DataDidFlow(NSNotification notification)
        {
            if (notification.Object is CommandStatus commandStatus)
            {
                if (commandStatus.Command == this.command && commandStatus.Phrase != Phrase.Failed)
                {
                    this.TransfersStore = null;
                    this.LoadTable();
                }
            }
        }
    }
}