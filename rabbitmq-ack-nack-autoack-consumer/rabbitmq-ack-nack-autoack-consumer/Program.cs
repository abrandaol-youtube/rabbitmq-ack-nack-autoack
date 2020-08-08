using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace rabbitmq_ack_nack_autoack_consumer
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "hello",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                BuildAndRunWorker(channel, "Worker A");
                
                Console.ReadLine();
            }
        }

        private static void BuildAndRunWorker(IModel channel, string workerName)
        {
            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"{channel.ChannelNumber} - {workerName}: [x] Received {message}");
            };

            channel.BasicConsume(queue: "order", autoAck: true, consumer: consumer);

            Console.WriteLine(" Press [enter] to exit.");
        }
    }
}
