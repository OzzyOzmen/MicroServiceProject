using System;
using EventBus.Base.Abstraction;
using EventBus.Base.SubscriptionManager;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace EventBus.Base.Events
{
    // There will be dual methods for RabbitMQ and service Bus.
    // Also we set orher methods as abstract.
    // We also have to import DependncyInjection.Abstract dll to our project for using CreateScope.
    public abstract class BaseEventBus : IEventBus
    {
        //called Our Interfaces and Config
        // Also we call ServiceProvider to use CreateScope method.
        public readonly IServiceProvider ServiceProvider;
        public readonly IEventBusSubscriptionManager subsManager;
        public EventBusConfig eventBusConfig { get; set; }

        // set constructure with parameters and equaling them with the interfaces and config
        public BaseEventBus(EventBusConfig config, IServiceProvider serviceProvider)
        {
            eventBusConfig = config;
            ServiceProvider = serviceProvider;
            subsManager = new InMemoryEventBusSubscriptionManager(ProcessEventName);
        }

        // Trimmed the hole eventName 
        public virtual string ProcessEventName(string eventName)
        {
            if (eventBusConfig.DeleteEventPrefix)
            {
                eventName = eventName.TrimStart(eventBusConfig.EventNamePrefix.ToArray());
            }
            if (eventBusConfig.DeleteEventSuffix)
            {
                eventName = eventName.TrimStart(eventBusConfig.EventNameSuffix.ToArray());
            }
            return eventName;
        }

        // called subscription name and make the method as virtual to override it.
        public virtual string GetSubName(string eventName)
        {
            return $"{eventBusConfig.SubscribeClientAppName}.{ProcessEventName(eventName)}";
        }

        // we Dispose to empty eventbusConfig and subManager. Also set it as virtual to override it.
        public virtual void Dispose()
        {
            eventBusConfig = null;
            subsManager.Clear();
        }

        // Processing the event.
        // When event name and message comes as parameters, then 
        public async Task<bool> ProcessEvent(string eventName, string message)
        {
            // trimming eventName as above.
            eventName = ProcessEventName(eventName);

            var processed = false;

            // check if we consumed eventName
            if (subsManager.HasSubscriptionForEvent(eventName))
            {
                // Then give us all subscribers
                var subscription = subsManager.GetHandlerForEvent(eventName);

                // creating scope to derive from the same for belows (subs).
                var scope = ServiceProvider.CreateScope();

                // create loop to sending all subs.
                foreach (var subs in subscription)
                {
                    // geting service from sub.HandlerType.
                    var handler = ServiceProvider.GetService(subs.HandlerType);

                    if (handler == null) continue;

                    // called eventName with full and untrimmed to use.
                    var eventType = subsManager.GetEventTypeByName($"{eventBusConfig.EventNamePrefix}{eventName}{eventBusConfig.EventNameSuffix}");

                    //serializing message 
                    var integrationEvent = JsonConvert.DeserializeObject(message, eventType);

                    // integration event.
                    var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);

                    // reach to handle method to invoke with reflection.
                    await (Task)concreteType.GetMethod("Handle").Invoke(handler, new object[] { integrationEvent });
                }

                processed = true;
            }
            return processed;
        }

        // these 3 abstracts method will be for only message brokers.
        public abstract void Publish(IntegrationEvent @event);


        public abstract void Subscribe<T, Thandler>() where T : IntegrationEvent where Thandler : IIntegrationEventHandler<T>;


        public abstract void UnSubscribe<T, Thandler>() where T : IntegrationEvent where Thandler : IIntegrationEventHandler<T>;

       
    }
}

