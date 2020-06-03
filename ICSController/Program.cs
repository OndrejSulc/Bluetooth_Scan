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
        public static void Main()
        {
            MainAsync().GetAwaiter().GetResult();
        }


        static async Task MainAsync()
        {
            var client = new MqttClient(Options.mqttServerIP);

            client.MqttMsgPublishReceived += MqttMessageCapturing.MeasurementReceived;

            var clientId = Guid.NewGuid().ToString();
            client.Connect(clientId, Options.mqttServerUser, Options.mqttServerPW);

            client.Subscribe(
                new string[] { Options.mqttServerTopic },
                new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });
            


            Console.WriteLine("Measurement receiving thread started..");
            await Evaluation.StartEvaluationThread();
        }
    }
}

