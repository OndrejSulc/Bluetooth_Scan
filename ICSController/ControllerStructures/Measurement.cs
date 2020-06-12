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

        public Measurement CreateDeepCopy()
        {
            Measurement returnedM = new Measurement();

            returnedM.SensorCategory = SensorCategory;
            returnedM.SensorName = SensorName;
            returnedM.BLE_Name = BLE_Name;
            returnedM.BLE_MAC = BLE_MAC;
            returnedM.BLE_RSSI = BLE_RSSI;

            return returnedM;
        }

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
