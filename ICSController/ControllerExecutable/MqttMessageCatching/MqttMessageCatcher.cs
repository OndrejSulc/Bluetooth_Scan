using MQTTnet;
using System;
using System.Text;


namespace ICSController.MqttMessageCatching
{
    internal class MqttMessageCatcher
    {
        private readonly MeasurementsChannel savedMeasurementsChannel;

        public MqttMessageCatcher(MeasurementsChannel channelForSavingMeasurements)
        {
            savedMeasurementsChannel = channelForSavingMeasurements;
        }

        public void MeasurementReceived(MqttApplicationMessageReceivedEventArgs message)
        {
            var parsedMessage = ParseMessage(Encoding.UTF8.GetString(message.ApplicationMessage.Payload));

            if (parsedMessage.CorrectParse)
            {
                var parsedNameAndTopic = ParseNameAndCategory(message.ApplicationMessage.Topic);
                
                var newMeasurement = new Measurement
                {
                    Time = DateTime.Now,
                    SensorCategory = parsedNameAndTopic.Category,
                    SensorName = parsedNameAndTopic.Name,
                    BleName = parsedMessage.BleName,
                    BleMac = parsedMessage.BleMac,
                    BleRssi = parsedMessage.BleRssi
                }; 

                Console.WriteLine("\nreceived measurement");
                Console.WriteLine(newMeasurement);
                savedMeasurementsChannel.AddMeasurement(newMeasurement);
            }
            else
            {
                Console.WriteLine("measurement receive failed.. message dropped ");
            }
        }

        private ParsedMqttMessage ParseMessage(string message)
        {
            var returnObj = new ParsedMqttMessage();

            var messageArray = message.Split(";");
            if (messageArray.Length == 4)
            {
                returnObj.BleName = messageArray[0];
                returnObj.BleMac = messageArray[1];
                returnObj.BleRssi = (sbyte)int.Parse(messageArray[2]);

                returnObj.CorrectParse = true;
                return returnObj;
            }
            else 
            {
                returnObj.CorrectParse = false;
                return returnObj;
            }  
        }

        private ParsedMqttTopic ParseNameAndCategory(string mqttMessageTopic) 
        {
            var returnObj = new ParsedMqttTopic();
            for (var i = mqttMessageTopic.Length - 1; i != 0; i--)
            {
                if (mqttMessageTopic[i] == '/')
                {
                    returnObj.Name = mqttMessageTopic.Substring(i+1);
                    returnObj.Category = mqttMessageTopic.Remove(i+1);
                    return returnObj;
                }
            }

            returnObj.Name = mqttMessageTopic;
            returnObj.Category = mqttMessageTopic;
            return returnObj;
        }
    }
}
