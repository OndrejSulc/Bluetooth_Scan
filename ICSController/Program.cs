using System.Threading.Tasks;

namespace ICSController
{
    public class Program
    {
        public static async Task Main()
        {
            Options.LoadSettings();

            var captureToProcessChannel = new MeasurementsChannel();
            var mqttMessageCapturingObj = new MqttMessageCatching.MqttMessageCatcher(captureToProcessChannel);
            var mqttClientObj = new MqttClient(mqttMessageCapturingObj);
            var evaluator = new Evaluation.Evaluator(captureToProcessChannel);

            evaluator.StartEvaluation();
            await mqttClientObj.SetupAndRunMqttClient();

            while (true)
            {
                await Task.Delay(10_000);
            }
        }
    }
}

