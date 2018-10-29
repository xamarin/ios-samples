using System;
using CoreGraphics;
using Foundation;
using Intents;
using IntentsUI;
using UIKit;
using CoreFoundation;

namespace SoupChefIntentsUI
{
    public partial class IntentViewController : UIViewController, IINUIHostedViewControlling
    {
        SoupMenuManager MenuManager { get; } = new SoupMenuManager();

        [Export("configureViewForParameters:ofInteraction:interactiveBehavior:context:completion:")]
        public void ConfigureView(
            NSSet<INParameter> parameters,
            INInteraction interaction,
            INUIInteractiveBehavior interactiveBehavior,
            INUIHostedViewContext context,
            INUIHostedViewControllingConfigureViewHandler completion)
        {

            var intent = interaction.Intent as OrderSoupIntent;
            if (intent is null)
            {
                completion(false, new NSSet<INParameter>(), CGSize.Empty);
            }

            var order = Order.FromOrderSoupIntent(intent);

            foreach (var view in View.Subviews)
            {
                view.RemoveFromSuperview();
            }

            // Different UIs can be displayed depending if the intent is in the 
            // confirmation phase or the handle phase.
            var desiredSize = CGSize.Empty;

            if (interaction.IntentHandlingStatus == INIntentHandlingStatus.Ready)
            {
                desiredSize = DisplayInvoice(order, intent);
            }
            else if( interaction.IntentHandlingStatus == INIntentHandlingStatus.Success)
            {
                var response = interaction.IntentResponse as OrderSoupIntentResponse;
                if (!(response is null))
                {
                    desiredSize = DisplayOrderConfirmation(order, intent, response);
                }
            }

            completion(true, parameters, desiredSize);
        }

        // This method will not be called if ConfigureView is implemented,
        // according to: https://developer.apple.com/documentation/sirikit/inuihostedviewcontrolling/1649168-configurewithinteraction
        [Export("configureWithInteraction:context:completion:")]
        public void Configure(INInteraction interaction, INUIHostedViewContext context, Action<CGSize> completion)
        {
            throw new NotImplementedException();
        }

        CGSize DisplayInvoice(Order order, OrderSoupIntent intent)
        {
            invoiceView.ItemNameLabel.Text = order.MenuItem.LocalizedString;
            invoiceView.TotalPriceLabel.Text = order.LocalizedCurrencyValue;
            invoiceView.UnitPriceLabel.Text = $"{order.Quantity} @ {order.MenuItem.LocalizedCurrencyValue}";

            var intentImage = intent.GetImage("soup");
            if (!(intentImage is null))
            {
                var weakThis = new WeakReference<IntentViewController>(this);
                intentImage.FetchImage((image) =>
                {
                    DispatchQueue.MainQueue.DispatchAsync(() =>
                    {
                        if (weakThis.TryGetTarget(out var intentViewController))
                        {
                            intentViewController.invoiceView.ImageView.Image = image;
                        }
                    });
                });
            }

            var optionText = (!(intent.Options is null)) ? order.LocalizedOptionString : "";

            invoiceView.OptionsLabel.Text = optionText;

            View.AddSubview(invoiceView);

            var width = this.ExtensionContext?.GetHostedViewMaximumAllowedSize().Width ?? 320;
            var frame = new CGRect(0, 0, width, 170);
            invoiceView.Frame = frame;

            return frame.Size;
        }

        CGSize DisplayOrderConfirmation(Order order, OrderSoupIntent intent, OrderSoupIntentResponse response)
        {
            confirmationView.ItemNameLabel.Text = order.MenuItem.LocalizedString;
            confirmationView.TotalPriceLabel.Text = order.LocalizedCurrencyValue;
            confirmationView.ImageView.Layer.CornerRadius = 8;
            var waitTime = response.WaitTime;
            if (!(waitTime is null))
            {
                confirmationView.TimeLabel.Text = $"{waitTime} Minutes";
            }

            var intentImage = intent.GetImage("soup");
            if (!(intentImage is null))
            {
                var weakThis = new WeakReference<IntentViewController>(this);
                intentImage.FetchImage((image) =>
                {
                    DispatchQueue.MainQueue.DispatchAsync(() =>
                    {
                        if (weakThis.TryGetTarget(out var intentViewController))
                        {
                            // NOTE: In the original Swift-based sample,
                            // the original code sets this image on the
                            // invoice view, which seems incorrect.
                            intentViewController.confirmationView.ImageView.Image = image;
                        }
                    });
                });
            }

            View.AddSubview(confirmationView);

            var width = this.ExtensionContext?.GetHostedViewMaximumAllowedSize().Width ?? 320;
            var frame = new CGRect(0, 0, width, 170);
            confirmationView.Frame = frame;

            return frame.Size;
        }

        #region xamarin
        // This constructor is used when Xamarin.iOS needs to create a new
        // managed object for an already-existing native object.
        public IntentViewController(IntPtr handle) : base(handle) { }
        #endregion
    }
}
