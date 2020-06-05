﻿using ICSController.ControllerStructures;
using System;
using System.Collections.Generic;
using System.Text;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;


namespace ICSController
{
    class MqttMessageCapturing
    {
        public static void MeasurementReceived(object sender, MqttMsgPublishEventArgs e)
        {
            ParsedMqttTopic parsedNameAndTopic = ParseNameAndCategory(e.Topic);
            ParsedMqttMessage parsedMessage = ParseMessage(e.Message);

            if (parsedMessage.correctParse)
            {
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
                SavedMeasurements.AddMeasurement(newMeasurement);
            }
            else
            {
                Console.WriteLine("measurement receive failed.. message dropped ");
            }
        }


        private static ParsedMqttMessage ParseMessage(byte[] msg)
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


        private static ParsedMqttTopic ParseNameAndCategory(string mqttMessageTopic) 
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
