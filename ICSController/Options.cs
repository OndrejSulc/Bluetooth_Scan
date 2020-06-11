using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace ICSController
{
    class Options
    {
        // MQTT Broker connection info
        public static string mqttServerIP;
        public static string mqttServerUser;
        public static string mqttServerPW;
        public static string mqttServerTopic;

        // Functionality setup
        public static int EvaluationIntervalMiliseconds;
        public static int RssiCutoff;// = 0; // RSSI < RssiCutoff will be ignored , value 0 means no Cutoff
        
        public static void LoadSettings()
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                mqttServerIP = appSettings["mqttServerIP"];
                mqttServerUser = appSettings["mqttServerUser"];
                mqttServerPW = appSettings["mqttServerPW"];
                mqttServerTopic = appSettings["mqttServerTopic"];

                EvaluationIntervalMiliseconds = int.Parse(appSettings["EvaluationIntervalMiliseconds"]);
                RssiCutoff = int.Parse(appSettings["RssiCutoff"]);
            }
            catch (ConfigurationErrorsException e)
            {
                Console.WriteLine("Error reading app settings");
                Console.WriteLine(e);
                System.Environment.Exit(1);
            }
        }

    }
}
