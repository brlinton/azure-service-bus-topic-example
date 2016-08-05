# azure-service-bus-topic-example

Demonstrating publish / subscribe with Azure Service Bus topics and resource templates

## Getting Started

From a fresh clone, you'll need to create a secrets.json file and drop it in the Subscriber project as well as in `App_Data` underneath the MessageGenerator project.  It just needs one value for your service bus connection string:

```
{
  "ServiceBusConnectionString": "Endpoint=sb://{your full connection string with key}"
}
```

You'll also need to run the **Deploy-AzureResourceGroup.ps1** script to stand up the required resources.  Something like this:

```
Login-AzureRmAccount

Push-Location {your script directory}

.\Deploy-AzureResourceGroup.ps1 -ResourceGroupLocation "East US" -ResourceGroupName "azure-service-bus-topic-example"

Pop-Location
```

From there, you'll get a sample website that just drops 10 sample messages on a topic.  Run the Subscriber console app first, which will register a topic subscription.  Then every time you drop the sample messages on the topic, the console app will read them and spit out the contents.  Note that once the subscription is created, any messages created with the Subscriber offline will be read when it starts up.

## Sources and Links
- [How to Use Service Bus Topics and Subscriptions] (https://azure.microsoft.com/en-us/documentation/articles/service-bus-dotnet-how-to-use-topics-subscriptions/)
- [Azure Resource Manager Quickstart Templates for Service Bus Topics](https://github.com/Azure/azure-quickstart-templates/tree/master/101-servicebus-topic-subscription)
