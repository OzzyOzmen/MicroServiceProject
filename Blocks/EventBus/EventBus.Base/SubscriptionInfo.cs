using System;
namespace EventBus.Base
{
    // We will ensure that the data given to us from outside is keeping inside.
    public class SubscriptionInfo
    {
        // We set this propperty for keeping integration type 
        // Through this type, we will reach its handle method and call the relevant method with using reflaction.
       
        public Type HandlerType { get; }

        // we take handler type as parameters . Thats way we will not need "set;" for our property abow. and deleted dthe "set;"
        public SubscriptionInfo(Type handlerType)
        {
            HandlerType = handlerType ?? throw new ArgumentNullException(nameof(handlerType));
        }

        // We created this static method for returning type with static method not as subscriptionInfo.
        public static SubscriptionInfo Typed(Type handlerType)
        {
            return new SubscriptionInfo(handlerType);
        }
    }
}

