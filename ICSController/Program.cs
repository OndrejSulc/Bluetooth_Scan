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
        public const string mqttServerIP = "192.168.1.15";
        public const string mqttServerUser = "luni";
        public const string mqttServerPW = "1641999";
        public const string mqttServerTopic = "testTopic/#";
        //

        // Functionality setup
        public static int EvaluationIntervalMiliseconds { get; set; } = 10_000;
        public static int RssiCutoff { get; set; } = 0; // RSSI < RssiCutoff will be ignored , value 0 means no Cutoff
        //

        static void Main(string[] args)
        {
            //setup connection to MQTT Broker
            var client = new MqttClient(mqttServerIP);

            client.MqttMsgPublishReceived += MqttMessageCapturing.MeasurementRecieved;

            var clientId = Guid.NewGuid().ToString();
            client.Connect(clientId, mqttServerUser, mqttServerPW);

            client.Subscribe(
                new string[] { mqttServerTopic },
                new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });
            //

            
            //continue to evaluation loop
            Console.WriteLine("Controller running...");
            Evaluation.EvaluationThread();
        }
        
    }
}

