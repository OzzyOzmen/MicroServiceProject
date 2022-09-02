using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using EventBus.Base.Abstraction;
using EventBus.Base.Events;

namespace EventBus.Base.SubscriptionManager
{
    public class InMemoryEventBusSubscriptionManager: IEventBusSubscriptionManager
    {
        // this dictonary will store the handlers.
        private readonly Dictionary<string, List<SubscriptionInfo>> _handlers;

        // this dictonary will store the event typess.
        private readonly List<Type> _eventTypes;

        // 
        public event EventHandler<string> OnEventRemoved;

        // 
        public Func<string,string> eventNameGetter;



        public InMemoryEventBusSubscriptionManager(Func<string,string> eventNameGetter)
        {
            _handlers = new Dictionary<string, List<SubscriptionInfo>>();
            _eventTypes = new List<Type>();
            this.eventNameGetter = eventNameGetter;
        }

        // checked if the handlers has key or not
        public bool IsEmpty => !_handlers.Keys.Any();

        // cleared handlers
        public void Clear() => _handlers.Clear();

        //
        public void AddSubscription<T, Thandler>() where T : IntegrationEvent where Thandler : IIntegrationEventHandler<T>
        {
            //get the eventkey for event name
            var eventName = GetEventKey<T>();

            // and sending to addsub method
            AddSubscription(typeof(Thandler),eventName);

            // id event types does not contains T, then add to insside.
            if (!_eventTypes.Contains(typeof(T)))
            {
                _eventTypes.Add(typeof(T));
            }
        }

        private void AddSubscription(Type handlerType, string eventName)
        {
            //checking if there is a key of current event in dictionary List
            // if no then add the subscripton list to subscribe
            if (!HasSubscriptionForEvent(eventName))
            {
                _handlers.Add(eventName, new List<SubscriptionInfo>());
            }
            // if there is a handler that subscribed with this eventkey
            // and ignore to listen same event more than 1 for the same app
            if (_handlers[eventName].Any(x=>x.HandlerType== handlerType))
            {
                throw new ArgumentException($"Handler Type {handlerType.Name} already registered for'{eventName}'");
            }

            // create new typed and give it to subscription Info
            _handlers[eventName].Add(SubscriptionInfo.Typed(handlerType));
        }

        // get event key
        public string GetEventKey<T>()
        {
            string eventName = typeof(T).Name;
            return eventNameGetter(eventName);
        }

        // get event type by name ( eventName )
        public Type GetEventTypeByName(string eventName) => _eventTypes.SingleOrDefault(x => x.Name == eventName);

        public IEnumerable<SubscriptionInfo> GetHandlerForEvent<T>() where T : IntegrationEvent
        {
            var key = GetEventKey<T>();
            return GetHandlerForEvent(key);
        }

        // return the event handlers and types in dictioanry and could be cast
        public IEnumerable<SubscriptionInfo> GetHandlerForEvent(string eventName) =>_handlers[eventName];
        public bool HasSubscriptionForEvent<T>() where T : IntegrationEvent
        {
            var key = GetEventKey<T>();
            return HasSubscriptionForEvent(key);
        }

        // checking if there is a event name with the current key
        public bool HasSubscriptionForEvent(string eventName) => _handlers.ContainsKey(eventName);

        // if there is a key and then find it and remove it.
        public void RemoveSubscription<T, Thandler>()
            where T : IntegrationEvent
            where Thandler : IIntegrationEventHandler<T>
        {
            var handlertoRemove = FindSubscriptionToRemove<T, Thandler>();
            var eventName = GetEventKey<T>();
            RemoveHandler(eventName, handlertoRemove);
        }

        // finding subscriptions to remove.
        private SubscriptionInfo FindSubscriptionToRemove<T, Thandler>()
            where T : IntegrationEvent
            where Thandler : IIntegrationEventHandler<T>
        {
            var eventName = GetEventKey<T>();
            return FindSubscriptionToRemove(eventName,typeof(Thandler));
        }

        // checking if there is an eventname as current eventname with the same handler type.
        private SubscriptionInfo FindSubscriptionToRemove(string eventName, Type handlerType)
        {
            if (!HasSubscriptionForEvent(eventName))
            {
                return null;
            }

            return _handlers[eventName].SingleOrDefault(x=>x.HandlerType == handlerType);
        }

        // if there is a handler and then find it and remove it.
        private void RemoveHandler(string eventName, SubscriptionInfo subsToRemove)
        {
            if (subsToRemove !=null)
            {
                _handlers[eventName].Remove(subsToRemove);

                if (!_handlers[eventName].Any())
                {
                    _handlers.Remove(eventName);
                    var eventType = _eventTypes.SingleOrDefault(x => x.Name == eventName);
                    if (eventType !=null)
                    {
                        _eventTypes.Remove(eventType);
                    }

                    RaisedOneventRemoved(eventName);
                }

            }
        }

        // if any event in handler deleted or unsubscribed, then announce it to the servives that using it.
        private void RaisedOneventRemoved(string eventName)
        {
            var handler = OnEventRemoved;
            handler?.Invoke(this,eventName);

        }

       
    }
}

