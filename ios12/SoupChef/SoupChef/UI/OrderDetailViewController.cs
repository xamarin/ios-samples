/*
See LICENSE folder for this sample’s licensing information.

Abstract:
This class shows soup order details. It can be configured for two possible order types.
 When configured with a 'new' order type, the view controller collects details of a new order.
 When configured with a 'historical' order type, the view controller displays details of a previously placed order.
*/

using Foundation;
using System;
using UIKit;
using SoupKit.Data;
using System.Collections.Generic;
using SoupKit.Support;
using CoreFoundation;
using System.Threading;
using IntentsUI;
using Intents;

namespace SoupChef
{
    public partial class OrderDetailViewController : UITableViewController, IINUIAddVoiceShortcutViewControllerDelegate, IINUIEditVoiceShortcutViewControllerDelegate
    {
        public Order Order { get; private set; }

        OrderDetailTableConfiguration TableConfiguration = new OrderDetailTableConfiguration(OrderDetailTableConfiguration.OrderTypeEnum.New);

        UILabel QuantityLabel;

        UILabel TotalLabel;

        Dictionary<string, string> OptionMap = new Dictionary<string, string>();

        VoiceShortcutDataManager VoiceShortcutDataManager;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            if (TableConfiguration.OrderType == OrderDetailTableConfiguration.OrderTypeEnum.Historical)
            {
                NavigationItem.RightBarButtonItem = null;
            }
            ConfigureTableViewHeader();
        }

        void ConfigureTableViewHeader()
        {
            SoupDetailView.ImageView.Image = UIImage.FromBundle(Order.MenuItem.IconImageName);
            SoupDetailView.TitleLabel.Text = Order.MenuItem.LocalizedString;
            TableView.TableHeaderView = TableViewHeader;
        }

        public void Configure(OrderDetailTableConfiguration tableConfiguration, Order order, VoiceShortcutDataManager voiceShortcutDataManager)
        {
            TableConfiguration = tableConfiguration;
            Order = order;
            VoiceShortcutDataManager = voiceShortcutDataManager;
        }

        partial void PlaceOrder(UIBarButtonItem sender)
        {
            if (Order.Quantity == 0)
            {
                Console.WriteLine("Quantity must be greater than 0 to add to order");
                return;

            }
            PerformSegue("Place Order Segue", this);
        }

        protected void StepperDidChange(object sender, EventArgs args)
        {
            var stepper = sender as UIStepper;
            if (!(stepper is null))
            {
                Order.Quantity = (int)(stepper.Value);
                QuantityLabel.Text = $"{Order.Quantity}";
                UpdateTotalLabel();
            }
        }

        void UpdateTotalLabel()
        {
            if (!(TotalLabel is null))
            {
                TotalLabel.Text = NSNumberFormatterHelper.CurrencyFormatter.StringFromNumber(Order.Total);
            }
        }

        void UpdateVoiceShortcuts()
        {
            var weakThis = new WeakReference<OrderDetailViewController>(this);
            VoiceShortcutDataManager.UpdateVoiceShortcuts(() =>
            {
                var indexPath = NSIndexPath.FromRowSection(0, 3);
                DispatchQueue.MainQueue.DispatchAsync(() =>
                {
                    if (weakThis.TryGetTarget(out var orderDetailViewController))
                    {
                        orderDetailViewController.TableView.ReloadRows(new NSIndexPath[] { indexPath }, UITableViewRowAnimation.Automatic);
                    }
                });
            });
            DismissViewController(true, null);
        }

        #region table view data source
        public override nint NumberOfSections(UITableView tableView)
        {
            return TableConfiguration.Sections.Length;
        }

        public override nint RowsInSection(UITableView tableView, nint section)
        {
            return TableConfiguration.Sections[section].RowCount;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var sectionModel = TableConfiguration.Sections[indexPath.Section];
            var reuseIdentifier = sectionModel.CellReuseIdentifier;
            var cell = TableView.DequeueReusableCell(reuseIdentifier, indexPath);
            Configure(cell, indexPath, sectionModel);
            return cell;
        }

        private void Configure(UITableViewCell cell, NSIndexPath indexPath, OrderDetailTableConfiguration.SectionModel sectionModel)
        {
            switch (sectionModel.Type)
            {
                case OrderDetailTableConfiguration.SectionType.Price:
                    if (!(cell.TextLabel is null))
                    {
                        cell.TextLabel.Text = NSNumberFormatterHelper.CurrencyFormatter.StringFromNumber(Order.MenuItem.Price);
                    }
                    break;
                case OrderDetailTableConfiguration.SectionType.Quantity:
                    var quantityCell = cell as QuantityCell;
                    if (!(quantityCell is null))
                    {
                        if (TableConfiguration.OrderType == OrderDetailTableConfiguration.OrderTypeEnum.New)
                        {
                            QuantityLabel = quantityCell.GetQuantityLabel();
                            UIStepper stepper = quantityCell.GetStepper();
                            if (!(stepper is null))
                            {
                                stepper.AddTarget(StepperDidChange, UIControlEvent.ValueChanged);
                            }
                        }
                        else
                        {
                            quantityCell.GetQuantityLabel().Text = $"{Order.Quantity}";
                            quantityCell.GetStepper().Hidden = true;
                        }
                    }
                    break;
                case OrderDetailTableConfiguration.SectionType.Options:
                    // Maintains a mapping of values to localized values in 
                    // order to help instantiate Order.MenuItemOption later
                    // when an option is selected in the table view
                    var option = new MenuItemOption(MenuItemOption.All[indexPath.Row]);
                    var localizedValue =
                        Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(
                            option.LocalizedString
                        );
                    OptionMap[localizedValue] = option.Value;

                    if (!(cell.TextLabel is null))
                    {
                        cell.TextLabel.Text = localizedValue;
                    }

                    if (TableConfiguration.OrderType == OrderDetailTableConfiguration.OrderTypeEnum.Historical)
                    {
                        cell.Accessory = Order.MenuItemOptions.Contains(option) ? UITableViewCellAccessory.Checkmark : UITableViewCellAccessory.None;
                    }
                    break;
                case OrderDetailTableConfiguration.SectionType.Total:
                    TotalLabel = cell.TextLabel;

                    UpdateTotalLabel();
                    break;
                case OrderDetailTableConfiguration.SectionType.VoiceShortcut:
                    if (!(cell.TextLabel is null))
                    {
                        cell.TextLabel.TextColor = TableView.TintColor;
                        var shortcut = VoiceShortcutDataManager.VoiceShortcutForOrder(Order);
                        if (!(shortcut is null))
                        {
                            cell.TextLabel.Text = $"“{shortcut.InvocationPhrase}”";
                        }
                        else
                        {
                            cell.TextLabel.Text = "Add to Siri";
                        }
                    }
                    break;
                default:
                    return;
            }
        }
        #endregion

        #region table view delegate
        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            if (TableConfiguration.Sections[indexPath.Section].Type == OrderDetailTableConfiguration.SectionType.Options && TableConfiguration.OrderType == OrderDetailTableConfiguration.OrderTypeEnum.New)
            {
                var cell = TableView.CellAt(indexPath);
                if (cell is null) { return; }

                var cellText = cell.TextLabel?.Text;
                if (cellText is null) { return; }

                var optionRawValue = OptionMap[cellText];
                if (optionRawValue is null) { return; }

                var option = new MenuItemOption(optionRawValue);
                if (option is null) { return; }

                if (Order.MenuItemOptions.Contains(option))
                {
                    Order.MenuItemOptions.Remove(option);
                    cell.Accessory = UITableViewCellAccessory.None;
                }
                else
                {
                    Order.MenuItemOptions.Add(option);
                    cell.Accessory = UITableViewCellAccessory.Checkmark;
                }
            }
            else if (TableConfiguration.Sections[indexPath.Section].Type == OrderDetailTableConfiguration.SectionType.VoiceShortcut)
            {
                INVoiceShortcut existingShortcut = VoiceShortcutDataManager?.VoiceShortcutForOrder(Order);
                if (!(existingShortcut is null))
                {
                    var editVoiceShortcutViewController = new INUIEditVoiceShortcutViewController(existingShortcut);
                    editVoiceShortcutViewController.Delegate = this;
                    PresentViewController(editVoiceShortcutViewController, true, null);
                }
                else
                {
                    // Since the app isn't yet managing a voice shortcut for
                    // this order, present the add view controller
                    INShortcut newShortcut = new INShortcut(Order.Intent);
                    if (!(newShortcut is null))
                    {
                        var addVoiceShortcutVC = new INUIAddVoiceShortcutViewController(newShortcut);
                        addVoiceShortcutVC.Delegate = this;
                        PresentViewController(addVoiceShortcutVC, true, null);
                    }
                }
            }
        }
        #endregion

        #region INUIAddVoiceShortcutViewControllerDelegate
        public void DidFinish(INUIAddVoiceShortcutViewController controller, INVoiceShortcut voiceShortcut, NSError error)
        {
            if (!(error is null))
            {
                Console.WriteLine($"error adding voice shortcut", error);
                return;
            }
            UpdateVoiceShortcuts();
        }

        public void DidCancel(INUIAddVoiceShortcutViewController controller)
        {
            DismissViewController(true, null);
        }
        #endregion

        #region INUIEditVoiceShortcutViewControllerDelegate
        public void DidUpdate(INUIEditVoiceShortcutViewController controller, INVoiceShortcut voiceShortcut, NSError error)
        {
            if (!(error is null))
            {
                Console.WriteLine($"error updating voice shortcut", error);
                return;
            }
            UpdateVoiceShortcuts();
        }

        public void DidDelete(INUIEditVoiceShortcutViewController controller, NSUuid deletedVoiceShortcutIdentifier)
        {
            UpdateVoiceShortcuts();
        }

        public void DidCancel(INUIEditVoiceShortcutViewController controller)
        {
            DismissViewController(true, null);
        }
        #endregion

        #region xamarin
        // This constructor is used when Xamarin.iOS needs to create a new
        // managed object for an already-existing native object.
        public OrderDetailViewController(IntPtr handle) : base(handle) { }
        #endregion
    }
}