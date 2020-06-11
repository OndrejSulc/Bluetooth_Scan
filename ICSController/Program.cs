using System;
using System.Text;
using System.Net;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Client.Options;
using MQTTnet;

namespace ICSController
{
    class Program
    {
        public static async Task Main()
        {
            MeasurementsChannel captureToProccessChannel = new MeasurementsChannel();
            MqttMessageCatching.MqttMessageCatcher mqttMessageCapturingObj = new MqttMessageCatching.MqttMessageCatcher(captureToProccessChannel);
            Evaluation.Evaluator evaluator = new Evaluation.Evaluator(captureToProccessChannel);

            var clientId = Guid.NewGuid().ToString();

            var options = new ManagedMqttClientOptionsBuilder()
            .WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
            .WithClientOptions(new MqttClientOptionsBuilder()
                .WithClientId(clientId)
                .WithTcpServer(Options.mqttServerIP)
                .WithTls().Build())
            .Build();

            var mqttClient = new MqttFactory().CreateManagedMqttClient();
            await mqttClient.SubscribeAsync(new TopicFilterBuilder().WithTopic(Options.mqttServerTopic).Build());
            await mqttClient.StartAsync(options);

            mqttClient.ApplicationMessageProcessedHandler = mqttMessageCapturingObj.MeasurementReceived;



            
            Console.WriteLine("Measurement receiving thread started..");
            evaluator.StartEvaluation();
        }
    }
}

