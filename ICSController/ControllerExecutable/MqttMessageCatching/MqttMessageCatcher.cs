using System;
using System.Collections.Generic;
using System.Text;


namespace ICSController.MqttMessageCatching
{
    class MqttMessageCatcher
    {
        private readonly MeasurementsChannel SavedMeasurementsChannel;

        public MqttMessageCatcher(MeasurementsChannel channelForSavingMeasurements)
        {
            SavedMeasurementsChannel = channelForSavingMeasurements;
        }

        public void MeasurementReceived(ManagedMqttApplicationMessage message, Exception e)
        {
            ParsedMqttMessage parsedMessage = ParseMessage(e.Message);

            if (parsedMessage.correctParse)
            {
                ParsedMqttTopic parsedNameAndTopic = ParseNameAndCategory(e.Topic);
                
                Measurement newMeasurement = new Measurement
                {
                    Time = DateTime.Now,
                    SensorCategory = parsedNameAndTopic.category,
                    SensorName = parsedNameAndTopic.name,
                    BLE_Name = parsedMessage.bleName,
                    BLE_MAC = parsedMessage.bleMAC,
                    BLE_RSSI = parsedMessage.bleRSSI
                }; 

                Console.WriteLine("\nreceived measurement");
                Console.WriteLine(newMeasurement);
                SavedMeasurementsChannel.AddMeasurement(newMeasurement);
            }
            else
            {
                Console.WriteLine("measurement receive failed.. message dropped ");
            }
        }


        private ParsedMqttMessage ParseMessage(byte[] msg)
        {
            ParsedMqttMessage returnObj = new ParsedMqttMessage();
            var messageString = "";

            for (int i = 0; i < msg.Length; i++)
            {
                messageString += ((char)msg[i]);
            }

            var messageArray = messageString.Split(";");
            if (messageArray.Length == 4)
            {
                returnObj.bleName = messageArray[0];
                returnObj.bleMAC = messageArray[1];
                returnObj.bleRSSI = (sbyte)int.Parse(messageArray[2]);

                returnObj.correctParse = true;
                return returnObj;
            }
            else 
            {
                returnObj.correctParse = false;
                return returnObj;
            }      
        }


        private ParsedMqttTopic ParseNameAndCategory(string mqttMessageTopic) 
        {
            ParsedMqttTopic returnObj = new ParsedMqttTopic();
            for (int i = mqttMessageTopic.Length - 1; i != 0; i--)
            {
                if (mqttMessageTopic[i] == '/')
                {
                    returnObj.name = mqttMessageTopic.Substring(i+1);
                    returnObj.category = mqttMessageTopic.Remove(i+1);
                    return returnObj;
                }
            }

            returnObj.name = mqttMessageTopic;
            returnObj.category = mqttMessageTopic;
            return returnObj;
        }
    }
}
