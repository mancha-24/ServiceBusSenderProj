using Azure.Messaging.ServiceBus.Administration;

namespace ServiceBusSenderProj
{
    public class TopicInitializator
    {
        public TopicInitializator(string connectionString)
        {
            Task.Run(async () =>
            {
                await Initialize(connectionString);
            }).GetAwaiter().GetResult();
        }
        
        private async Task Initialize(string connectionString)
        {
            ServiceBusAdministrationClient adminClient;

            adminClient = new ServiceBusAdministrationClient(connectionString);

            string topicName = "topicfiltersampletopic";
            try
            {
                await CreateTopic(topicName, adminClient);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while creating the topic. error: {ex.Message}");
                Console.ReadLine();
                throw;
            }
                        
            // names of subscriptions to the topic
            string subscriptionAllOrders = "AllOrders";
            string subscriptionColorBlueSize10Orders = "ColorBlueSize10Orders";
            string subscriptionColorRed = "ColorRed";
            string subscriptionHighPriorityRedOrders = "HighPriorityRedOrders";

            // create subscriptions with rules
            await CreateSubscriptionWithOptionAndRules(adminClient, topicName, subscriptionAllOrders, 
                new TrueRuleFilter());

            await CreateSubscriptionWithOptionAndRules(adminClient, topicName, subscriptionColorBlueSize10Orders, 
                new SqlRuleFilter("color='blue' AND quantity=10"));

            await CreateSimpleSubscription(adminClient, topicName, subscriptionColorRed);

            await DeleteRuleAsync(adminClient, topicName, subscriptionColorRed, "$Default");

            await CreateRuleAsync(adminClient, topicName, subscriptionColorRed, 
                new CreateRuleOptions
                {
                    Name = "RedOrdersWithAction",
                    Filter = new SqlRuleFilter("user.color='red'"),
                    Action = new SqlRuleAction("SET quantity = quantity / 2; REMOVE priority;SET sys.CorrelationId = 'low';")

                });

            await CreateSubscriptionWithOptionAndRules(adminClient, topicName, subscriptionHighPriorityRedOrders, 
                new CorrelationRuleFilter(){Subject = "red", CorrelationId = "high"});
        }

        private async Task CreateTopic(string topicName, ServiceBusAdministrationClient adminClient)
        {
            Console.WriteLine($"Creating the topic {topicName}");
            // create a topic
            await adminClient.CreateTopicAsync(topicName);
        }

        private async Task CreateSubscriptionWithOptionAndRules(ServiceBusAdministrationClient adminClient, string topicName, string subscriptionName, RuleFilter filter)
        {
            Console.WriteLine($"Creating the subscription {subscriptionName} for the topic {topicName}");
            // create a subscription
            await adminClient.CreateSubscriptionAsync(
                new CreateSubscriptionOptions(topicName, subscriptionName),
                new CreateRuleOptions(subscriptionName, filter)
            );
        }

        private async Task CreateSimpleSubscription(ServiceBusAdministrationClient adminClient, string topicName, string subscriptionName)
        {
            Console.WriteLine($"Creating the subscription {subscriptionName} for the topic {topicName}");
            // create a subscription
            await adminClient.CreateSubscriptionAsync(topicName, subscriptionName);
        }

        private async Task DeleteRuleAsync(ServiceBusAdministrationClient adminClient, string topicName, string subscriptionName, string ruleName)
        {
            Console.WriteLine($"Deleting the rule {ruleName} from the subscription {subscriptionName} for the topic {topicName}");
            // delete a rule
            await adminClient.DeleteRuleAsync(topicName, subscriptionName, ruleName);
        }

        private async Task CreateRuleAsync(ServiceBusAdministrationClient adminClient, string topicName, string subscriptionName, CreateRuleOptions ruleOptions)
        {
            Console.WriteLine($"Creating the rule {ruleOptions.Name} for the subscription {subscriptionName} for the topic {topicName}");
            // create a rule
            await adminClient.CreateRuleAsync(topicName, subscriptionName, ruleOptions);
        }
    }
}