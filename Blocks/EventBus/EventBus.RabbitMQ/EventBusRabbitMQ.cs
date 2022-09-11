using System;
using System.Net.Sockets;
using System.Text;
using System.Text.Unicode;
using EventBus.Base;
using EventBus.Base.Events;
using Newtonsoft.Json;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace EventBus.RabbitMQ
{
    public class EventBusRabbitMQ : BaseEventBus
    {

        RabbitMQPersistantConnection persistantConnection;
        private readonly IConnectionFactory connectionFactory;
        private readonly IModel consumerChannel;
        

        public EventBusRabbitMQ(EventBusConfig config, IServiceProvider serviceProvider) : base(config, serviceProvider)
        {
            if (config.Connection != null)
            {
                var connJson = JsonConvert.SerializeObject(config.Connection, new JsonSerializerSettings()
                {
                    // self referencing loop detected for property
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore

                });
                connectionFactory = JsonConvert.DeserializeObject<IConnectionFactory>(connJson);
            }
            else connectionFactory = new ConnectionFactory();


            persistantConnection = new RabbitMQPersistantConnection(connectionFactory,config.ConnectionRetryCount);

            consumerChannel = CreateConsumerChannel();

            subsManager.OnEventRemoved += SubsManager_OnEventRemoved;
        }

        private void SubsManager_OnEventRemoved(object sender, string eventName)
        {
            eventName = ProcessEventName(eventName);

            if (!persistantConnection.IsConnected)
            {
                persistantConnection.TryConnect();
            }
            consumerChannel.QueueBind(queue: GetSubName(eventName),
                   exchange: eventBusConfig.DefaultTopicName,
                   routingKey: eventName);

            if (subsManager.IsEmpty)
            {
                consumerChannel.Close();
            }
        }

        public IModel CreateConsumerChannel()
        {
            if (!persistantConnection.IsConnected)
            {
                persistantConnection.TryConnect();
            }
            var channel = persistantConnection.CreateModel();

            channel.ExchangeDeclare(exchange: eventBusConfig.DefaultTopicName, type: "direct");

            return channel;
        }

        public override void Publish(IntegrationEvent @event)
        {
            if (!persistantConnection.IsConnected)
            {
                persistantConnection.TryConnect();
            }
            var policy = Policy.Handle<BrokerUnreachableException>()
                .Or<SocketException>()
                .WaitAndRetry(eventBusConfig.ConnectionRetryCount, retryAttemp => TimeSpan.FromSeconds(Math.Pow(2, retryAttemp)), (ex, time) =>
                {
                    // log here...
                });
            var eventName = @event.GetType().Name;
            eventName = ProcessEventName(eventName);

            consumerChannel.ExchangeDeclare(exchange: eventBusConfig.DefaultTopicName, type: "direct"); // ensure exchange exist while publishing.

            var message = JsonConvert.SerializeObject(@event);
            var body = Encoding.UTF8.GetBytes(message);

            policy.Execute(() =>
            {
                var properties = consumerChannel.CreateBasicProperties();
                properties.DeliveryMode = 2; //persistant

                consumerChannel.QueueDeclare(queue:GetSubName(eventName),
                    durable:true,
                    exclusive:false,
                    autoDelete:false,
                    arguments:null);

                consumerChannel.BasicPublish(
                    exchange: eventBusConfig.DefaultTopicName,
                    routingKey:eventName,
                    mandatory:true,
                    basicProperties:properties,
                    body:body);
            });
        }

       
        public override void Subscribe<T, Thandler>()
        {
            var eventName = typeof(T).Name;
            eventName = ProcessEventName(eventName);

            if (!subsManager.HasSubscriptionForEvent(eventName))
            {
                if (!persistantConnection.IsConnected)
                {
                    persistantConnection.TryConnect();
                }

                consumerChannel.QueueDeclare(queue:GetSubName(eventName), // ensure queu exist while consuming
                    durable:true,
                    exclusive:false,
                    autoDelete:false,
                    arguments:null);

                consumerChannel.QueueBind(queue: GetSubName(eventName),
                    exchange: eventBusConfig.DefaultTopicName,
                    routingKey:eventName);
            }

            subsManager.AddSubscription<T, Thandler>();
            StartBasicConsume(eventName);

        }

        public override void UnSubscribe<T, Thandler>()
        {
            subsManager.RemoveSubscription<T, Thandler>();
        }

        private void StartBasicConsume(string eventName)
        {
            if (consumerChannel !=null)
            {
                var consumer = new EventingBasicConsumer(consumerChannel);
                consumer.Received += Consumer_Received;

                consumerChannel.BasicConsume(queue: GetSubName(eventName),
                 autoAck:false,
                 consumer:consumer);
            }
        }

        private async void Consumer_Received(object sender, BasicDeliverEventArgs eventArgs)
        {
            var eventName = eventArgs.RoutingKey;
            eventName = ProcessEventName(eventName);
            var message = Encoding.UTF8.GetString(eventArgs.Body.Span);

            try
            {
                await ProcessEvent(eventName,message);
            }
            catch(Exception exception)
            {
                // logging here...
            }

            consumerChannel.BasicAck(eventArgs.DeliveryTag, multiple:false);
        }
    }
}

