using System;
using System.Text;
using System.Net;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using System.Collections.Generic;


namespace ICSController
{
    class Program
    {

        // MQTT Broker connection info
        public const string MqttServerIP = "192.168.1.15";
        public const string MqttServerUser = "luni";
        public const string MqttServerPW = "1641999";
        public const string MqttServerTopic = "testTopic/#";
        //


        // Functionality setup
        public static int EvaluationIntervalMiliseconds { get; set; } = 10_000;
        public static int RssiCutoff { get; set; } = 0; // RSSI < RssiCutoff will be ignored , value 0 means no Cutoff
        //


        // global data
        public static List<Measurement> ReceivedMeasurements { get; set; } = new List<Measurement>();
        public static object RMlock { get; set; } = new object();
        //


        static void Main(string[] args)
        {
            //setup connection to MQTT Broker
            var client = new MqttClient(MqttServerIP);

            client.MqttMsgPublishReceived += MqttMessageCapturing.MeasurementRecieved;

            var clientId = Guid.NewGuid().ToString();
            client.Connect(clientId, MqttServerUser, MqttServerPW);

            client.Subscribe(
                new string[] { MqttServerTopic },
                new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });
            //

            
            //continue to evaluation loop
            Console.WriteLine("Controller running...");
            Evaluation.EvaluationThread();
        }
        
    }
}

