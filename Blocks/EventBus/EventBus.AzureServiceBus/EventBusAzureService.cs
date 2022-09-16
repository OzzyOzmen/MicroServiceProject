using System;
using System.Data;
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
    // Implemented methods from BaseeventBus
    public class EventBusAzureService : BaseEventBus
    {
        // set variables.
        private ITopicClient topicClient; // comes with Microsoft.Azure.ServiceBus Nudget package
        private ManagementClient managementClient; // comes with Microsoft.Azure.ServiceBus Management Nudget package
        private ILogger logger; // we set it for logging

        // Set constructer with parameters.
        public EventBusAzureService(EventBusConfig config, IServiceProvider serviceProvider) : base(config, serviceProvider)
        {
            logger = serviceProvider.GetService(typeof(ILogger<EventBusAzureService>)) as ILogger<EventBusAzureService>;

            //Created as ServiceBus Singleton and we need management client once. Also set  connectionstring.
            managementClient = new ManagementClient(config.EventBusConnectionString);
            // equals topClient to creating new TopClient.
            topicClient = createTopicClient();
        }

        // created topicClient and checking if its null or closed & closing then create new Topclient with connectionstring and defaultTopicName with policy.
        // P.S we already set DefaultTopicName in configuration class.
        private ITopicClient createTopicClient()
        {
            if (topicClient == null && topicClient.IsClosedOrClosing)
            {
                topicClient = new TopicClient(eventBusConfig.EventBusConnectionString, eventBusConfig.DefaultTopicName, RetryPolicy.Default);
            }

            // Ensure that if topic already exist or created

            if (!managementClient.TopicExistsAsync(eventBusConfig.DefaultTopicName).GetAwaiter().GetResult())
            {
                managementClient.CreateTopicAsync(eventBusConfig.DefaultTopicName).GetAwaiter().GetResult();
            }
            return topicClient;
        }

        public override void Publish(IntegrationEvent @event)
        {

            var eventName = @event.GetType().Name;// example : SomethingCreatedIntegrationEvent

            eventName = ProcessEventName(eventName); // example : trimmed as SomethingCreated with the method we created on config class.

            // convert body in messsage to the json for using it.
            var eventStr = JsonConvert.SerializeObject(@event);

            // encoding the body msg from eventStr.
            var bodyArr = Encoding.UTF8.GetBytes(eventStr);

            // created message for sending it to the topicClient.
            var message = new Message()
            {
                MessageId = Guid.NewGuid().ToString(), // message id must be uniqe thats way we created as Guid.
                Body = bodyArr,
                Label = eventName // eventName set on above.

            };

            topicClient.SendAsync(message).GetAwaiter().GetResult();
        }

        public override void Subscribe<T, Thandler>()
        {
            var eventName = typeof(T).Name; // example : SomethingCreatedIntegrationEvent

            eventName = ProcessEventName(eventName);  // example : trimmed as SomethingCreated with the method we created on config class.


            // if subManager has any subsction for the current event.
            if (!subsManager.HasSubscriptionForEvent(eventName))
            {
                //Created Subscription Clint on Azure side if its not exist.
                var subScriptionClient = CreateSubscriptionClintIfNotExist(eventName);

                //  Registered to Subscription Client Message Handler 
                RegisterSubscriptionClientMessageHandler(subScriptionClient);
            }

            // logged the process
            logger.LogInformation("Subscribing to event {EventName} with {EventHandler}", eventName, typeof(Thandler).Name);

            // adding subscription
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

        // when it handles to message, then notify it.
        private Task ExceptionRecievedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            var ex = exceptionReceivedEventArgs.Exception;
            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;

            logger.LogError(ex, "Error handling message: {ExceptionMessage} - Context: {@ExceptionContext}", ex.Message, context);

            return Task.CompletedTask;
        }

       // Created Subscription Clint
        private SubscriptionClient CreateSubscriptionClient(string eventName)
        {
            return new SubscriptionClient(eventBusConfig.EventBusConnectionString, eventBusConfig.DefaultTopicName, GetSubName(eventName));
        }

        // Created Method if Subscription Clint If Not Exist
        private ISubscriptionClient CreateSubscriptionClintIfNotExist(string eventName)
        {
            var subClient = CreateSubscriptionClient(eventName);

            var exist = managementClient.SubscriptionExistsAsync(eventBusConfig.DefaultTopicName, GetSubName(eventName)).GetAwaiter().GetResult();

            if (!exist)
            {
                managementClient.CreateSubscriptionAsync(eventBusConfig.DefaultTopicName, GetSubName(eventName)).GetAwaiter().GetResult();

                // remove default Rule to use our rule that we set for the subscription.
                RemoveDefaultRule(subClient);
            }

            // and creating the rule with eventname and subclient.
            CreateRuleIfNotExist(ProcessEventName(eventName), subClient);

            return subClient;
        }

        // removed Default Rule 
        private void RemoveDefaultRule(SubscriptionClient subscriptionClient)
        {
            try // try to remove it
            {
                subscriptionClient
                    .RemoveRuleAsync(RuleDescription.DefaultRuleName)
                    .GetAwaiter()
                    .GetResult();
            }
            catch (MessagingEntityAlreadyExistsException) // if its not exist then logging it.
            {
                logger.LogWarning("The messaging entiity{DefaultRuleName} could not be found.", RuleDescription.DefaultRuleName);
            }
        }

        // Created Method if Rule If Not Exist
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
            if (!ruleExist) // if rule is  not exist then create it.
            {
                subscriptionClient.AddRuleAsync(new RuleDescription
                {
                    // used Correlation Filter if its fit for the name ( equals Label.EventName to Name.EventName)
                    Filter = new CorrelationFilter { Label = eventName },
                    Name = eventName
                }).GetAwaiter().GetResult();
            }
        }

        // Created Unsubscribing method.
        public override void UnSubscribe<T, Thandler>()
        {
            var eventName = typeof(T).Name;

            try
            {
                //Subscription will be there but we dont subscripe

                var subscriptionClient = CreateSubscriptionClient(eventName);

                subscriptionClient
                    .RemoveRuleAsync(eventName) //(removing rule for the current subscribe)
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

        //overrided the Dispose method  for disposing the jobs in this class.
        public override void Dispose()
        {
            topicClient.CloseAsync().GetAwaiter().GetResult();
            managementClient.CloseAsync().GetAwaiter().GetResult();
            topicClient = null;
            managementClient = null;
        }
    }
}

