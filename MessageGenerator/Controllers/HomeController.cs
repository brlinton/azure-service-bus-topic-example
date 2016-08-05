using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Microsoft.ServiceBus;
using System.Dynamic;
using System.Configuration;
using Microsoft.ServiceBus.Messaging;

namespace MessageGenerator.Controllers
{
    public class HomeController : Controller
    {
        private readonly string _topic;

        public HomeController()
        {
            _topic = ConfigurationManager.AppSettings["TopicName"];
        }

        [HttpGet]
        public ActionResult Home()
        {
            MakeSureTopicExists();
            return View();
        }

        [HttpPost]
        public ViewResult Home(string id)
        {
            var connectionString = GetConnectionString();
            var client = TopicClient.CreateFromConnectionString(connectionString, _topic);

            for (var i = 0; i < 10; i++)
            {
                // Create message, passing a string message for the body.
                var message = new BrokeredMessage("Test message " + i);

                // Set additional custom app-specific property.
                message.Properties["MessageNumber"] = i;

                // Send message to the topic.
                client.Send(message);
            }

            return View();
        }

        private string GetConnectionString()
        {
            var json = System.IO.File.ReadAllText(Server.MapPath(@"~/App_Data/secrets.json"));
            var converter = new ExpandoObjectConverter();
            dynamic secrets = JsonConvert.DeserializeObject<ExpandoObject>(json, converter);
            return secrets.ServiceBusConnectionString;
        }

        private void MakeSureTopicExists()
        {
            var connectionString = GetConnectionString();
            var namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);

            if (!namespaceManager.TopicExists(_topic))
            {
                namespaceManager.CreateTopic(_topic);
            }
        }
    }
}