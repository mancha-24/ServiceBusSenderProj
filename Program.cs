// See https://aka.ms/new-console-template for more information

using ServiceBusSenderProj;

try
{
    string connectionString = "Endpoint=sb://mcservicebusaz.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=ETvNtPyBPCFcAst9PKyH01rmX2nMayYQv+ASbDu8C68=";
    //TopicInitializator topicInitializator = new TopicInitializator(connectionString);
    TopicMessageSender topicSender = new TopicMessageSender();
    await topicSender.SendMessagesToTopicAsync(connectionString);
    Console.ReadLine();
}
catch (Exception)
{
    Console.ReadLine();
    throw;
}
