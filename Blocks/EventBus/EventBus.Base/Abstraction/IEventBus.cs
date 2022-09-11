using System;
using System.Reflection.Metadata;
using EventBus.Base.Events;

namespace EventBus.Base.Abstraction
{
    // We set this eventbus for noticing which events that our apps, services and microservices are tracking.
    // Also we will create it for Azure and RabbitMQ both as Azure eventbus and RabbitMQbus.
    // And these both busses will use this interface for implementation.
   // there will be only these 3 methods below
    public interface IEventBus
    {
       // Services will use this method when throwing an event and event will be type of integrationevent. There for we set integration event as Parameter in method.
        void Publish(IntegrationEvent @event);

        // this method will trigger the Handle method in IIntegrationevent when subscription goes 
        void Subscribe<T, Thandler>() where T:IntegrationEvent where Thandler:IIntegrationEventHandler<T>;

        // this method will trigger the Handle method in IIntegrationevent when unsubscription goes
        void UnSubscribe<T, Thandler>() where T : IntegrationEvent where Thandler : IIntegrationEventHandler<T>;
    }
}

