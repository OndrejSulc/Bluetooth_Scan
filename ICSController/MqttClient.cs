
using System;
using MQTTnet.Client;
using MQTTnet.Exceptions;
using MQTTnet.Client.Options;
using MQTTnet;
using System.Threading.Tasks;

namespace ICSController
{
    internal class MqttClient
    {
        private readonly MqttMessageCatching.MqttMessageCatcher mqttMessageCapturingObj;
        private readonly IMqttClient mqttClient;
        private readonly IMqttClientOptions options;

        public MqttClient(MqttMessageCatching.MqttMessageCatcher catcherObj) : this(catcherObj,Guid.NewGuid().ToString()){}

        public MqttClient(MqttMessageCatching.MqttMessageCatcher catcherObj, string clientId )
        {
            mqttMessageCapturingObj = catcherObj;
            var factory = new MqttFactory();
                mqttClient = factory.CreateMqttClient();
                options = new MqttClientOptionsBuilder()
                .WithClientId(clientId)
                .WithTcpServer(Options.MqttServerIp, Options.MqttServerPort)
                .WithCredentials(Options.MqttServerUser, Options.MqttServerPw)
                .WithCleanSession()
                .Build();
        }

        public async Task SetupAndRunMqttClient()
        {
            SetupMqttClient();
            await RunMqttClient();
        }

        private void SetupMqttClient()
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

        private async Task RunMqttClient()
        {
            try
            {
                await mqttClient.ConnectAsync(options);
            }
            catch (MqttCommunicationException e)
            {
                Console.WriteLine("Connection to MQTT Broker failed..");
            }

            Console.WriteLine("Measurement receiving thread started..");
        }
    }
}
