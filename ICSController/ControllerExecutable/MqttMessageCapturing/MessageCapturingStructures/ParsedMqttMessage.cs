using System;
using System.Collections.Generic;
using System.Text;

namespace ICSController
{
    class ParsedMqttMessage
    {
        public string bleName;
        public string bleMAC;
        public sbyte bleRSSI;
        public bool correctParse;
    }
}
