using System;
using System.Collections.Generic;
using System.Text;

namespace ICSController
{
    class Options
    {
        // MQTT Broker connection info
        public const string mqttServerIP = "192.168.1.15";
        public const string mqttServerUser = "luni";
        public const string mqttServerPW = "1641999";
        public const string mqttServerTopic = "testTopic/#";


        // Functionality setup
        public static int EvaluationIntervalMiliseconds { get; set; } = 3000;
        public static int RssiCutoff { get; set; } = 0; // RSSI < RssiCutoff will be ignored , value 0 means no Cutoff

    }
}
