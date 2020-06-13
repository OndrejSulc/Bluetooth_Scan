using System;
using System.Configuration;

namespace ICSController
{
    internal static class Options
    {
        public static string MqttServerIp;
        public static int MqttServerPort;
        public static string MqttServerUser;
        public static string MqttServerPw;
        public static string MqttServerTopic;
        public static int EvaluationIntervalMilliseconds;
        public static int RssiCutoff;
        
        public static void LoadSettings()
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                MqttServerIp = appSettings["mqttServerIP"];
                MqttServerPort = int.Parse(appSettings["mqttServerPort"]);
                MqttServerUser = appSettings["mqttServerUser"];
                MqttServerPw = appSettings["mqttServerPW"];
                MqttServerTopic = appSettings["mqttServerTopic"];
                EvaluationIntervalMilliseconds = int.Parse(appSettings["EvaluationIntervalMilliseconds"]);
                RssiCutoff = int.Parse(appSettings["RssiCutoff"]);
            }
            catch (ConfigurationErrorsException e)
            {
                Console.WriteLine("Error reading app settings");
                Console.WriteLine(e);
                Environment.Exit(1);
            }
        }
    }
}
