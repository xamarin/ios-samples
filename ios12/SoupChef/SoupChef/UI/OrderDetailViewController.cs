
namespace SoupChef
{
    using Foundation;
    using Intents;
    using IntentsUI;
    using SoupChef.Support;
    using SoupChef.Data;
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using UIKit;

    /// <summary>
    /// This class shows soup order details. It can be configured for two possible order types.
    /// When configured with a 'new' order type, the view controller collects details of a new order.
    /// When configured with a 'historical' order type, the view controller displays details of a previously placed order.
    /// </summary>
    partial class OrderDetailViewController : UITableViewController, 
                                              IINUIAddVoiceShortcutButtonDelegate,
                                              IINUIAddVoiceShortcutViewControllerDelegate,
                                              IINUIEditVoiceShortcutViewControllerDelegate
    {
        private OrderDetailTableConfiguration tableConfiguration = new OrderDetailTableConfiguration(OrderDetailTableConfiguration.OrderTypeEnum.New);

        private Dictionary<string, string> optionMap = new Dictionary<string, string>();

        private UILabel quantityLabel;

        private UILabel totalLabel;

        public OrderDetailViewController(IntPtr handle) : base(handle) { }
       
        public Order Order { get; private set; }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            if (tableConfiguration.OrderType == OrderDetailTableConfiguration.OrderTypeEnum.Historical)
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
            if (tableConfiguration.OrderType ==  OrderDetailTableConfiguration.OrderTypeEnum.Historical)
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
            this.tableConfiguration = tableConfiguration;
            Order = order;
        }

        #region Target Action

        partial void PlaceOrder(UIBarButtonItem sender)
        {
            if (Order.Quantity != 0)
            {
                PerformSegue("Place Order Segue", this);
            }
            else
            {
                Console.WriteLine("Quantity must be greater than 0 to add to order");
            }
        }

        private void StepperDidChange(object sender, EventArgs args)
        {
            if (sender is UIStepper stepper)
            {
                Order.Quantity = (int)(stepper.Value);
                quantityLabel.Text = $"{Order.Quantity}";
                UpdateTotalLabel();
            }
        }

        private void UpdateTotalLabel()
        {
            if (totalLabel != null)
            {
                totalLabel.Text = Order.LocalizedCurrencyValue;
            }
        }

        #endregion

        #region TableView DataSource

        public override string TitleForHeader(UITableView tableView, nint section)
        {
            return tableConfiguration.Sections[(int)section].Type;
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return tableConfiguration.Sections.Count;
        }

        public override nint RowsInSection(UITableView tableView, nint section)
        {
            return tableConfiguration.Sections[(int)section].RowCount;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var sectionModel = tableConfiguration.Sections[indexPath.Section];
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
                    if (cell.TextLabel != null)
                    {
                        cell.TextLabel.Text = NSNumberFormatterHelper.CurrencyFormatter.StringFromNumber(Order.MenuItem.Price);
                    }
                    break;
                case OrderDetailTableConfiguration.SectionType.Quantity:
                    if (cell is QuantityCell quantityCell)
                    {
                        quantityCell.Stepper.ValueChanged -= StepperDidChange;
                        if (tableConfiguration.OrderType == OrderDetailTableConfiguration.OrderTypeEnum.New)
                        {
                            // Save a weak reference to the quantityLabel for quick udpates, later.
                            quantityLabel = quantityCell.QuantityLabel;
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
                    optionMap[localizedValue] = option.ToString();

                    if (cell.TextLabel != null)
                    {
                        cell.TextLabel.Text = localizedValue;
                    }

                    if (tableConfiguration.OrderType == OrderDetailTableConfiguration.OrderTypeEnum.Historical)
                    {
                        cell.Accessory = Order.MenuItemOptions.Contains(option) ? UITableViewCellAccessory.Checkmark : UITableViewCellAccessory.None;
                    }
                    break;
                case OrderDetailTableConfiguration.SectionType.Total:
                    totalLabel = cell.TextLabel;
                    UpdateTotalLabel();
                    break;
            }
        }

        #endregion

        #region TableViewDelegate

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            if (tableConfiguration.Sections[indexPath.Section].Type == OrderDetailTableConfiguration.SectionType.Options && 
                tableConfiguration.OrderType == OrderDetailTableConfiguration.OrderTypeEnum.New)
            {
                var cell = TableView.CellAt(indexPath);
                if (!string.IsNullOrEmpty(cell?.TextLabel?.Text))
                {
                    var optionRawValue = optionMap[cell.TextLabel.Text];
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