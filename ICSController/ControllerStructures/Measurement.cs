using System;
using System.Collections.Generic;
using System.Text;

namespace ICSController
{
    class Measurement
    {
        public DateTime Time;

        public string SensorCategory;
        public string SensorName;
        public string BLE_Name;
        public string BLE_MAC;
        public sbyte BLE_RSSI;

        public override string ToString() 
        {
             return "TIME:" + Time.ToString() +
                    "\nSensor category: " + SensorCategory +
                    "\nSensor name: " + SensorName +
                    "\nBLE Name: " + BLE_Name +
                    "\nBLE MAC: " + BLE_MAC +
                    "\nBLE RSSI: " + BLE_RSSI +
                    "\n---\n";
        }
    }
}
