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

        public TransitionType CurrentTransition { get; set; }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            switch (this.CurrentTransition)
            {
                case TransitionType.CrossDissolveTransition:
                    this.crossDissolveCell.Accessory = UITableViewCellAccessory.Checkmark;
                    break;

                case TransitionType.DiagonalWipeTransition:
                    this.diagonalWipeCell.Accessory = UITableViewCellAccessory.Checkmark;
                    break;
            }
        }

        partial void TransitionSelected(UIBarButtonItem sender)
        {
            this.Delegate.DidPickTransitionType(this.CurrentTransition);
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            var selectedCell = tableView.CellAt(indexPath);
            selectedCell.Accessory = UITableViewCellAccessory.Checkmark;

            if (selectedCell == this.diagonalWipeCell)
            {
                this.crossDissolveCell.Accessory = UITableViewCellAccessory.None;
                this.CurrentTransition = TransitionType.DiagonalWipeTransition;
                this.Delegate.DidPickTransitionType(TransitionType.DiagonalWipeTransition);
            }
            else if (selectedCell == this.crossDissolveCell)
            {
                this.diagonalWipeCell.Accessory = UITableViewCellAccessory.None;
                this.CurrentTransition = TransitionType.CrossDissolveTransition;
                this.Delegate.DidPickTransitionType(TransitionType.CrossDissolveTransition);
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