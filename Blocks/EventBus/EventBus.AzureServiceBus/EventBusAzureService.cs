using System;
using System.Reflection;
using System.Text;
using EventBus.Base;
using EventBus.Base.Events;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace EventBus.AzureServiceBus
{
    public class EventBusAzureService : BaseEventBus
    {
        private ITopicClient topicClient;
        private ManagementClient managementClient;
        private ILogger logger;

        public EventBusAzureService(EventBusConfig config, IServiceProvider serviceProvider) : base(config, serviceProvider)
        {
            logger = serviceProvider.GetService(typeof(ILogger<EventBusAzureService>)) as ILogger<EventBusAzureService>;
            managementClient = new ManagementClient(config.EventBusConnectionString);
            topicClient = createTopicClient();
        }

        private ITopicClient createTopicClient()
        {
            if (topicClient == null && topicClient.IsClosedOrClosing)
            {
                topicClient = new TopicClient(eventBusConfig.EventBusConnectionString, eventBusConfig.DefaultTopicName, RetryPolicy.Default);
            }

            // Ensure that topic already exist

            if (!managementClient.TopicExistsAsync(eventBusConfig.DefaultTopicName).GetAwaiter().GetResult())
            {
                managementClient.CreateTopicAsync(eventBusConfig.DefaultTopicName).GetAwaiter().GetResult();
            }
            return topicClient;
        }

        public override void Publish(IntegrationEvent @event)
        {
            var eventName = @event.GetType().Name;// example : SomethingCreatedIntegrationEvent

            eventName = ProcessEventName(eventName); // example : trimmed as SomethingCreated

            var eventStr = JsonConvert.SerializeObject(@event);
            var bodyArr = Encoding.UTF8.GetBytes(eventStr);

            var message = new Message()
            {
                MessageId = Guid.NewGuid().ToString(),
                Body = bodyArr,
                Label = eventName

            };

            topicClient.SendAsync(message).GetAwaiter().GetResult();
        }

        public override void Subscribe<T, Thandler>()
        {
            var eventName = typeof(T).Name;
            eventName = ProcessEventName(eventName);

            if (!subsManager.HasSubscriptionForEvent(eventName))
            {
                var subScriptionClient = CreateSubscriptionClintIfNotExist(eventName);

                RegisterSubscriptionClientMessageHandler(subScriptionClient);
            }

            logger.LogInformation("Subscribing to event {EventName} with {EventHandler}", eventName, typeof(Thandler).Name);
            subsManager.AddSubscription<T, Thandler>();

        }

        private void RegisterSubscriptionClientMessageHandler(ISubscriptionClient subscriptionClient)
        {
            subscriptionClient.RegisterMessageHandler(

                async (message, token) =>
                {
                    var eventName = $"{message.Label}";
                    var messageData = Encoding.UTF8.GetString(message.Body);

                    //Complate the message for let it not recieved again.
                    if (await ProcessEvent(ProcessEventName(eventName), messageData))
                    {
                        await subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);
                    }
                },
                new MessageHandlerOptions(ExceptionRecievedHandler) { MaxConcurrentCalls = 10, AutoComplete = false });
        }

        private Task ExceptionRecievedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            var ex = exceptionReceivedEventArgs.Exception;
            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;

            logger.LogError(ex, "Error handling message: {ExceptionMessage} - Context: {@ExceptionContext}", ex.Message, context);

            return Task.CompletedTask;
        }

        private SubscriptionClient CreateSubscriptionClient(string eventName)
        {
            return new SubscriptionClient(eventBusConfig.EventBusConnectionString, eventBusConfig.DefaultTopicName, GetSubName(eventName));
        }

        private ISubscriptionClient CreateSubscriptionClintIfNotExist(string eventName)
        {
            var subClient = CreateSubscriptionClient(eventName);

            var exist = managementClient.SubscriptionExistsAsync(eventBusConfig.DefaultTopicName, GetSubName(eventName)).GetAwaiter().GetResult();

            if (!exist)
            {
                managementClient.CreateSubscriptionAsync(eventBusConfig.DefaultTopicName, GetSubName(eventName)).GetAwaiter().GetResult();
                RemoveDefaultRule(subClient);
            }

            CreateRuleIfNotExist(ProcessEventName(eventName), subClient);

            return subClient;
        }

        private void RemoveDefaultRule(SubscriptionClient subscriptionClient)
        {
            try
            {
                subscriptionClient
                    .RemoveRuleAsync(RuleDescription.DefaultRuleName)
                    .GetAwaiter()
                    .GetResult();
            }
            catch (MessagingEntityAlreadyExistsException)
            {
                logger.LogWarning("The messaging entiity{DefaultRuleName} could not be found.", RuleDescription.DefaultRuleName);
            }
        }

        private void CreateRuleIfNotExist(string eventName, ISubscriptionClient subscriptionClient)
        {
            bool ruleExist;

            try
            {
                var rule = managementClient.GetRuleAsync(eventBusConfig.DefaultTopicName, eventName, eventName).GetAwaiter().GetResult();
                ruleExist = rule != null;
            }
            catch (MessagingEntityNotFoundException)
            {
                // Azure Management Client doesnt have ruleExist method
                ruleExist = false;
            }
            if (!ruleExist)
            {
                subscriptionClient.AddRuleAsync(new RuleDescription
                {
                    Filter = new CorrelationFilter { Label = eventName },
                    Name = eventName
                }).GetAwaiter().GetResult();
            }
        }

        public override void UnSubscribe<T, Thandler>()
        {
            var eventName = typeof(T).Name;

            try
            {
                //Subscription will be there but we dont subscripe

                var subscriptionClient = CreateSubscriptionClient(eventName);

                subscriptionClient
                    .RemoveRuleAsync(eventName)
                    .GetAwaiter()
                    .GetResult();
            }
            catch (MessagingEntityNotFoundException)
            {
                logger.LogWarning("The messaging entiity{EventName} could not be found.", eventName);
            }
            logger.LogInformation("Unsubscribing from event {EventName}", eventName);

            subsManager.RemoveSubscription<T, Thandler>();
        }

        public override void Dispose()
        {
            topicClient.CloseAsync().GetAwaiter().GetResult();
            managementClient.CloseAsync().GetAwaiter().GetResult();
            topicClient = null;
            managementClient = null;
        }
    }
}

