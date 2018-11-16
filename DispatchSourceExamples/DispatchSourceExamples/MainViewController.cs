using DispatchSourceExamples.Models;
using Foundation;
using System;
using System.Collections.Generic;
using UIKit;

namespace DispatchSourceExamples
{
    public partial class MainViewController : UITableViewController
    {
        private const string OpenDetailsSegueIndentifier = "openDetailsSegue";
        private const string CellIdentifier = "sourceCellIdentifier";

        private readonly List<DispatchSourceItem> items = new List<DispatchSourceItem>
        {
            new DispatchSourceItem { Type = DispatchSourceType.Timer, Title = "Timer", Subtitle = "Submit the event to the target queue"},
            new DispatchSourceItem { Type = DispatchSourceType.Vnode, Title = "Vnode", Subtitle = "Monitor the file system nodes for changes"},
            new DispatchSourceItem { Type = DispatchSourceType.MemoryPressure, Title = "Memory Pressure", Subtitle = "Monitor the system memory pressure"},
            new DispatchSourceItem { Type = DispatchSourceType.ReadMonitor, Title = "Read Monitor", Subtitle = "Monitor file descriptors for pending data"},
            new DispatchSourceItem { Type = DispatchSourceType.WriteMonitor, Title = "Write Monitor", Subtitle = "Monitor file descriptors for write buffer space"}
        };

        private DispatchSourceItem selectedItem;

        public MainViewController(IntPtr handle) : base(handle) { }

        public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
        {
            if(!string.IsNullOrEmpty(segue?.Identifier) && 
                segue.Identifier == OpenDetailsSegueIndentifier)
            {
                if(segue.DestinationViewController is DetailsViewController controller)
                {
                    controller.DispatchItem = selectedItem;
                    selectedItem = null;
                }
            }
        }

        public override nint RowsInSection(UITableView tableView, nint section)
        {
            return items.Count;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var item = items[indexPath.Row];

            var cell = tableView.DequeueReusableCell(CellIdentifier, indexPath);
            cell.TextLabel.Text = item.Title;
            cell.DetailTextLabel.Text = item.Subtitle;

            return cell;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            selectedItem = items[indexPath.Row];
            PerformSegue(OpenDetailsSegueIndentifier, this);
        }
    }
}