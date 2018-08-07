using System;
using System.Collections.Generic;

using Foundation;
using Intents;
using SoupChef;
using SoupKit;

namespace SoupChefIntents
{
    [Register("IntentHandler")]
    public class IntentHandler : INExtension
    {
        public override NSObject GetHandler(INIntent intent) 
        {
            if (intent is OrderSoupIntent)
            {
                return new OrderSoupIntentHandler();
            }
            throw new Exception("Unhandled intent type: ${intent}");
        }

        protected IntentHandler(IntPtr handle) : base(handle) { }
    }
}
