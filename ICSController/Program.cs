using System;
using System.Text;
using System.Net;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Client.Options;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Exceptions;

namespace ICSController
{
    class Program
    {
        public static async Task Main()
        {
            Options.LoadSettings();

            MeasurementsChannel captureToProccessChannel = new MeasurementsChannel();
            MqttMessageCatching.MqttMessageCatcher mqttMessageCapturingObj = new MqttMessageCatching.MqttMessageCatcher(captureToProccessChannel);
            MqttClient mqttClientObj = new MqttClient(mqttMessageCapturingObj);
            Evaluation.Evaluator evaluator = new Evaluation.Evaluator(captureToProccessChannel);

            evaluator.StartEvaluation();
            await mqttClientObj.SetupAndRunMqttClient();

            while (true)
            {
                await Task.Delay(10_000);
            }
        }
    }
}

