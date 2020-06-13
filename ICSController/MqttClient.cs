
using System;
using MQTTnet.Client;
using MQTTnet.Exceptions;
using MQTTnet.Client.Options;
using MQTTnet;
using System.Threading.Tasks;
using System.Threading;

namespace ICSController
{
    internal class MqttClient
    {
        private readonly MqttMessageCatching.MqttMessageCatcher mqttMessageCapturingObj;

        public MqttClient(MqttMessageCatching.MqttMessageCatcher catcherObj)
        {
            mqttMessageCapturingObj = catcherObj;
        }

        public async Task SetupAndRunMqttClient()
        {
            var clientId = Guid.NewGuid().ToString();

            var factory = new MqttFactory();
            var mqttClient = factory.CreateMqttClient();
            var options = new MqttClientOptionsBuilder()
            .WithClientId(clientId)
            .WithTcpServer(Options.MqttServerIp, Options.MqttServerPort)
            .WithCredentials(Options.MqttServerUser, Options.MqttServerPw)
            .WithCleanSession()
            .Build();

            SetupMqttClient(mqttClient, options);
            await RunMqttClient(mqttClient, options);
        }

        private void SetupMqttClient(IMqttClient mqttClient, IMqttClientOptions options)
        {
            mqttClient.UseConnectedHandler(async e =>
            {
                Console.WriteLine("Connected to MQTT Broker : " + Options.MqttServerIp + ":" + Options.MqttServerPort);
                await mqttClient.SubscribeAsync(new MqttTopicFilter().Topic = Options.MqttServerTopic);
                Console.WriteLine("Subscribed to topic: " + Options.MqttServerTopic);
            });

            mqttClient.UseDisconnectedHandler(async e =>
            {
                Console.WriteLine("Disconnected from MQTT Broker" + Options.MqttServerIp);
                await Task.Delay(5_000);
                try
                {
                    await mqttClient.ConnectAsync(options);
                }
                catch
                {
                    Console.WriteLine("Reconnecting failed..");
                }
            });

            mqttClient.UseApplicationMessageReceivedHandler(e => mqttMessageCapturingObj.MeasurementReceived(e));
        }

        private async Task RunMqttClient(IMqttClient mqttClient, IMqttClientOptions options)
        {
            var tokenSource = new CancellationTokenSource();
            var ct = tokenSource.Token;

            try
            {
                await mqttClient.ConnectAsync(options, ct);
            }
            catch (MqttCommunicationException e)
            {
                Console.WriteLine("Connection to MQTT Broker failed..");
                Console.WriteLine(e);
                Environment.Exit(1);
            }

            Console.WriteLine("Measurement receiving thread started..");
        }
    }
}
