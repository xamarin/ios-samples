using Foundation;
using System;
using UIKit;

namespace AVCustomEdit
{
    public interface ITransitionTypePickerDelegate
    {
        void DidPickTransitionType(TransitionType transitionType);
    }

    public partial class TransitionTypeController : UITableViewController
    {
        public TransitionTypeController(IntPtr handle) : base(handle) { }

        public ITransitionTypePickerDelegate Delegate { get; set; }

        public TransitionType currentTransition;

        partial void transitionSelected(UIBarButtonItem sender)
        {
            this.Delegate.DidPickTransitionType(this.currentTransition);
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            var selectedCell = tableView.CellAt(indexPath);
            selectedCell.Accessory = UITableViewCellAccessory.Checkmark;

            if (selectedCell == this.diagonalWipeCell)
            {
                this.crossDissolveCell.Accessory = UITableViewCellAccessory.None;
                this.Delegate.DidPickTransitionType(TransitionType.DiagonalWipeTransition);
                this.currentTransition = TransitionType.DiagonalWipeTransition;
            }
            else if (selectedCell == this.crossDissolveCell)
            {
                this.diagonalWipeCell.Accessory = UITableViewCellAccessory.None;
                this.Delegate.DidPickTransitionType(TransitionType.CrossDissolveTransition);
                this.currentTransition = TransitionType.CrossDissolveTransition;
            }

            tableView.DeselectRow(indexPath, true);
        }
    }

    public enum TransitionType 
    {
        DiagonalWipeTransition = 0,
        CrossDissolveTransition = 1,
    }
}