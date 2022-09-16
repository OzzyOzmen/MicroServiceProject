using System;
using System.Net.Sockets;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace EventBus.RabbitMQ
{
    // crerated persistant connection and implemented by IDisposable.
    public class RabbitMQPersistantConnection : IDisposable
    {
        // add rabbitMQ client nudget package.
        // if the connection open or closed
        private readonly IConnectionFactory connectionFactory;

        // create retry count property
        private readonly int retryCount;

        // created connection factory with IConnection.
        private IConnection connection;

        // created lock object for multi thread 
        private object lock_object = new object();

        private bool _dispossed;

        // getting paramters from const and set retry count
        public RabbitMQPersistantConnection(IConnectionFactory connectionFactory, int retryCount = 5)
        {
            this.connectionFactory = connectionFactory;
            this.retryCount = retryCount;
        }


        // if thconnection not null or is WaitHandleCannotBeOpenedException then return true
        public bool IsConnected => connection != null && connection.IsOpen;

        // Created CreateModel method
        public IModel CreateModel()
        {
            // then return the model
            return connection.CreateModel();
        }

        // Created Dispose Method
        public void Dispose()
        {
            _dispossed = true;
            connection.Dispose();
        }

        // trying the connect 
        public bool TryConnect()
        {
            // set lock object for multi thread 
            lock (lock_object)
            {
                // use netsocket nudget Package and polly package.
                // This will perform a reTry mechanism for us.
                var policy = Policy.Handle<SocketException>()
                    .Or<BrokerUnreachableException>()
                    .WaitAndRetry(retryCount, retryAttemp => TimeSpan.FromSeconds(Math.Pow(2, retryAttemp)),
                    (ex, time) => { }
                );

                // execute the policy
                policy.Execute(() => { connection = connectionFactory.CreateConnection(); });

            }

            // if its already connected then return true else return false
            if (IsConnected)
            {
                // handle the events to make it works
                connection.ConnectionShutdown += Connection_ConnectionShutdown;
                connection.CallbackException += Connection_CallbackException;
                connection.ConnectionBlocked += Connection_ConnectionBlocked;

                // log here...

                return true;
            }
            return false;
        }


        private void Connection_ConnectionBlocked(object? sender, global::RabbitMQ.Client.Events.ConnectionBlockedEventArgs e)
        {
            // log here...
            if (_dispossed) return;
            TryConnect();
        }

        private void Connection_CallbackException(object? sender, global::RabbitMQ.Client.Events.CallbackExceptionEventArgs e)
        {
            // log here...
            if (_dispossed) return;
            TryConnect();
        }

        private void Connection_ConnectionShutdown(object? sender, ShutdownEventArgs e)
        {
            // log here...
            if (_dispossed) return;
            TryConnect();
        }
    }
}

