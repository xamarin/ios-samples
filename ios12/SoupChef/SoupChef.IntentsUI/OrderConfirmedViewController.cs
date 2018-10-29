
using CoreFoundation;
using Foundation;
using SoupChef.Data;
using SoupChef.Support;
using UIKit;

namespace SoupChef
{
    /// <summary>
    /// A view controller that confirms an order was placed.
    /// </summary>
    internal class OrderConfirmedViewController : UIViewController
    {
        private OrderSoupIntentResponse intentResponse;

        private OrderSoupIntent intent;

        //@IBOutlet var confirmationView: OrderConfirmedView!
        [Outlet]
        OrderConfirmedView confirmationView { get; set; }

        public OrderConfirmedViewController(OrderSoupIntent soupIntent, OrderSoupIntentResponse response) : base("OrderConfirmedView", NSBundle.MainBundle) 
        {
            intent = soupIntent;
            intentResponse = response;
            //super.init(nibName: "OrderConfirmedView", bundle: Bundle(for: OrderConfirmedViewController.self))
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            confirmationView = View as OrderConfirmedView;

            var order = Order.FromOrderSoupIntent(intent);
            if(order != null)
            {
                confirmationView.ItemNameLabel.Text = order.MenuItem.ItemName;
                confirmationView.ImageView.ApplyRoundedCorners();
                if (intentResponse.WaitTime != null)
                {
                    confirmationView.TimeLabel.Text = intentResponse.WaitTime;
                }

                var intentImage = intent.GetImage("OrderSoupIntent.Soup");
                intentImage?.FetchImage(image => DispatchQueue.MainQueue.DispatchAsync(() => confirmationView.ImageView.Image = image));
            }
        }
    }
}