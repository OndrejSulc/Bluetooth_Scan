using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace ICSController
{
    class Options
    {
        public static string mqttServerIP;
        public static int mqttServerPort;
        public static string mqttServerUser;
        public static string mqttServerPW;
        public static string mqttServerTopic;
        public static int EvaluationIntervalMiliseconds;
        public static int RssiCutoff;
        
        public static void LoadSettings()
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                mqttServerIP = appSettings["mqttServerIP"];
                mqttServerPort = int.Parse(appSettings["mqttServerPort"]);
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
