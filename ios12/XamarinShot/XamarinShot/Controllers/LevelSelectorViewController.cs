﻿
namespace XamarinShot
{
    using Foundation;
    using XamarinShot.Models;
    using XamarinShot.Utils;
    using System;
    using UIKit;

    public interface ILevelSelectorViewControllerDelegate
    {
        void OnLevelSelected(LevelSelectorViewController controller, GameLevel level);
    }

    /// <summary>
    /// View controller for choosing levels.
    /// </summary>
    public partial class LevelSelectorViewController : UITableViewController
    {
        public LevelSelectorViewController(IntPtr handle) : base(handle) { }

        public ILevelSelectorViewControllerDelegate Delegate { get; set; }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            this.TableView.Delegate = this;
            this.TableView.DataSource = this;
        }

        #region UITableViewDataSource

        public override nint RowsInSection(UITableView tableView, nint section)
        {
            return GameLevel.AllLevels.Count;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var level = GameLevel.Level(indexPath.Row);
            if (level == null)
            {
                throw new Exception($"Level {indexPath.Row} not found");
            }

            var cell = this.TableView.DequeueReusableCell("LevelCell", indexPath);
            cell.TextLabel.Text = level.Name;

            return cell;
        }

        #endregion

        #region UITableViewDelegate

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            var level = GameLevel.Level(indexPath.Row);
            if (level != null)
            {
                UserDefaults.SelectedLevel = level;
                this.Delegate?.OnLevelSelected(this, level);

                this.NavigationController.PopViewController(true);
            }
            else
            {
                throw new Exception($"Level {indexPath.Row} not found");
            }
        }

        #endregion
    }
}