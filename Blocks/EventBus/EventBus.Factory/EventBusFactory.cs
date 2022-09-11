using System;
using EventBus.Base;
using EventBus.Base.Abstraction;
using EventBus.AzureServiceBus;
using EventBus.RabbitMQ;

namespace EventBus.Factory
{
    public static class EventBusFactory
    {
        // this is the factory that will deciede sending to azureservice or rabbitMQ
       public static IEventBus Create(EventBusConfig config, IServiceProvider serviceProvider)
        {
            //switch (config.EventBusType)
            //{
            //    case EventBusType.RubbitMQ:
            //        break;
            //    case EventBusType.AzureServiceBus:
            //        break;
            //    case EventBusType.MongoDbServiceBus:
            //        break;
            //    default:
            //        break;
            //}

            return config.EventBusType switch // this is another using type of switch case 
            {
                EventBusType.AzureServiceBus => new EventBusAzureService(config, serviceProvider),
                _ => new EventBusRabbitMQ(config, serviceProvider),
            };
        }
    }
}

