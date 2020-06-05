using System;
using System.Collections.Generic;
using System.Text;

namespace ICSController.ControllerStructures
{
    class ParsedMqttMessage
    {
        public string bleName;
        public string bleMAC;
        public sbyte bleRSSI;
        public bool correctParse;
    }
}
