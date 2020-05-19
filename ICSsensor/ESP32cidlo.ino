#include <BLEDevice.h>

#include <SPI.h>
#include <WiFi.h>
#include <PubSubClient.h>

 
//// ENTER NETWORK INFO HERE ////////////

const char* networkSSID = "";
const char* networkPW = "";

const char* mqttServer = "";
const int mqttPort = 1883;

const char* mqttID = "";
const char* mqttUser = "";
const char* mqttPW = "";

//////////////////////////////////////////

WiFiClient espClient;
PubSubClient mqttClient(espClient);


//const int RSSI_CUTOFF = -60;


void setupBluetooth()
{
  BLEDevice::init("");
  Serial.println("Bluetooth device initialized");
}


void setupWifi()
{
  WiFi.begin(networkSSID, networkPW);
 
  while (WiFi.status() != WL_CONNECTED)
  {
    delay(500);
    Serial.println("Connecting to WiFi..");
  }
  
  Serial.println("Connected to the WiFi network");
}


void setupMQTTclient()
{
  mqttClient.setServer(mqttServer, mqttPort);   

  Serial.println("Connecting to MQTT server");
  bool mqttConnection = 0;
  while(!mqttConnection)
  {
    mqttConnection = mqttClient.connect(mqttID, mqttUser, mqttPW);
    if (mqttConnection) 
    {
      Serial.println("Connected to the MQTT server");
    } 
    else 
    {
      delay(500);
    }
  } 
}


void setup() 
{
  Serial.begin(115200);

  setupBluetooth();

  setupWifi();
  
  setupMQTTclient();

  Serial.println("Sensor is ready");
 
}

void loop() {

  Serial.println("\n------------- BEGIN -------------------\n");
 
  BLEScan *scan = BLEDevice::getScan();
  scan->setActiveScan(true);
  BLEScanResults results = scan->start(1);

  for (int i = 0; i < results.getCount(); i++)
  {
    
    BLEAdvertisedDevice device = results.getDevice(i);

    int rssi = device.getRSSI();

    BLEAddress address = device.getAddress();
    std::string addSTR = address.toString();

    char mqttMessage[40];
   
    strcpy(mqttMessage,"ESP32;");
    
    strcat(mqttMessage,device.getName().c_str());
    strcat(mqttMessage,";");
    
    strcat(mqttMessage,addSTR.c_str()); 
    strcat(mqttMessage,";");

    char rssiStr[3];
    itoa(rssi,rssiStr,10);
    
    strcat(mqttMessage,rssiStr);   
    strcat(mqttMessage,";");
    
   
    // This is needed at the top of the loop!
    mqttClient.loop();
  
    // Ensure that we are subscribed to the topic "testTopic"
    mqttClient.subscribe("testTopic");
  
    // Attempt to publish a value to the topic "testTopic"
    if(mqttClient.publish("testTopic", mqttMessage))
    {
      Serial.print("Published: ");
      Serial.println(mqttMessage);
    }
    else
    {
      Serial.println("Could not send message!");
    }
  
    // Dont overload the server!
    delay(1000);
  }
  
  delay(3000);
  Serial.println("\n------------- END -------------------\n");
}
