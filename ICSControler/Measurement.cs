using System;
using System.Collections.Generic;
using System.Text;

namespace ICSControler
{
    class Measurement
    {
        public DateTime Time { get; set; }
        public string Sensor { get; set; }
        public string Name { get; set; }
        public string MAC { get; set; }
        public sbyte RSSI { get; set; }

        public int ConsolePrint() 
        {
            Console.WriteLine("TIME:" + Time.ToString() + "\nSensor: " + Sensor + "\nName: " + Name + "\nMAC: " + MAC + "\nRSSI: " + RSSI + "\n---\n");
            return 0;
        }
    }
}
