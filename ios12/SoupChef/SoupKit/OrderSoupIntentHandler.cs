/*
See LICENSE folder for this sample’s licensing information.

Abstract:
Intent handler for OrderSoupIntents delivered by the system.
*/

using System;
//using SoupChef;
using SoupKit.Data;

namespace SoupKit
{
    public class OrderSoupIntentHandler //: OrderSoupIntentHandling
    {
        //override public void ConfirmOrderSoup(OrderSoupIntent intent, Action<OrderSoupIntentResponse> completion)
        //{
        //    // The confirm phase provides an opportunity for you to perform any 
        //    // final validation of the intent parameters and to verify that any 
        //    // needed services are available. You might confirm that you can 
        //    // communicate with your company’s server.
        //    var soupMenuManager = new SoupMenuManager();

        //    var soup = intent.Soup;
        //    if (soup is null)
        //    {
        //        completion(new OrderSoupIntentResponse(OrderSoupIntentResponseCode.Failure, null));
        //        return;
        //    }

        //    var identifier = soup.Identifier;
        //    if (identifier is null)
        //    {
        //        completion(new OrderSoupIntentResponse(OrderSoupIntentResponseCode.Failure, null));
        //        return;
        //    }

        //    var menuItem = soupMenuManager.FindItem(identifier);
        //    if (menuItem is null)
        //    {
        //        completion(new OrderSoupIntentResponse(OrderSoupIntentResponseCode.Failure, null));
        //        return;
        //    }

        //    if (!menuItem.IsAvailable)
        //    {
        //        // Here's an example of how to use a custom response for a 
        //        // failure case when a particular soup item is unavailable.
        //        completion(OrderSoupIntentResponse.FailureSoupUnavailableIntentResponseWithSoup(soup));
        //        return;
        //    }

        //    // Once the intent is validated, indicate that the intent is ready 
        //    // to handle.
        //    completion(new OrderSoupIntentResponse(OrderSoupIntentResponseCode.Ready, null));
        //}

        //public override void HandleOrderSoup(OrderSoupIntent intent, Action<OrderSoupIntentResponse> completion)
        //{
        //    var soup = intent.Soup;
        //    if (soup is null)
        //    {
        //        completion(new OrderSoupIntentResponse(OrderSoupIntentResponseCode.Failure, null));
        //        return;
        //    }

        //    var order = Order.FromOrderSoupIntent(intent);
        //    if (order is null)
        //    {
        //        completion(new OrderSoupIntentResponse(OrderSoupIntentResponseCode.Failure, null));
        //        return;
        //    }

        //    // The handle method is also an appropriate place to handle payment 
        //    // via Apple Pay. A declined payment is another example of a 
        //    // failure case that could take advantage of a custom response.

        //    // Place the soup order via the order manager.
        //    var orderManager = new SoupOrderDataManager();
        //    orderManager.PlaceOrder(order);

        //    // For the success case, we want to indicate a wait time to the 
        //    // user so that they know when their soup order will be ready.
        //    // This sample uses a hardcoded value, but your implementation could 
        //    // use a time interval returned by your server.
        //    completion(OrderSoupIntentResponse.SuccessIntentResponseWithSoup(soup, 10));
        //}
    }
}
