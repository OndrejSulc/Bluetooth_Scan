using System;
using System.Text;
using System.Net;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Client.Options;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Exceptions;

namespace ICSController
{
    class Program
    {
        public static async Task Main()
        {
            Options.LoadSettings();

            MeasurementsChannel captureToProccessChannel = new MeasurementsChannel();
            MqttMessageCatching.MqttMessageCatcher mqttMessageCapturingObj = new MqttMessageCatching.MqttMessageCatcher(captureToProccessChannel);
            Evaluation.Evaluator evaluator = new Evaluation.Evaluator(captureToProccessChannel);

            var clientId = Guid.NewGuid().ToString();

            var factory = new MqttFactory();
            var mqttClient = factory.CreateMqttClient();
            var options = new MqttClientOptionsBuilder()
            .WithClientId(clientId)
            .WithTcpServer(Options.mqttServerIP)
            .WithCredentials(Options.mqttServerUser, Options.mqttServerPW)
            .WithCleanSession()
            .Build();


            mqttClient.UseConnectedHandler(async e =>
            {
                Console.WriteLine("### CONNECTED WITH SERVER ###");

                await mqttClient.SubscribeAsync(new MqttTopicFilter().Topic = Options.mqttServerTopic);

                Console.WriteLine("### SUBSCRIBED ###");
            });


            mqttClient.UseDisconnectedHandler(async e =>
            {
                Console.WriteLine("### DISCONNECTED FROM SERVER ###");
                await Task.Delay(5_000);
                try
                {
                    await mqttClient.ConnectAsync(options);
                }
                catch
                {
                    Console.WriteLine("### RECONNECTING FAILED ###");
                }
            });

            mqttClient.UseApplicationMessageReceivedHandler( e => mqttMessageCapturingObj.MeasurementReceived(e));

            var tokenSource = new CancellationTokenSource();
            CancellationToken ct = tokenSource.Token;

            try
            {
                await mqttClient.ConnectAsync(options, ct);
            }
            catch (MqttCommunicationException e)
            {
                Console.WriteLine("Connection to MQTT Broker failed..");
                Console.WriteLine(e);
                System.Environment.Exit(1);
            }

            Console.WriteLine("Measurement receiving thread started..");

            evaluator.StartEvaluation();

            await Task.Delay(500_000);

        }
    }
}

