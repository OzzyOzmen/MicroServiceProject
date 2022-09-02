using System;
using EventBus.Base.Events;

namespace EventBus.Base.Abstraction
{
    public interface IEventBusSubscriptionManager
    {
        // are we listening any event for subscription?
        bool IsEmpty { get;}

        // when event removed than we created event to  trigger to this method
        event EventHandler<string> OnEventRemoved;

        // adding Subsription
        void AddSubscription<T, Thandler>() where T : IntegrationEvent where Thandler : IIntegrationEventHandler<T>;

        //removing Subsription
        void RemoveSubscription<T, Thandler>() where Thandler : IIntegrationEventHandler<T> where T : IntegrationEvent;

        // if we recieve event from outside then we will controll if its subscription event or not as generic type
        bool HasSubscriptionForEvent<T>() where T : IntegrationEvent;

        // if we recieve eventName from outside then we will controll if its subscription event or not
        bool HasSubscriptionForEvent(string eventName);

        // when the eventname throwed, then we will send back integration handle type
        Type GetEventTypeByName(string eventName);

        // clearing all Subscription
        void Clear();

        // We will be able to return all subscriptions and handles of an event that comes to this list as generic types.
        IEnumerable<SubscriptionInfo> GetHandlerForEvent<T>() where T : IntegrationEvent;

        //We will be able to return all subscriptions and handles of an event that comes as eventName
        IEnumerable<SubscriptionInfo> GetHandlerForEvent(string eventName);

        // All event will have their name as uniq and this key will be for  the intergrationevent. And will return routşng key.
        string GetEventKey<T>();

    }
}

