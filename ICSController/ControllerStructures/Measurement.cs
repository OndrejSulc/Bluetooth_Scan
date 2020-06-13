using System;

namespace ICSController
{
    internal class Measurement
    {
        public DateTime Time;

        public string SensorCategory;
        public string SensorName;
        public string BleName;
        public string BleMac;
        public sbyte BleRssi;

        public Measurement CreateDeepCopy()
        {
            var returnedM = new Measurement
            {
                SensorCategory = SensorCategory,
                SensorName = SensorName,
                BleName = BleName,
                BleMac = BleMac,
                BleRssi = BleRssi
            };

            return returnedM;
        }

        public override string ToString() 
        {
             return "TIME:" + Time +
                    "\nSensor category: " + SensorCategory +
                    "\nSensor name: " + SensorName +
                    "\nBLE Name: " + BleName +
                    "\nBLE MAC: " + BleMac +
                    "\nBLE RSSI: " + BleRssi +
                    "\n---\n";
        }
    }
}
