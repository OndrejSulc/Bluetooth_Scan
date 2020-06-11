
using System;
using System.Collections.Generic;
using System.Text;
using MQTTnet.Client;
using MQTTnet.Exceptions;
using MQTTnet.Client.Options;
using MQTTnet;
using System.Threading.Tasks;
using System.Threading;

namespace ICSController
{
    class MqttClient
    {
        private MqttMessageCatching.MqttMessageCatcher mqttMessageCapturingObj;

        public MqttClient(MqttMessageCatching.MqttMessageCatcher CatcherObj)
        {
            mqttMessageCapturingObj = CatcherObj;
        }

        public async Task SetupAndRunMqttClient()
        {
            var clientId = Guid.NewGuid().ToString();

            var factory = new MqttFactory();
            var mqttClient = factory.CreateMqttClient();
            var options = new MqttClientOptionsBuilder()
            .WithClientId(clientId)
            .WithTcpServer(Options.mqttServerIP)
            .WithCredentials(Options.mqttServerUser, Options.mqttServerPW)
            .WithCleanSession()
            .Build();

            SetupMqttClient(mqttClient, options);
            await RunMqttClient(mqttClient, options);
        }

        public void SetupMqttClient(IMqttClient mqttClient, IMqttClientOptions options)
        {
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

            mqttClient.UseApplicationMessageReceivedHandler(e => mqttMessageCapturingObj.MeasurementReceived(e));

            
        }

        public async Task RunMqttClient(IMqttClient mqttClient, IMqttClientOptions options)
        {
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
        }
    }
}
