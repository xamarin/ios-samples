
namespace SimpleWatchConnectivity.WatchAppExtension
{
    using CoreFoundation;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using WatchConnectivity;

    public partial class FileTransfersController : UserInfoTransfersController
    {
        // Hold the file transfer observers to keep observing the progress.
        private FileTransferObservers fileTransferObservers;

        public FileTransfersController(IntPtr handle) : base(handle) { }

        protected override string RowType { get; } = nameof(FileTransferRowController);

        /// <summary>
        /// Rebuild the fileTransferObservers every time transfersStore is rebuilt.
        /// </summary>
        protected override List<SessionTransfer> Transfers
        {
            get
            {
                if (this.TransfersStore == null)
                {
                    this.fileTransferObservers = new FileTransferObservers();

                    var fileTransfers = WCSession.DefaultSession.OutstandingFileTransfers;
                    this.TransfersStore = fileTransfers.Select(transfer => new SessionTransfer { SessionFileTransfer = transfer }).ToList();

                    // Observing handler can be called from background so dispatch
                    // the UI update code to main queue and use the table data at the moment.
                    foreach (var transfer in fileTransfers)
                    {
                        this.fileTransferObservers.Observe(transfer, (progress) =>
                        {
                            DispatchQueue.MainQueue.DispatchAsync(() =>
                            {
                                var index = this.Transfers.FindIndex(t => t.SessionFileTransfer?.Progress == progress);
                                if (index != -1)
                                {
                                    if (this.Table.GetRowController(index) is FileTransferRowController row)
                                    {
                                        row.Progress = progress.LocalizedDescription;
                                    }
                                }
                            });
                        });
                    }
                }

                return this.TransfersStore;
            }
        }
    }
}