using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Configuration;
using System.Dynamic;
using System.Threading;

namespace Subscriber
{
    class Program
    {
        static void Main(string[] args)
        {
            var topic = ConfigurationManager.AppSettings["Topic"];
            var subscription = ConfigurationManager.AppSettings["Subscription"];
            var connectionString = GetConnectionString();
            var namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);

            RegisterSubscription(namespaceManager, topic, subscription);

            var client = SubscriptionClient.CreateFromConnectionString(connectionString, topic, subscription);
            RegisterOnMessageCallback(client);

            Console.WriteLine("Press ESC to stop");
            do
            {
                while (!Console.KeyAvailable)
                {
                    Thread.Sleep(1000);
                }
            } while (Console.ReadKey(true).Key != ConsoleKey.Escape);
        }

        private static void RegisterSubscription(NamespaceManager namespaceManager, string topic, string subscription)
        {
            if (!namespaceManager.SubscriptionExists(topic, subscription))
            {
                namespaceManager.CreateSubscription(topic, subscription);
            }
        }

        private static void RegisterOnMessageCallback(SubscriptionClient client)
        {
            var options = new OnMessageOptions();
            options.AutoComplete = false;
            options.AutoRenewTimeout = TimeSpan.FromMinutes(1);

            client.OnMessage((message) =>
            {
                try
                {
                    // Process message from subscription.
                    Console.WriteLine("Body: " + message.GetBody<string>());
                    Console.WriteLine("MessageID: " + message.MessageId);
                    Console.WriteLine("Message Number: " + message.Properties["MessageNumber"]);

                    // Remove message from subscription.
                    message.Complete();
                }
                catch (Exception)
                {
                    // Indicates a problem, unlock message in subscription.
                    message.Abandon();
                }
            }, options);
        }

        private static string GetConnectionString()
        {
            var json = System.IO.File.ReadAllText(@".\secrets.json");
            var converter = new ExpandoObjectConverter();
            dynamic secrets = JsonConvert.DeserializeObject<ExpandoObject>(json, converter);
            return secrets.ServiceBusConnectionString;
        }
    }
}
