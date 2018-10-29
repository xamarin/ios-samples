/*
 * This class shows soup order details. It can be configured for two possible order types.
 * When configured with a 'new' order type, the view controller collects details of a new order.
 * When configured with a 'historical' order type, the view controller displays details of a previously placed order.
 */

namespace SoupChef
{

    using Foundation;
    using System;
    using UIKit;
    using System.Collections.Generic;
    using IntentsUI;
    using Intents;
    using SoupKit.Data;
    using SoupKit.Support;
    using SoupChef.Support;

    partial class OrderDetailViewController : UITableViewController, 
                                              IINUIAddVoiceShortcutButtonDelegate,
                                              IINUIAddVoiceShortcutViewControllerDelegate,
                                              IINUIEditVoiceShortcutViewControllerDelegate
    {
        public Order Order { get; private set; }

        private OrderDetailTableConfiguration TableConfiguration = new OrderDetailTableConfiguration(OrderDetailTableConfiguration.OrderTypeEnum.New);

        private UILabel QuantityLabel;

        private UILabel TotalLabel;

        Dictionary<string, string> OptionMap = new Dictionary<string, string>();

        public OrderDetailViewController(IntPtr handle) : base(handle) { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            if (TableConfiguration.OrderType == OrderDetailTableConfiguration.OrderTypeEnum.Historical)
            {
                NavigationItem.RightBarButtonItem = null;
            }

            ConfigureTableViewHeader();
            ConfigureTableFooterView();
        }

        private void ConfigureTableViewHeader()
        {
            HeaderImageView.Image = UIImage.FromBundle(Order.MenuItem.IconImageName);
            HeaderImageView.ApplyRoundedCorners();
            HeaderLabel.Text = Order.MenuItem.ItemName;

            TableView.TableHeaderView = TableViewHeader;
        }

        private void ConfigureTableFooterView()
        {
            if (TableConfiguration.OrderType ==  OrderDetailTableConfiguration.OrderTypeEnum.Historical)
             {
                var addShortcutButton = new INUIAddVoiceShortcutButton(INUIAddVoiceShortcutButtonStyle.WhiteOutline);
                addShortcutButton.Shortcut = new INShortcut(Order.Intent);
                addShortcutButton.Delegate = this;

                addShortcutButton.TranslatesAutoresizingMaskIntoConstraints = false;
                TableFooterView.AddSubview(addShortcutButton);
                TableFooterView.CenterXAnchor.ConstraintEqualTo(addShortcutButton.CenterXAnchor).Active = true;
                TableFooterView.CenterYAnchor.ConstraintEqualTo(addShortcutButton.CenterYAnchor).Active = true;

                TableView.TableFooterView = TableFooterView;
            }
        }

        public void Configure(OrderDetailTableConfiguration tableConfiguration, Order order)
        {
            TableConfiguration = tableConfiguration;
            Order = order;
        }

        #region Target Action

        partial void PlaceOrder(UIBarButtonItem sender)
        {
            if (Order.Quantity == 0)
            {
                Console.WriteLine("Quantity must be greater than 0 to add to order");
                return;
            }

            PerformSegue("Place Order Segue", this);
        }

        private void StepperDidChange(object sender, EventArgs args)
        {
            if (sender is UIStepper stepper)
            {
                Order.Quantity = (int)(stepper.Value);
                QuantityLabel.Text = $"{Order.Quantity}";
                UpdateTotalLabel();
            }
        }

        void UpdateTotalLabel()
        {
            if (TotalLabel != null)
            {
                TotalLabel.Text = Order.LocalizedCurrencyValue;
            }
        }

        #endregion

        #region TableView DataSource

        public override nint NumberOfSections(UITableView tableView)
        {
            return TableConfiguration.Sections.Count;
        }

        public override nint RowsInSection(UITableView tableView, nint section)
        {
            return TableConfiguration.Sections[(int)section].RowCount;
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
                    if (cell is QuantityCell quantityCell)
                    {
                        quantityCell.Stepper.ValueChanged -= StepperDidChange;
                        if (TableConfiguration.OrderType == OrderDetailTableConfiguration.OrderTypeEnum.New)
                        {
                            // Save a weak reference to the quantityLabel for quick udpates, later.
                            QuantityLabel = quantityCell.QuantityLabel;
                            quantityCell.Stepper.ValueChanged += StepperDidChange;
                        }
                        else
                        {
                            quantityCell.QuantityLabel.Text = $"{Order.Quantity}";
                            quantityCell.Stepper.Hidden = true;
                        }
                    }
                    break;
                case OrderDetailTableConfiguration.SectionType.Options:
                    // Maintains a mapping of values to localized values in 
                    // order to help instantiate Order.MenuItemOption later
                    // when an option is selected in the table view
                    var option = (MenuItemOption)indexPath.Row;
                    var localizedValue = option.ToString();
                    OptionMap[localizedValue] = option.ToString();

                    if (cell.TextLabel != null)
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
            }
        }

        #endregion

        #region TableViewDelegate

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            if (TableConfiguration.Sections[indexPath.Section].Type == OrderDetailTableConfiguration.SectionType.Options && 
                TableConfiguration.OrderType == OrderDetailTableConfiguration.OrderTypeEnum.New)
            {
                var cell = TableView.CellAt(indexPath);
                if (!string.IsNullOrEmpty(cell?.TextLabel?.Text))
                {
                    var optionRawValue = OptionMap[cell.TextLabel.Text];
                    var option = (MenuItemOption)Enum.Parse(typeof(MenuItemOption), optionRawValue, false);

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
            }
        }
        #endregion

        #region INUIAddVoiceShortcutButtonDelegate

        public void PresentAddVoiceShortcut(INUIAddVoiceShortcutViewController addVoiceShortcutViewController, INUIAddVoiceShortcutButton addVoiceShortcutButton)
        {
            addVoiceShortcutViewController.Delegate = this;
            PresentViewController(addVoiceShortcutViewController, true, null);
        }

        public void PresentEditVoiceShortcut(INUIEditVoiceShortcutViewController editVoiceShortcutViewController, INUIAddVoiceShortcutButton addVoiceShortcutButton)
        {
            editVoiceShortcutViewController.Delegate = this;
            PresentViewController(editVoiceShortcutViewController, true, null);
        }

        #endregion

        #region INUIAddVoiceShortcutViewControllerDelegate

        public void DidFinish(INUIAddVoiceShortcutViewController controller, INVoiceShortcut voiceShortcut, NSError error)
        {
            if (error != null)
            {
                Console.WriteLine($"error adding voice shortcut", error);
            }
            else
            {
                controller.DismissViewController(true, null);
            }
        }

        public void DidCancel(INUIAddVoiceShortcutViewController controller)
        {
            DismissViewController(true, null);
        }

        #endregion

        #region INUIEditVoiceShortcutViewControllerDelegate

        public void DidUpdate(INUIEditVoiceShortcutViewController controller, INVoiceShortcut voiceShortcut, NSError error)
        {
            if (error != null)
            {
                Console.WriteLine($"error updating voice shortcut", error);
            }
            else
            {
                controller.DismissViewController(true, null);
            }
        }

        public void DidDelete(INUIEditVoiceShortcutViewController controller, NSUuid deletedVoiceShortcutIdentifier)
        {
            controller.DismissViewController(true, null);
        }

        public void DidCancel(INUIEditVoiceShortcutViewController controller)
        {
            controller.DismissViewController(true, null);
        }

        #endregion
    }
}