﻿using System;
using System.Collections.Generic;
using System.Text;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;


namespace ICSController
{
    class MqttMessageCapturing
    {
        private static string msgSensorCategory;
        private static string msgSensorName;
        private static string msgBLEName;
        private static string msgBLEMAC;
        private static string msgBLERSSI;
        private static bool correct;


        /// <summary>
        /// Main function for message capturing thread
        /// </summary>
        public static void MeasurementRecieved(object sender, MqttMsgPublishEventArgs e)
        {
            msgSensorCategory = e.Topic;
            msgSensorName = "";
            msgBLEName = "";
            msgBLEMAC = "";
            msgBLERSSI = "";
            correct = true;
            
            // process measurement data
            ParseNameAndCategory();
            ParseMessage(e.Message);

            //add to list if processing was successful 
            if (correct)
            {
                SaveMeasurement();
            }
        }

        /// <summary>
        /// Parses data from MQTT message and saves it to relevant variables
        /// </summary>
        /// <param name="msg">MQTT message in byte array</param>
        private static void ParseMessage(byte[] msg)
        {
            byte operationCounter = 0;
            char character;

            for (int i = 0; i < msg.Length; i++)
            {
                character = ((char)msg[i]);

                if (character == ';')
                {
                    operationCounter += 1;
                }
                else
                {
           
                    if (operationCounter == 0)
                    {
                        msgBLEName = msgBLEName + character;
                    }
                    else if (operationCounter == 1)
                    {
                        msgBLEMAC = msgBLEMAC + character;
                    }
                    else if (operationCounter == 2)
                    {
                        msgBLERSSI = msgBLERSSI + character;
                    }
                    else
                    {
                        Console.WriteLine("measurement receive failed.. message dropped ");
                        correct = false;
                        return;
                    }
                }
            }
        }


        /// <summary>
        /// Creates new measurement object from private data and saves it to ReceivedMeasurements list
        /// </summary>
        private static void SaveMeasurement() 
        {
            //create new measurement object
            Measurement newMeasurement = new Measurement { 
                Time = DateTime.Now,
                SensorCategory = msgSensorCategory,
                SensorName = msgSensorName,
                BLE_Name = msgBLEName,
                BLE_MAC = msgBLEMAC,
                BLE_RSSI = (sbyte)int.Parse(msgBLERSSI) };

            //check print
            Console.WriteLine("\nreceived measurement");
            newMeasurement.ConsolePrint();

            //assign to list on free lock
            lock (Program.RMlock)
            {
                Program.ReceivedMeasurements.Add(newMeasurement);
                Console.WriteLine("Count of received: " + Program.ReceivedMeasurements.Count);
            }
        }


        /// <summary>
        /// Parses sensor name and Category from MQTT message topic
        /// </summary>
        private static void ParseNameAndCategory() 
        {
            for (int i = msgSensorCategory.Length - 1; i != 0; i--)
            {
                if (msgSensorCategory[i] == '/')
                {
                    msgSensorName = msgSensorCategory.Substring(i+1);
                    msgSensorCategory = msgSensorCategory.Remove(i+1);
                    return;
                }
            }

            // if there is no name then copy category
            msgSensorName = msgSensorCategory;
        }

    }
}
