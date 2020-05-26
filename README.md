# BLE based presence detection system 

## How does it work?
System is composed of multiple **sensors** and one **controller**. Each **sensor** is represented by ESP32 board with BLE and WiFi modules. **Controller** is application running on network device.

**Sensors** regularly scan their surroundings for any devices having their BLE turned on. Then they send collected data over MQTT protocol to **controller**.

**Controller** then time from time evaluates data by finding sensor with the strongest RSSI at each scanned device for that particular scanning period. 

## Setting up
### Sensor on ESP32 board using Arduino IDE
 1) Open **ESP32cidlo.ino** file in [Arduino IDE](https://www.arduino.cc/en/Main/Software).
 2) Change constant at the top of program corresponding to your network.

```C++
//// ENTER NETWORK INFO HERE ////////////

const char* networkSSID = "";  // your WiFi SSID
const char* networkPW = "";    // your WiFi password

const char* mqttServer = "192.168.1.15";   // IPv4 address to your MQTT Broker
const int mqttPort = 1883;     // Port at which MQTT Broker is listening (1883 is standard for MQTT protocol)

const char* mqttID = "testTopic/ESP01"; // your_topic/your_subtopic/your_sensor_ID (subtopics are not required)
const char* mqttUser = "user";     // user name to your MQTT Broker
const char* mqttPW = "user123pw";       // user password to your MQTT Broker

//////////////////////////////////////////
```
 3) Plug in ESP32 board to your computer
 4) Download needed libraries using IDE (Project>Add library>Manage libraries) 
 5) Set relevant port at Tools
 6) Compile and upload to ESP32

 ### Controller on your computer using Visual Studio
  1) Open ICSController.sln using Visual Studio
  2) Change constant at the top of program corresponding to your network.
  ```C#
namespace ICSController
{
    class Program
    {

        // MQTT Broker connection info
        public const string MqttServerIP = "192.168.1.15";   // IPv4 address to your MQTT Broker, port is always 1883
        public const string MqttServerUser = "user";         // user name to your MQTT Broker
        public const string MqttServerPW = "user123pw";        // user password to your MQTT Broker
        public const string MqttServerTopic = "testTopic/#"; // your_topic/#
        //
  ```
  3) Change values based on your need
  ```C#
        // Functionality setup
        public static int EvaluationIntervalMiliseconds { get; set; } = 10_000;
        public static int RssiCutoff { get; set; } = 0; // RSSI < RssiCutoff will be ignored value 0 means no Cutoff
        //
  ```
  4) Build and run solution
  5) Check for message
  ```
  Controller running...
  ```












