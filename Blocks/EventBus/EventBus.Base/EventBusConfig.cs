using System;
namespace EventBus.Base
{
    // we set our EventBus Config here...
    public class EventBusConfig
    {
        // Tries only 5 times to connect to the RabbitMQ when any connection problem comes out.
        public int ConnectionRetryCound { get; set; } = 5;

        // We will create ques more then one under the DefaultTopicName.And when topicname doesnt exist then equal it with default name as set below.
        public string DefaultTopicNane { get; set; } = "MicroService EventBus";

        // This is connection string.
        public string EventBusConnectionString { get; set; } = string.Empty;

        // It will tell us which service will create a new que on rabbitMq or Azure or MongoDb.
        // And will take name of service
        public string SubscribeClientAppName { get; set; } = string.Empty;

        // We will use it for Trimming EventName
        public string EventNamePrefix { get; set; } = string.Empty;

        // We will use it for Trimming EventName
        public string EventNameSuffix { get; set; } = "IntegrationEvent";

        // It will be our default EventBus and will join to RabbitMq.
        // We will be able to use RabbitMq even not recieve any parameters
        public EventBusType EventBusType { get; set; } = EventBusType.RabbirMQ;

        // The purpose of using "object" is, It will be more elastic about dependencies while connecting to RabbitMQ vs Azure Service or Mongodb.
        public object Connection { get; set; }

        // Delete
        public bool DeleteEventPrefix => !String.IsNullOrEmpty(EventNamePrefix);

        // Delete
        public bool DeleteEventSuffix => !String.IsNullOrEmpty(EventNameSuffix);
    }

    // We set Eventbus Types in enum to equal when we need.
    public enum EventBusType
    {
        RabbirMQ=0,
        AzureServiceBus=1,
        //MongoDb =0
       
    }
}

