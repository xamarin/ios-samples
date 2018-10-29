
namespace SoupChef
{
    using Foundation;
    using Intents;
    using System;

    [Register("IntentHandler")]
    public class IntentHandler : INExtension
    {
        protected IntentHandler(IntPtr handle) : base(handle) { }

        public override NSObject GetHandler(INIntent intent)
        {
            if(intent is OrderSoupIntent)
            {
                throw new Exception($"Unhandled intent type: {intent}");
            }

            return new OrderSoupIntentHandler();
        }
    }
}