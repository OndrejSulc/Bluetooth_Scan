# BLE based presence detection system 

## Table of contents
- [How does it work?](##-How-does-it-work?)
- [MQTT message structure](##MQTT-message-structure)
- [Setting up sensor](###-Sensor-on-ESP32-board-using-Arduino-IDE)
- [Setting up controller](###-Controller-on-your-computer-using-Visual-Studio)

## How does it work?
System is composed of multiple **sensors** and one **controller**. Each **sensor** is represented by ESP32 board with BLE and WiFi modules. **Controller** is application running on network device.

**Sensors** regularly scan their surroundings for any devices having their BLE turned on. Then they send collected data over MQTT protocol to **controller**.

**Controller** then time from time evaluates data by finding sensor with the strongest RSSI at each scanned device for that particular scanning period. 

## MQTT message structure
MQTT topic is divided into category and name by last "/".
```example
"building1/Floor3/area1"

category == "building1/Floor3/"
name == "area1"
```
in case there isn't any "/" , category and name are the same.

MQTT message it self has following syntax:
```string
"BLE_name;BLE_MAC;BLE_RSSI"
```


## Setting up
### Sensor on ESP32 board using Arduino IDE
 1) Open **ESP32cidlo.ino** file in [Arduino IDE](https://www.arduino.cc/en/Main/Software).
 2) Change constants at the top of program corresponding to your network.

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
  1) Open **app.config** file
  2) Change values in appSettings tag
  ```xml
  <?xml version="1.0" encoding="utf-8" ?>
  <configuration>
    <appSettings>
      <add key="mqttServerIP" value="192.168.1.15" />
      <add key="mqttServerPort" value="1883" />
      <add key="mqttServerUser" value="user" />
      <add key="mqttServerPW" value="user123pw" />
      <add key="mqttServerTopic" value="testTopic/#" />
      <add key="EvaluationIntervalMiliseconds" value="10000" />
      <add key="RssiCutoff" value="0" /> <!-- RSSI < RssiCutoff will be ignored , value 0 means no Cutoff -->
    </appSettings>
  </configuration>

  ```
  3) Build and run solution
  4) Check for control messages
  ```
Measurement processing task started..
Evaluation printing task started..
Connected to MQTT Broker : x.x.x.x:x
Subscribed to topic: testTopic/#
Measurement receiving thread started..
  ```












