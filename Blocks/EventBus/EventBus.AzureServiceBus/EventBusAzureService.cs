using System;
using EventBus.Base;
using EventBus.Base.Events;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;

namespace EventBus.AzureServiceBus
{
    public class EventBusAzureService : BaseEventBus
    {
        private ITopicClient topicClient;
        private ManagementClient managementClient;

        public EventBusAzureService(EventBusConfig config, IServiceProvider serviceProvider) : base(config, serviceProvider)
        {
            managementClient = new ManagementClient(config.EventBusConnectionString);
        }

        private ITopicClient createTopicClient()
        {
            if (topicClient == null && topicClient.IsClosedOrClosing)
            {
                topicClient = new TopicClient(eventBusConfig.EventBusConnectionString, eventBusConfig.DefaultTopicName);
            }


        }

        public override void Publish(IntegrationEvent @event)
        {
            throw new NotImplementedException();
        }

        public override void Subscribe<T, Thandler>()
        {
            throw new NotImplementedException();
        }

        public override void UnSubscribe<T, Thandler>()
        {
            throw new NotImplementedException();
        }
    }
}

