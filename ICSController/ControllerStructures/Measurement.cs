using System;
using System.Collections.Generic;
using System.Text;

namespace ICSController
{
    class Measurement
    {
        public DateTime Time { get; set; }

        public string SensorCategory { get; set; }
        public string SensorName { get; set; }
        public string BLE_Name { get; set; }
        public string BLE_MAC { get; set; }
        public sbyte BLE_RSSI { get; set; }

        public void PrintToConsole() 
        {
            Console.WriteLine("TIME:" + Time.ToString() +
                              "\nSensor category: " + SensorCategory +
                              "\nSensor name: " + SensorName +
                              "\nBLE Name: " + BLE_Name +
                              "\nBLE MAC: " + BLE_MAC +
                              "\nBLE RSSI: " + BLE_RSSI +
                              "\n---\n");

        }
    }
}
