
namespace SoupChef
{
    using CoreFoundation;
    using Foundation;
    using IntentsUI;
    using SoupChef.Data;
    using SoupChef.Support;
    using UIKit;

    /// <summary>
    /// A view that lists the order invoice.
    /// </summary>
    internal class InvoiceViewController : UIViewController
    {
        private OrderSoupIntent intent;

        //@IBOutlet weak var invoiceView: InvoiceView!
        [Outlet]
        InvoiceView invoiceView { get; set; }

        public InvoiceViewController(OrderSoupIntent soupIntent) : base("InvoiceView", NSBundle.MainBundle) 
        {
            intent = soupIntent;
            //super.init(nibName: "InvoiceView", bundle: Bundle(for: InvoiceViewController.self))
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var order = Order.FromOrderSoupIntent(intent);
            if(order != null)
            {
                invoiceView.ItemNameLabel.Text = order.MenuItem.ItemName;
                invoiceView.ImageView.ApplyRoundedCorners();
                invoiceView.TotalPriceLabel.Text = order.LocalizedCurrencyValue;
                invoiceView.UnitPriceLabel.Text = $"{order.Quantity} @ {order.MenuItem.LocalizedCurrencyValue}";

                var intentImage = intent.GetImage("OrderSoupIntent.Soup");
                intentImage?.FetchImage((image) => DispatchQueue.MainQueue.DispatchAsync(() => this.invoiceView.ImageView.Image = image));

                var flattenedOptions = string.Join(',', order.MenuItemOptions);
                invoiceView.OptionsLabel.Text = flattenedOptions;
            }
        }
    }
}