using System;
using EventBus.Base.Events;


namespace EventBus.Base.Abstraction
{
    // We will use this interface to handle these events.
    // TIntegration event must be type of Integrationevent
    // When we create Integrationeventhandler so we will use this interface
    // And we will send the integrationevent the handle method and we will implementation in it.
    public interface IIntegrationEventHandler<TIntegrationEvent> : IntegrationEventHandler where TIntegrationEvent : IntegrationEvent
    {
        // handle method
        Task Handle(TIntegrationEvent @event);
    }

    public interface IntegrationEventHandler
    {

    }

}

