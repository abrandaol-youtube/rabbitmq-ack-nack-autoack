using RabbitMQ.Client;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace rabbitmq_ack_nack_autoack_publish
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            var manualResetEvent = new ManualResetEvent(false);

            manualResetEvent.Reset();


            using (var connection = factory.CreateConnection())
            {
                var queueName = "order";

                var channel = CreateChannel(connection, queueName);

                BuildAndRunPublishers(channel, queueName, "Produtor A");

                manualResetEvent.WaitOne();
            }

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }

        private static IModel CreateChannel(IConnection connection, string queueName)
        {
            var channel = connection.CreateModel();

            channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

            return channel;
        }

        private static void BuildAndRunPublishers(IModel channel, string queue, string publisherName)
        {
            Task.Run(() =>
            {
                int count = 0;

                while (true)
                {
                        Console.WriteLine("Pressione qualquer teclaa para produzir 100 msg");
                        Console.ReadLine();

                        for (var index = 0; index < 100; index++)
                        {
                            var message = $"OrderNumber: {count++} from {publisherName}";
                            var body = Encoding.UTF8.GetBytes(message);

                            channel.BasicPublish("", queue, null, body);

                            Console.WriteLine($"{publisherName} - [x] Sent {count} - {message}");
                        }
                }
            });
        }
    }
}
