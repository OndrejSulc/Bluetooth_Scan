using System;
using System.Text;
using System.Net;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;



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

            var client = new MqttClient(Options.mqttServerIP);

            client.MqttMsgPublishReceived += mqttMessageCapturingObj.MeasurementReceived;

            var clientId = Guid.NewGuid().ToString();

            try
            {
                client.Connect(clientId, Options.mqttServerUser, Options.mqttServerPW);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                System.Environment.Exit(1);
            }

            client.Subscribe(
                new string[] { Options.mqttServerTopic },
                new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });

            
            Console.WriteLine("Measurement receiving thread started..");
            evaluator.StartEvaluation();
        }
    }
}

