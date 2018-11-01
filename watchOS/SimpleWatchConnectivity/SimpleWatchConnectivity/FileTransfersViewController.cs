
namespace SimpleWatchConnectivity
{
    using CoreFoundation;
    using Foundation;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UIKit;
    using WatchConnectivity;

    /// <summary>
    /// FileTransfersViewController manages the file transfer of the iOS app.
    /// </summary>
    public partial class FileTransfersViewController : UserInfoTransfersViewController
    {
        // Hold the file transfer observers to keep observing the progress.
        private FileTransferObservers fileTransferObservers;

        public FileTransfersViewController(IntPtr handle) : base(handle) { }

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
                                    var indexPath = NSIndexPath.FromRowSection(index, 0);
                                    var cell = this.TableView.CellAt(indexPath);
                                    if (cell != null)
                                    {
                                        cell.DetailTextLabel.Text = progress.LocalizedDescription;
                                    }
                                }
                            });
                        });
                    }
                }

                return this.TransfersStore;
            }
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = base.GetCell(tableView, indexPath);

            var transfer = this.Transfers[indexPath.Row]?.SessionFileTransfer;
            if (transfer != null)
            {
                cell.DetailTextLabel.Text = transfer.Progress.LocalizedDescription;
            }

            return cell;
        }
    }
}