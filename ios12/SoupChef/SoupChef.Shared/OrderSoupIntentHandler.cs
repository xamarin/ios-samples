/*
See LICENSE folder for this sample’s licensing information.

Abstract:
Intent handler for OrderSoupIntents delivered by the system.
*/

using System;
using Foundation;
using ObjCRuntime;
using SoupChef;
using SoupChef.Data;
using SoupChef.Support;

namespace SoupChef
{
    internal class OrderSoupIntentHandler : BaseWrapper, IOrderSoupIntentHandling
    {
        [Preserve(Conditional = true)]
        public OrderSoupIntentHandler(IntPtr handle, bool owns) : base(handle, owns) { }

        [Export("confirmOrderSoup:completion:")]
        void ConfirmOrderSoup(OrderSoupIntent intent, Action<OrderSoupIntentResponse> completion)
        {
            /*
             * The confirm phase provides an opportunity for you to perform any final validation of the intent parameters and to
             * verify that any needed services are available. You might confirm that you can communicate with your company’s server
             */
            var soupMenuManager = new SoupMenuManager();

            var soup = intent.Soup;
            if (intent.Soup != null && !string.IsNullOrEmpty(intent.Soup.Identifier))
            {
                var menuItem = soupMenuManager.FindItem(intent.Soup.Identifier);
                if (menuItem == null)
                {
                    completion(new OrderSoupIntentResponse(OrderSoupIntentResponseCode.Failure, null));
                    return;
                }
                else
                {
                    if (!menuItem.IsAvailable)
                    {
                        //  Here's an example of how to use a custom response for a failure case when a particular soup item is unavailable.
                        completion(OrderSoupIntentResponse.FailureOutOfStockIntentResponseWithSoup(soup));
                        return;
                    }
                }
            }

            // Once the intent is validated, indicate that the intent is ready to handle.
            completion(new OrderSoupIntentResponse(OrderSoupIntentResponseCode.Ready, null));
        }

        void IOrderSoupIntentHandling.HandleOrderSoup(OrderSoupIntent intent, OrderSoupResponseCallback response)
        {
            var soup = intent.Soup;
            if (soup == null)
            {
                response(new OrderSoupIntentResponse(OrderSoupIntentResponseCode.Failure, null));
            }
            else
            {
                var order = Order.FromOrderSoupIntent(intent);
                if (order == null)
                {
                    response(new OrderSoupIntentResponse(OrderSoupIntentResponseCode.Failure, null));
                }
                else
                {
                    //  The handle method is also an appropriate place to handle payment via Apple Pay.
                    //  A declined payment is another example of a failure case that could take advantage of a custom response.

                    //  Place the soup order via the order manager.
                    var orderManager = new SoupOrderDataManager();
                    orderManager.PlaceOrder(order);

                    //  For the success case, we want to indicate a wait time to the user so that they know when their soup order will be ready.
                    //  Ths sample uses a hardcoded value, but your implementation could use a time returned by your server.
                    var orderDate = new NSDate();
                    var readyDate = NSDate.FromTimeIntervalSinceNow(10 * 60); // 10 minutes

                    var userActivity = new NSUserActivity(NSUserActivityHelper.OrderCompleteActivityType);
                    userActivity.AddUserInfoEntries(NSDictionary.FromObjectAndKey(new NSString(order.Identifier.ToString()), 
                                                                                  new NSString(NSUserActivityHelper.ActivityKeys.OrderId)));

                    var formatter = new NSDateComponentsFormatter();
                    formatter.UnitsStyle = NSDateComponentsFormatterUnitsStyle.Full;

                    var formattedWaitTime = formatter.StringFromDate(orderDate, readyDate);
                    if (!string.IsNullOrWhiteSpace(formattedWaitTime))
                    {
                        var response2 = OrderSoupIntentResponse.SuccessIntentResponseWithSoup(soup, new NSString(formattedWaitTime));
                        response2.UserActivity = userActivity;

                        response(response2);
                    }
                    else
                    {
                        // A fallback success code with a less specific message string
                        var response2 = OrderSoupIntentResponse.SuccessReadySoonIntentResponseWithSoup(soup);
                        response2.UserActivity = userActivity;

                        response(response2);
                    }
                }
            }
        }
    }
}